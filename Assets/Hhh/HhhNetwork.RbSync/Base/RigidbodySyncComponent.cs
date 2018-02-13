namespace HhhNetwork.RbSync
{
    using HhhPrefabManagement;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(IPrefabComponent))]
    public class RigidbodySyncComponent : MonoBehaviour
    {
        [SerializeField]
        private bool _interpolate = true;

        [SerializeField]
        private Vector2 _syncDeltaTimeRange = new Vector2(0.001f, 0.05f);

        [SerializeField, Tooltip("If true, will make sure that the synced rigidbody is kinematic on clients.")]
        private bool _ensureKinematic = false;

        [SerializeField]
        private float _minDeltaPositionThreshold = 0.005f;

        [SerializeField, ReadOnly]
        private int _syncId;

        [SerializeField, ReadOnly]
        private bool _registered;

        [SerializeField, ReadOnly]
        private bool _hasBeenUpdated;

        [SerializeField]
        private bool _printDebug;

        private IPrefabComponent _prefab;
        private Vector3 _lastSyncPos;
        private Quaternion _lastSyncRot;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private Vector3 _lastSyncVel;
        private Vector3 _lastSyncAV;
        private Vector3 _targetVel;
        private Vector3 _targetAV;
        private int _framesLerped;

        // 2 network frames ago
        private float _prevSyncTime;

        // 1 network frame ago
        private float _lastSyncTime;

        private Rigidbody _rb;
        public new Rigidbody rigidbody
        {
            get
            {
                return _rb;
            }
        }

        Joint _j;
        private Joint joint
        {
            get
            {
                if (_j == null)
                {
                    _j = _rb.GetComponent<Joint>();
                }
                return _j;
            }
        }

        [Range(0, 1f)]
        [SerializeField]
        private float _maxLerpSmoothness = 1f;

        /// <summary>
        /// Gets the synchronize identifier.
        /// </summary>
        /// <value>
        /// The synchronize identifier.
        /// </value>
        public int syncId
        {
            get { return _syncId; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="RigidbodySyncComponent"/> has changed position or rotation since last update. Only called on server.
        /// </summary>
        public bool changed
        {
            get { return (_lastSyncPos != this.transform.position) || (_lastSyncRot != this.transform.rotation); }
        }

        /// <summary>
        /// Gets the prefab type representing this synced rigidbody.
        /// </summary>
        public PrefabType prefabType
        {
            get
            {
                if (_prefab == null)
                {
                    _prefab = this.GetComponent<IPrefabComponent>();
                }

                return _prefab.prefabType;
            }
        }

        public HashSet<GameObject> stopUpdatingSenders = new HashSet<GameObject>();

        public void StopUpdating(GameObject whosAskin)
        {
            stopUpdatingSenders.Add(whosAskin);
        }

        public void ContinueUpdating(GameObject whosAskin)
        {
            stopUpdatingSenders.Remove(whosAskin);
        }

        private void OnEnable()
        {
            //GameServices.messageBus.Subscribe(this);

            if (syncId > 0)
            {
                // only register on enable if there is valid sync ID
                Initialize();
            }

            _rb = GetComponent<Rigidbody>();

            if (_ensureKinematic && NetServices.isClient)
            {
                _rb.isKinematic = true;
            }

            //OriginShiftManager.OriginShiftersAdd(this);
        }

        private void OnDisable()
        {
            //GameServices.messageBus.Unsubscribe(this);

            // To avoid exceptions when shutting down
            var manager = RbSyncManager.instance;
            if (manager != null)
            {
                manager.Unregister(this);
            }

            if (_ensureKinematic && NetServices.isClient)
            {
                _rb.isKinematic = false;
            }

            _lastSyncRot = _targetRotation = new Quaternion(0f, 0f, 0f, 0f);
            _lastSyncPos = _targetPosition = Vector3.zero;
            _lastSyncVel = _targetVel = Vector3.zero;
            _lastSyncAV = _targetAV = Vector3.zero;
            _hasBeenUpdated = false;
            _registered = false;
            _prevSyncTime = 0f;
            _lastSyncTime = 0f;
            _framesLerped = 0;
            _syncId = 0;

            //OriginShiftManager.OriginShiftersRemove(this);

        }

        private void FixedUpdate()
        {
            if (!_interpolate || !_hasBeenUpdated || NetServices.isServer)
            {
                return;
            }

            // these are objects that ask the rbSync to stop updating. Example: while being grabbed, impaled with joint..? part of ragdoll..? etc.
            if (stopUpdatingSenders.Count > 0)
            {
                return;
            }

            if ((transform.position - _targetPosition).sqrMagnitude < (_minDeltaPositionThreshold * _minDeltaPositionThreshold))
            {
                // if the position change is super small then stop lerping to it
                return;
            }

#if UNITY_EDITOR
            // for builds, only do it on enable and disable
            if (_ensureKinematic && NetServices.isClient)
            {
                _rb.isKinematic = true;
            }
            else
            {
                _rb.isKinematic = false;
            }
#endif

            UpdatePosTheNewWay();
        }

        private void UpdatePosTheNewWay()
        {
            var lastTimeDelta = Mathf.Clamp(_lastSyncTime - _prevSyncTime, _syncDeltaTimeRange.x, _syncDeltaTimeRange.y);
            var t = Mathf.Min(_maxLerpSmoothness, (_framesLerped * Time.fixedDeltaTime) / lastTimeDelta);

            // only update position if there is no joint...
            if (joint == null)
            {
                _rb.velocity = Vector3.LerpUnclamped(_lastSyncVel, _targetVel, t);
                _rb.angularVelocity = Vector3.LerpUnclamped(_lastSyncAV, _targetAV, t);

                // compensate for serverLag by adding velocity (continuing the motion?)
                // might need some raycasting to account for collisions
                transform.position = Vector3.LerpUnclamped(_lastSyncPos, _targetPosition, t);
            }
            transform.rotation = Quaternion.SlerpUnclamped(_lastSyncRot, _targetRotation, t);

            _framesLerped++;
        }

        private void UpdatePosTheOldWay()
        {
            var lastTimeDelta = Mathf.Clamp(_lastSyncTime - _prevSyncTime, _syncDeltaTimeRange.x, _syncDeltaTimeRange.y);
            var t = _framesLerped * Time.fixedDeltaTime / lastTimeDelta;
            transform.rotation = Quaternion.Slerp(_lastSyncRot, _targetRotation, t);
            transform.position = Vector3.Lerp(_lastSyncPos, _targetPosition, t);
            if (_printDebug)
                Debug.Log(transform.name + " pos " + transform.position + " lerping to " + _targetPosition + "; HBU is" + _hasBeenUpdated, transform.gameObject);
            _rb.velocity = Vector3.Lerp(_lastSyncVel, _targetVel, t);
            _rb.angularVelocity = Vector3.Lerp(_lastSyncAV, _targetAV, t);
            _framesLerped++;
        }

        public void Initialize(int syncId)
        {
            SetSyncId(syncId);
            Initialize();
        }

        public void Initialize()
        {
            // rb sync manager is in the additive scene, so it is null on Awake until the scene is loaded.
            // So we wait with registering with a coroutine...
            if (!_registered)
            {
                StartCoroutine(InitializeCor());
            }

        }

        private IEnumerator InitializeCor()
        {
            var manager = RbSyncManager.instance;
            if (manager == null)
            {
                yield return 0;
                Initialize();
                yield break;
            }
            if (manager != null && !_registered)
            {
                // only register if the manager has already loaded and we have not already registered
                manager.Register(this);
                _registered = true;
            }
            yield break;
        }

        //public void Handle(RigidbodySyncManagerInitializationMessage message)
        //{
        //    if (!_registered)
        //    {
        //        message.manager.Register(this);
        //        _registered = true;
        //    }
        //}

        /// <summary>
        /// Handles a <see cref="RigidbodySyncData"/> update. Only called on clients.
        /// </summary>
        /// <param name="syncData">The synchronized data.</param>
        public void HandleUpdate(RigidbodySyncData syncData, bool initialSetup = false)
        {
            if (!_interpolate || initialSetup)
            {
                _rb.velocity = syncData.velocity;
                _rb.angularVelocity = syncData.angularVelocity;
                this.transform.position = syncData.position;
                this.transform.rotation = syncData.rotation;
            }
            else
            {
                _hasBeenUpdated = true;

                if (_targetPosition == Vector3.zero)
                {
                    _lastSyncPos = syncData.position;
                }
                else
                {
                    _lastSyncPos = _targetPosition;
                }

                if (_targetRotation.IsZero())
                {
                    _lastSyncRot = syncData.rotation;
                }
                else
                {
                    _lastSyncRot = _targetRotation;
                }

                if (_targetVel == Vector3.zero)
                {
                    _lastSyncVel = syncData.velocity;
                }
                else
                {
                    _lastSyncVel = _targetVel;
                }

                if (_targetAV == Vector3.zero)
                {
                    _lastSyncAV = syncData.angularVelocity;
                }
                else
                {
                    _lastSyncAV = _targetAV;
                }

                _targetPosition = syncData.position;
                _targetRotation = syncData.rotation;
                _targetVel = syncData.velocity;
                _targetAV = syncData.angularVelocity;

                _prevSyncTime = _lastSyncTime;
                _lastSyncTime = Time.timeSinceLevelLoad;
                _framesLerped = 0;
            }
        }

        /// <summary>
        /// Gets the synchronize data. Also records the position and rotation for use in changed check. Only called on server.
        /// </summary>
        /// <returns></returns>
        public RigidbodySyncData GetSyncData()
        {
            var pos = this.transform.position;
            var rot = this.transform.rotation;
            var vel = _rb.velocity;
            var av = _rb.angularVelocity;

            _lastSyncPos = pos;
            _lastSyncRot = rot;
            _lastSyncVel = vel;
            _lastSyncAV = av;

            return new RigidbodySyncData()
            {
                syncId = _syncId,
                prefabType = this.prefabType,
                position = pos,
                rotation = rot,
                velocity = vel,
                angularVelocity = av
            };
        }

        /// <summary>
        /// Sets the synchronize identifier. Should normally NOT be changed at runtime!
        /// </summary>
        /// <param name="syncId">The synchronize identifier.</param>
        public void SetSyncId(int syncId)
        {
            _syncId = syncId;

#if UNITY_EDITOR

            this.name += string.Concat(" (ID: ", _syncId.ToString(), ")");

#endif
        }

        public void OnWorldMove(Vector3 originShiftDelta)
        {
            _lastSyncPos -= originShiftDelta;
            _targetPosition -= originShiftDelta;
        }
    }
}