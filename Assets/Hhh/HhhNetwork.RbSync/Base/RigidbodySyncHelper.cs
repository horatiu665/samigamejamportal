namespace HhhNetwork.RbSync.Editor
{
#if UNITY_EDITOR
    using HhhNetwork.RbSync;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public static class RigidbodySyncHelper
    {
        [MenuItem("Build/Set All Synced Rigidbody IDs", false, 898)]
        public static void SetAllSyncedRigidbodies()
        {
            var objects = Object.FindObjectsOfType<RigidbodySyncComponent>();
            for (int i = 0; i < objects.Length; i++)
            {
                Undo.RecordObject(objects[i], "Set RigidbodySyncComponent SyncId");
                objects[i].SetSyncId(i + 1);
            }

            Debug.Log("Rigidbody Synchronization Helper:: Set sequential Sync IDs on " + objects.Length.ToString() + " objects in the current scene(s).");
            EditorSceneManager.MarkAllScenesDirty();
            //EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        }

        [MenuItem("Build/Set All Synced Rigidbody IDs", true)]
        public static bool SetAllSyncedRigidbodiesValidate()
        {
            return Object.FindObjectOfType<RigidbodySyncComponent>() != null;
        }
    }
#endif
}