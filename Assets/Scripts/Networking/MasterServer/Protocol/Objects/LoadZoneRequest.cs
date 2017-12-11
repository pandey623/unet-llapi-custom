namespace Networking.MasterServer
{
    public class LoadZoneRequest : MasterServerMessageBase
    { 
        public LoadZoneRequest()
        {
            Opcode = (int)MasterServerOpcode.LoadZoneRequest;
        }

        public string UnitySceneFileName;
    }
}
