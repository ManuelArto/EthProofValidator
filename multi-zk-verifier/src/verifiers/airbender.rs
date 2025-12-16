use super::Verifier;
use anyhow::Result;

pub struct AirbenderVerifier;

impl Verifier for AirbenderVerifier {
    fn verify(proof: &[u8], _vk: &[u8]) -> Result<bool> {
        // Validate proof data is not empty
        if proof.is_empty() {
            return Ok(false);
        }

        // Placeholder: always return true for valid proof data
        Ok(true)
    }
}
