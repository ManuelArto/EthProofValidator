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
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                MaxConnectionsPerServer = 20
            };
            _httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<List<ClusterVerifier>?> GetActiveKeysAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ClusterVerifier>>("/api/v0/verification-keys/active");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Error] Failed to fetch active clusters: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GetVerificationKeyBinaryAsync(long proofId)
        {
            try
            {
                var vkBytes = await _httpClient.GetByteArrayAsync($"/api/verification-keys/download/{proofId}");
                return Convert.ToBase64String(vkBytes);
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"[API Error] Failed to download verification key for {proofId}: {ex.Message}");
                return null;
            }
        }

        public async Task<ProofResponse?> GetProofsForBlockAsync(long blockId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProofResponse>($"/api/blocks/{blockId}/proofs?page_size=20");
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
