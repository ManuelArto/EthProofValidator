using System.Net.Http.Json;
using dotnet_zk_verifier.src.Clients;
using dotnet_zk_verifier.src.Models;
using dotnet_zk_verifier.src.Verifiers;

namespace dotnet_zk_verifier.src
{
    public class ZkValidator
    {
        private readonly EthProofsApiClient _apiClient;
        private readonly VerifierRegistry _verifierRegistry;

        public ZkValidator()
        {
            _apiClient = new EthProofsApiClient();
            _verifierRegistry = new VerifierRegistry(_apiClient);
        }

        public async Task InitializeAsync()
        {
            await _verifierRegistry.InitializeAsync();
        }

        public async Task ValidateBlockAsync(long blockId)
        {
            Console.WriteLine($"\nProcessing Block #{blockId}...");
            
            var proofResponse = await _apiClient.GetProofsForBlockAsync(blockId);
            
            if (proofResponse == null || proofResponse.Rows.Count == 0)
            {
                Console.WriteLine("No proofs found for this block.");
                return;
            }

            
            var tasks = proofResponse.Rows.Select(ProcessProofAsync);
            var results = await Task.WhenAll(tasks);

            int validCount = results.Count(r => r == 1);
            int totalCount = results.Count(r => r != -1);

            Console.WriteLine("   -----------------------------");
            Console.WriteLine((double)validCount / totalCount >= 0.5
                ? $"✅ BLOCK #{blockId} ACCEPTED ({validCount}/{totalCount})"
                : $"❌ BLOCK #{blockId} REJECTED ({validCount}/{totalCount})");
        }

        private async Task<int> ProcessProofAsync(ProofMetadata proof)
        {
            if (proof.Status != "proved")
            {
                return -1;
            }

            var verifier = _verifierRegistry.GetVerifier(proof.ClusterId);
            // verifier ??= await _verifierRegistry.TryAddVerifierAsync(proof);
            if (verifier == null)
            {
                var zkType = proof.Cluster.ZkvmVersion.ZkVm.Type;
                Console.WriteLine($"   ⚠️  Skipping proof {proof.ProofId}: No verifier for cluster {zkType}_{proof.ClusterId}");
                return -1;
            }

            var proofBytes = await _apiClient.DownloadProofAsync(proof.ProofId);
            if (proofBytes == null) return -1;

            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                bool isValid = verifier.Verify(proofBytes);
                sw.Stop();
                
                Console.WriteLine($"   Proof {proof.ProofId,-10} : {(isValid ? "✅ Valid" : "❌ Invalid")} ({verifier.ZkType}, {sw.ElapsedMilliseconds} ms)");
                return isValid ? 1 : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error processing proof {proof.ProofId}: {ex.Message}");
                return 0;
            }
        }
    }
}
