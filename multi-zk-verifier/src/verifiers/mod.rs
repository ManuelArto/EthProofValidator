use anyhow::Result;

#[repr(u32)]
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum VerifierType {
    Zisk = 0,
    OpenVm = 1,
    Pico = 2,
    Airbender = 3,
    Sp1Hypercube = 4,
    ZKCloud = 5,
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
            5 => Ok(VerifierType::ZKCloud),
            _ => Err(anyhow::anyhow!("Unknown verifier type: {}", value)),
        }
    }
}

pub trait Verifier {
    fn verify(proof: &[u8], vk: &[u8]) -> Result<bool>;
}

pub mod zisk;
pub mod openvm;
pub mod pico;
pub mod sp1_hypercube;
pub mod airbender;
