using System.Runtime.InteropServices;

namespace dotnet_zk_verifier.src.Native
{

    public static class NativeZKVerifier
    {
        const string LibName = "./multi-zk-verifier/target/release/libmulti_zk_verifier.so";

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr alloc(nuint len);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void dealloc(IntPtr ptr, nuint len);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int verify(int zk_type, IntPtr proof_ptr, nuint proof_len, IntPtr vk_ptr, nuint vk_len);

        public static bool Verify(int zkType, byte[] proofBytes, IntPtr pVk, nuint vkLen)
        {
            IntPtr pProof = IntPtr.Zero;

            try
            {
                pProof = CopyToRust(proofBytes);

                int result = verify(zkType, pProof, (nuint)proofBytes.Length, pVk, vkLen);
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
            }
        }

        public static IntPtr CopyToRust(byte[] data)
        {
            IntPtr ptr = alloc((nuint)data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);
            return ptr;
        }
    }
}