use super::Verifier;
use anyhow::Result;
use sp1_verifier::compressed::SP1CompressedVerifierRaw;

pub struct Sp1HypercubeVerifier;

impl Verifier for Sp1HypercubeVerifier {
    fn verify(proof: &[u8], vk: &[u8]) -> Result<bool> {
        // Call the sp1-verifier verify function via SP1CompressedVerifierRaw
        match SP1CompressedVerifierRaw::verify(proof, vk) {
            Ok(()) => Ok(true),
            Err(e) => {
                println!("SP1-Hypercube verification failed: {:?}", e);
                Ok(false)
            }
        }
    }
}
