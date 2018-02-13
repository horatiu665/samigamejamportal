namespace HhhNetwork.RbSync
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public abstract class RbSyncManager : SingletonMonoBehaviour<RbSyncManager>
    {
        protected virtual void Start()
        {

        }

        public abstract void Register(RigidbodySyncComponent sync);

        public abstract void Unregister(RigidbodySyncComponent sync);

        public abstract RigidbodySyncComponent Get(int syncId);

    }
}