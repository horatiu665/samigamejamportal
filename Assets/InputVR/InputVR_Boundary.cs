using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class InputVR_Boundary : MonoBehaviour
{
    public Material baseMaterial;
    public Color color = new Color(0, 0, 0, (55f / 255f));

    private void OnEnable()
    {
        InitBoundary();
    }

    private void InitBoundary()
    {
        var chap = GetChaperone();

        var vertices = new Vector3[chap.corners.Length /** 2*/];
        for (int i = 0; i < chap.corners.Length; i++)
        {
            var c = chap.corners[i];
            vertices[i] = new Vector3(c.x, 0.01f, c.z);
        }

        var triangles = new int[] {
            0, 1, 2,
            0, 2, 3
        };

        var uv = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f),
            //new Vector2(0.0f, 1.0f),
            //new Vector2(1.0f, 1.0f),
            //new Vector2(0.0f, 1.0f),
            //new Vector2(1.0f, 1.0f)
        };

        var colors = new Color[]
        {
            color,
            color,
            color,
            color,
            //new Color(color.r, color.g, color.b, 0.0f),
            //new Color(color.r, color.g, color.b, 0.0f),
            //new Color(color.r, color.g, color.b, 0.0f),
            //new Color(color.r, color.g, color.b, 0.0f)
        };

        var mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.colors = colors;
        mesh.triangles = triangles;

        var renderer = GetComponent<MeshRenderer>();
        //var tempMaterial = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        renderer.sharedMaterial = baseMaterial;
        renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
#if !(UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        renderer.lightProbeUsage = LightProbeUsage.Off;
#else
        renderer.useLightProbes = false;
#endif

    }


    public class ChaperoneData
    {
        public Vector3[] corners = new Vector3[4];

        public static ChaperoneData Zero
        {
            get
            {
                return new ChaperoneData()
                {
                    corners = new Vector3[]
                    {
                        Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero
                    }
                };
            }
        }
    }

    public static ChaperoneData GetChaperone()
    {
        // if SteamVR
        if (InputVR.controllerType == InputVR.VRControllerType.SteamVR)
        {
            return GetChaperoneSteamVR();
        }
        else if (InputVR.controllerType == InputVR.VRControllerType.Windows)
        {
            return GetChaperoneGeneric();
        }
        else if (InputVR.controllerType == InputVR.VRControllerType.Oculus)
        {
            return GetChaperoneOculus();
        }

        // fallback empty chaperione
        return ChaperoneData.Zero;
    }

    private static ChaperoneData GetChaperoneOculus()
    {
#if OCULUS
        if (OVRManager.boundary.GetConfigured())
        {
            var dimensions = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);
            return new ChaperoneData()
            {
                corners = new Vector3[]
                {
                    new Vector3(dimensions.x, 0, dimensions.z) / 2,
                    new Vector3(dimensions.x, 0, -dimensions.z) / 2,
                    new Vector3(-dimensions.x, 0, -dimensions.z) / 2,
                    new Vector3(-dimensions.x, 0, dimensions.z) / 2,

                }
            };
        }
#endif
        return GetChaperoneGeneric();
    }

    private static ChaperoneData GetChaperoneGeneric()
    {
        //#if UNITY_WSA
        // if detected play space, return the corners of that
        Vector3 dimensions;
        List<Vector3> geometry = new List<Vector3>();
        if (UnityEngine.Experimental.XR.Boundary.TryGetDimensions(out dimensions, UnityEngine.Experimental.XR.Boundary.Type.PlayArea))
        {
            return new ChaperoneData()
            {
                corners = new Vector3[]
                {
                    new Vector3(dimensions.x, 0, dimensions.z) / 2,
                    new Vector3(dimensions.x, 0, -dimensions.z) / 2,
                    new Vector3(-dimensions.x, 0, -dimensions.z) / 2,
                    new Vector3(-dimensions.x, 0, dimensions.z) / 2,
                }
            };
        }
        else
        if (UnityEngine.Experimental.XR.Boundary.TryGetGeometry(geometry, UnityEngine.Experimental.XR.Boundary.Type.TrackedArea))
        {
            if (geometry.Count > 0)
            {
                Vector3 min = new Vector3(geometry.Min(v => v.x), 0, geometry.Min(v => v.z));
                Vector3 max = new Vector3(geometry.Max(v => v.x), 0, geometry.Max(v => v.z));

                return new ChaperoneData()
                {
                    corners = new Vector3[]
                    {
                    new Vector3(min.x, 0, min.z),
                    new Vector3(min.x, 0, max.z),
                    new Vector3(max.x, 0, max.z),
                    new Vector3(max.x, 0, min.z),
                    }
                };
            }
        }
        else
        {
            Debug.Log("Did not find geometry");


        }


        //#endif
        return ChaperoneData.Zero;
    }

    private static ChaperoneData GetChaperoneSteamVR()
    {
#if false//!UNITY_WSA
        var chaperone = OpenVR.Chaperone;
        HmdQuad_t pRect = new HmdQuad_t();

        bool success = (chaperone != null) && chaperone.GetPlayAreaRect(ref pRect);
        if (success)
        {
            return new ChaperoneData()
            {
                corners = new Vector3[4] {
                        new Vector3(
                            pRect.vCorners0.v0,
                            pRect.vCorners0.v1,
                            pRect.vCorners0.v2),
                        new Vector3(
                            pRect.vCorners1.v0,
                            pRect.vCorners1.v1,
                            pRect.vCorners1.v2),
                        new Vector3(
                            pRect.vCorners2.v0,
                            pRect.vCorners2.v1,
                            pRect.vCorners2.v2),
                        new Vector3(
                            pRect.vCorners3.v0,
                            pRect.vCorners3.v1,
                            pRect.vCorners3.v2)
                        }
            };
        }
        else if (!success)
        {
            // THIS IS WAT USED TO HAPPEN BEFORE WINMR FUX UP THE GAME
            // offset to a random spot close to the ball landing place (assume standing position)
            //playerSpaceOffset = Random.onUnitSphere;
            //playerSpaceOffset.Scale(new Vector3(1, 0, 1));
        }
#endif

        return ChaperoneData.Zero;
    }

}