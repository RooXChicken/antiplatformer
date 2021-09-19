using LiteNetLib;
using LiteNetLib.Utils;

namespace antiplatformer
{
    public class networking
    {
        public bool isConnected = false;
        public EventBasedNetListener listener;
        NetManager client = null;
        public bool waiting = true;
        public string recentPacket = "NULL";
        public void init(string ip, int port, string password)
        {
            listener = new EventBasedNetListener();
            client = new NetManager(listener);
            client.Start();
            client.Connect(ip, port, password);

            isConnected = true;

            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                utils.Log("Packet recieved from server!");
                recentPacket = dataReader.GetString();
                dataReader.Recycle();
            };
        }

        public string sendGetString(string message, bool blocking, int milli)
        {
            string packet = "NULL";
            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            client.SendToAll(writer, DeliveryMethod.Sequenced);
            writer.Reset();

            if(blocking)
            {
                SFML.System.Clock timer = new SFML.System.Clock();
                while(waiting)
                {
                    if(timer.ElapsedTime.AsMilliseconds() >= milli)
                    {
                        waiting = false;
                        recentPacket = "Timed out! Is the server running?";
                        gameManager.deltaClock.Restart();
                        gameManager.deltaTime = 0;
                    }
                    client.PollEvents();
                    if(packet != recentPacket)
                    {
                        packet = recentPacket;
                        waiting = false;
                    }
                }
            }
            else
            {
                return recentPacket;
            }
            waiting = true;

            return recentPacket;
        }

        public void disconnect()
        {
            isConnected = false;
            client.DisconnectAll();
        }

        public void update()
        {
            if(isConnected)
            {
                client.PollEvents();
            }
        }

        public void shutdown()
        {
            client.Stop();
        }
    }
}