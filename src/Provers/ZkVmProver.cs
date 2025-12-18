namespace dotnet_zk_verifier.Provers
{
    public class ZkVmProver
    {
        private readonly ZKType _zkType;
        private readonly byte[] _vkBytes;

        public ZkVmProver(ZKType zkType, byte[] vkBytes)
        {
            _zkType = zkType;
            _vkBytes = vkBytes;
        }

        public ZKType ZkType => _zkType;

        public bool Verify(byte[] proof)
        {
            return NativeMethods.Verify((int)_zkType, proof, _vkBytes);
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
}
