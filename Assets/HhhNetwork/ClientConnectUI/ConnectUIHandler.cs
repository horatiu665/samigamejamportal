namespace HhhNetwork
{
    using System.Net;
    using UnityEngine;
    using UnityEngine.UI;
    using Client;

    public sealed class ConnectUIHandler : SingletonMonoBehaviour<ConnectUIHandler>
    {
        private const string _lastClientConnectIpKey = "LastClientConnectIp";
        private const string _lastClientConnectPortKey = "LastClientConnectPort";
        private const string _localHostToggleName = "Localhost";
        private const string _serverHostToggleName = "Server";

        private int lastServerPort = 8080;

        [Header("References Are Auto-Found In Children If Not Set:")]
        [SerializeField]
        private InputField _input = null;

        [SerializeField]
        private Button _connectBtn = null;

        [SerializeField]
        private ToggleGroup _toggles = null;

        protected override void Awake()
        {
            base.Awake();

            _input = _input ?? this.GetComponentInChildren<InputField>(true);
            _connectBtn = _connectBtn ?? this.GetComponentInChildren<Button>(true);
            _toggles = _toggles ?? this.GetComponentInChildren<ToggleGroup>(true);
            _toggles.allowSwitchOff = true;
        }

        private void OnEnable()
        {
            _connectBtn.onClick.AddListener(OnConnectClick);

            var toggles = _toggles.GetComponentsInChildren<Toggle>(true);
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].isOn = false;
                toggles[i].onValueChanged.AddListener(OnToggleChange);
            }

            if (PlayerPrefs.HasKey(_lastClientConnectIpKey))
            {
                _input.text = PlayerPrefs.GetString(_lastClientConnectIpKey) + ":" + PlayerPrefs.GetString(_lastClientConnectPortKey, lastServerPort.ToString());
            }
        }

        private void OnToggleChange(bool isSelected)
        {
            _input.interactable = !_toggles.AnyTogglesOn();
            if (!_input.interactable)
            {
                foreach (var toggle in _toggles.ActiveToggles())
                {
                    if (toggle.name == _serverHostToggleName)
                    {
                        _input.text = BuildConstants.hetznerIp + ":" + lastServerPort;
                        break;
                    }
                    else if (toggle.name == _localHostToggleName)
                    {
                        _input.text = BuildConstants.localhost + ":" + lastServerPort;
                        break;
                    }
                }
            }
        }

        private void OnConnectClick()
        {
            var ip = _input.text.Substring(0, _input.text.IndexOf(":"));
            var portt = _input.text.Contains(":") ? _input.text.Substring(_input.text.IndexOf(":") + 1) : lastServerPort.ToString();
            var port = lastServerPort;
            if (int.TryParse(portt, out port))
            {
                lastServerPort = port;
            }
            if (!string.IsNullOrEmpty(ip))
            {
                IPAddress addr;
                if (IPAddress.TryParse(ip, out addr))
                {
                    PlayerPrefs.SetString(_lastClientConnectIpKey, ip);
                    PlayerPrefs.SetString(_lastClientConnectPortKey, portt);

                    ClientNetSender.instance.Connect(ip, lastServerPort);

                    _connectBtn.interactable = false;
                }
                else
                {
                    // show text on this interface instead.... would be nice ;)
                    //if (OnScreenTextUIHandler.instance != null)
                    //{
                    //    OnScreenTextUIHandler.instance.ShowText("Invalid IP entered!", Color.red);
                    //}

                    Debug.LogWarning(this.ToString() + " could not parse the given IP, IP is invalid!");
                }
            }
            else
            {
                Debug.LogWarning(this.ToString() + " cannot connect to an empty string!");
            }
        }

        public void Disable()
        {
            this.gameObject.SetActive(false);
        }
    }
}