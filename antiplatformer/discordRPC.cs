using DiscordRPC;
using DiscordRPC.Logging;

namespace antiplatformer
{
    public class discordRPC
    {
        public DiscordRpcClient client;

        public void Initialize()
        {
            client = new DiscordRpcClient("745845613723385876");
            client.Initialize();
        }

        //The main loop of your application, or some sort of timer. Literally the Update function in Unity3D
        public void Update(string details, string message)
        {
            //Invoke all the events, such as OnPresenceUpdate
            client.SetPresence(new RichPresence()
            {
                Details = "Singleplayer V" + Game.GAME_VERSION,
                State = message,
                Assets = new Assets()
                {
                    LargeImageKey = "roo_head_large",
                    LargeImageText = "The anti-Platformer"
                }
            });
            client.Invoke();
            utils.Log("Updated discord rich presence");
        }

        public void Deinitialize()
        {
            client.Dispose();
        }

        public discordRPC()
        {

        }
    }
}
