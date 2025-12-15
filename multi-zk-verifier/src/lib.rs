use std::slice;
use std::mem;

#[no_mangle]
pub extern "C" fn alloc(len: usize) -> *mut u8 {
    let mut buf = Vec::with_capacity(len);
    let ptr = buf.as_mut_ptr();
    mem::forget(buf); // "Leak" the memory so C# can use it
    ptr
}

#[no_mangle]
pub unsafe extern "C" fn dealloc(ptr: *mut u8, len: usize) {
    let _ = Vec::from_raw_parts(ptr, 0, len);
}

#[no_mangle]
pub extern "C" fn verify(
    proof_ptr: *const u8, proof_len: usize, 
    vk_ptr: *const u8, vk_len: usize
) -> i32 {
    let proof = unsafe { slice::from_raw_parts(proof_ptr, proof_len) };
    let vk = unsafe { slice::from_raw_parts(vk_ptr, vk_len) };

    // ZISK VERIFIER
    match proofman_verifier::verify(proof, vk) {
        true => 1, 
        false => 0,
    }
}