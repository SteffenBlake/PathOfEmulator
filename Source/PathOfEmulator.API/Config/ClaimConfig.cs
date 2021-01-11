using System.Collections.Generic;

namespace PathOfEmulator.API.Config
{
    public class ClaimConfig
    {
        public string Code { get; set; }
        public List<TokenConfig> Tokens { get; set; } = new List<TokenConfig>();
        public string[] Scopes { get; set; }
    }
}
