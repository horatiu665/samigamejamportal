namespace HhhNetwork.Client
{
    using UnityEngine;

    /// <summary>
    /// Receives network events and handles them for a client.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="VRNetwork.INetReceiver{VRNetwork.ClientNetReceiver}" />
    /// <seealso cref="VRNetwork.ClientNetReceiver"/>
    [RequireComponent(typeof(ClientNetSender))]
    public abstract class ClientNetReceiverBase<T> : NetReceiverBase<T, ClientNetSender> where T : ClientNetReceiverBase<T>
    {
    }
}