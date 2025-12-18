using System.Net.Http.Json;
using dotnet_zk_verifier.Models;
using dotnet_zk_verifier.Provers;

namespace dotnet_zk_verifier
{
    public class VerifierManager
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, ZkVmProver> _provers = new();
        private const string BaseUrl = "https://ethproofs.org";

        public VerifierManager()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task InitializeAsync()
        {
            Console.WriteLine("Fetching active verification keys...");
            try
            {
                var keys = await _httpClient.GetFromJsonAsync<List<VerificationKey>>("/api/v0/verification-keys/active");
                
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing keys: {ex.Message}");
            }
        }

        public async Task ValidateBlockAsync(long blockId)
        {
            Console.WriteLine($"\nProcessing Block #{blockId}...");
            
            try
            {
                var proofResponse = await _httpClient.GetFromJsonAsync<ProofResponse>($"/api/blocks/{blockId}/proofs?page_index=0&page_size=20&filter_type=multi");
                
                if (proofResponse == null || proofResponse.Rows.Count == 0)
                {
                    Console.WriteLine("No proofs found for this block.");
                    return;
                }

                var tasks = proofResponse.Rows.Select(ProcessProofAsync);
                var results = await Task.WhenAll(tasks);

                int validCount = results.Count(r => r);
                int totalCount = results.Length;

                Console.WriteLine("   -----------------------------");
                Console.WriteLine((double)validCount / totalCount >= 0.5
                    ? $"✅ BLOCK #{blockId} ACCEPTED ({validCount}/{totalCount})"
                    : $"❌ BLOCK #{blockId} REJECTED ({validCount}/{totalCount})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating block {blockId}: {ex.Message}");
            }
        }

        private async Task<bool> ProcessProofAsync(ProofMetadata proof)
        {
            if (!_provers.ContainsKey(proof.ClusterId))
            {
                Console.WriteLine($"   ⚠️  Skipping proof {proof.ProofId}: No prover for cluster {proof.ClusterId}");
                return false;
            }

            ZkVmProver prover = _provers[proof.ClusterId];
            try
            {
                byte[] proofBytes = await _httpClient.GetByteArrayAsync($"/api/proofs/download/{proof.ProofId}");
                bool isValid = prover.Verify(proofBytes);
                
                Console.WriteLine($"   Proof {proof.ProofId,-10} : {(isValid ? "✅ Valid" : "❌ Invalid")} ({prover.ZkType.ToString()})");
                return isValid;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error processing proof {proof.ProofId}: {ex.Message}");
                return false;
            }
        }
    }
}
