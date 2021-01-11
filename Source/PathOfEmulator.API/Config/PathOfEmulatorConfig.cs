using System.Linq;

namespace PathOfEmulator.API.Config
{
    public class PathOfEmulatorConfig
    {
        public string ActiveUser { get; set; }
        public ClientConfig Client { get; set; }

        public OAuthConfig OAuth { get; set; }
        public DataConfig Data { get; set; }

        public UserConfig GetActiveUser() => Data.Users.Single(u => u.Name == ActiveUser);
    }
}