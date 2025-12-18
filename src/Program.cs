namespace dotnet_zk_verifier
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting zkNethermind");

            var manager = new VerifierManager();
            await manager.InitializeAsync();

            var blockIds = new List<long> { 24039523, 24039524, 24039525 };

            foreach (var blockId in blockIds)
            {
                await manager.ValidateBlockAsync(blockId);
            }
        }
    }
}