namespace EthProofValidator.src
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Starting Multi-zkVM Verifier...");

            var verifier = new ZkValidator();
            await verifier.InitializeAsync();


            // Include Single-GPU (1:100) test blocks
            // var blockIds = new List<long> { 24046700, 24046800, 24046900, 24047000, 24047100, 24047200, 24047300, 24047400, 24047500, 24047600, 24047700 };

            var latestBlockId = 24068932;
            int N = 25;
            var blockIds = Enumerable.Range((int)(latestBlockId - N), N+1).Select(i => (long)i).ToList();

            foreach (var blockId in blockIds)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                await verifier.ValidateBlockAsync(blockId);
                sw.Stop();
                Console.WriteLine($"⏱️  VALIDATED in {sw.ElapsedMilliseconds} ms");
            }
        }
    }
}