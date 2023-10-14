using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

namespace Assets.Scripts.NetCode
{
    public class NetworkObjectDespawner
    {
        public static void DespawnNetworkObject(NetworkObject networkObject)
        {
            // if I'm an active on the networking session, tell all clients to remove
            // the instance that owns this NetworkObject
            if (networkObject != null && networkObject.IsSpawned)
                networkObject.Despawn();
        }
    }
}
