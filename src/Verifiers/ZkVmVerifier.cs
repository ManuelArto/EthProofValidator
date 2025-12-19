
using dotnet_zk_verifier.src.Native;

namespace dotnet_zk_verifier.src.Verifiers
{
    public class ZkVmVerifier
    {
        private readonly ZKType _zkType;
        private readonly IntPtr _vkPtr;
        private readonly nuint _vkLen;

        public ZkVmVerifier(ZKType zkType, string vkBinary)
        {
            _zkType = zkType;
            byte[] vkBytes = Convert.FromBase64String(vkBinary);
            _vkLen = (nuint)vkBytes.Length;
            _vkPtr = NativeZKVerifier.CopyToRust(vkBytes);
        }

        public ZKType ZkType => _zkType;

        public bool Verify(byte[] proof)
        {
            return NativeZKVerifier.Verify((int)_zkType, proof, _vkPtr, _vkLen);
        }

        public static ZKType ParseZkType(string zkvmName)
        {
            var name = zkvmName.ToLower();
            if (name.Contains("zisk")) return ZKType.Zisk;
            if (name.Contains("openvm")) return ZKType.OpenVM;
            if (name.Contains("pico")) return ZKType.Pico;
            if (name.Contains("airbender")) return ZKType.Airbender;
            if (name.Contains("sp1")) return ZKType.Sp1Hypercube;

            return ZKType.Unknown;
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
