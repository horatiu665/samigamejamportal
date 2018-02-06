namespace HhhNetwork
{
    using System;
    using UnityEngine;

    public sealed class PlayerTypeManager : SingletonMonoBehaviour<PlayerTypeManager>
    {
        [SerializeField]
        private PlayerType curPlayerType = PlayerType.Normal;

        [SerializeField]
        private PlayerPrefab[] _playerSetup = new PlayerPrefab[1];

        protected override void Awake()
        {
            if (_playerSetup.Length == 0)
            {
                throw new System.ArgumentNullException("_playerSetup", "An empty PlayerPrefab setup array was provided to the PlayerTypeManager, this is not allowed!");
            }

            base.Awake();
        }

        public TPlayer InstantiatePlayer<TPlayer>(PlayerType type, GameType gameType, Vector3 pos, Quaternion rotation = default(Quaternion), Transform parent = null) where TPlayer : INetPlayer
        {
            return InstantiatePlayer(type, gameType, pos, rotation, parent).GetComponent<TPlayer>();
        }

        public GameObject InstantiatePlayer(PlayerType type, GameType gameType, Vector3 pos, Quaternion rotation, Transform parent)
        {
            for (int i = 0; i < _playerSetup.Length; i++)
            {
                var setup = _playerSetup[i];
                if (setup.type != type)
                {
                    continue;
                }

                GameObject prefab = null;
                switch (gameType)
                {
                case GameType.Local:
                    {
                        prefab = setup.local;
                        break;
                    }

                case GameType.Remote:
                    {
                        prefab = setup.remote;
                        break;
                    }

                case GameType.Server:
                    {
                        prefab = setup.server;
                        break;
                    }
                }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (prefab == null)
                {
                    Debug.LogError(this.ToString() + " is missing a prefab for player type == " + type.ToString() + " for game type == " + gameType.ToString());
                }
#endif

                var go = Instantiate(prefab, pos, rotation, parent);
                go.gameObject.SetActive(true);

                return go;
            }

            return null;
        }

        public PlayerType GetCurrentPlayerType()
        {
            return curPlayerType;
        }

        public void SetPlayerType(PlayerType playerType)
        {
            this.curPlayerType = playerType;
        }

        public void Return(GameObject player)
        {
            Destroy(player, 0.001f);
        }
    }
}