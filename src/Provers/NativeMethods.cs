using System.Runtime.InteropServices;

namespace dotnet_zk_verifier.Provers
{
    public static class NativeMethods
    {
        const string LibName = "./multi-zk-verifier/target/release/libmulti_zk_verifier.so";

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr alloc(nuint len);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void dealloc(IntPtr ptr, nuint len);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int verify(int curve_type, IntPtr proof_ptr, nuint proof_len, IntPtr vk_ptr, nuint vk_len);

        public static bool Verify(int zkType, byte[] proofBytes, byte[] vkBytes)
        {
            IntPtr pProof = IntPtr.Zero;
            IntPtr pVk = IntPtr.Zero;

            try
            {
                pProof = CopyToRust(proofBytes);
                pVk = CopyToRust(vkBytes);

                int result = verify(zkType, pProof, (nuint)proofBytes.Length, pVk, (nuint)vkBytes.Length);
                return result == 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FFI Error] {ex.Message}");
                return false;
            }
            finally
            {
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
    }

    public enum ZKType
    {
        Zisk = 0,
        OpenVM = 1,
        Pico = 2,
        Airbender = 3,
        Sp1Hypercube = 4,
        Unknown = -1
    }
}
