use anyhow::Result;

/// Enum representing the type of zkVM verifier
#[repr(u32)]
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum VerifierType {
    Zisk = 0,
    OpenVm = 1,
    Pico = 2,
    Airbender = 3,
    Sp1Hypercube = 4,
}

impl TryFrom<u32> for VerifierType {
    type Error = anyhow::Error;

    fn try_from(value: u32) -> Result<Self> {
        match value {
            0 => Ok(VerifierType::Zisk),
            1 => Ok(VerifierType::OpenVm),
            2 => Ok(VerifierType::Pico),
            3 => Ok(VerifierType::Airbender),
            4 => Ok(VerifierType::Sp1Hypercube),
            _ => Err(anyhow::anyhow!("Unknown verifier type: {}", value)),
        }
    }
}

pub trait Verifier {
    /// Verify a proof given the proof data and verification key
    fn verify(proof: &[u8], vk: &[u8]) -> Result<bool>;
}

pub mod zisk;
pub mod openvm;
pub mod pico;
pub mod sp1_hypercube;
pub mod airbender;
