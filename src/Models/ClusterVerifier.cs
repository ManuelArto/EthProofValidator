using System.Text.Json.Serialization;

namespace dotnet_zk_verifier.src.Models
{
    public class ClusterVerifier
    {
        [JsonPropertyName("cluster_id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("zkvm")]
        public string ZkType { get; set; } = string.Empty;

        [JsonPropertyName("vk_path")]
        public string VkPath { get; set; } = string.Empty;

        [JsonPropertyName("vk_binary")]
        public string VkBinary { get; set; } = string.Empty;
    }
}
