namespace HhhNetwork.RbSync
{
    using HhhNetwork.Server;
    using HhhPrefabManagement;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Networking;
    using Random = UnityEngine.Random;

    public class ServerRbSyncManager : RbSyncManager
    {
        public static new ServerRbSyncManager instance
        {
            get
            {
                return RbSyncManager.instance as ServerRbSyncManager;
            }
        }

        [SerializeField, Range(0.1f, 90f), Tooltip("How many times per second the rigidbody sync manager may send sync package updates over the network.")]
        private float _sendRate = 10f;

        [SerializeField]
        // 1400 bytes / 40 bytes = 35. 4 is the max byte size of a PrefabType field, so 40 bytes is the top size of an Update message. so we go with 35 max rigidbodies per message.
        private int _maxRigidbodiesPerMessage = 35;

        /// <summary>
        /// Funky UnityNet LLAPI byte size limit found thru trial and error..? 1471 sounds like grandma's pin code
        /// </summary>
        [SerializeField, Range(30, 1471)]
        private int _maxMessageSize = 1400;

        [SerializeField]
        private int _listPreallocation = 500;

        private readonly RigidbodySyncUpdateMessage _updateMsg = new RigidbodySyncUpdateMessage();

        // only used by debug shit
        private readonly RigidbodySyncSpawnMessage _spawnMsg = new RigidbodySyncSpawnMessage();

        /// <summary>
        /// Hashset of IDs in case some are destroyed... or some are not spawned in order... or something else is wrong.
        /// </summary>
        private readonly Dictionary<int, RigidbodySyncComponent> _syncIds = new Dictionary<int, RigidbodySyncComponent>();

        /// <summary>
        /// The list of rigidbodies. Will be parsed in order and looped so the update messages are sent per N amount of rigidbodies, a certain amount per frame... Messages are split because of unity's surreal message size limit
        /// </summary>
        private IList<RigidbodySyncComponent> _rigidbodies;
        private RigidbodySyncData[] _syncs;
        private int _maxSyncsPerMessage;
        private int _nextSyncId = 1;

        // used by the send timer
        private float _lastSend;

        /// <summary>
        /// Debug function.
        /// </summary>
        private void PrintRbs()
        {
            var p = "ServerNetSender.instance.players.Count  +" + ServerNetSender.instance.players.Count + "|\n";
            p += _rigidbodies.Count + " rigidbodies and " + _syncIds.Count + " sync ids";
            for (int i = 0; i < _rigidbodies.Count; i++)
            {
                p += "\n" + _rigidbodies[i].name;
            }
            Debug.Log(p);
        }

        protected override void Awake()
        {
            base.Awake();
            _rigidbodies = new List<RigidbodySyncComponent>(_listPreallocation);

            // Each rigidbody sync takes up 18 bytes, messages in Unity cannot be larger than 1472 bytes (leave a small buffer)
            _maxSyncsPerMessage = Mathf.Min(_maxRigidbodiesPerMessage, _maxMessageSize / RigidbodySyncUpdateMessage.bytesPerData);
            _syncs = new RigidbodySyncData[_maxSyncsPerMessage];
        }
        
        protected override void Start()
        {
            base.Start();

            // We expect the network to have initialized in OnEnable, so that we can expect it to be ready in Start.
            if (!NetServices.isServer)
            {
                Debug.LogWarning(this.ToString() + " the ServerRbSyncManager is only meant to be used on the Server! Destroying it now.");
                Destroy(this);
                return;
            }
        }

        private void Update()
        {
            var time = Time.timeSinceLevelLoad;
            if (time < _lastSend)
            {
                return;
            }

            if (ServerNetSender.instance.players.Count == 0)
            {
                return;
            }

            _lastSend = time + (1f / _sendRate);
            OnSend();
        }

        private void OnSend()
        {
            // for all rbs
            var count = _rigidbodies.Count;
            if (count == 0)
            {
                return;
            }

            _syncs.Clear();

            // sync id - used for the looping thing.
            var idx = 0;
            for (int i = count - 1; i >= 0; i--)
            {
                var rb = _rigidbodies[i];
                if (rb == null || rb.Equals(null))
                {
                    // rigidbody was most likely destroyed
                    _rigidbodies.RemoveAt(i);
                    continue;
                }

                if (!rb.changed)
                {
                    // only update the rigidbodies that have actually changed
                    continue;
                }

                // sets rb.changed to false by setting the last changed values of all synced vars.
                _syncs[idx++] = rb.GetSyncData();

                if (idx >= _maxSyncsPerMessage)
                {
                    // if we cannot send any more syncs in one message, we stop adding more and instead continue sending in the next frame (rather than on next interval tick)
                    // consider sending more messages in one frame, in case too many rigidbodies being sent clogs the network.
                    _lastSend = 0f;
                    break;
                }
            }

            if (idx == 0)
            {
                // no rigidbodies changed this time
                return;
            }

            _updateMsg.initial = false;
            _updateMsg.SetData(_syncs);
            ServerNetSender.instance.SendToAll(_updateMsg, QosType.UnreliableSequenced);
        }

