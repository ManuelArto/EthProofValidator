using System.Runtime.InteropServices;
using dotnet_zk_verifier.src.Models;
using dotnet_zk_verifier.src.Native;

namespace dotnet_zk_verifier.src.Verifiers
{
    public class ZkProofVerifier : IDisposable
    {
        private readonly ZKType _zkType;
        private IntPtr _vkPtr;
        private nuint _vkLen;
        private bool _disposed;

        public ZKType ZkType => _zkType;

        public ZkProofVerifier(ZKType zkType, string vkBinary)
        {
            _zkType = zkType;
            AllocateVkMemory(vkBinary);
        }

        public bool Verify(byte[] proof)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var isValid = NativeZKVerifier.verify((int)_zkType, proof, (nuint)proof.Length, _vkPtr, _vkLen);
            return isValid == 1;
        }

        private void AllocateVkMemory(string vkBinary)
        {
            byte[] vkBytes = Convert.FromBase64String(vkBinary);
            _vkLen = (nuint)vkBytes.Length;
            // Allocate unmanaged memory and copy the verification key bytes
            _vkPtr = Marshal.AllocHGlobal(vkBytes.Length);
            Marshal.Copy(vkBytes, 0, _vkPtr, vkBytes.Length);
        }

        // --- Disposal Pattern ---
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (_vkPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_vkPtr);
                _vkPtr = IntPtr.Zero;
            }
            _disposed = true;
        }

        ~ZkProofVerifier() => Dispose(false);
    }
}
