using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HhhNetwork;

public abstract class NetMessageHandlerBase : MonoBehaviour
{
    /// <summary>
    /// Used by NetReceiver to filter the message handlers that will actually handle this type of message, so we don't have to check every time in the HandleMessage() method
    /// Empty means it tries to handle all message types
    /// </summary>
    public abstract HashSet<NetMessageType> handleTypes { get; }

    /// <summary>
    /// Handles messages of the handleTypes type. Further filtering using the messageType param.
    /// This function only gets called on a client.
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="buffer"></param>
    public abstract void HandleMessageFromServer(NetMessageType messageType, byte[] buffer);

    /// <summary>
    /// Handles messages from the client. This only happens on the server.
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="buffer"></param>
    /// <param name="clientPlayerNetId"></param>
    public abstract void HandleMessageFromClient(NetMessageType messageType, byte[] buffer, short clientPlayerNetId);

}
