using System;
using System.Collections.Generic;

namespace PathOfEmulator.API.Config
{
    public class UserConfig
    {
        public string Name { get; set; }
        public bool FailsLogin { get; set; }
        public List<ClaimConfig> Claims { get; set; } = new List<ClaimConfig>();
        public Guid UUID { get; set; }
        public string Realm { get; set; }
        public TwitchConfig Twitch { get; set; }
        public GuildConfig Guild { get; set; }
        public CharacterConfig[] Characters { get; set; } = new CharacterConfig[0];
    }
}