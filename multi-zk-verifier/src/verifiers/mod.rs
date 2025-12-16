use anyhow::Result;

/// Enum representing the type of zkVM verifier
#[repr(u32)]
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum VerifierType {
    Zisk = 0,
    OpenVm = 1,
}

impl TryFrom<u32> for VerifierType {
    type Error = anyhow::Error;

    fn try_from(value: u32) -> Result<Self> {
        match value {
            0 => Ok(VerifierType::Zisk),
            1 => Ok(VerifierType::OpenVm),
            _ => Err(anyhow::anyhow!("Unknown verifier type: {}", value)),
        }
    }
}

/// Trait for proof verifiers
pub trait Verifier {
    /// Verify a proof given the proof data and verification key
    fn verify(proof: &[u8], vk: &[u8]) -> Result<bool>;
}

pub mod zisk;
pub mod openvm;
