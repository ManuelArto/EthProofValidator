using System.Runtime.InteropServices;

class Program
{
    // ---------------------------------------------------------
    // Define the ZK Verifier FFI
    // ---------------------------------------------------------
    
    const string LibName = "libmulti_zk_verifier.so"; 

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr alloc(nuint len);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void dealloc(IntPtr ptr, nuint len);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int verify(IntPtr proofPtr, nuint proofLen, IntPtr vkPtr, nuint vkLen);


    static void Main(string[] args)
    {
        var proofPath = "proofs/2643736/zkcloud_884fcc21-d522-4b4a-b535-7cfde199485c_2643736.bin";
        var vkPath = "proofs/vks/zkcloud.bin";

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

            int result = verify(pProof, (nuint)proofBytes.Length, pVk, (nuint)vkBytes.Length);

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