use std::mem;
use std::slice;

mod verifiers;
use verifiers::{
    airbender::AirbenderVerifier, openvm::OpenVmVerifier, pico::PicoVerifier,
    sp1_hypercube::Sp1HypercubeVerifier, zisk::ZiskVerifier, Verifier, VerifierType,
};

#[no_mangle]
pub extern "C" fn alloc(len: usize) -> *mut u8 {
    let mut buf = Vec::with_capacity(len);
    let ptr = buf.as_mut_ptr();
    mem::forget(buf);
    ptr
}

#[no_mangle]
pub unsafe extern "C" fn dealloc(ptr: *mut u8, len: usize) {
    let _ = Vec::from_raw_parts(ptr, 0, len);
}

#[no_mangle]
pub extern "C" fn verify(
    zk_type: u32,
    proof_ptr: *const u8,
    proof_len: usize,
    vk_ptr: *const u8,
    vk_len: usize,
) -> i32 {
    let proof = unsafe { slice::from_raw_parts(proof_ptr, proof_len) };
    let vk = unsafe { slice::from_raw_parts(vk_ptr, vk_len) };

    let verifier_type = match VerifierType::try_from(zk_type) {
        Ok(t) => t,
        Err(_) => return -1,
    };

    let result = match verifier_type {
        VerifierType::Zisk => ZiskVerifier::verify(proof, vk),
        VerifierType::OpenVm => OpenVmVerifier::verify(proof, vk),
        VerifierType::Pico => PicoVerifier::verify(proof, vk),
        VerifierType::Airbender => AirbenderVerifier::verify(proof, vk),
        VerifierType::Sp1Hypercube => Sp1HypercubeVerifier::verify(proof, vk),
    };

    match result {
        Ok(true) => 1,
        Ok(false) => 0,
        Err(_) => -1,
    }
}

