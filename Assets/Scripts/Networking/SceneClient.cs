using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class SceneClient : MonoBehaviour
    {
        public string SceneServerIP = "127.0.0.1";
        public int SceneServerPort = 8888;
        public int MinSimulatedPing = 20;
        public int MaxSimulatedPing = 500;
    
        private byte _reliableChannel;
        private byte _unreliableChannel;
        private int _hostId = -1;
        private int _connectionId;
    
        // Use this for initialization
        void Start()
        {
            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            _reliableChannel = config.AddChannel(QosType.Reliable);
            _unreliableChannel = config.AddChannel(QosType.Unreliable);
        
            HostTopology topology = new HostTopology(config, 1);
        
#if UNITY_EDITOR
            _hostId = NetworkTransport.AddHostWithSimulator(topology, MinSimulatedPing, MaxSimulatedPing);
#else
            _hostId = NetworkTransport.AddHost(topology);
#endif
        }

        // Update is called once per frame
        void Update()
        {
            if (_hostId == -1)
                return;

            int outConnectionId;
            int outChannelId;
            byte error;
            int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            int receivedSize;
            
            NetworkEventType net = NetworkTransport.ReceiveFromHost(
                _hostId,
                out outConnectionId,
                out outChannelId,
                buffer,
                bufferSize,
                out receivedSize,
                out error);

            if (net == NetworkEventType.Nothing)
                return;
            
            Debug.Log("SceneClient recieved NetworkEventType " + net);
        }

        public void Connect()
        {
            byte error;
            int newConnectionId = NetworkTransport.Connect(_hostId, SceneServerIP, SceneServerPort, 0, out error);
            NetworkError networkError = (NetworkError) error;
            
            if (newConnectionId != 0)
            {
                // connected
                _connectionId = newConnectionId;
                
                Debug.Log("SceneClient connected. connectionId = " + newConnectionId + " error = " + networkError);
            }
            else
            {
                Debug.Log("SceneClient could not connect, error = " + networkError);
            }
        }

        public void Disconnect()
        {
            byte error;
            bool ret = NetworkTransport.Disconnect(_hostId, _connectionId, out error);

            NetworkError networkError = (NetworkError) error;
            Debug.Log("SceneClient Disconnect returned " + ret + " error " + networkError);
        }

        public void SendData()
        {
            int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            byte error;
            
            bool result = NetworkTransport.Send(_hostId, _connectionId, _reliableChannel, buffer, bufferSize, out error);
            NetworkError networkError = (NetworkError) error;
            
            Debug.Log("SceneClient SendData returned " + result + " error = " + networkError);
        }
    }
}