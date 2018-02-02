namespace HhhNetwork
{
    using System.Collections.Generic;

    public interface INetPlayerManager
    {
        IDictionary<short, INetPlayer> players { get; }

        /// <summary>
        /// Get connection ID from a netId of a player. this only makes sense for server. netId is used to id players but connection ID can change while a player netID can id a player across sessions.
        /// </summary>
        int GetConnectionId(short netId);
    }
}