namespace HhhNetwork
{
    using UnityEngine;

    /// <summary>
    /// This script shows how one can call a function N times per second. Does not actually do network stuff, but it is useful for regular updates of network stuff, like syncing transforms etc.
    /// </summary>
    public abstract class NetBehaviourBase : MonoBehaviour
    {
        [SerializeField, Tooltip("How many times per second this component sends updates over the network.")]
        private float _sendRate = 10f;

        private float _lastSend;

        protected virtual void OnEnable()
        {
            // wait 'one update' before starting to send (to ensure proper initialization has had time)
            _lastSend = Time.timeSinceLevelLoad + (1f / _sendRate);
        }

        protected virtual void Update()
        {
            var time = Time.timeSinceLevelLoad;
            if (time < _lastSend)
            {
                return;
            }

            _lastSend = time + (1f / _sendRate);
            OnSend();
        }

        /// <summary>
        /// Called on interval depending on the set sendRate on this component. Implement whatever message(s) need to be sent in this method.
        /// </summary>
        protected abstract void OnSend();
    }
}