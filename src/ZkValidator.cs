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

            
            var tasks = proofResponse.Rows.Select(async proof =>
            {
                var verifier = _verifierRegistry.GetVerifier(proof.ClusterId);
                return await ProcessProofAsync(proof, verifier);
            });
            var results = await Task.WhenAll(tasks);

            int validCount = results.Count(r => r);
            int totalCount = results.Length;

            Console.WriteLine("   -----------------------------");
            Console.WriteLine((double)validCount / totalCount >= 0.5
                ? $"✅ BLOCK #{blockId} ACCEPTED ({validCount}/{totalCount})"
                : $"❌ BLOCK #{blockId} REJECTED ({validCount}/{totalCount})");
        }

        private async Task<bool> ProcessProofAsync(ProofMetadata proof, ZkProofVerifier? verifier)
        {
            if (verifier == null)
            {
                Console.WriteLine($"   ⚠️  Skipping proof {proof.ProofId}: No verifier for cluster {proof.ClusterId}");
                return false;
            }

            var proofBytes = await _apiClient.DownloadProofAsync(proof.ProofId);
            if (proofBytes == null) return false;

            try
            {
                bool isValid = verifier.Verify(proofBytes);
                
                Console.WriteLine($"   Proof {proof.ProofId,-10} : {(isValid ? "✅ Valid" : "❌ Invalid")} ({verifier.ZkType})");
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
