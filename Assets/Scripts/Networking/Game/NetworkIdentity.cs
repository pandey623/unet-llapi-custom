using UnityEngine;

namespace Networking
{
    /// <summary>
    /// (custom version inspired by Unity Networking's NetworkIdentity)
    /// 
    /// Put a NetworkIdentity component on prefabs that will be spawned.
    /// A NetworkIdentity is required for every object that is networked with Network
    /// </summary>
    public class NetworkIdentity : MonoBehaviour
    {
        #region editor fields
        /// <summary>
        /// The local player authority setting allows this object to be controlled by the client that owns it - 
        /// the local player object on that client has authority over it. 
        /// 
        /// This is used by other components such as NetworkTransform.
        /// </summary>
        public bool LocalPlayerAuthority;
        
        /// <summary>
        /// Scene objects are treated a bit differently than dynamically instantiated objects. 
        /// These objects are all present in the scene on both client and server. 
        /// However, when building the game all the scene objects with network identities are disabled. 
        /// 
        /// When the client connects to the server the server tells the client which scene objects should be enabled,
        ///     and what their most up to date state information is through spawn messages. 
        /// 
        /// This ensures the client will not contain objects placed at now incorrect locations when they start playing, 
        ///     or even objects which will be deleted immediately on connection because some event removed them before 
        ///     the client connected. 
        /// 
        /// The ServerOnlySceneObject checkbox will ensure this a particular object will not be spawned or enabled on clients.
        /// </summary>
        public bool ServerOnlySceneObject;
        #endregion
        
        /// <summary>
        /// The network ID is the ID of this particular object instance.
        /// There might be multiple objects instantiated of a particular object type and the network ID is used to 
        /// identity which object, for example, a network update should be applied to.
        /// Assigned by the server when spawned.
        /// </summary>
        private int _networkId;
        
        /// <summary>
        /// The asset ID refers to what source asset the object was instantiated from. 
        /// This is used internally when an particular object prefab is spawned over the network.
        /// </summary>
        private string _assetId;

        /// <summary>
        /// The client that has authority for this object. This will be null if no client has authority.
        /// </summary>
        private int _clientAuthorityOwner;

        /// <summary>
        /// The connection associated with this NetworkIdentity. This is only valid for player objects on the server.
        /// </summary>
        private int _connectionToClient;

        /// <summary>
        /// The connection associated with this NetworkIdentity. This is only valid for player objects on a local client.
        /// </summary>
        private int _connectionToServer;

        /// <summary>
        /// True if this object is the authoritative version of the object. So either on a the server, 
        /// or on the client with localPlayerAuthority.
        /// </summary>
        private bool _hasAuthority;

        /// <summary>
        /// True if this object is running on a client. (not sure exactly what this means "running on a client")
        /// </summary>
        private bool _isClient;

        /// <summary>
        /// This returns true if this object is the one that represents the player on the local machine.
        /// </summary>
        private bool _isLocalPlayer;

        /// <summary>
        /// True if this object is running on the server, and has been spawned.
        /// </summary>
        private bool _isServer;

        /// <summary>
        /// True if this object is controlled by the client that owns it - the local player object on that client has authority over it. 
        /// This is used by other components such as NetworkTransform.
        /// </summary>
        private bool _localPlayerAuthority;
    }
}