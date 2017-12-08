using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class NetworkServer : MonoBehaviour
    {
        public int Port = 8888;
        public int MaxConnections = 10;
        public int MinSimulatedPing = 40;
        public int MaxSimulatedPing = 400;

        private byte _unreliableChannel;
        private byte _reliableChannel;
        private int _serverHostId = -1;

        #region vars assigned by NetworkReceive 

        private int _outHostId;
        private int _outChannelId;
        private int _outConnectionId;
        private byte[] _buffer;
        private int bufferSize = 1024;
        private int _dataLength;
        private byte _error;
        private NetworkError _networkError;

        #endregion

        private Dictionary<int, IPEndPoint> _connectionDictionary = new Dictionary<int, IPEndPoint>();

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

            // listen for incoming network events
            // NOTE - this will only process 1 incoming network message per frame.
            // (May need to change this in the future, and make it continue to call
            // NetworkTransport.Receive until the NetworkEventType is none.)
            NetworkEventType net = NetworkTransport.Receive(
                out _outHostId,
                out _outConnectionId,
                out _outChannelId,
                _buffer,
                bufferSize,
                out _dataLength,
                out _error
            );

            // return if there were no incoming network events
            if (net == NetworkEventType.Nothing)
                return;

            // cast any error to NetworkError for more information
            _networkError = (NetworkError) _error;

            Debug.Log("NetworkServer received NetworkEventType: " + net + " error = " + _networkError);

            // handle the NetworkEvent - route it to appropriate function
            if (net == NetworkEventType.ConnectEvent)
                OnConnectEvent();
            else if (net == NetworkEventType.DisconnectEvent)
                OnDisconnectEvent();
            else if (net == NetworkEventType.DataEvent)
                OnDataEvent();
            else if (net == NetworkEventType.BroadcastEvent)
                OnBroadcastEvent();
        }

        public void Stop()
        {
            bool ret = NetworkTransport.RemoveHost(_serverHostId);
            if (ret)
            {
                _serverHostId = -1;
            }
            Debug.Log("NetworkServer RemoveHost (stop listening) returned " + ret);
        }

        private void OnConnectEvent()
        {
            string address;
            int port;
            UnityEngine.Networking.Types.NetworkID network;
            UnityEngine.Networking.Types.NodeID dstNode;
            
            NetworkTransport.GetConnectionInfo(_outHostId, _outConnectionId, out address, out port, out network,
                out dstNode, out _error);
            Debug.Log("NetworkServer received ConnectEvent. connectionId = " + _outConnectionId + " address = " +
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

        private void OnDisconnectEvent()
        {
            _connectionDictionary.Remove(_outConnectionId);
            Debug.Log("NetworkServer received DisconnectEvent. connectionId = " + _outConnectionId);
        }
        
        private void OnDataEvent()
        {
            Debug.Log("NetworKServer received DataEvent. connectionId " + _outConnectionId);
        }
        
        private void OnBroadcastEvent()
        {
            
        }

    }
}