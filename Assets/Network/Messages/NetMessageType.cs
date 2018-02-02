namespace HhhNetwork
{
    /// <summary>
    /// Enum representing all data messages. Byte-based.
    /// </summary>
    public enum NetMessageType : byte
    {
        /* Standard Messages */

        /// <summary>
        /// The first message sent from client to server connecting the local client to the server.
        /// </summary>
        LocalConnect = 0,

        /// <summary>
        /// The first message sent from server to client informing the client of his local player setup.
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

        /// <summary>
        /// Sent at a high frequency from clients to server in order to inform the server of the placement of player VR hands and head.
        /// </summary>
        VRBodyInput = 4,

        /// <summary>
        /// Sent at a relatively high frequency from server to clients in order to inform clients of remote players' placement of player VR hands and head.
        /// </summary>
        VRBodyUpdate = 5,

        /// <summary>
        /// Clients to server message signalling that the player wants to spawn an <see cref="INetSpawnable"/>.
        /// </summary>
        SpawnRequest = 6,

        /// <summary>
        /// Server to clients message signalling that a player spawned an <see cref="VRNetwork.INetSpawnable"/>.
        /// </summary>
        SpawnResponse = 7,

        /// <summary>
        /// Clients to server message signalling that the player wants to spawn and equip an <see cref="INetSpawnableEquippable"/>.
        /// </summary>
        SpawnEquipRequest = 8,

        /// <summary>
        /// Server to clients message signalling that a player spawned and equipped an <see cref="VRNetwork.INetSpawnableEquippable"/>.
        /// </summary>
        SpawnEquipResponse = 9,

        /// <summary>
        /// Both-ways message signalling that a player equipped an <see cref="VRNetwork.INetEquippable"/>.
        /// </summary>
        Equip = 10,

        /// <summary>
        /// Both-ways message signalling that a player unequipped an <see cref="VRNetwork.INetEquippable"/>.
        /// </summary>
        Unequip = 11,

        /// <summary>
        /// Both-ways message for syncing player scale.
        /// </summary>
        PlayerScale = 12,

        /// <summary>
        /// Both-ways message informing of a preview change.
        /// </summary>
        ChangePreview = 13,

        /// <summary>
        /// Server to clients message updating a number of equippables.
        /// </summary>
        EquippableSyncUpdate = 14,

        /// <summary>
        /// Server to clients message informing of a newly spawned synced rigidbody.
        /// </summary>
        RigidbodySyncSpawn = 15,

        /// <summary>
        /// Server to clients message updating a number of synced rigidbodies.
        /// </summary>
        RigidbodySyncUpdate = 16,

        /* Game Specific: Utopia */

        /// <summary>
        /// Server to clients message signalling that an impalement happened.
        /// </summary>
        Impalement = 17,

        /// <summary>
        /// Server to clients message signalling that an unimpalement needs to happen.
        /// </summary>
        Unimpalement = 18,

        /// <summary>
        /// Voice Chat
        /// </summary>
        Voip = 19,

        /// <summary>
        /// Server to client message signalling that an <see cref="Core.AI.AISkier"/> was spawned.
        /// </summary>
        AI_Spawned = 20,

        /// <summary>
        /// Server to client message updating an instance of an AI in regards to e.g. position and rotation.
        /// </summary>
        AI_Update = 21,

        /// <summary>
        /// Both-ways message applies force to another player (client send to server: "hit this other player!" server sends the player to be hit "please get hit")
        /// </summary>
        ApplyForceToPlayer = 22,

        /// <summary>
        /// Both-ways mesage signalling that a player started recording on a specific selfie stick.
        /// </summary>
        SelfieStickStartRecording = 23,

        /// <summary>
        /// Both-ways message signalling that a player deleted a recording on a specific selfie stick.
        /// </summary>
        SelfieStickDeleteRecording = 24,

        /// <summary>
        /// Server to clients message informing that a particular equippable has been destroyed (returned to its pool).
        /// </summary>
        DestroyNetEntity = 25,

        /// <summary>
        /// Clients to server message indicating that the player stored an object for the spawn count increase.
        /// </summary>
        PlayerStorage = 26,

        /// <summary>
        /// Server to clients message telling them to spawn a prefab type at a position.
        /// </summary>
        SpawnExplosion = 27,

        /// <summary>
        /// Server to clients telling them to spawn an equippable (with its id) at position.
        /// </summary>
        SpawnEquippable = 28,

        /// <summary>
        /// Both-ways message specifying the direction where the player has moved the origin. all players know of each other's origin shifts.
        /// </summary>
        PlayerOriginShift = 29,

        /// <summary>
        /// Client to server - time sync request with client time and id.
        /// </summary>
        GhettoTimeSyncClient = 30,

        /// <summary>
        /// Server to clients - time sync with client and server time so client can determine the server's time.
        /// </summary>
        GhettoTimeSyncServer = 31,

        /// <summary>
        /// Client to server + server to other clients - player tells server about highscore made after a jump, spawns a highscore pole
        /// </summary>
        HighscorePole = 32,
    }
}