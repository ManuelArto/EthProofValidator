namespace dotnet_zk_verifier
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting zkNethermind");

            var manager = new VerifierManager();
            await manager.InitializeAsync();

            var blockIds = new List<long> { 24040929, 24040930, 24040931, 24040932, 24040933, 24040934, 24040935, 24040936, 24040937, 24040938, 24040939, 24040940, 24040941, 24040942, 24040943, 24040944, 24040945, 24040946, 24040947, 24040948 };

            foreach (var blockId in blockIds)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                await manager.ValidateBlockAsync(blockId);
                sw.Stop();
                Console.WriteLine($"⏱️ Validated in {sw.Elapsed}");
            }
        }
    }
}