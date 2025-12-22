# .NET ZK Verifier

This project provides a .NET application that acts as a wrapper for various Zero-Knowledge (ZK) verifiers implemented in Rust. It allows .NET applications to interact with and verify ZK proofs using different underlying ZK proof systems.

## Project Structure

The repository is composed of two main parts:

-   **`dotnet-zk-verifier/`**: The main C# .NET project. This contains the application logic, models, and interfaces for interacting with the ZK verifiers.
-   **`multi-zk-verifier/`**: A Rust project that implements the actual ZK verifier logic for different proof systems (e.g., Airbender, OpenVM, Pico, SP1 Hypercube, Zisk). The .NET application communicates with this Rust library.

## Building and Running

### Prerequisites

-   .NET SDK (e.g., .NET 10)
-   Rust Toolchain (e.g., `rustup`)

### Build Steps

1.  **Build the .NET Application (includes Rust Verifiers)**:
    Navigate to the root directory of the .NET project and build the C# application. This process will automatically build the Rust verifier library and copy the necessary native libraries into the output directory.

    ```bash
    dotnet build
    ```

### Running the Application

After building both components, you can run the .NET application from the root directory:

```bash
dotnet run
```

This will execute the `Program.cs` which should then utilize the compiled Rust verifiers.

