namespace HhhNetwork.RbSync
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class ClientRbSyncManager : RbSyncManager
    {
        public static new ClientRbSyncManager instance
        {
            get
            {
                return RbSyncManager.instance as ClientRbSyncManager;
            }
        }

        [SerializeField]
        private int _preallocation = 500;

        private IDictionary<int, RigidbodySyncComponent> _rigidbodies;

        protected override void Awake()
        {
            base.Awake();
            _rigidbodies = new Dictionary<int, RigidbodySyncComponent>(_preallocation);
        }
        
        protected override void Start()
        {
            base.Start();

            // We expect the network to have initialized in OnEnable, so that we can expect it to be ready in Start.
            if (!NetServices.isClient)
            {
                Debug.LogWarning(this.ToString() + " the ClientRbSyncManager is only meant to be used on the Client! Destroying it now.");
                Destroy(this);
                return;
            }
        }
        
        public RigidbodySyncComponent Get(int syncId)
        {
            return _rigidbodies.GetValueOrDefault(syncId);
        }

        public bool Has(int syncId)
        {
            return _rigidbodies.ContainsKey(syncId);
        }

        public override void Register(RigidbodySyncComponent sync)
        {
            if (!_rigidbodies.ContainsKey(sync.syncId))
            {
                _rigidbodies.Add(sync.syncId, sync);
            }
            else
            {
                // override if someone is trying to register twice. for instance, if we delete some shit in the editor, we want the new shit to take over
                _rigidbodies[sync.syncId] = sync;
            }
        }

        public override void Unregister(RigidbodySyncComponent sync)
        {
            _rigidbodies.Remove(sync.syncId);
        }

    }
}