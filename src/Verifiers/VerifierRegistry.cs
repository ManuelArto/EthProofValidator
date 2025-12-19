using dotnet_zk_verifier.src.Clients;
using dotnet_zk_verifier.src.Models;

namespace dotnet_zk_verifier.src.Verifiers
{
    public class VerifierRegistry(EthProofsApiClient apiClient): IDisposable
    {
        private readonly EthProofsApiClient _apiClient = apiClient;
        private readonly Dictionary<string, ZkProofVerifier> _verifiers = [];

        public async Task InitializeAsync()
        {
            Console.WriteLine("Fetching active verification keys...");
            var clusters = await _apiClient.GetActiveKeysAsync();

            if (clusters == null)
            {
                Console.WriteLine("No keys found.");
                return;
            }

            foreach (var cluster in clusters)
            {
                RegisterVerifier(cluster.Id, cluster.ZkType, cluster.VkBinary);
            }
        }

        public async Task<ZkProofVerifier?> TryAddVerifierAsync(ProofMetadata proof)
        {
            var type = proof.Cluster.ZkvmVersion.ZkVm.Type;
            var vkBinary = await _apiClient.GetVerificationKeyBinaryAsync(proof.ProofId);

            this.RegisterVerifier(proof.ClusterId, type, vkBinary);
            return this.GetVerifier(proof.ClusterId);
        }

        public ZkProofVerifier? GetVerifier(string clusterId)
        {
            return _verifiers.TryGetValue(clusterId, out var verifier) ? verifier : null;
        }

        private void RegisterVerifier(string cluster_id, string zkVm, string? vkBinary)
        {
            if (string.IsNullOrEmpty(vkBinary)) return;

            ZKType zkType = ZkTypeMapper.Parse(zkVm);
            if (zkType != ZKType.Unknown)
            {
                _verifiers[cluster_id] = new ZkProofVerifier(zkType, vkBinary);
            }
        }

        public void Dispose()
        {
            foreach (var verifier in _verifiers.Values)
            {
                verifier.Dispose();
            }
            _verifiers.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
