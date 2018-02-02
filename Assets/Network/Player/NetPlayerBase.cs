namespace HhhNetwork
{
    using UnityEngine;

    /// <summary>
    /// Base class for all networked VR players.
    /// </summary>
    public abstract class NetPlayerBase : INetPlayer
    {
        [Header("Network")]
        [SerializeField, ReadOnly]
        protected short _netId;

        /// <summary>
        /// Gets the net identifier - used for uniquely identifying players on the network.
        /// </summary>
        /// <value>
        /// The net identifier.
        /// </value>
        public short netId
        {
            get { return _netId; }
        }

        [SerializeField, ReadOnly]
        protected bool _isLocal;
        
        /// <summary>
        /// Gets a value indicating whether this instance is local on the network.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is local; otherwise, <c>false</c>.
        /// </value>
        public bool isLocal
        {
            get { return _isLocal; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is an actual player - to whom we need to send DataMessages, i.e. NOT an AI or bot.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is player; otherwise, <c>false</c>.
        /// </value>
        public virtual bool isPlayer
        {
            get { return true; }
        }

        /// <summary>
        /// Sets the specified net identifier.
        /// </summary>
        /// <param name="netId">The net identifier.</param>
        void INetPlayer.SetNetId(short netId)
        {
            _netId = netId;
        }

        /// <summary>
        /// Sets this player to be local.
        /// </summary>
        void INetPlayer.SetIsLocal()
        {
            _isLocal = true;
        }

        ///// <summary>
        ///// Handles the <see cref="VRBodyUpdateData" /> received over network.
        ///// </summary>
        ///// <param name="data">The VR body data.</param>
        //public abstract void HandleVRBodyUpdate(VRBodyUpdateData data);
        
        //public abstract void HandleOriginShift(Vector3 originShiftDelta);

        /// <summary>
        /// Implements the operator ==.
        /// For <see cref="INetPlayer"/>s, they are considered equal if their net IDs match.
        /// Does some null checks.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(NetPlayerBase a, NetPlayerBase b)
        {
            var oa = (object)a;
            var ob = (object)b;

            if (oa == null && ob == null)
            {
                return true;
            }

            if (oa == null || ob == null)
            {
                return false;
            }

            return a.netId == b.netId;
        }

        /// <summary>
        /// Implements the operator !=.
        /// For <see cref="INetPlayer"/>s, they are considered equal if their net IDs match.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(NetPlayerBase a, NetPlayerBase b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// For <see cref="INetPlayer"/>s, they are considered equal if their net IDs match.
        /// </summary>
        /// <param name="o">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object o)
        {
            var other = o as INetPlayer;
            if (other == null)
            {
                return false;
            }

            return other.netId == _netId;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return _netId.GetHashCode();
        }
    }
}