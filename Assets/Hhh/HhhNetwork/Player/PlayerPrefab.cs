namespace HhhNetwork
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct PlayerPrefab
    {
        public PlayerType type;
        public GameObject local;
        public GameObject remote;
        public GameObject server;
    }
}