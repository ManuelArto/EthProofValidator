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

        public bool Verify(byte[] proof)
        {
            return NativeMethods.Verify((int)_zkType, proof, _vkBytes);
        }
    }
}
