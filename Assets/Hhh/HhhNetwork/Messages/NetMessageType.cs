namespace HhhNetwork
{
    /// <summary>
    /// Enum representing all data messages. Byte-based.
    /// Even though the network library is dependency-free, this enum must be filled with every new type of message used in every prototype in the project.
    /// 
    /// Recommended: Do not explicit values to the items, in order to easily transfer systems between projects.
    /// However BEWARE that builds made with multiple reorderings of the enum will not work in multiplayer!!!
    /// </summary>
    public enum NetMessageType : byte
    {
        // 0. STANDARD MESSAGES (required for HhhNetwork)

        /// <summary>
        /// The first message sent from client to server connecting the local client to the server.
        /// </summary>
        LocalConnect = 0,

        /// <summary>
        /// The first message sent from server to client informing the client of his local player setup, and init the world.
        /// </summary>
        LocalStart = 1,

        /// <summary>
        /// Sent from server to clients to inform the clients of remote players joining their game.
        /// </summary>
        RemoteConnect = 2,

        /// <summary>
        /// Sent from server to clients to inform them of remote players leaving the game.
        /// </summary>
        Disconnect = 3,

        // 1. VR UPDATE MESSAGES

        /// <summary>
        /// Client to server VR body update
        /// </summary>
        VRBodyUpdateC2S,

        /// <summary>
        /// Server to clients VR body update (contains 2 extra bytes for the client ID)
        /// </summary>
        VRBodyUpdateS2C,

        // 2. RB Sync Messages

        /// <summary>
        /// Server to clients. Big message with a lot of data about rigidbody syncing. 
        /// </summary>
        RigidbodySyncUpdate,

        /// <summary>
        /// Just debug shit
        /// </summary>
        RigidbodySyncSpawn,

        // 3. Grab messages / spawn messages

        /// <summary>
        /// Client to server and server to clients - tells everyone about grabbing a synced object...
        /// </summary>
        Grab,

        /// <summary>
        /// Client to server and server to clients - tells everyone about throwing an object (coupled with ungrabbing, but it can also mean shooting an arrow or any other rigidbody force operation)
        /// </summary>
        Throw,

        /// <summary>
        /// Request sent by client to server - informing that the client wants to spawn a certain object and grab it
        /// </summary>
        SpawnGrabRequest,

        /// <summary>
        /// Response to the SpawnGrabRequest sent by server to clients - informing that the client is allowed to spawn a certain object and grab it.
        /// </summary>
        SpawnGrabResponse,

        

    }
}