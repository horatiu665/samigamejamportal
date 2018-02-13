namespace HhhNetwork
{
    /// <summary>
    /// OPEN ENUM
    /// This is used over the network to define the set of player prefabs spawned for each player (local, remote, server) per game mode.
    /// Unfortunately it also cannot avoid non-dependency shit. Must be completed by hand (or auto) based on the PlayerTypeManager setup.
    /// </summary>
    public enum PlayerType : byte
    {
        Normal = 0,
        VRDemo,

    }
}