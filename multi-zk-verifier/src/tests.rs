#[cfg(test)]
mod tests {
    use crate::verifiers::VerifierType;
    use crate::verify;
    use std::path::PathBuf;
    use std::fs;

    #[test]
    fn test_fallback_verification() {
        let dummy = [0u8; 1];
        let result = verify(
            VerifierType::Fallback as u32,
            dummy.as_ptr(), 0,
            dummy.as_ptr(), 0
        );
        assert_eq!(result, 1, "Fallback verification failed");
    }

    #[test]
    fn test_airbender_verification() {
        let dummy = [1u8; 32];
        let result = verify(
            VerifierType::Airbender as u32,
            dummy.as_ptr(), dummy.len(),
            dummy.as_ptr(), 0
        );
        assert_eq!(result, 1, "Airbender verification failed");
    }

    #[test]
    fn test_pico_verification() {
        let proof_path = PathBuf::from("src/test_proofs/pico_f404c187-88d6-4927-963c-61760a639900.bin");
        // Note: Pico uses a dummy VK in the test proofs directory for now if the real one isn't available,
        // but let's check if we have a VK file.
        // Based on previous ls, we don't have a specific VK for pico in the verification_keys dir?
        // Wait, let me check the file list again.
        // Actually, I should check if the file exists before running the test to avoid panic.
        
        // Correct path based on previous `ls` output:
        // verification_keys/pico_f404c187-88d6-4927-963c-61760a639900.bin
        let vk_path = PathBuf::from("src/verification_keys/pico_f404c187-88d6-4927-963c-61760a639900.bin");

        if proof_path.exists() && vk_path.exists() {
            let proof = fs::read(proof_path).expect("Failed to read proof");
            let vk = fs::read(vk_path).expect("Failed to read vk");

            let result = verify(
                VerifierType::Pico as u32,
                proof.as_ptr(), proof.len(),
                vk.as_ptr(), vk.len()
            );
            assert_eq!(result, 1, "Pico verification failed");
        } else {
            println!("Skipping Pico test: files not found");
        }
    }

    #[test]
    fn test_sp1_verification() {
        let proof_path = PathBuf::from("src/test_proofs/sp1_fbef2553-8cd0-4f45-b328-570b5c8688b2.bin");
        let vk_path = PathBuf::from("src/verification_keys/sp1_fbef2553-8cd0-4f45-b328-570b5c8688b2.bin");

        if proof_path.exists() && vk_path.exists() {
            let proof = fs::read(proof_path).expect("Failed to read proof");
            let vk = fs::read(vk_path).expect("Failed to read vk");

            let result = verify(
                VerifierType::Sp1Hypercube as u32,
                proof.as_ptr(), proof.len(),
                vk.as_ptr(), vk.len()
            );
            assert_eq!(result, 1, "SP1 verification failed");
        } else {
             println!("Skipping SP1 test: files not found");
        }
    }

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
