namespace HhhVRGrabber
{
    using System;
    using UnityEngine;

    // This holds the functionality for grabbing and keeps track of grabbing stuff, but is updated by the master or by the network
    // non serializable. generated in list upon access. [Serializable] just for the debug and for the refs
    [Serializable]
    public class ControllerGrabberData
    {
        public VRPlayerGrabSystem grabSystem;
        public GameObject controller;

        [SerializeField]
        private GameObject _grabPointGO;
        public GameObject grabPointGO
        {
            get
            {
                if (_grabPointGO == null)
                {
                    var gpoh = controller.GetComponentInChildren<GrabPointComponentOnHands>();
                    _grabPointGO = gpoh != null ? gpoh.gameObject : controller;
                }
                return _grabPointGO;
            }
            set
            {
                _grabPointGO = value;
            }
        }

        public bool isLeft;

        // change to the nice generic system
        public bool GetPressDownButton()
        {
            return InputVR.GetPressDown(isLeft, InputVR.ButtonMask.Trigger);
            //return controllerWrapper.GetPressDown(ControllerWrapper.ButtonMask.Trigger);
        }

        // change to the nice generic system
        public bool GetPressUpButton()
        {
            return InputVR.GetPressUp(isLeft, InputVR.ButtonMask.Trigger);
            //return controllerWrapper.GetPressUp(ControllerWrapper.ButtonMask.Trigger);
        }

        public Vector3 grabPoint
        {
            get
            {
                return grabPointGO.transform.position;

            }
        }

        public bool isGrabbing { get; private set; }
        private IHandleGrabbing _curGrabbed;
        public IHandleGrabbing curGrabbed
        {
            get
            {
                return _curGrabbed;
            }
        }

        private IHandleGrabbing _prevGrabbed;
        public IHandleGrabbing prevGrabbed
        {
            get
            {
                if (isGrabbing)
                {
                    return _prevGrabbed;
                }
                else
                {
                    return _curGrabbed;
                }
            }
        }

        public bool isHighlighting { get; private set; }
        private IHandleGrabbing _curHighlighted;
        private IHandleGrabbing _prevHighlighted;
        public IHandleGrabbing curHighlighted
        {
            get
            {
                if (isHighlighting)
                {
                    return _curHighlighted;
                }
                else
                {
                    return null;
                }
            }
        }
        public IHandleGrabbing prevHighlighted
        {
            get
            {
                if (isHighlighting)
                {
                    return _prevHighlighted;
                }
                else
                {
                    return _curHighlighted;
                }
            }
        }

        public void Highlight(IHandleGrabbing obj)
        {
            _prevHighlighted = _curHighlighted;
            _curHighlighted = obj;
            isHighlighting = true;

            obj.OnHighlight(controller.gameObject);
        }

        public void Unhighlight()
        {
            if (isHighlighting)
            {
                isHighlighting = false;

                if (_curHighlighted != null)
                {
                    _curHighlighted.OnUnhighlight(controller.gameObject);
                }
            }

            // no null so we can get prev highlighted info
            //_curHighlighted = null;

        }

        public void Grab(IHandleGrabbing closestGrabber)
        {
            this._prevGrabbed = this._curGrabbed;
            this._curGrabbed = closestGrabber;
            isGrabbing = true;

            if (isHighlighting)
            {
                Unhighlight();
            }

            closestGrabber.OnGrab(grabPointGO);

        }

        public void Ungrab()
        {
            if (isGrabbing)
            {
                this.isGrabbing = false;

                _curGrabbed.OnUngrab(grabPointGO);
                // NO NULL for keeping prev grab info
                //curGrabbed = null;
                
            }
        }

    }


}