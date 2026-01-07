using System.Net.Http.Json;
using EthProofValidator.src.Clients;
using EthProofValidator.src.Models;
using EthProofValidator.src.Verifiers;

namespace EthProofValidator.src
{
    public class BlockValidator
    {
        private readonly EthProofsApiClient _apiClient;
        private readonly VerifierRegistry _registry;

        public BlockValidator()
        {
            _apiClient = new EthProofsApiClient();
            _registry = new VerifierRegistry(_apiClient);
        }

        public async Task InitializeAsync() => await _registry.InitializeAsync();

        public async Task ValidateBlockAsync(long blockId)
        {
            Console.WriteLine($"\n📦 Processing Block #{blockId}");
            
            var proofs = await _apiClient.GetProofsForBlockAsync(blockId);
            if (proofs == null || proofs.Count == 0)
            {
                Console.WriteLine("No proofs found.");
                return;
            }

            var tasks = proofs.Select(async proof =>
            {
                var verifier = _registry.GetVerifier(proof.ClusterId) ?? await _registry.TryAddVerifierAsync(proof);
                return await ProcessProofAsync(proof, verifier);
            });
            var results = await Task.WhenAll(tasks);

            int validCount = results.Count(r => r == 1);
            int totalCount = results.Count(r => r != -1); // Exclude skipped proofs

            Console.WriteLine("   -----------------------------");
            Console.WriteLine((double)validCount / totalCount >= 0.5
                ? $"✅ BLOCK #{blockId} ACCEPTED ({validCount}/{totalCount})"
                : $"❌ BLOCK #{blockId} REJECTED ({validCount}/{totalCount})");
        }

        private async Task<int> ProcessProofAsync(ProofMetadata proof, ZkProofVerifier? verifier)
        {
            if (verifier == null)
            {
                var zkType = proof.Cluster.ZkvmVersion.ZkVm.Type;
                Console.WriteLine($"   Proof {proof.ProofId} - {zkType, -15} : ⚠️  Skipped (No verifier for cluster {proof.ClusterId})");
                return -1;
            }

            var proofBytes = await _apiClient.DownloadProofAsync(proof.ProofId);
            if (proofBytes == null) return -1;

            var sw = System.Diagnostics.Stopwatch.StartNew();
            int result = verifier.Verify(proofBytes);
            sw.Stop();
            
            string status = result switch
            {
                1 => "✅ Valid",
                0 => "❌ Invalid",
                -1 => "❌ Failed",
            };
            Console.WriteLine($"   Proof {proof.ProofId} - {verifier.ZkType, -15} : {status} ({sw.ElapsedMilliseconds} ms)");
            return result;
        }
    }
}
