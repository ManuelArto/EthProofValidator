using System;
using System.IO;
using Wasmtime;

class Program
{
    static void Main(string[] args)
    {
        // 1. Initialize Engine
        using var engine = new Engine();
        using var module = Module.FromFile(engine, "./zisk-wasm-stark-verifier/target/wasm32-unknown-unknown/release/zisk_wasm_stark_verifier.wasm");
        using var linker = new Linker(engine);
        using var store = new Store(engine);

        // 2. Instantiate
        var instance = linker.Instantiate(store, module);

        // 3. Get Exports
        var memory = instance.GetMemory("memory");
        var malloc = instance.GetFunction("alloc");
        var verifyFunc = instance.GetFunction("verify_stark");

        if (memory is null || malloc is null || verifyFunc is null)
        {
            Console.Error.WriteLine("Error: Wasm module is missing required exports.");
            return;
        }

        // Helper: Write bytes to Wasm Memory
        int WriteToWasm(byte[] data)
        {
            // 1. Allocate memory inside Wasm (malloc returns an address)
            var ptrObj = malloc.Invoke(data.Length);
            int ptr = Convert.ToInt32(ptrObj);

            // 2. Write data using GetSpan
            var targetSpan = memory.GetSpan((long)ptr, data.Length);
            data.CopyTo(targetSpan);

            return ptr;
        }

        // 4. Load Data
        var proofPath = "proofs/2643736/zkcloud_884fcc21-d522-4b4a-b535-7cfde199485c_2643736.proof.bin";
        var vkPath = "proofs/2643736/zkcloud_884fcc21-d522-4b4a-b535-7cfde199485c.vk.bin";
        byte[] proofBytes = File.ReadAllBytes(proofPath);
        byte[] vkBytes = File.ReadAllBytes(vkPath);

        try
        {
            int proofPtr = WriteToWasm(proofBytes);
            int vkPtr = WriteToWasm(vkBytes);

            Console.WriteLine($"Calling verify_stark(ptr={proofPtr}, len={proofBytes.Length}, ...)");

            // 5. Invoke verify_stark
            var result = verifyFunc.Invoke(proofPtr, proofBytes.Length, vkPtr, vkBytes.Length);

            Console.WriteLine("verify_stark returned: " + result);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Wasm Execution Failed: {ex.Message}");
        }
    }
}