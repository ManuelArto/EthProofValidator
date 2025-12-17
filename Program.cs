using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq; // Powerful tool for counting/filtering (>50% logic)

public static class ZKVerifier
{
    const string LibName = "./multi-zk-verifier/target/release/libmulti_zk_verifier.so";

    // ---------------------------------------------------------
    // Define the Native Interface (Rust FFI)
    // ---------------------------------------------------------
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr alloc(nuint len);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void dealloc(IntPtr ptr, nuint len);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int verify(int curve_type, IntPtr proof_ptr, nuint proof_len, IntPtr vk_ptr, nuint vk_len);

    // Wrapper to handle memory safety automatically
    public static bool Verify(int zkType, byte[] proofBytes, byte[] vkBytes)
    {
        IntPtr pProof = IntPtr.Zero;
        IntPtr pVk = IntPtr.Zero;

        try
        {
            pProof = CopyToRust(proofBytes);
            pVk = CopyToRust(vkBytes);

            int result = verify(zkType, pProof, (nuint)proofBytes.Length, pVk, (nuint)vkBytes.Length);
            if (result == -1)
            {
                Console.WriteLine($"[FFI Error] Unknown verifier type: {zkType}");
                return false;
            }
            
            return result == 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FFI Error] {ex.Message}");
            return false;
        }
        finally
        {
            // Always clean up unmanaged memory!
            if (pProof != IntPtr.Zero) dealloc(pProof, (nuint)proofBytes.Length);
            if (pVk != IntPtr.Zero) dealloc(pVk, (nuint)vkBytes.Length);
        }
    }

    private static IntPtr CopyToRust(byte[] data)
    {
        IntPtr ptr = alloc((nuint)data.Length);
        Marshal.Copy(data, 0, ptr, data.Length);
        return ptr;
    }

    public enum ZKType
    {
        Zisk = 0,
        OpenVM = 1,
        Pico = 2,
        Airbender = 3,
        Sp1Hypercube = 4,
        Unknown = 99
    }
}

class Program
{

    static void Main(string[] args)
    {
        string baseTestDir = "test";
        string vksDir = Path.Combine(baseTestDir, "vks");
        string blocksDir = Path.Combine(baseTestDir, "blocks");

        // Load all VKs
        var vkCache = LoadVerificationKeys(vksDir);
        if (vkCache.Count == 0) 
        {
            Console.WriteLine("No verification keys found. Exiting.");
            return;
        }

        // Process Blocks
        if (!Directory.Exists(blocksDir))
        {
            Console.WriteLine($"❌ Blocks directory not found: {blocksDir}");
            return;
        }
        string[] blockFolders = Directory.GetDirectories(blocksDir);
        Console.WriteLine($"\nFound {blockFolders.Length} blocks to process.\n");

        foreach (var blockPath in blockFolders)
        {
            ProcessBlock(blockPath, vkCache);
        }
    }

    static Dictionary<ZKVerifier.ZKType, byte[]> LoadVerificationKeys(string vksDir)
    {
        var cache = new Dictionary<ZKVerifier.ZKType, byte[]>();
        
        if (!Directory.Exists(vksDir)) return cache;

        Console.WriteLine("--------------------------------------------------");
        foreach (var file in Directory.GetFiles(vksDir, "*.bin"))
        {
            ZKVerifier.ZKType type = ParseTypeFromFilename(file);
            if (type != ZKVerifier.ZKType.Unknown)
            {
                cache[type] = File.ReadAllBytes(file);
                Console.WriteLine($"🔹 Loaded VK for: {type}");
            }
        }
        Console.WriteLine("--------------------------------------------------");
        return cache;
    }

    static void ProcessBlock(string blockPath, Dictionary<ZKVerifier.ZKType, byte[]> vkCache)
    {
        string blockId = new DirectoryInfo(blockPath).Name;
        string[] proofFiles = Directory.GetFiles(blockPath, "*.bin");

        Console.WriteLine($"\nProcessing Block #{blockId} ({proofFiles.Length} proofs provided)");

        int validProofs = 0;
        int totalProofs = 0;

        foreach (var proofFile in proofFiles)
        {
            if (ValidateProof(proofFile, vkCache))
            {
                validProofs++;
            }
            totalProofs++;
        }

        Console.WriteLine("   -----------------------------");
        Console.WriteLine((double)validProofs / totalProofs >= 0.5
            ? $"✅ BLOCK #{blockId} ACCEPTED"
            : $"❌ BLOCK #{blockId} REJECTED");
    }

    static bool ValidateProof(string proofFile, Dictionary<ZKVerifier.ZKType, byte[]> vkCache)
    {
        ZKVerifier.ZKType type = ParseTypeFromFilename(proofFile);
        var fileName = Path.GetFileName(proofFile);
        
        if (type == ZKVerifier.ZKType.Unknown || !vkCache.ContainsKey(type))
        {
            Console.WriteLine($"   ⚠️  Skipping unknown or missing VK for: {fileName}");
            return false;
        }

        byte[] proofBytes = File.ReadAllBytes(proofFile);
        byte[] vkBytes = vkCache[type];

        // Rust FFI
        bool isValid = ZKVerifier.Verify((int)type, proofBytes, vkBytes);
        
        Console.WriteLine($"   {fileName,-10} : {(isValid ? "✅ Valid" : "❌ Invalid")}");
        return isValid;
    }

    static ZKVerifier.ZKType ParseTypeFromFilename(string filePath)
    {
        string fileName = Path.GetFileName(filePath).ToLower();

        if (fileName.Contains("zisk")) return ZKVerifier.ZKType.Zisk;
        if (fileName.Contains("openvm")) return ZKVerifier.ZKType.OpenVM;
        if (fileName.Contains("pico")) return ZKVerifier.ZKType.Pico;
        if (fileName.Contains("airbender")) return ZKVerifier.ZKType.Airbender;
        if (fileName.Contains("sp1")) return ZKVerifier.ZKType.Sp1Hypercube;
        // TODO: ZkCloud is an alias for Zisk ???
        if (fileName.Contains("zkcloud")) return ZKVerifier.ZKType.Zisk;

        return ZKVerifier.ZKType.Unknown;
    }
}