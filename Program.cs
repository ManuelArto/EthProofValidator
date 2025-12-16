using System.Runtime.InteropServices;

class Program
{
    // ---------------------------------------------------------
    // Define the ZK Verifier FFI
    // ---------------------------------------------------------
    
    const string LibName = "./multi-zk-verifier/target/release/libmulti_zk_verifier.so"; 

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr alloc(nuint len);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void dealloc(IntPtr ptr, nuint len);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int verify(int curve_type, IntPtr proof_ptr, nuint proof_len, IntPtr vk_ptr, nuint vk_len);

    public enum ZKType : int
    {
        Zisk = 0,
        OpeVM = 1,
    }

    static void Main(string[] args)
    {
        // var zkType = ZKType.Zisk;
        // var vkPath = "test/vks/zkcloud.bin";
        // var proofPath = "test/blocks/24025347/zkcloud_3117958.bin";

        var zkType = ZKType.OpeVM;
        var vkPath = "test/vks/openVM.bin";
        var proofPath = "test/blocks/24025347/openVM_3117967.bin";
        
        VerifyProof(proofPath, vkPath, (int)zkType);
    }

    private static void VerifyProof(string proofPath, string vkPath, int zkType)
    {
        if (!File.Exists(proofPath) || !File.Exists(vkPath))
        {
            Console.WriteLine("Files not found!");
            return;
        }

        byte[] proofBytes = File.ReadAllBytes(proofPath);
        byte[] vkBytes = File.ReadAllBytes(vkPath);

        IntPtr pProof = IntPtr.Zero;
        IntPtr pVk = IntPtr.Zero;

        try
        {
            pProof = CopyToRust(proofBytes);
            pVk = CopyToRust(vkBytes);

            int result = verify(zkType, pProof, (nuint)proofBytes.Length, pVk, (nuint)vkBytes.Length);

            Console.WriteLine("-----------------------------");
            Console.WriteLine(result == 1 ? "✅ PROOF VALID" : "❌ PROOF INVALID");
            Console.WriteLine("-----------------------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            if (pProof != IntPtr.Zero) dealloc(pProof, (nuint)proofBytes.Length);
            if (pVk != IntPtr.Zero) dealloc(pVk, (nuint)vkBytes.Length);
        }
    }

    // Helper to move C# Byte Array -> Rust Heap
    static IntPtr CopyToRust(byte[] data)
    {
        // Ask Rust to allocate memory
        IntPtr ptr = alloc((nuint)data.Length);

        // Copy C# managed bytes to that unmanaged Rust memory
        Marshal.Copy(data, 0, ptr, data.Length);

        return ptr;
    }
}