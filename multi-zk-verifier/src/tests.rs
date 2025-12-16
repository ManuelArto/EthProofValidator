#[cfg(test)]
mod tests {
    use crate::verifiers::VerifierType;
    use crate::verify;
    use std::path::PathBuf;
    use std::fs;

    #[test]
    fn test_zisk_verification() {
        let proof_path = PathBuf::from("src/test_proofs/zisk_817bbf03-07b4-466d-879b-e476322bd080.bin");
        let vk_path = PathBuf::from("src/verification_keys/zisk_817bbf03-07b4-466d-879b-e476322bd080.bin");

        let proof = fs::read(proof_path).expect("Failed to read proof");
        let vk = fs::read(vk_path).expect("Failed to read vk");

        let result = verify(
            VerifierType::Zisk as u32,
            proof.as_ptr(), proof.len(),
            vk.as_ptr(), vk.len()
        );

        assert_eq!(result, 1, "Zisk verification failed");
    }

    #[test]
    fn test_openvm_verification() {
        let proof_path = PathBuf::from("src/test_proofs/openvm_9b6768c0-831d-488c-ba72-05f93975a3be.bin");
        let vk_path = PathBuf::from("src/verification_keys/openvm_9b6768c0-831d-488c-ba72-05f93975a3be.bin");

        let proof = fs::read(proof_path).expect("Failed to read proof");
        let vk = fs::read(vk_path).expect("Failed to read vk");

        let result = verify(
            VerifierType::OpenVm as u32,
            proof.as_ptr(), proof.len(),
            vk.as_ptr(), vk.len()
        );

        assert_eq!(result, 1, "OpenVm verification failed");
    }

    #[test]
    fn test_invalid_verifier_type() {
        let dummy = [0u8; 1];
        let result = verify(
            999, // Invalid type
            dummy.as_ptr(), 0,
            dummy.as_ptr(), 0
        );

        assert_eq!(result, 0, "Should return 0 for invalid verifier type");
    }
}