        public int GetNextSyncId()
        {
            while (_syncIds.ContainsKey(++_nextSyncId))
            {
                if (_nextSyncId >= int.MaxValue)
                {
                    _nextSyncId = 0;
                }
            }

            return _nextSyncId;
        }

        public override void Register(RigidbodySyncComponent rbSync)
        {
            if (rbSync.syncId > 0 && !_syncIds.ContainsKey(rbSync.syncId))
            {
                _syncIds.Add(rbSync.syncId, rbSync);
                _rigidbodies.Add(rbSync);
            }
            else
            {
                var id = GetNextSyncId();
                Debug.LogWarning(this.ToString() + " a RigidbodySyncComponent (" + rbSync.ToString() + ") with an already taken or invalid ID (" + rbSync.syncId.ToString() + ") is attempting to register, assigning a new ID == " + id.ToString());

                _syncIds[rbSync.syncId] = rbSync;
                rbSync.SetSyncId(id);
                _rigidbodies.Add(rbSync);
            }

        }

        public override void Unregister(RigidbodySyncComponent rbSync)
        {
            _syncIds.Remove(rbSync.syncId);
            _rigidbodies.Remove(rbSync);
        }

        public override RigidbodySyncComponent Get(int syncId)
        {
            return _syncIds.GetValueOrDefault(syncId);
        }
        
        /// <summary>
        /// Sends new player data about all current rbs
        /// </summary>
        public void HandleNewPlayer(short netId)
        {
            var count = _rigidbodies.Count;
            if (count == 0)
            {
                return;
            }

            var serverNetSender = ServerNetSender.instance;
            _syncs.Clear();
            var idx = 0;

            _updateMsg.initial = true;

            if (count < _maxSyncsPerMessage)
            {
                // there are not more rigidbodies than what can be sent in one message
                for (int i = 0; i < count; i++)
                {
                    var rb = _rigidbodies[i];
                    if (rb == null || rb.Equals(null))
                    {
                        continue;
                    }

                    _syncs[idx++] = rb.GetSyncData();
                }

                _updateMsg.SetData(_syncs);
                serverNetSender.Send(netId, _updateMsg, QosType.Reliable);
                return;
            }

            // there are more rigidbodies than what can be contained in one message
            var messageCount = Mathf.CeilToInt((float)count / _maxSyncsPerMessage);
            for (int i = 0; i < messageCount; i++)
            {
                var step = _maxSyncsPerMessage * i;
                var end = step + Mathf.Min(_maxSyncsPerMessage, count - step);
                for (int j = step; j < end; j++)
                {
                    var rb = _rigidbodies[j];
                    if (rb == null || rb.Equals(null))
                    {
                        continue;
                    }

                    _syncs[idx++] = rb.GetSyncData();
                }

                idx = 0;
                _updateMsg.SetData(_syncs);
                serverNetSender.Send(netId, _updateMsg, QosType.Reliable);
                _syncs.Clear();
            }
        }

        // only used by debug shit
        public RigidbodySyncComponent HandleRigidbodySyncSpawn(PrefabType prefabType, Vector3 position, Quaternion rotation)
        {
            var prefab = PrefabManager.instance.Spawn(prefabType, this.transform.position, this.transform.rotation);
            if (prefab == null)
            {
                Debug.LogError(this.ToString() + " missing prefab for VRPrefabType == " + prefabType.ToString());
                return null;
            }

            return HandleRigidbodySyncSpawn(prefab);
        }

        // only used by debug shit
        private RigidbodySyncComponent HandleRigidbodySyncSpawn(IPrefabComponent prefab)
        {
            var rbSync = prefab.gameObject.GetComponent<RigidbodySyncComponent>();
            if (rbSync == null)
            {
                Debug.LogError(this.ToString() + " supplid prefab (" + prefab.ToString() + ") is missing a RigidbodySyncComponent!");
                return null;
            }

            return HandleRigidbodySyncSpawn(rbSync, prefab.prefabType);
        }

        // only used by debug shit
        private RigidbodySyncComponent HandleRigidbodySyncSpawn(RigidbodySyncComponent rbSync, PrefabType prefabType)
        {
            if (rbSync.syncId <= 0)
            {
                rbSync.SetSyncId(GetNextSyncId());
            }

            rbSync.Initialize();
            _spawnMsg.data = rbSync.GetSyncData();
            ServerNetSender.instance.SendToAll(_spawnMsg, QosType.Reliable);
            return rbSync;
        }

    }
}