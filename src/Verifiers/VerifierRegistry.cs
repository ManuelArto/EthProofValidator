using dotnet_zk_verifier.src.Clients;

namespace dotnet_zk_verifier.src.Verifiers
{
    public class VerifierRegistry(EthProofsApiClient apiClient)
    {
        private readonly EthProofsApiClient _apiClient = apiClient;
        private readonly Dictionary<string, ZkProofVerifier> _verifiers = [];

        public async Task InitializeAsync()
        {
            Console.WriteLine("Fetching active verification keys...");
            var keys = await _apiClient.GetActiveKeysAsync();

            if (keys == null)
            {
                Console.WriteLine("No keys found.");
                return;
            }

            foreach (var key in keys)
            {
                if (string.IsNullOrEmpty(key.VkBinary)) continue;

                ZKType zkType = ZkProofVerifier.ParseZkType(key.ZkVm);
                if (zkType != ZKType.Unknown)
                {
                    _verifiers[key.ClusterId] = new ZkProofVerifier(zkType, key.VkBinary);
                    Console.WriteLine($"Initialized Verifier for Cluster: {key.ClusterId} ({zkType})");
                }
            }
        }

        public ZkProofVerifier? GetVerifier(string clusterId)
        {
            return _verifiers.TryGetValue(clusterId, out var verifier) ? verifier : null;
        }
    }
}
