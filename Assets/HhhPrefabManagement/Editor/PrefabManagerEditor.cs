namespace HhhPrefabManagement.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;
    using UnityEditor;
    using System;
    using System.IO;
    using System.Text;

    [CustomEditor(typeof(PrefabManager))]
    public class PrefabManagerEditor : Editor
    {
        public const string FILE_NAME = "PrefabTypeEnumGen.cs";

        protected SerializedProperty _prefabTypeMode, _prefabs;
        protected FileInfo _prefabFile;

        protected virtual void OnEnable()
        {
            _prefabTypeMode = this.serializedObject.FindProperty("_prefabTypeMode");
            _prefabs = this.serializedObject.FindProperty("_prefabs");
        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.HelpBox("The PrefabType enum is NOT generated during runtime, since that would trigger a recompilation of the project and thus break the execution. " +
                    "Therefore, the PrefabManager cannot be edited at runtime.", MessageType.Warning);
                GUI.enabled = false;
            }
            
            // eval prev prefabs array
            bool prefabArrayChanged = false;
            // calculate hash?
            var hash = CalculatePrefabsHash();

            base.OnInspectorGUI();

            // conclude if prefabs array has changed at all
            var newHash = CalculatePrefabsHash();
            prefabArrayChanged = newHash != hash;

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Remember: Do not reference PrefabType items in code! The references might break when the PrefabType enum is regenerated.", MessageType.Info);
            }

            if (!Application.isPlaying)
            {
                if (prefabArrayChanged || GUILayout.Button(new GUIContent("Generate enum PrefabType", "Generate the PrefabType enum on command. The file is also automatically generated when a change is detected.")))
                {
                    GenerateVRPrefabTypeEnum();
                }
            }
            else
            {
                GUI.enabled = true;
            }

            // show pool data...?

        }

        // shitty way to decide if prefabs list has changed
        private long CalculatePrefabsHash()
        {
            long hash = 0;
            for (int i = 0; i < _prefabs.arraySize; i++)
            {
                var item = _prefabs.GetArrayElementAtIndex(i);
                var charIndex = 1;
                var charSum = item.name.Sum(c => (int)c * charIndex * 317);
                var id = item.objectReferenceInstanceIDValue;
                hash += charSum + id;
            }

            return hash;
        }

        protected virtual void GenerateVRPrefabTypeEnum()
        {
            var prefabTypeMode = ((PrefabTypeMode[])Enum.GetValues(typeof(PrefabTypeMode)))[_prefabTypeMode.enumValueIndex];
            EnsureMaxPrefabsLength(prefabTypeMode);

            var prefabFile = LocatePrefabTypeFile();
            if (prefabFile != null)
            {
                var types = GeneratePrefabTypeContents();
                WritePrefabTypeFile(prefabTypeMode, prefabFile, types);
            }
        }

        protected virtual void EnsureMaxPrefabsLength(PrefabTypeMode mode)
        {
            // do not allow prefabs to surpass max value
            var max = 0;
            switch (mode)
            {
            case PrefabTypeMode.Byte:
                {
                    max = byte.MaxValue;
                    break;
                }

            case PrefabTypeMode.Short:
                {
                    max = short.MaxValue;
                    break;
                }

            case PrefabTypeMode.Int:
                {
                    max = int.MaxValue;
                    break;
                }

            default:
                {
                    throw new NotImplementedException(this.ToString() + " VRPrefabTypeMode unhandled type == " + mode.ToString());
                }
            }

            if (_prefabs.arraySize > max)
            {
                Debug.LogWarning(this.ToString() + " does not support more than " + max.ToString() + " prefabs, due to the currently selected prefab type mode (" + mode.ToString() + "). Please change the VRPrefab Type Mode first if you need more prefabs than the current maximum. Removing the last " + (_prefabs.arraySize - max).ToString() + " prefabs from array.");

                for (int i = _prefabs.arraySize - 1; i >= max; i--)
                {
                    _prefabs.DeleteArrayElementAtIndex(i);
                }
            }
        }

        protected virtual string GeneratePrefabTypeContents()
        {
            var prefabEnumEntries = new List<string>();
            var prefabIntValues = new List<int>();
            for (int i = 0; i < _prefabs.arraySize; i++)
            {
                var prefab = _prefabs.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
                if (prefab == null)
                {
                    continue;
                }

                // Remove whitespace at start and end of string, as well as a range of illegal characters
                var name = prefab.name.Trim()
                    .Replace(" ", string.Empty)
                    .Replace("-", string.Empty)
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty)
                    .Replace("}", string.Empty)
                    .Replace("{", string.Empty)
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty)
                    .Replace(".", string.Empty)
                    .Replace(",", string.Empty)
                    .Replace("@", string.Empty)
                    .Replace("#", string.Empty)
                    .Replace("!", string.Empty)
                    .Replace(";", string.Empty)
                    .Replace(":", string.Empty);

                if (string.IsNullOrEmpty(name))
                {
                    Debug.LogWarning(this.ToString() + " the generated prefab name, for prefab == " + prefab.ToString() + ", is empty, and thus cannot be used. Please rename!");
                    continue;
                }

                if (IsReservedWord(name))
                {
                    Debug.LogWarning(this.ToString() + " the generated prefab name (\"" + name + "\") is a reserved word, and thus cannot be used. Please rename!");
                    continue;
                }

                if (!System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(name))
                {
                    Debug.LogWarning(this.ToString() + " the generated prefab name (\"" + name + "\") is not a valid language independent identifier, and thus cannot be used. Please rename!");
                    continue;
                }

                if (prefabEnumEntries.Contains(name))
                {
                    Debug.LogWarning(this.ToString() + " the generated prefab name (\"" + name + "\"), for prefab == " + prefab.ToString() + ", is duplicated, another prefab already uses the same name. Please rename!");
                    continue;
                }

                prefabEnumEntries.Add(name);
                prefabIntValues.Add(i);
            }

            var count = prefabEnumEntries.Count;
            var sb = new StringBuilder(count * sizeof(char) * 7); // assuming average name length is 7 characters... doesn't matter, it's just string capacity
            for (int i = 0; i < count; i++)
            {
                sb.AppendFormat(Template.ItemTemplate, prefabEnumEntries[i], prefabIntValues[i]);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        // this is a lame attempt to replace Apex.EditorUtilities.IsReservedWord(string name); but we don't want to depend on Apex
        private bool IsReservedWord(string name)
        {
            switch (name)
            {
            case "bool":
            case "int":
            case "string":
            case "class":
            case "if":
            case "for":
            case "while":
            case "typeof":
            case "do":
            case "as":
            case "is":
            case "true":
            case "false":
            case "struct":
            case "switch":
            case "case":
            case "return":
                return true;
            }
            return false;
        }

        protected virtual FileInfo LocatePrefabTypeFile()
        {
            if (_prefabFile == null)
            {
                // only search for prefab file if we don't have a cached version
                var di = new DirectoryInfo(Application.dataPath);
                var results = di.GetFiles(FILE_NAME, SearchOption.AllDirectories);
                if (results.Length == 0)
                {
                    Debug.LogError(this.ToString() + " could not find any " + FILE_NAME + " file.");
                    return null;
                }

                if (results.Length > 1)
                {
                    Debug.LogWarning(this.ToString() + " more than one " + FILE_NAME + " file found in project. Ensure that there is only one!");
                }

                _prefabFile = results[0];
            }

            return _prefabFile;
        }

        protected virtual void WritePrefabTypeFile(PrefabTypeMode prefabTypeMode, FileInfo file, string rawContents)
        {
            try
            {
                var prefabTypeModeStr = prefabTypeMode.ToString().ToLower();
                var contents = string.Format(Template.FileTemplate, rawContents, prefabTypeModeStr).Replace("{{", "{").Replace("}}", "}");
                using (var stream = file.Open(FileMode.Create))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(contents);
                }

                AssetDatabase.Refresh();
                Debug.Log(this.ToString() + " Updated " + FILE_NAME);
            }
            catch (Exception e)
            {
                Debug.LogError(this.ToString() + " error in attempting to update " + FILE_NAME + ": " + e.Message.ToString() + "\n" + e.StackTrace.ToString());
            }
        }

        protected class Template
        {
            public const string ItemTemplate = "\t\t{0} = {1},";
            public const string FileTemplate = @"//------------------------------------------------------------------------------
// <auto-generated>
// This enum was auto generated by HhhPrefabManager.
// </auto-generated>
//------------------------------------------------------------------------------
namespace HhhPrefabManagement
{{
    /// <summary>
    /// Auto-generated enum representing all prefabs spawned by the HhhPrefabManager
    /// </summary>
    public enum PrefabType : {1}
    {{
{0}
    }}
}}";
        }
    }
}