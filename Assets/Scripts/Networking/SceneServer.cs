using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class SceneServer : MonoBehaviour
    {
        public int Port = 8888;
        public int MaxConnections = 10;
        public int MinSimulatedPing = 200;
        public int MaxSimulatedPing = 400;

        private byte _unreliableChannel;
        private byte _reliableChannel;
        private int _serverHostId = -1;

        #region vars for update 

        private int _outHostId;
        private int _outChannelId;
        private int _outConnectionId;
        private byte[] _buffer;
        private int bufferSize = 1024;
        private int _dataLength;
        private byte _error;
        private Dictionary<int, IPEndPoint> _connectionDictionary 
            = new Dictionary<int, IPEndPoint>();
        #endregion

        // Use this for initialization
        void Start()
        {
            NetworkTransport.Init();
        }

        public void StartServer()
        {
            
            ConnectionConfig config = new ConnectionConfig();
            _reliableChannel = config.AddChannel(QosType.Reliable);
            _unreliableChannel = config.AddChannel(QosType.Unreliable);

            HostTopology topology = new HostTopology(config, MaxConnections);
#if UNITY_EDITOR
            _serverHostId = NetworkTransport.AddHostWithSimulator(topology, MinSimulatedPing, MaxSimulatedPing, Port);
            Debug.Log("SceneServer started (editor)");
#else
			_serverHostId = NetworkTransport.AddHost(topology, Port);
            Debug.Log("SceneServer started");
#endif
        }
        
        // Update is called once per frame
        void Update()
        {
            NetworkReceive();
        }

        private void NetworkReceive()
        {
            if (_serverHostId == -1)
                return;

            _buffer = new byte[bufferSize];

            NetworkEventType net =
                NetworkTransport.Receive(
                    out _outHostId,
                    out _outConnectionId,
                    out _outChannelId,
                    _buffer,
                    bufferSize,
                    out _dataLength,
                    out _error);

            if (net == NetworkEventType.Nothing)
                return;

            NetworkError networkError = (NetworkError) _error;

            Debug.Log("SceneServer received NetworkEventType: " + net + " error = " + networkError);

            if (net == NetworkEventType.ConnectEvent)
            {
                string address;
                int port;
                UnityEngine.Networking.Types.NetworkID network;
                UnityEngine.Networking.Types.NodeID dstNode;
                NetworkTransport.GetConnectionInfo(_outHostId, _outConnectionId, out address, out port, out network,
                    out dstNode, out _error);
                Debug.Log("SceneServer received ConnectEvent. connectionId = " + _outConnectionId + " address = " +
                          address);

                if (!_connectionDictionary.ContainsKey(_outConnectionId))
                {
                    _connectionDictionary.Add(_outConnectionId, new IPEndPoint(IPAddress.Parse(address), port));
                }
                else
                {
                    Debug.Log("_connectionDictionary already ContainsKey for connectionId " + _outConnectionId);
                }
            }

            else if (net == NetworkEventType.DisconnectEvent)
            {
                _connectionDictionary.Remove(_outConnectionId);
                Debug.Log("SceneServer received DisconnectEvent. connectionId = " + _outConnectionId);
            }

            else if (net == NetworkEventType.DataEvent)
            {
                Debug.Log("SceneServer received DataEvent. connectionId " + _outConnectionId);
            }
        }

        public void Stop()
        {
            bool ret = NetworkTransport.RemoveHost(_serverHostId);
            if (ret)
            {
                _serverHostId = -1;
            }
            Debug.Log("SceneServer RemoveHost (stop listening) returned " + ret);
        }
    }
}