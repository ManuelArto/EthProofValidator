using System.Net.Http.Json;
using dotnet_zk_verifier.Clients;
using dotnet_zk_verifier.Models;
using dotnet_zk_verifier.Provers;

namespace dotnet_zk_verifier
{
    public class VerifierManager
    {
        private readonly EthProofsApiClient _apiClient;
        private readonly KeyManager _keyManager;

        public VerifierManager()
        {
            _apiClient = new EthProofsApiClient();
            _keyManager = new KeyManager(_apiClient);
        }

        public async Task InitializeAsync()
        {
            await _keyManager.InitializeAsync();
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

            int validCount = results.Count(r => r);
            int totalCount = results.Length;

            Console.WriteLine("   -----------------------------");
            Console.WriteLine((double)validCount / totalCount >= 0.5
                ? $"✅ BLOCK #{blockId} ACCEPTED ({validCount}/{totalCount})"
                : $"❌ BLOCK #{blockId} REJECTED ({validCount}/{totalCount})");
        }

        private async Task<bool> ProcessProofAsync(ProofMetadata proof)
        {
            var prover = _keyManager.GetProver(proof.ClusterId);
            if (prover == null)
            {
                Console.WriteLine($"   ⚠️  Skipping proof {proof.ProofId}: No prover for cluster {proof.ClusterId}");
                return false;
            }

            var proofBytes = await _apiClient.DownloadProofAsync(proof.ProofId);
            if (proofBytes == null) return false;

            try
            {
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
