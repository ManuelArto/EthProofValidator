using System.Runtime.InteropServices;

namespace dotnet_zk_verifier.src.Native
{
    internal static class NativeZKVerifier
    {
        const string LibName = "./multi-zk-verifier/target/release/libmulti_zk_verifier.so";

        // Standard P/Invoke: .NET automatically handles the translation of 'byte[]' to a pointer
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int verify(
            int zk_type,
            [In] byte[] proof_ptr, // Pins automatically for the duration of call
            nuint proof_len,
            IntPtr vk_ptr,
            nuint vk_len
        );
    }
}