using Unity.Netcode.Components;

namespace Assets.Scripts.NetCode
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}

