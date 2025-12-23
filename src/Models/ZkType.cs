namespace EthProofValidator.src.Models
{
    public enum ZKType
    {
        Zisk = 0,
        OpenVM = 1,
        Pico = 2,
        Airbender = 3,
        Sp1Hypercube = 4,
        Unknown = -1
    }

    public static class ZkTypeMapper
    {
        private static readonly Dictionary<string, ZKType> TypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "zisk", ZKType.Zisk },
            { "openvm", ZKType.OpenVM },
            { "pico", ZKType.Pico },
            { "airbender", ZKType.Airbender },
            { "sp1", ZKType.Sp1Hypercube },
            { "sp1-hypercube", ZKType.Sp1Hypercube },
            { "sp1-turbo", ZKType.Sp1Hypercube },
        };

        public static ZKType Parse(string name)
        {
            return TypeMap.TryGetValue(name, out var type) ? type : ZKType.Unknown;
        }
    }
}