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
            // var blockIds = new List<long> { 24046913, 24046914, 24046915, 24046916, 24046917, 24046918, 24046919, 24046920, 24046921, 24046922, 24046923, 24046924, 24046925, 24046926, 24046927, 24046928, 24046929, 24046930, 24046931, 24046932, 24046933, 24046934, 24046935, 24046936, 24046937, 24046938, 24046939, 24046940, 24046941, 24046942, 24046943, 24046944, 24046945, 24046946, 24046947, 24046948, 24046949, 24046950, 24046951, 24046952, 24046953, 24046954, 24046955, 24046956, 24046957, 24046958, 24046959, 24046960, 24046961 };

            // Include Single-GPU
            var blockIds = new List<long> {
                // 24045000,
                // 24045100,
                // 24045200,
                // 24045300,
                // 24045400,
                // 24045500,
                // 24045600,
                // 24045700,
                // 24045800,
                // 24045900,
                // 24046000,
                // 24046100,
                // 24046200,
                // 24046300,
                // 24046400,
                // 24046500,
                // 24046600,
                24046700,
                24046800,
                24046900,
                24047000,
                24047100,
                24047200,
                24047300,
                24047400,
                24047500,
                24047600,
                24047700
            };

            var durations = new List<long>();
            foreach (var blockId in blockIds)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                await verifier.ValidateBlockAsync(blockId);
                sw.Stop();
                durations.Add(sw.ElapsedMilliseconds);
                Console.WriteLine($"⏱️  VALIDATED in {sw.ElapsedMilliseconds} ms");
            }

            if (durations.Count > 0)
            {
                double average = durations.Average();
                Console.WriteLine($"\nAverage validation time: {average} ms over {durations.Count} blocks.");
            }
        }
    }
}