using Unity.Netcode.Components;
using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// Used for syncing a transform with client side changes. This includes host. Pure server as owner isn't supported by this. Please use NetworkTransform
    /// for transforms that'll always be owned by the server.
    /// </summary>
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        /// <returns>True if server-authoritative, False otherwise.</returns>
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
