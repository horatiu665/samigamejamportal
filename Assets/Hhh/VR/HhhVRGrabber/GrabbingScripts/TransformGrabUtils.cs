namespace HhhVRGrabber
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    public static class TransformGrabUtils
    {
        private static Dictionary<GameObject, HVG_FakeParenting> controllerGrabbedTransformDict = new Dictionary<GameObject, HVG_FakeParenting>();
        private static Dictionary<GameObject, HVG_FakeParentingRigidbody> controllerGrabbedTransformDictRb = new Dictionary<GameObject, HVG_FakeParentingRigidbody>();

        public static void TransformGrab(GameObject grabGO, Transform grabbedObj, bool pos = true, bool rot = true)
        {
            // get/add fake parenting component to controller
            HVG_FakeParenting fp;
            if (!controllerGrabbedTransformDict.TryGetValue(grabGO, out fp))
            {
                fp = grabGO.gameObject.AddComponent<HVG_FakeParenting>();
                controllerGrabbedTransformDict[grabGO] = fp;
            }

            // ungrab grabbedObj from all other controllers
            foreach (var c in controllerGrabbedTransformDict)
            {
                if (c.Value.fakeChild == grabbedObj)
                {
                    c.Value.SetFakeParenting(null);
                }
            }
            fp.SetFakeParenting(grabbedObj, pos, rot);
        }

        public static void TransformUngrab(GameObject grabGO, Transform grabbedObj)
        {
            HVG_FakeParenting fp;
            if (controllerGrabbedTransformDict.TryGetValue(grabGO, out fp))
            {
                fp.SetFakeParenting(null);
            }
        }


        public static void RigidbodyGrab(GameObject grabGO, Rigidbody grabbedObj)
        {
            // get/add fake parenting component to controller
            HVG_FakeParentingRigidbody fp;
            if (!controllerGrabbedTransformDictRb.TryGetValue(grabGO, out fp))
            {
                fp = grabGO.gameObject.AddComponent<HVG_FakeParentingRigidbody>();
                controllerGrabbedTransformDictRb[grabGO] = fp;
            }

            // ungrab grabbedObj from all other controllers
            foreach (var c in controllerGrabbedTransformDictRb)
            {
                if (c.Value.fakeChild == grabbedObj)
                {
                    c.Value.SetFakeParenting(null);
                }
            }
            fp.SetFakeParenting(grabbedObj);
        }

        public static void RigidbodyUngrab(GameObject grabGO, Rigidbody grabbedObj)
        {
            HVG_FakeParentingRigidbody fp;
            if (controllerGrabbedTransformDictRb.TryGetValue(grabGO, out fp))
            {
                fp.SetFakeParenting(null);
            }
        }


    }
}
