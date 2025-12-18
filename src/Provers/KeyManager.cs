using dotnet_zk_verifier.Clients;
using dotnet_zk_verifier.Models;

namespace dotnet_zk_verifier.Provers
{
    public class KeyManager
    {
        private readonly EthProofsApiClient _apiClient;
        private readonly Dictionary<string, ZkVmProver> _provers = new();

        public KeyManager(EthProofsApiClient apiClient)
        {
            _apiClient = apiClient;
        }

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

                byte[] vkBytes = Convert.FromBase64String(key.VkBinary);
                ZKType zkType = ZkVmProver.ParseZkType(key.ZkVm);

                if (zkType != ZKType.Unknown)
                {
                    _provers[key.ClusterId] = new ZkVmProver(zkType, vkBytes);
                    Console.WriteLine($"Initialized Prover for Cluster: {key.ClusterId} ({zkType})");
                }
            }
        }

        public ZkVmProver? GetProver(string clusterId)
        {
            return _provers.TryGetValue(clusterId, out var prover) ? prover : null;
        }
    }
}
