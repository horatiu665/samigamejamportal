#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Algorithm
///     Clear hashsets that mark prefabs
///     Loop through all root gameobjects in all scenes
///         Caching lists of root gameobjects for performance
///     Recursively loop through all transform hierarchies of root GOs
///         In recursive loop: mark which transform are prefabs, for the regular prefab icon.
///         If a child is a prefab, and we already have a parentID, mark that parent as a nested parent prefab.
///         If a child is a prefab, send child id along to its children, instead of its parent's prefab ID.
///         Run a loop again for the children missed before setting the nested parent prefab
///     When to run this algorithm is a bit more tricky because of weird way unity calls OnGUI and Update
///         just hack it
///
/// Strictly used for showing prefab icons on hierarchy, next to prefab names. This helps figuring out which prefab is nested under which, if so.
/// drawing icons on hierarchy items inspired from https://gist.github.com/edwardrowe/acda1ee53eb037b31d54d583afc13973
/// 
/// Copyright (lol) Horatiu Roman @ VRUnicorns, 2017
/// </summary>
[InitializeOnLoad]
class HierarchyPrefabPimp
{
    static HierarchyPrefabPimp()
    {
        // remove delegates before adding them so we don't duplicate add ;)
        EditorApplication.hierarchyWindowChanged -= OnHierarchyWindowChanged;
        EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged; // Happens on reparent, destroy, create, or whatever other change in hierarchy structure. (but not aesthetic changes like changing gameobject foldouts)
        EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI; // Happens once per visible hierarchy item, on mouse enter, move and exit and probably more.
        EditorApplication.update -= OnUpdate;
        EditorApplication.update += OnUpdate; // Happens once per editor frame, but before/after one entire group of HierarchyItemOnGUI -> can be used to update hierarchy gui icons.

        // even without a change, initialize the hashsets
        InitHashsets();
    }

    /// <summary>
    /// Add context menu that takes you from a nested parent to the first nested child prefab
    /// inspired from: http://answers.unity3d.com/questions/22947/adding-to-the-context-menu-of-the-hierarchy-tab.html
    /// </summary>
    [MenuItem("GameObject/GoTo Nested Child Prefab", validate = false, priority = -2)]
    private static void GoToNestedChildPrefab()
    {
        var so = Selection.activeGameObject;
        var soid = so.GetInstanceID();
        Transform firstPrefabChild = null;
        if (nestedParentPrefabIds.Contains(soid))
        {
            // we are the parent
            // just find first prefab child
            firstPrefabChild = FindFirstPrefabChild(so.transform);
        }
        else if (prefabChildIds.Contains(soid))
        {
            // we are a child of a nested parent with problems. find first prefab child starting from that parent!
            var pare = PrefabUtility.FindPrefabRoot(so);
            firstPrefabChild = FindFirstPrefabChild(pare.transform);
        }

        if (firstPrefabChild != null)
        {
            EditorGUIUtility.PingObject(firstPrefabChild.gameObject);
        }
    }

