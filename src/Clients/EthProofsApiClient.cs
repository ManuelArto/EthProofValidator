using System.Net.Http.Json;
using dotnet_zk_verifier.src.Models;

namespace dotnet_zk_verifier.src.Clients
{
    public class EthProofsApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://ethproofs.org";

        public EthProofsApiClient()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<List<VerificationKey>?> GetActiveKeysAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<VerificationKey>>("/api/v0/verification-keys/active");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Error] Failed to fetch active keys: {ex.Message}");
                return null;
            }
        }

        public async Task<ProofResponse?> GetProofsForBlockAsync(long blockId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProofResponse>($"/api/blocks/{blockId}/proofs?page_index=0&page_size=20&filter_type=multi");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Error] Failed to fetch proofs for block {blockId}: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]?> DownloadProofAsync(long proofId)
        {
            try
            {
                return await _httpClient.GetByteArrayAsync($"/api/proofs/download/{proofId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Error] Failed to download proof {proofId}: {ex.Message}");
                return null;
            }
        }
    }
}
