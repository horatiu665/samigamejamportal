namespace HhhNetwork
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// This class documents and provides examples for some things in HhhNetwork.
    /// </summary>
    public class Docs
    {
        /// How to use NetPlayerBase
        void HowToUseNetPlayerBase(NetPlayerBase player)
        {
            // given a reference to a player, received from the player manager or whatever

            // get references to other scripts, do not extend the class. this way, keep dependencies to a minimum.
            var r = player.GetComponent<Rigidbody>();
            r.AddForce(0, 1, 0);

        }

    }
}