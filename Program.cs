using System;
using System.IO;
using Wasmtime;

class Program
{
    static void Main(string[] args)
    {
        // Initialize Engine
        using var engine = new Engine();
        using var module = Module.FromFile(engine, "zisk-wasm-stark-verifier/target/wasm32-unknown-unknown/release/zisk_wasm_stark_verifier.wasm");
        using var linker = new Linker(engine);
        using var store = new Store(engine);

        // Instantiate
        var instance = linker.Instantiate(store, module);

        // Get Exports
        var memory = instance.GetMemory("memory");
        var malloc = instance.GetFunction("alloc");
        var verifyStark = instance.GetFunction("verify_stark");

        if (memory is null || malloc is null || verifyStark is null)
        {
            Console.Error.WriteLine("Error: Wasm module is missing required exports.");
            return;
        }

        // Write bytes to Wasm Memory
        int WriteToWasm(byte[] data)
        {
            // Allocate memory inside Wasm (malloc returns an address)
            var ptrObj = malloc.Invoke(data.Length);
            int ptr = Convert.ToInt32(ptrObj);

            // Write data using GetSpan
            var targetSpan = memory.GetSpan((long)ptr, data.Length);
            data.CopyTo(targetSpan);

            return ptr;
        }

        var proofPath = "proofs/2643736/zkcloud_884fcc21-d522-4b4a-b535-7cfde199485c_2643736.bin";
        var vkPath = "proofs/vks/zkcloud.bin";
        byte[] proofBytes = File.ReadAllBytes(proofPath);
        byte[] vkBytes = File.ReadAllBytes(vkPath);

        try
        {
            int proofPtr = WriteToWasm(proofBytes);
            int vkPtr = WriteToWasm(vkBytes);

            var result = verifyStark.Invoke(proofPtr, proofBytes.Length, vkPtr, vkBytes.Length);

            int resultInt = Convert.ToInt32(result);
            Console.WriteLine(resultInt == 1 ? "Proof is valid." : "Proof is invalid.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Wasm Execution Failed: {ex.Message}");
        }
    }
}