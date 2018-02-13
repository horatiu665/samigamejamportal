namespace HhhVRGrabber
{
    using UnityEngine;

    public static class ColliderExtensions
    {
        /// <summary>
        /// Similar to ClosestPointOnBounds, but uses a custom algorithm to return the closest point on a collider surface.
        /// Perfect for sphere, capsule, box colliders, and less accurate for mesh colliders.
        /// (c) Horatiu/VRUnicorns ~2016-2017
        /// </summary>
        /// <param name="c">Collider it applies to</param>
        /// <param name="pos">Position to which the result will be closest to</param>
        /// <returns>Position on collider surface or inside it, which is closest to the input position</returns>
        public static Vector3 ClosestPointOnSurface(this Collider c, Vector3 pos)
        {
            if (c is SphereCollider)
            {
                var sc = (SphereCollider)c;

                // calculate world center based on collider data and transformations.
                // parenting collider to nonuniform scale objects will cause negligible inaccuracies,
                // so the recommended approach is to only parent to objects with scale 1,1,1.
                var worldCenter = sc.center;
                worldCenter.Scale(sc.transform.lossyScale);
                worldCenter = sc.transform.rotation * worldCenter;
                worldCenter = sc.transform.position + worldCenter;

                // calculate radius based on scaling. tested in Unity and it matches the way that unity scales colliders.
                var realRadius = sc.radius;
                var ls = sc.transform.lossyScale;
                realRadius *= Mathf.Max(ls.x, ls.y, ls.z);

                // returns closest position to sphere of position worldCenter and radius realRadius (or point inside it)
                var toPos = pos - worldCenter;
                var toPosMag = toPos.magnitude;
                return worldCenter + ((toPos / toPosMag) * Mathf.Clamp(toPosMag, 0f, realRadius));
            }
            else if (c is CapsuleCollider)
            {
                var cc = (CapsuleCollider)c;
                // calculate world position based on transform.
                var worldCenter = cc.center;
                worldCenter.Scale(cc.transform.lossyScale);
                worldCenter = cc.transform.rotation * worldCenter;
                worldCenter = cc.transform.position + worldCenter;

                var axis = cc.direction;

                // determine line segment that represents the center of the collider
                Vector3 dir = Vector3.zero;

                // height modified by transform
                var realHeight = cc.height;

                // radius modified by transform
                var realRadius = cc.radius;

                var ls = cc.transform.lossyScale;

                // direction means which local axis does collider height represent. in each case, height is: 0=x, 1=y, 2=z.
                switch (axis)
                {
                case 0: // X
                    {
                        dir = cc.transform.right;
                        realHeight *= ls.x;
                        realRadius *= Mathf.Max(ls.y, ls.z);
                        break;
                    }
                case 1: // Y
                    {
                        dir = cc.transform.up;
                        realHeight *= ls.y;
                        realRadius *= Mathf.Max(ls.x, ls.z);
                        break;
                    }
                case 2: // Z
                    {
                        dir = cc.transform.forward;
                        realHeight *= ls.z;
                        realRadius *= Mathf.Max(ls.x, ls.y);
                        break;
                    }
                }

                // subtract realRadius because the height represents the distance from top to bottom of collider, which already consists of a radius on each side.
                realHeight = Mathf.Clamp(realHeight - realRadius * 2f, 0f, float.MaxValue);

                // find closest point on line segment from worldcenter-dir*height/2 to worldcenter + dir*height/2, then return the point on the sphere of radius realRadius from that point.
                // beyond the caps, the closest point is one of the caps. so that's easy. between then it's gonna be a perpendicular line. also easy.

                // dot in world units, divide by realHeight/2 to find percent of segment. Clamp -1 to 1 to only accept points on the segment
                var percentAlongSegmentFromCenter = Mathf.Clamp(Vector3.Dot(pos - worldCenter, dir) / (realHeight * 0.5f), -1f, 1f);

                var onSegmentPos = worldCenter + dir * realHeight * 0.5f * percentAlongSegmentFromCenter;
                var toPos = pos - onSegmentPos;
                var toPosMag = toPos.magnitude;
                return onSegmentPos + Mathf.Clamp(toPosMag, 0f, realRadius) * toPos / toPosMag;
            }
            else if (c is BoxCollider)
            {
                var cc = (BoxCollider)c;

                // we will bring the point in question to the object space, calculate collision, then return it to world space
                var posLocal = c.transform.InverseTransformPoint(pos);
                // closest point is the closest on 1D on each axis, appended to form a V3
                var x = Mathf.Clamp(posLocal.x, cc.center.x - cc.size.x * 0.5f, cc.center.x + cc.size.x * 0.5f);
                var y = Mathf.Clamp(posLocal.y, cc.center.y - cc.size.y * 0.5f, cc.center.y + cc.size.y * 0.5f);
                var z = Mathf.Clamp(posLocal.z, cc.center.z - cc.size.z * 0.5f, cc.center.z + cc.size.z * 0.5f);

                return c.transform.TransformPoint(new Vector3(x, y, z));
            }
            else if (c is MeshCollider && (c as MeshCollider).convex)
            {
                // final fallback is the new ClosestPoint (would have been ClosestPointOnBounds in unity ~5.5? and before)
                return c.ClosestPoint(pos);
            }
            else
            {
                return c.ClosestPointOnBounds(pos);
            }
        }

        /// <summary>
        /// Quick version of looping and finding closest position among multiple colliders. 
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Vector3 ClosestPointOnSurface(this Collider[] cs, Vector3 pos)
        {
            var minSqrDist = float.MaxValue;
            Vector3 realClosestPoint = pos;
            for (int i = 0; i < cs.Length; i++)
            {
                var closestPoint = cs[i].ClosestPointOnSurface(pos);
                var newSqrDist = (closestPoint - pos).sqrMagnitude;
                if (newSqrDist < minSqrDist)
                {
                    minSqrDist = newSqrDist;
                    realClosestPoint = closestPoint;
                }
            }
            return realClosestPoint;
        }
    }
}