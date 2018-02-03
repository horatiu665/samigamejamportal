namespace HhhNetwork
{
    using UnityEngine;

    /// <summary>
    /// Basic interface representing all networked VR players.
    /// </summary>
    public interface INetPlayer
    {
        /// <summary>
        /// Gets the root gameObject of this player (monobeh takes care)
        /// </summary>
        GameObject gameObject { get; }

        /// <summary>
        /// Gets the net identifier - used for uniquely identifying entities on the network.
        /// </summary>
        /// <value>
        /// The net identifier.
        /// </value>
        short netId { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is local on the network.
        /// True for single player and local player.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is local; otherwise, <c>false</c>.
        /// </value>
        bool isLocal { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is an actual player - to whom we need to send DataMessages, i.e. NOT an AI or bot.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is player; otherwise, <c>false</c>.
        /// </value>
        bool isPlayer { get; }

        /// <summary>
        /// Sets the specified net identifier.
        /// </summary>
        /// <param name="netId">The net identifier.</param>
        void SetNetId(short netId);

        /// <summary>
        /// Sets this player to be local.
        /// </summary>
        void SetIsLocal();

        ///// <summary>
        ///// Handles the <see cref="VRBodyUpdateData"/> received over network.
        ///// </summary>
        ///// <param name="data">The VR body data.</param>
        //void HandleVRBodyUpdate(VRBodyUpdateData data);

        ///// <summary>
        ///// Handles the OriginShift, updates lerping to the new origin shifted position.
        ///// </summary>
        ///// <param name="originShiftDelta"></param>
        //void HandleOriginShift(Vector3 originShiftDelta);
    }
}