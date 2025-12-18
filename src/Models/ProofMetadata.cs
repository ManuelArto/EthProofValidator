using System.Text.Json.Serialization;

namespace dotnet_zk_verifier.Models
{
    public class ProofMetadata
    {
        [JsonPropertyName("proof_id")]
        public long ProofId { get; set; }

        [JsonPropertyName("block_number")]
        public long BlockNumber { get; set; }

        [JsonPropertyName("cluster_id")]
        public string ClusterId { get; set; } = string.Empty;
    }

    public class ProofResponse
    {
        [JsonPropertyName("rows")]
        public List<ProofMetadata> Rows { get; set; } = new();
    }
}