    // same as recursive but without first transform.
    private static Transform FindFirstPrefabChild(Transform transform)
    {
        Transform found = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            found = FindFirstPrefabChildRecursive(transform.GetChild(i));
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private static Transform FindFirstPrefabChildRecursive(Transform so)
    {
        if (IsPrefabRoot(so.gameObject))
            return so;

        Transform found = null;
        for (int i = 0; i < so.childCount; i++)
        {
            found = FindFirstPrefabChildRecursive(so.GetChild(i));
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    [MenuItem("GameObject/GoTo Nested Child Prefab", validate = true, priority = -2)]
    private static bool GoToNestedChildPrefabValidation()
    {
        if (Selection.activeGameObject != null)
        {
            var id = Selection.activeGameObject.GetInstanceID();
            return nestedParentPrefabIds.Contains(id) || prefabChildIds.Contains(id);
        }
        return false;
    }

    [MenuItem("GameObject/Show Modified Props", validate = false, priority = -1)]
    private static void ShowModifiedProps()
    {
        if (Selection.activeGameObject != null)
        {
            var go = Selection.activeGameObject;

            // show window with these mods
            HierarchyPimpModsWindow.Get(go);

        }
    }

    [MenuItem("GameObject/Show Modified Props", validate = true, priority = -1)]
    private static bool ShowModifiedPropsValidation()
    {
        return HierarchyPimpModsWindow.CanShowModifiedProps();

    }

    private const float iconWidth = 15;

    // A list of all the built-in EditorGUI icons in Unity. Use EditorGUIUtility.IconContent([icon name]) to access them. Editor included icons: https://gist.github.com/MattRix/c1f7840ae2419d8eb2ec0695448d4321

    private static Texture _prefabIcon;
    public static Texture prefabIcon
    {
        get
        {
            if (_prefabIcon == null)
            {
                _prefabIcon = (Texture)AssetDatabase.LoadAssetAtPath(@"Assets\HierarchyPrefabPimp\Editor\Resources\Icons\prefabIcon.png", typeof(Texture));
                //(Texture)Resources.Load("Icons/prefabIcon.png", typeof(Texture));
            }
            if (_prefabIcon == null)
            {
                _prefabIcon = EditorGUIUtility.IconContent("PrefabNormal Icon").image;
            }
            return _prefabIcon;
        }
    }

    private static Texture _prefabIconPlus;
    public static Texture prefabIconPlus
    {
        get
        {
            if (_prefabIconPlus == null)
            {
                _prefabIconPlus = (Texture)AssetDatabase.LoadAssetAtPath(@"Assets\HierarchyPrefabPimp\Editor\Resources\Icons\prefabIconPlus.png", typeof(Texture));
                //(Texture)Resources.Load("Icons/prefabIconPlus.png", typeof(Texture));

            }
            if (_prefabIconPlus == null)
            {
                _prefabIconPlus = EditorGUIUtility.IconContent("Prefab Icon").image;
            }
            return _prefabIconPlus;
        }
    }

    private static Texture _prefabIconWarning;
    public static Texture prefabIconWarning
    {
        get
        {
            if (_prefabIconWarning == null)
            {
                _prefabIconWarning = (Texture)AssetDatabase.LoadAssetAtPath(@"Assets\HierarchyPrefabPimp\Editor\Resources\Icons\prefabIconWarning.png", typeof(Texture));
                //(Texture)Resources.Load("Icons/prefabIconWarning.png", typeof(Texture));

            }
            if (_prefabIconWarning == null)
            {
                _prefabIconWarning = EditorGUIUtility.IconContent("Toolbar Minus").image;
            }
            return _prefabIconWarning;
        }
    }

    private static Texture _prefabIconQuestion;
    public static Texture prefabIconQuestion
    {
        get
        {
            if (_prefabIconQuestion == null)
            {
                _prefabIconQuestion = (Texture)AssetDatabase.LoadAssetAtPath(@"Assets\HierarchyPrefabPimp\Editor\Resources\Icons\prefabIconQuestion.png", typeof(Texture));
                //(Texture)Resources.Load("Icons/prefabIconQuestion.png", typeof(Texture));
            }
            if (_prefabIconQuestion == null)
            {
                _prefabIconQuestion = EditorGUIUtility.IconContent("_Help").image;
            }
            return _prefabIconQuestion;
        }
    }

    // this is the coolest pattern for caching private/serializablefield getters btw

    private static GUIContent _prefabIconGuiContent;
    public static GUIContent prefabIconGuiContent
    {
        get
        {
            if (_prefabIconGuiContent == null)
            {
                _prefabIconGuiContent = new GUIContent(prefabIcon);
            }
            return _prefabIconGuiContent;
        }
    }

    private static GUIContent _prefabIconPlusGuiContent;
    public static GUIContent prefabIconPlusGuiContent
    {
        get
        {
            if (_prefabIconPlusGuiContent == null)
            {
                _prefabIconPlusGuiContent = new GUIContent(prefabIconPlus);
            }
            return _prefabIconPlusGuiContent;
        }
    }

    private static GUIContent _prefabIconWarningGuiContent;
    public static GUIContent prefabIconWarningGuiContent
    {
        get
        {
            if (_prefabIconWarningGuiContent == null)
            {
                _prefabIconWarningGuiContent = new GUIContent(prefabIconWarning);
            }
            return _prefabIconWarningGuiContent;
        }
    }

    private static GUIContent _prefabIconQuestionGuiContent;
    public static GUIContent prefabIconQuestionGuiContent
    {
        get
        {
            if (_prefabIconQuestionGuiContent == null)
            {
                _prefabIconQuestionGuiContent = new GUIContent(prefabIconQuestion);
            }
            return _prefabIconQuestionGuiContent;
        }
    }

    private static HashSet<int> regularPrefabIds = null, nestedParentPrefabIds = null, prefabChildIds = null, visibleInHierarchyIds = null;
    private static List<List<GameObject>> rootGameObjectCaches = null;

    // this is made true when the OnGUI fires (or whenever the hierarchy changed, such as on init), and false in Update, so we know the OnGUI has executed before we try to update. all item OnGUIs always execute together sequentially
    private static bool a_visibleInHierarchyIdsChanged = false;

    // true when the MarkTransforms executes, false when it is time to update
    private static bool b_nestedPrefabMarksReady = false;

    // these vars keep track of when the visible hierarchy item count has changed due to collapsing/expanding some gameobjects, or other manipulations that do not correspond to unity events
    private static int unreliableVisibleObjectCount = 0;
    private static int oldUnreliableVisibleObjectCount = 0;

    private static bool showAllPrefabChangeCounts = false;

    private static void OnHierarchyWindowChanged()
    {
        InitHashsets();
    }

    /// <summary>
    /// Initializes hashsets and flags
    /// </summary>
    private static void InitHashsets()
    {
        if (regularPrefabIds == null)
        {
            regularPrefabIds = new HashSet<int>();
        }
        regularPrefabIds.Clear();

        if (prefabChildIds == null)
        {
            prefabChildIds = new HashSet<int>();
        }
        prefabChildIds.Clear();

        if (nestedParentPrefabIds == null)
        {
            nestedParentPrefabIds = new HashSet<int>();
        }
        nestedParentPrefabIds.Clear();

        if (visibleInHierarchyIds == null)
        {
            visibleInHierarchyIds = new HashSet<int>();
        }
        visibleInHierarchyIds.Clear();

        a_visibleInHierarchyIdsChanged = true;
        b_nestedPrefabMarksReady = false;

        // funky way of resetting the counter thing
        oldUnreliableVisibleObjectCount = 0;

    }

    /// <summary>
    /// Should be done only when <see cref="a_visibleInHierarchyIdsChanged"/> is true, to save performance
    /// </summary>
    private static void MarkNestedPrefabs()
    {
        // some debug
        //Debug.Log("Running MarkNestedPrefabs on " + visibleInHierarchyIds.Count + " ids:");
        //var ss = "";
        //foreach (var vis in visibleInHierarchyIds)
        //{
        //    var go = EditorUtility.InstanceIDToObject(vis) as GameObject;
        //    if (go != null)
        //    {
        //        ss += go.name + "\n";
        //    }
        //}
        //Debug.Log(ss);

        if (rootGameObjectCaches == null)
        {
            rootGameObjectCaches = new List<List<GameObject>>(SceneManager.sceneCount);
        }

        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            var scene = SceneManager.GetSceneAt(sceneIndex);
            if (rootGameObjectCaches.Count <= sceneIndex)
            {
                rootGameObjectCaches.Add(new List<GameObject>(100));
            }
            var rootGameObjectCache = rootGameObjectCaches[sceneIndex];
            scene.GetRootGameObjects(rootGameObjectCache);
            for (int rootIndex = 0; rootIndex < rootGameObjectCache.Count; rootIndex++)
            {
                var rootGO = rootGameObjectCache[rootIndex];

                // loop through transform hierarchy and mark prefabs. keep track of latest prefab parent so we can set its nested parent flag.
                MarkTransformRecursive(rootGO.transform, -1);

                // run through again so we can set the first nested children that were not set because the nested parent only knew it was nested when we found the first prefab under it
                MarkFirstChildrenRecursive(rootGO.transform, -1);

            }
        }

        b_nestedPrefabMarksReady = true;
    }

    private static bool MarkTransformRecursive(Transform transform, int parentPrefabId)
    {
        var myId = transform.gameObject.GetInstanceID();
        bool weAreVisibleInHierarchy = visibleInHierarchyIds.Contains(myId);

        // if self is prefab
        if (IsPrefabRoot(transform.gameObject))
        {
            // mark self as prefab
            regularPrefabIds.Add(myId);

            // if is child of another prefab because it was called by another marked prefab
            if (parentPrefabId != -1)
            {
                // mark the other ID as nested parent. is hashset so we Add() multiple times.
                nestedParentPrefabIds.Add(parentPrefabId);
            }

            // overwrite the parent prefab ID so the children can mark me if they are prefabs.
            parentPrefabId = myId;

            // if this obj is hidden, we do not care to mark it or even its siblings, because the parent already is shown to be nested.
            // we only care to mark the visible objects, and their children, until we find a prefab down the line.
            if (!weAreVisibleInHierarchy)
            {
                return false;
            }
        }
        else
        {
            // NOTE: first children of the nested parent, before we find the first nested child prefab, will not be set, because the nested parent doesn't know he is a nested parent yet.

            // if parent is nested parent prefab
            if (nestedParentPrefabIds.Contains(parentPrefabId))
            {
                // mark self as child
                prefabChildIds.Add(myId);
            }
        }

        // mark all children of prefab, and their children
        for (int i = 0; i < transform.childCount; i++)
        {
            bool continueMarking = MarkTransformRecursive(transform.GetChild(i), parentPrefabId);
            if (!continueMarking)
            {
                // let siblings check? break if not.
                break;
            }
        }

        return true;
    }

    private static bool MarkFirstChildrenRecursive(Transform transform, int parentPrefabId)
    {
        var myId = transform.gameObject.GetInstanceID();
        bool weAreVisibleInHierarchy = visibleInHierarchyIds.Contains(myId);

        // clone behaviour of MarkTransformsRecursive but without the overhead
        // if self is prefab
        if (IsPrefabRoot(transform.gameObject))
        {
            // overwrite the parent prefab ID so the children can mark me if they are prefabs.
            parentPrefabId = myId;

            // if this obj is hidden, we do not care to mark it or even its siblings, because the parent already is shown to be nested
            // we only care to mark the visible objects, and their children, until we find a prefab down the line.
            if (!weAreVisibleInHierarchy)
            {
                return false;
            }
        }
        else
        {
            // if parent is a nested parent
            if (nestedParentPrefabIds.Contains(parentPrefabId))
            {
                // make me a child.
                prefabChildIds.Add(myId);
            }
        }

        // mark all children of prefab, and their children
        for (int i = 0; i < transform.childCount; i++)
        {
            bool continueMarking = MarkFirstChildrenRecursive(transform.GetChild(i), parentPrefabId);
            if (!continueMarking)
            {
                // let siblings check? break if not.
                break;
            }
        }

        return true;
    }

    /// <summary>
    /// Called when Hierarchy runs the OnGUI for each element.
    /// After some testing, it turns out:
    ///     We define a set of OnGUI to be a sequence of calls for each one of the elements visible in the hierarchy at the moment.
    ///     Every mouseEnter, mouseExit, and mouseMove into the hierarchy pane, along with the startup and some other times, two sets of OnGUI are called in the same frame, then sometimes once again in the next frame.
    ///     Hard to know the reason, but probably unity wants to make sure the interfaces always look nice without too much glitching. 
    ///     Hopefully we can abuse this fact by making use of the visibleIds.Count, to determine when the second call is made.
    /// </summary>
    /// <param name="instanceID">instance of the gameObject being drawn. beware, it is not the transform.instanceid</param>
    /// <param name="rect">gui rect where the label is drawn.</param>
    private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect rect)
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (visibleInHierarchyIds == null)
        {
            visibleInHierarchyIds = new HashSet<int>();
        }

        // debug: how often is OnGUI called?
        //var isNew = !visibleInHierarchyIds.Contains(instanceID);

        visibleInHierarchyIds.Add(instanceID);
        a_visibleInHierarchyIdsChanged = true;
        unreliableVisibleObjectCount++;

        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        // debug: how often is OnGUI called?
        //if (isNew)
        //{
        //    Debug.Log(frameId + " (" + visibleInHierarchyIds.Count + ") Added new " + gameObject);
        //}
        //else
        //{
        //    Debug.Log(frameId + " (" + visibleInHierarchyIds.Count + ") Repeating " + gameObject);
        //}

        // sometimes the instanceID is a scene, not a gameObject. so check for null.
        if (gameObject != null)
        {
            // only setting parents on the very first HierarchyItem (using the unreliable counter heh) because the others will either have the same parents or be children of the first one, so they will be covered.
            if (unreliableVisibleObjectCount == 1)
            {
                // must set parents to visible, so we can set nested status on them. must set all siblings visible as well, and parents' siblings. in case we scrolled up and found some new siblings nobody knew about.
                SetVisibleParentsAndSiblings(gameObject);
            }
        }

        // only continue to draw icons if the nested prefab hashset has been setup. this happens on update, after a set of N OnHierarchyWindowItemOnGUI() has been called (one for each hierarchy item).
        if (b_nestedPrefabMarksReady)
        {
            bool drawIcon = false;
            bool drawPrefabIcon = false;
            bool drawPrefabIconPlus = false;
            bool drawPrefabIconWarning = false;
            //bool drawPrefabIconQuestion = false;

            if (gameObject != null)
            {
                var pt = GetPrefabType(gameObject);
                bool disconnected = pt == PrefabType.DisconnectedModelPrefabInstance || pt == PrefabType.DisconnectedPrefabInstance || pt == PrefabType.MissingPrefabInstance;

                if (disconnected)
                {
                    //drawPrefabIconQuestion = true;
                    drawIcon = true;
                }
                else if (nestedParentPrefabIds.Contains(instanceID))
                {
                    drawPrefabIconPlus = true;
                    drawIcon = true;
                }
                else if (regularPrefabIds.Contains(instanceID))
                {
                    drawPrefabIcon = true;
                    drawIcon = true;
                }
                else if (prefabChildIds.Contains(instanceID))
                {
                    drawPrefabIconWarning = true;
                    drawIcon = true;
                }

                var padding = 5f;
                var iconDrawRect = new Rect(
                                       rect.xMax - (iconWidth + padding),
                                       rect.yMin,
                                       rect.width,
                                       rect.height);
                if (drawIcon)
                {
                    EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconWidth));



                    // draw prefab icon in a label field heheh
                    EditorGUI.LabelField(iconDrawRect, drawPrefabIconPlus ? prefabIconPlusGuiContent : drawPrefabIcon ? prefabIconGuiContent : drawPrefabIconWarning ? prefabIconWarningGuiContent : prefabIconQuestionGuiContent);

                    GUI.color = Color.white;
                }

                //GUIStyle ggg = new GUIStyle(GUI.skin.label);
                //if (IsPrefabRoot(gameObject))
                //{
                //    ggg.normal.textColor = Color.green;
                //}

                //EditorGUI.LabelField(iconDrawRect, new GUIContent(((int)GetPrefabType(gameObject)).ToString()), ggg);

                // show modification count for prefabs?
                // warning: the mods count is minimum 8, even for prefabs fresh down from the project folder. This includes one line for x, y, z of localPosition and rotation of the prefab transform, one for the root scene order, etc.
                // not worth this overhead unless we do a clever solution that is more optimized... this modifications array with strings must be expensive.
#pragma warning disable 0162
                if (showAllPrefabChangeCounts || Selection.activeGameObject == gameObject)
                {
                    var mods = PrefabUtility.GetPropertyModifications(gameObject);
                    if (mods != null)
                    {
                        bool changed = mods.Length > 0;
                        if (changed && IsPrefabRoot(gameObject))
                        {
                            GUI.color = Color.red;
                            EditorGUI.LabelField(iconDrawRect, new GUIContent(mods.Length.ToString()));
                            GUI.color = Color.white;
                        }
                    }
                }
#pragma warning restore 0162 // Unreachable code detected
            }

        }
    }

    /// <summary>
    /// Sets all parents and siblings of this object as visible. It is a necessary operation because there is no way to determine if gameobjects are foldout/collapsed in the hierarchy.
    /// and we might be scrolling upwards and finding children that were not set as visible, and neither were their parents, and now they are visible but their parents not.
    /// </summary>
    /// <param name="gameObject">gameobject whose parents and siblings we must set visible</param>
    private static void SetVisibleParentsAndSiblings(GameObject gameObject)
    {
        Transform pointer = gameObject.transform;
        while (pointer != null)
        {
            // set all siblings first, if it has any.
            for (int i = 0; i < pointer.childCount; i++)
            {
                var id = pointer.GetChild(i).gameObject.GetInstanceID(); // BEWARE!!! GetInstanceID() is different for transform and transform.gameObject !!!!! :/
                visibleInHierarchyIds.Add(id);
                unreliableVisibleObjectCount++;
            }
            // now add parent itself
            visibleInHierarchyIds.Add(pointer.gameObject.GetInstanceID());
            unreliableVisibleObjectCount++;

            // now move to parent's parent, and set its siblings and itself and so on until the root
            pointer = pointer.parent;
        }
    }

    private static void OnUpdate()
    {
        // debug: how often is OnGUI called?
        //frameId++;

        // make sure only run Update when the OnGUI has changed stuff
        if (a_visibleInHierarchyIdsChanged)
        {
            a_visibleInHierarchyIdsChanged = false;
        }
        else
        {
            return;
        }

        // when the unreliable count has changed, it means we must update the nested prefab markers because something fishy is going on in the hierarchy.
        if (unreliableVisibleObjectCount != 0 && oldUnreliableVisibleObjectCount != unreliableVisibleObjectCount)
        {
            oldUnreliableVisibleObjectCount = unreliableVisibleObjectCount;
            MarkNestedPrefabs();

            EditorApplication.RepaintHierarchyWindow();
        }

        // reset to zero once per frame. even if the OnGUI happens 2*N times per frame, or 3 or 4 times N, 
        // it is required to update the item count when this count has changed, cause it's the only way to know the visibility has changed.
        unreliableVisibleObjectCount = 0;

    }

    /// <summary>
    /// Returns true when gameObject is the root of a prefab (as in, it is saved in the Project folder as a standalone prefab, not a child of other prefabs
    /// </summary>
    private static bool IsPrefabRoot(GameObject gameObject)
    {
        // prefab object is not null, and prefab root is self (not parent)
        return PrefabUtility.GetPrefabObject(gameObject) != null
            && PrefabUtility.FindPrefabRoot(gameObject) == gameObject;
    }

    private static PrefabType GetPrefabType(GameObject gameObject)
    {
        return PrefabUtility.GetPrefabType(gameObject);
    }

    /// <summary>
    /// Utility to see how many transforms in scene, so we know if we should even calculate this or it will lag too much... untested.
    /// </summary>
    /// <returns>count of transforms, in entire scene hierarchy.</returns>
    private static int GetTotalTransformCount()
    {
        int totalTransformCount = 0;

        // init root obj caches
        if (rootGameObjectCaches == null)
        {
            rootGameObjectCaches = new List<List<GameObject>>(SceneManager.sceneCount);
        }

        // check every scene
        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            var scene = SceneManager.GetSceneAt(sceneIndex);
            if (rootGameObjectCaches.Count <= sceneIndex)
            {
                rootGameObjectCaches.Add(new List<GameObject>(100));
            }
            var rootGameObjectCache = rootGameObjectCaches[sceneIndex];
            scene.GetRootGameObjects(rootGameObjectCache);
            for (int rootIndex = 0; rootIndex < rootGameObjectCache.Count; rootIndex++)
            {
                var rootGO = rootGameObjectCache[rootIndex];

                totalTransformCount += rootGO.transform.hierarchyCount;
            }
        }

        return totalTransformCount;
    }

}

