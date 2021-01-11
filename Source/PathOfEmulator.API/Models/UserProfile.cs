using System;
using PathOfEmulator.API.Config;

namespace PathOfEmulator.API.Models
{
    public class UserProfile
    {
        public string Name { get; set; }
        public Guid UUID { get; set; }
        public string Realm { get; set; }
        public GuildConfig Guild { get; set; }
        public TwitchConfig Twitch { get; set; }

        public static UserProfile FromConfig(UserConfig config)
        {
            return new UserProfile
            {
                Name = config.Name,
                UUID = config.UUID,
                Realm = config.Realm,
                Guild = config.Guild,
                Twitch = config.Twitch
            };
        }

    }
}