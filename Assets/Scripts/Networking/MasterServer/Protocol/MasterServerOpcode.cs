namespace Networking.MasterServer
{
    public enum MasterServerOpcode
    {
        /// <summary>
        /// sent periodically from ZoneServer to indicate it's still running
        /// </summary>
        Heartbeat = 0,

        /// <summary>
        /// message sent from Zone Server to the Master server, to establish the initial connection to master.
        /// </summary>
        OpenConnection = 1,

        /// <summary>
        /// message sent from Master server to Zone server, to aknowledge the ZoneServerOpenConnection message.
        /// </summary>
        OpenConnectionResponse = 2,

        /// <summary>
        /// message sent from Master server to Zone server, telling it which zone to load.
        /// </summary>
        LoadZoneRequest = 3,

        /// <summary>
        /// message sent from Zone server to master server, indicating that a zone is loading
        /// </summary>
        ZoneLoading = 4,

        /// <summary>
        /// message sent from Zone server to Master server, telling it a zone was loaded.
        /// </summary>
        ZoneLoaded = 5,
    }
}