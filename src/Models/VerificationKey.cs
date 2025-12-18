using System.Text.Json.Serialization;

namespace dotnet_zk_verifier.Models
{
    public class VerificationKey
    {
        [JsonPropertyName("cluster_id")]
        public string ClusterId { get; set; } = string.Empty;

        [JsonPropertyName("zkvm")]
        public string ZkVm { get; set; } = string.Empty;

        [JsonPropertyName("vk_path")]
        public string VkPath { get; set; } = string.Empty;

        [JsonPropertyName("vk_binary")]
        public string VkBinary { get; set; } = string.Empty;
    }
}
