using System;
using System.IO;
using System.Net.Sockets;
using Networking.MasterServer;
using UnityEngine;

namespace Networking
{
    public class MasterServerConnection : MonoBehaviour
    {
        public string MasterServerIP = "127.0.0.1";
        public int MasterServerPort = 13000;

        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private StreamWriter _streamWriter;
        private StreamReader _streamReader;
        private bool _tcpReady = false;

        private float _deltaTimeSinceLastHearbeat;
        private int _heartbeatIntervalSeconds = 10;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (!_tcpReady) return;

            Listen();
            UpdateHeartbeat();
        }

        private void UpdateHeartbeat()
        {
            _deltaTimeSinceLastHearbeat += Time.deltaTime;
            if (_deltaTimeSinceLastHearbeat >= _heartbeatIntervalSeconds) 
                SendHeartbeat();
        }

        void OnDestroy()
        {
            if (_tcpReady)
                CloseTcp();
        }

        private void CloseTcp()
        {
            _tcpReady = false;
            _streamReader.Close();
            _streamWriter.Close();
            _tcpClient.Close();
        }

        public void TcpInit()
        {
            try
            {
                _tcpClient = new TcpClient(MasterServerIP, MasterServerPort);
                _stream = _tcpClient.GetStream();
                _streamWriter = new StreamWriter(_stream);
                _streamReader = new StreamReader(_stream);
                _tcpReady = true;
                Debug.Log("MasterServerConnection TcpInit opened TcpClient on " + _tcpClient.Client.LocalEndPoint);
            }
            catch (SocketException e)
            {
                Debug.Log("MasterServerConnection TcpInit SocketException when opening socket to "
                          + MasterServerIP + ":" + MasterServerPort
                          + " : " + e);
                _tcpReady = false;
            }
            catch (Exception e)
            {
                Debug.Log("MasterServerConnection TcpInit Exception " + e);
                _tcpReady = false;
            }
        }

        public void SendHeartbeat()
        {
            if (!_tcpReady)
                return;

            _streamWriter.WriteLine("{Opcode:0}"); // heartbeat
            _streamWriter.Flush();

            _deltaTimeSinceLastHearbeat = 0; // reset timer
        }

        public void Send()
        {
            if (!_tcpReady)
                return;

            _streamWriter.WriteLine("{Opcode:1}"); // OpenConnection
            _streamWriter.Flush();
        }

        public void Listen()
        {
            if (!_tcpReady) return;

            if (_stream.DataAvailable)
            {
                // read one line, should contain a json object
                string json = _streamReader.ReadLine();

                var msgBase = (MasterServerMessageBase)JsonUtility.FromJson(json, typeof(MasterServerMessageBase));

                var opCode = (MasterServerOpcode) msgBase.Opcode;
                Debug.Log("MasterServerConnection received " + opCode);

                // process message
                ProcessMessage(opCode, json);
            }
        }

        private void ProcessMessage(MasterServerOpcode opCode, string json)
        {
            if (opCode == MasterServerOpcode.LoadZoneRequest)
            {
                var loadZoneRequest = (LoadZoneRequest) JsonUtility.FromJson(json, typeof(LoadZoneRequest));

                OnLoadZoneRequest(loadZoneRequest);
            }
        }

        private void OnLoadZoneRequest(LoadZoneRequest loadZoneRequest)
        {
            // create NetworkServer game object
            var networkServerGameObject = new GameObject("NetworkServer");

            // create a NetworkServer component on the game object
            var networkServer = networkServerGameObject.AddComponent<NetworkServer>();

            // configure networkServer
            networkServer.IsAuthoritative = true;

            // start NetworkServer
            networkServer.StartServer();
        }
    }
}