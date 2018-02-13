namespace HhhNetwork.VR
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class MovementTest : MonoBehaviour
    {
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



        private void Update()
        {
            var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            
            vrPlayer.transform.position += Time.deltaTime * new Vector3(input.x, 0, input.z);

        }
    }
}