/// <summary>
/// Window used in conjunction with the HierarchyPrefabPimp to show a list of modifications to a prefab instance
/// Copyright (lol) Horatiu Roman @ VRUnicorns, 2017
/// </summary>
public class HierarchyPimpModsWindow : EditorWindow
{
    private PropertyModification[] mods;
    private Vector2 scrollView;

    private GameObject targetInstance;
    private bool prefabRefs = false;

    public static HierarchyPimpModsWindow Get(GameObject targetObject)
    {
        var w = GetWindow<HierarchyPimpModsWindow>("Prefab Mods");
        w.SetTargetObject(targetObject);
        w.position = new Rect(w.position.x, w.position.y, Mathf.Max(w.position.width, 675), w.position.height);
        return w;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.ObjectField("Target Instance", targetInstance, typeof(GameObject), true, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(prefabRefs ? "Prefab refs" : "Instance refs", EditorStyles.miniButton, GUILayout.Width(113)))
            {
                prefabRefs = !prefabRefs;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        scrollView = EditorGUILayout.BeginScrollView(scrollView);

        if (mods == null || mods.Length == 0)
        {
            EditorGUILayout.LabelField("N/A");
        }
        else
        {
            for (int i = 0; i < mods.Length; i++)
            {
                var m = mods[i];
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(23));

                    // the object being modified
                    var target = m.target;
                    if (!prefabRefs)
                    {
                        target = GetInstanceTarget(target, targetInstance);
                    }
                    EditorGUILayout.ObjectField(GUIContent.none, target, typeof(UnityEngine.Object), true, GUILayout.ExpandWidth(false));

                    EditorGUILayout.LabelField(m.propertyPath);

                    // object value as string or object reference
                    if (string.IsNullOrEmpty(m.value))
                    {
                        EditorGUILayout.ObjectField(GUIContent.none, m.objectReference, typeof(UnityEngine.Object), true, GUILayout.ExpandWidth(false));
                    }
                    else
                    {
                        EditorGUILayout.TextField(GUIContent.none, m.value, GUILayout.ExpandWidth(true));
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Given a ref to an object in the hierarchy of a prefab, returns a reference to the respective object in the hierarchy of an instance of that prefab. Null if doesn't function.
    /// </summary>
    /// <param name="prefabTarget">Reference to an object in the hierarchy of a prefab</param>
    /// <param name="targetInstance">Root of the instance of the prefab we need a ref for</param>
    /// <returns>The instance reference relative to the instance, corresponding to the reference in the prefab</returns>
    private UnityEngine.Object GetInstanceTarget(UnityEngine.Object prefabTarget, GameObject targetInstance)
    {
        GameObject targetGo;
        if (prefabTarget is Component)
        {
            targetGo = (prefabTarget as Component).gameObject;
        }
        else if (prefabTarget is GameObject)
        {
            targetGo = prefabTarget as GameObject;
        }
        else
        {
            return null;
        }

        Stack<string> hier = new Stack<string>();
        //var prefabRoot = PrefabUtility.GetPrefabObject(targetGo);
        var monteCarlo = 255;
        while (targetGo.transform.parent != null && monteCarlo-- > 0)
        {
            hier.Push(targetGo.name);
            targetGo = targetGo.transform.parent.gameObject;
        }

        // we found the root! now find the instance
        targetGo = targetInstance;
        while (hier.Count > 0)
        {
            targetGo = targetGo.transform.Find(hier.Pop()).gameObject;
        }

        if (prefabTarget is Component)
        {
            var comp = targetGo.GetComponent(prefabTarget.GetType().Name);
            return comp;
        }
        return targetGo;
    }

    private void OnEnable()
    {
        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        if (Selection.activeGameObject != null && Selection.activeGameObject != targetInstance && Selection.gameObjects.Length == 1
            || mods == null || mods.Length == 0 || mods.Length != PrefabUtility.GetPropertyModifications(Selection.activeGameObject).Length)
        {
            SetTargetObject(Selection.activeGameObject);
            Repaint();
        }
    }

    private void SetTargetObject(GameObject go)
    {
        if (CanShowModifiedProps() || targetInstance == null)
        {
            targetInstance = go;
            this.mods = PrefabUtility.GetPropertyModifications(targetInstance);
        }
    }

    public static bool CanShowModifiedProps()
    {
        if (Selection.activeGameObject == null || Selection.gameObjects.Length != 1)
        {
            return false;
        }

        var pt = PrefabUtility.GetPrefabType(Selection.activeGameObject);
        if (pt == PrefabType.PrefabInstance || pt == PrefabType.ModelPrefabInstance || pt == PrefabType.MissingPrefabInstance || pt == PrefabType.DisconnectedPrefabInstance)
        {
            // only apply prefab instance roots.
            if (PrefabUtility.FindPrefabRoot(Selection.activeGameObject) == Selection.activeGameObject)
            {
                return true;
            }
        }

        return false;
    }

}

#endif