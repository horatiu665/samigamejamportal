namespace HhhNetwork.RbSync
{
    using HhhPrefabManagement;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class MessageHandlerRbSync : NetMessageHandlerBase
    {
        private HashSet<NetMessageType> _handleTypes = new HashSet<NetMessageType>()
        {
            NetMessageType.LocalConnect, // when server gets LocalConnect message - send world state to player
            NetMessageType.RigidbodySyncSpawn, // spawn 1 rb
            NetMessageType.RigidbodySyncUpdate, // update all rbs (from server)
        };

        public override HashSet<NetMessageType> handleTypes
        {
            get
            {
                return _handleTypes;
            }
        }

        public override void ServerHandleMessageFromClient(NetMessageType messageType, byte[] buffer, short clientPlayerNetId)
        {
            if (messageType == NetMessageType.LocalConnect)
            {
                ServerRbSyncManager.instance.HandleNewPlayer(clientPlayerNetId);
            }
        }

        public override void ClientHandleMessageFromServer(NetMessageType messageType, byte[] buffer)
        {
            if (messageType == NetMessageType.RigidbodySyncSpawn)
            {
                HandleRigidbodySyncSpawn(buffer);
            }
            else if (messageType == NetMessageType.RigidbodySyncUpdate)
            {
                HandleRigidbodySyncUpdate(buffer);
            }
        }

        // only used by debug shit
        private void HandleRigidbodySyncSpawn(byte[] buffer)
        {
            var msg = MessagePool.Get<RigidbodySyncSpawnMessage>(buffer);
            var manager = ClientRbSyncManager.instance;
            if (manager.Has(msg.data.syncId))
            {
                Debug.LogWarning(this.ToString() + " already has a synced rigidbody by id == " + msg.data.syncId.ToString() + ", aborting creation of a new one");
                MessagePool.Return(msg);
                return;
            }

            var data = msg.data;
            // we don't use this for now
            //data.position = RemotePos(data.position);
            NewRigidbodySync(data);
            MessagePool.Return(msg);
        }

        private void HandleRigidbodySyncUpdate(byte[] buffer)
        {
            var msg = MessagePool.Get<RigidbodySyncUpdateMessage>(buffer);
            var manager = ClientRbSyncManager.instance;
            var initial = msg.initial;
            var data = msg.data;
            var count = data.Count;
            for (int i = 0; i < count; i++)
            {
                var d = data[i];
                //d.position = RemotePos(d.position);
                var rb = manager.Get(d.syncId);
                if (rb == null)
                {
                    // if the client does not have the synced rigidbody, create if from the data received
                    rb = NewRigidbodySync(d);
                }

                rb.HandleUpdate(d, initial);
            }

            MessagePool.Return(msg);
        }

        // position already updated to RemotePos(d)
        private RigidbodySyncComponent NewRigidbodySync(RigidbodySyncData data)
        {
            var prefab = PrefabManager.instance.Spawn(data.prefabType, data.position, data.rotation);
            if (prefab == null)
            {
                Debug.LogError(this.ToString() + " NewRigidbodySync missing prefab setup for prefab of type == " + data.prefabType.ToString());
                return null;
            }

            var rb = prefab.gameObject.GetComponent<RigidbodySyncComponent>();
            if (rb == null)
            {
                rb = prefab.gameObject.AddComponent<RigidbodySyncComponent>();
                Debug.LogWarning(this.ToString() + " NewRigidbodySync prefab missing a RigidbodySyncComponent, prefab == " + prefab.ToString() + ", adding the component automatically.");
            }

            rb.Initialize(data.syncId);
            return rb;
        }

    }
}