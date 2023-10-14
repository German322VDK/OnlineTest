using Unity.Netcode.Components;

namespace Assets.Scripts.NetCode
{
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}

