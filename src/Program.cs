namespace dotnet_zk_verifier.src
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Starting Multi-zkVM Verifier...");

            var verifier = new ZkValidator();
            await verifier.InitializeAsync();

            // var blockIds = new List<long> { 24040929, 24040930 };
            var blockIds = new List<long> { 24040929, 24040930, 24040931, 24040932, 24040933, 24040934, 24040935, 24040936, 24040937, 24040938, 24040939, 24040940, 24040941, 24040942, 24040943, 24040944, 24040945, 24040946, 24040947, 24040948 };

            var durations = new List<long>();
            foreach (var blockId in blockIds)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                await verifier.ValidateBlockAsync(blockId);
                sw.Stop();
                durations.Add(sw.ElapsedMilliseconds);
                Console.WriteLine($"⏱️ Validated in {sw.ElapsedMilliseconds} ms");
            }

            if (durations.Count > 0)
            {
                double average = durations.Average();
                Console.WriteLine($"\nAverage validation time: {average} ms over {durations.Count} blocks.");
            }
        }
    }
}