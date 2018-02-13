namespace HhhNetwork.VR
{
    using UnityEngine;

    /// <summary>
    /// Component for server players and remote players, which updates positions and rotations of head and hands, as well as global position.
    /// also sets velocity even though it doesn't affect the position - for the simulation of the children RBs
    /// </summary>
    public class VRBodyUpdateSync : MonoBehaviour
    {
        [SerializeField]
        private NetPlayerBase _player;
        public NetPlayerBase player
        {
            get
            {
                if (_player == null)
                {
                    _player = GetComponent<NetPlayerBase>();
                }
                return _player;
            }
        }

        [SerializeField]
        private VRPlayerComponent _vrPlayer;
        public VRPlayerComponent vrPlayer
        {
            get
            {
                if (_vrPlayer == null)
                {
                    _vrPlayer = GetComponent<VRPlayerComponent>();
                }
                return _vrPlayer;
            }
        }

        [Header("Sync params")]
        [SerializeField, Range(0, 1f)]
        private float _lerpParam = 0.5f;

#if UNITY_EDITOR

        [SerializeField, Tooltip("Whether to use Gizmos for drawing the last received position updates for player, hands and head.")]
        private bool _debugShowTargets = false;

        [SerializeField]
        private Color _gizmoColor = Color.magenta;

#endif

        private Vector3 _targetPos, _targetHeadPos, _targetLHandPos, _targetRHandPos, _lastPos;
        private Quaternion _targetHeadRot, _targetLHandRot, _targetRHandRot;
        private float _lastUpdate, _lastTimeSpan = 1f;

        private void OnValidate()
        {
            if (player != null)
            {
            }
            if (vrPlayer != null)
            {
            }
        }

        protected virtual void OnEnable()
        {
            if (player != null)
            {
                _lastPos = _targetPos = player.gameObject.transform.position;
                _lastUpdate = Time.timeSinceLevelLoad;
            }
        }

        protected virtual void FixedUpdate()
        {
            if (player == null || _lastTimeSpan == 0f || vrPlayer == null)
            {
                return;
            }

            var playerTransform = player.transform;

            var time = Time.timeSinceLevelLoad;
            var deltaTime = time - _lastUpdate;

            var lastPlayerPos = playerTransform.position;
            // Locals decide rotation and position for themselves
            if (playerTransform.position != _targetPos)
            {
                playerTransform.position = Vector3.Lerp(_lastPos, _targetPos, Mathf.Clamp01(deltaTime / _lastTimeSpan));
            }

            var lerpTime = _lerpParam;

            var head = vrPlayer.head;
            var leftHand = vrPlayer.leftHand;
            var rightHand = vrPlayer.rightHand;

            // locals are expected to be VR players so they update their VR body parts themselves
            head.localPosition = Vector3.Lerp(head.localPosition, _targetHeadPos, lerpTime);
            leftHand.localPosition = Vector3.Lerp(leftHand.localPosition, _targetLHandPos, lerpTime);
            rightHand.localPosition = Vector3.Lerp(rightHand.localPosition, _targetRHandPos, lerpTime);

            head.localRotation = Quaternion.Slerp(head.localRotation, _targetHeadRot, lerpTime);
            leftHand.localRotation = Quaternion.Slerp(leftHand.localRotation, _targetLHandRot, lerpTime);
            rightHand.localRotation = Quaternion.Slerp(rightHand.localRotation, _targetRHandRot, lerpTime);
        }

        public void HandleUpdate(VRBodyUpdateData data)
        {
            _targetPos = data.position;

            _targetHeadPos = data.headPosition;
            _targetHeadRot = data.headRotation;

            _targetLHandPos = data.leftHandPosition;
            _targetLHandRot = data.leftHandRotation;

            _targetRHandPos = data.rightHandPosition;
            _targetRHandRot = data.rightHandRotation;

            var time = Time.timeSinceLevelLoad;
            _lastTimeSpan = time - _lastUpdate;
            _lastUpdate = time;
            _lastPos = _player.transform.position;
        }

        // When origin shift happens, the player's position should not be lerped from the non-originShifted position cause that's >1000 units away.
        public void HandleOriginShift(Vector3 originShiftDelta)
        {
            //_lastPos += originShiftDelta;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (!_debugShowTargets || _player == null)
            {
                return;
            }

            Gizmos.color = _gizmoColor;
            Gizmos.DrawWireSphere(_targetPos, 1f);
            Gizmos.DrawSphere(_targetPos + _targetHeadPos, 0.4f);
            Gizmos.DrawSphere(_targetPos + _targetRHandPos, 0.25f);
            Gizmos.DrawSphere(_targetPos + _targetLHandPos, 0.25f);
        }

#endif
    }
}