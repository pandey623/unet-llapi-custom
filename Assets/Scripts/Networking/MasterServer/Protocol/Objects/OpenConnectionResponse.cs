namespace Networking.MasterServer
{
    public class OpenConnectionResponse : MasterServerMessageBase
    {
        public OpenConnectionResponse()
        {
            this.Opcode = (int)MasterServerOpcode.OpenConnectionResponse;
        }
    }
}