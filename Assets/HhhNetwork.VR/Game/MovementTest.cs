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
            var left = Vector3.left * 0.4f;
            var right = Vector3.right * 0.4f;
            var head = Vector3.up * 0.7f;

            var input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            head.y += input.y * 0.5f;

            vrPlayer.leftHand.localPosition = left;
            vrPlayer.rightHand.localPosition = right;
            vrPlayer.head.localPosition = head;

            vrPlayer.transform.position += Time.deltaTime * input.x * Vector3.right;

        }
    }
}