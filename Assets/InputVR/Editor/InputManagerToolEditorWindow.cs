using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class InputManagerToolEditorWindow : EditorWindow
{
    private string pathToInputSettingsAsset = "ProjectSettings/InputManager.asset";

    [SerializeField]
    private InputManagerData inputManagerData = new InputManagerData();

    // foldouts:
    [SerializeField]
    private bool axesFold = true;
    [SerializeField]
    private List<bool> axesEachFold = new List<bool>();
    private Vector2 scroll;

    // cerate axis
    int axisId;
    string axisName = "New Axis";
    float aGravity, aDead, aSensitivity;

    [MenuItem("Edit/Project Settings/Input (Horatius tool)", priority = -1)]
    public static InputManagerToolEditorWindow Get()
    {
        var w = EditorWindow.GetWindow<InputManagerToolEditorWindow>(false, "InputMan", true);
        w.Load();
        return w;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Manual Get from Input Manager"))
        {
            // manual reads data from the input manager file
            Load();
        }
        if (GUILayout.Button("Save", GUILayout.Width(74f)))
        {
            Save();
        }
        EditorGUILayout.EndHorizontal();

        // nicer tools to create inputs
        //OnGUI_CreateAxisInterface();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Axes and buttons for VR"))
        {
            CreateAxesAndButtonsForVR();
        }

        OnGUI_AddEveryButtonAndAxis_Button();

        if (GUILayout.Button("Clear All"))
        {
            if (EditorUtility.DisplayDialog("Really?", "Are you sure you wanna clear all input axes? Cannot undo!!!", "Kill 'em", "Cancel"))
            {
                inputManagerData.axes.Clear();
                Debug.Log("Cleared all axes (no undo)");
            }
            else
            {
                Debug.Log("Cancel clearing all axes.");
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        scroll = EditorGUILayout.BeginScrollView(scroll);

        OnGUI_DefaultInputManager();

        EditorGUILayout.EndScrollView();

    }

    private void CreateAxesAndButtonsForVR()
    {
        // ========================================== axes ==========================================
        var axes = inputManagerData.axes;

        // triggers (all)
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.TriggerLeft, 8));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.TriggerRight, 9));

        // both triggers, -1..1. also for xbox
        //axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.Triggers, 2));

        // grips (all vr)
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.GripLeft, 10));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.GripRight, 11));

        // vive touchpad
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.ViveTouchpadLeftX, 0));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.ViveTouchpadLeftY, 1));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.ViveTouchpadRightX, 3));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.ViveTouchpadRightY, 4));

        // oculus, windows, xbox analog stick
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.AnalogStickLeftX, 0));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.AnalogStickLeftY, 1));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.AnalogStickRightX, 3));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.AnalogStickRightY, 4));

        // windows touchpad
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.WindowsTouchpadLeftX, 16));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.WindowsTouchpadLeftY, 17));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.WindowsTouchpadRightX, 18));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.WindowsTouchpadRightY, 19));

        // oculus remote (probably samsung/daydream/mobile remotes)
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.OculusRemoteTouchpadX, 4));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRAxis(InputVRConst.OculusRemoteTouchpadY, 5));

        // ========================================== buttons ==========================================

        // triggers (all)
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.TriggerLeftTouch, 14));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.TriggerRightTouch, 15));

        // windows grip press. it works, but it's not needed
        //axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.WindowsGripLeftPress, 4));
        //axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.WindowsGripRightPress, 5));

        // vive touchpad
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.ViveTouchpadLeftTouch, 16));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.ViveTouchpadRightTouch, 17));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.ViveTouchpadLeftPress, 8));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.ViveTouchpadRightPress, 9));

        // windows touchpad
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.WindowsTouchpadLeftPress, 16));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.WindowsTouchpadRightPress, 17));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.WindowsTouchpadLeftTouch, 18));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.WindowsTouchpadRightTouch, 19));

        // windows analog stick
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.WindowsAnalogStickLeftPress, 8));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.WindowsAnalogStickRightPress, 9));

        // oculus analog stick (and xbox)
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusAnalogStickLeftTouch, 16));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusAnalogStickRightTouch, 17));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusAnalogStickLeftPress, 8));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusAnalogStickRightPress, 9));

        // windows application menu
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.WindowsApplicationMenuLeft, 6));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.WindowsApplicationMenuRight, 7));

        // oculus application menu (start in the official docs)
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusApplicationMenuLeft, 7));

        // vive application menu
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.ViveApplicationMenuLeft, 2));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.ViveApplicationMenuRight, 0));

        // oculus A B X Y buttons (like xbox plus touch)
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusAPress, 0));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusBPress, 1));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusXPress, 2));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusYPress, 3));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusATouch, 10));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusBTouch, 11));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusXTouch, 12));
        axes.Add(InputManagerData.InputManagerEntryData.CreateVRButton(InputVRConst.OculusYTouch, 13));

    }

    // interface for filling in all data about an axis. not implemented properly for now, not gonna happen unless feeling determined
    private void OnGUI_CreateAxisInterface()
    {
        var rect = EditorGUILayout.GetControlRect();
        {
            var buttonRect = rect;
            buttonRect.width /= 3f;
            // create axis?
            if (GUI.Button(buttonRect, "Create Axis"))
            {
                //var axes = inputManagerData.axes;
                var newAxis = new InputManagerData.InputManagerEntryData();
                newAxis.name = axisName;
                newAxis.type = 2;
                newAxis.axis = axisId;
                newAxis.joyNum = 0; // all

            }
            var idRect = rect;
            idRect.width /= 12; // 4/12
            idRect.x += rect.width / 3f;
            EditorGUI.LabelField(idRect, "Id");
            idRect.x += idRect.width; // 5/12
            axisId = EditorGUI.IntField(idRect, axisId);
            var axisRect = idRect;
            axisRect.x += axisRect.width; // 6/12
            axisRect.width *= 2;
            EditorGUI.LabelField(axisRect, "Axis");
            axisRect.x += axisRect.width; // 8/12
            axisRect.width *= 2;
            axisName = EditorGUI.TextField(axisRect, axisName);
        }
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Gravity, dead, sensitivity");
            aGravity = EditorGUILayout.FloatField(aGravity);
            aDead = EditorGUILayout.FloatField(aDead);
            aSensitivity = EditorGUILayout.FloatField(aSensitivity);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnGUI_AddEveryButtonAndAxis_Button()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Buttons 0..32 and Axis 0..32"))
        {
            var axes = this.inputManagerData.axes;
            for (int i = 0; i < 32; i++)
            {
                var clone = (axes.Last().Clone());
                clone.positiveButton = "joystick button " + i;
                clone.name = "Button " + i;
                axes.Add(clone);
            }
            for (int i = 0; i < 32; i++)
            {
                var clone = (axes.Last().Clone());
                clone.name = "Axis " + i;
                clone.type = 2; // joystick axis
                clone.axis = i;
                axes.Add(clone);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void OnGUI_DefaultInputManager()
    {
        EditorGUILayout.LabelField("The raw data", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.PrefixLabel("Raw header");
        this.inputManagerData.initData = EditorGUILayout.TextArea(this.inputManagerData.initData);

        axesFold = EditorGUILayout.Foldout(axesFold, "Axes");
        if (axesFold)
        {
            EditorGUI.indentLevel++;

            // Size 
            var axisCount = this.inputManagerData.axes.Count;
            var newCount = EditorGUILayout.DelayedIntField("Size", axisCount);

            // clamp ONCE at 128 because it's fucked up to add more than that.... unless you REALLY want to
            if (axisCount < 128)
            {
                newCount = Mathf.Clamp(newCount, 0, 128);
            }

            // resize
            // add more empty ones (duplicate last actually)
            while (newCount > inputManagerData.axes.Count)
            {
                var newone = inputManagerData.axes.Any() ? inputManagerData.axes.Last().Clone() : new InputManagerData.InputManagerEntryData();
                this.inputManagerData.axes.Add(newone);
            }

            // remove excess
            while (newCount < this.inputManagerData.axes.Count)
            {
                this.inputManagerData.axes.RemoveAt(inputManagerData.axes.Count - 1);
            }

            if (axesEachFold.Count != this.inputManagerData.axes.Count)
            {
                axesEachFold = new bool[this.inputManagerData.axes.Count].ToList();
            }

            // output all axes..??
            for (int i = 0; i < this.inputManagerData.axes.Count; i++)
            {
                var a = this.inputManagerData.axes[i];
                axesEachFold[i] = EditorGUILayout.Foldout(axesEachFold[i], a.name);
                if (axesEachFold[i])
                {
                    EditorGUI.indentLevel++;
                    a.name = EditorGUILayout.TextField("Name", a.name);
                    //a.serializedVersion = EditorGUILayout.TextField("Serialized Version", a.serializedVersion);
                    a.descriptiveName = EditorGUILayout.TextField("Descriptive Name", a.descriptiveName);
                    a.descriptiveNegativeName = EditorGUILayout.TextField("Descriptive Negative Name", a.descriptiveNegativeName);
                    a.negativeButton = EditorGUILayout.TextField("Negative Button", a.negativeButton);
                    a.positiveButton = EditorGUILayout.TextField("Positive Button", a.positiveButton);
                    a.altNegativeButton = EditorGUILayout.TextField("Alt Negative Button", a.altNegativeButton);
                    a.altPositiveButton = EditorGUILayout.TextField("Alt Positive Button", a.altPositiveButton);
                    a.gravity = EditorGUILayout.FloatField("Gravity", a.gravity);
                    a.dead = EditorGUILayout.FloatField("Dead", a.dead);
                    a.sensitivity = EditorGUILayout.FloatField("Sensitivity", a.sensitivity);
                    a.snap = EditorGUILayout.Toggle("Snap", a.snap);
                    a.invert = EditorGUILayout.Toggle("Invert", a.invert);
                    a.type = EditorGUILayout.Popup("Type", a.type, InputManagerData.typeStrings);
                    a.axis = EditorGUILayout.Popup("Axis", a.axis, InputManagerData.axisStrings);
                    a.joyNum = EditorGUILayout.Popup("Joy Num", a.joyNum, InputManagerData.joyNumStrings);
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.indentLevel--;
        }

    }

    private void Load()
    {
        this.inputManagerData = new InputManagerData();

        // no way to read Input settings via API. time to delve into files.
        StreamReader sr = new StreamReader(pathToInputSettingsAsset);

        bool foundAxes = false;
        InputManagerData.InputManagerEntryData curEntry = null;
        string initData = "";
        while (!sr.EndOfStream)
        {
            // parse l and start evaluating data into the data class.
            var l = sr.ReadLine();

            /* // Contents of the InputManager.asset file in a new project as of 2017.3
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!13 &1
InputManager:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Axes:
  - serializedVersion: 3
    m_Name: Horizontal
    descriptiveName: 
<... etc ...>
             */

            if (!foundAxes)
            {
                if (l.Contains("m_Axes"))
                {
                    foundAxes = true;
                    initData += l;
                    inputManagerData.initData = initData;
                    continue;
                }
                else
                {
                    initData += l + "\n";
                    continue;
                }
            }

            int wordStart = 0;
            while (wordStart < l.Length && (l[wordStart] == ' ' || l[wordStart] == '-'))
            {
                wordStart++;
            }
            // name is the substring from wordStart (first alphanumeric char), until the first ":" (so length is indexof(:) - wordStart).
            var colonPos = l.IndexOf(':', wordStart);
            var propName = l.Substring(wordStart, colonPos - wordStart);
            var value = l.Substring(colonPos + 2); // skip a position because the format is "  serializedVersion: 3" or "  descriptiveName: " when null.

            if (propName == "serializedVersion")
            {
                if (curEntry != null)
                {
                    inputManagerData.axes.Add(curEntry);
                }
                curEntry = new InputManagerData.InputManagerEntryData();
                curEntry.serializedVersion = value;
                continue;
            }
            /* // Contents of a single axis in the axes list in empty file
    - serializedVersion: 3
    m_Name: Horizontal
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: left
    positiveButton: right
    altNegativeButton: a
    altPositiveButton: d
    gravity: 3
    dead: 0.001
    sensitivity: 3
    snap: 1
    invert: 0
    type: 0
    axis: 0
    joyNum: 0
             */
            switch (propName)
            {
            case "m_Name":
                curEntry.name = value;
                break;
            case "descriptiveName":
                curEntry.descriptiveName = value;
                break;
            case "descriptiveNegativeName":
                curEntry.descriptiveNegativeName = value;
                break;
            case "negativeButton":
                curEntry.negativeButton = value;
                break;
            case "positiveButton":
                curEntry.positiveButton = value;
                break;
            case "altNegativeButton":
                curEntry.altNegativeButton = value;
                break;
            case "altPositiveButton":
                curEntry.altPositiveButton = value;
                break;
            case "gravity":
                curEntry.gravity = float.Parse(value);
                break;
            case "dead":
                curEntry.dead = float.Parse(value);
                break;
            case "sensitivity":
                curEntry.sensitivity = float.Parse(value);
                break;
            case "snap":
                curEntry.snap = value == "1" ? true : false;
                break;
            case "invert":
                curEntry.invert = value == "1" ? true : false;
                break;
            case "type":
                curEntry.type = int.Parse(value);
                break;
            case "axis":
                curEntry.axis = int.Parse(value);
                break;
            case "joyNum":
                curEntry.joyNum = int.Parse(value);
                break;

            }


        }

        if (curEntry != null)
        {
            inputManagerData.axes.Add(curEntry);
        }

        sr.Close();
    }

    private void Save()
    {
        if (inputManagerData != null)
        {
            var finalText = "";
            // make a backup (if it exists don't do shit cause we already backuped)
            var backupPath = pathToInputSettingsAsset.Insert(pathToInputSettingsAsset.LastIndexOf("."), " - backup");
            if (File.Exists(pathToInputSettingsAsset) && !File.Exists(backupPath))
            {
                File.Copy(pathToInputSettingsAsset, backupPath);
            }

            finalText += (inputManagerData.initData + "\n");

            // now write all the other shit
            for (int i = 0; i < inputManagerData.axes.Count; i++)
            {
                var a = inputManagerData.axes[i];
                finalText += ("  - " + "serializedVersion: " + a.serializedVersion + "\n");
                finalText += ("    " + "m_Name: " + a.name + "\n");
                finalText += ("    " + "descriptiveName: " + a.descriptiveName + "\n");
                finalText += ("    " + "descriptiveNegativeName: " + a.descriptiveNegativeName + "\n");
                finalText += ("    " + "negativeButton: " + a.negativeButton + "\n");
                finalText += ("    " + "positiveButton: " + a.positiveButton + "\n");
                finalText += ("    " + "altNegativeButton: " + a.altNegativeButton + "\n");
                finalText += ("    " + "altPositiveButton: " + a.altPositiveButton + "\n");
                finalText += ("    " + "gravity: " + a.gravity + "\n");
                finalText += ("    " + "dead: " + a.dead + "\n");
                finalText += ("    " + "sensitivity: " + a.sensitivity + "\n");
                finalText += ("    " + "snap: " + (a.snap ? 1 : 0) + "\n");
                finalText += ("    " + "invert: " + (a.invert ? 1 : 0) + "\n");
                finalText += ("    " + "type: " + a.type + "\n");
                finalText += ("    " + "axis: " + a.axis + "\n");
                finalText += ("    " + "joyNum: " + a.joyNum + "\n");
            }

            StreamWriter sw = new StreamWriter(pathToInputSettingsAsset, false);
            sw.Write(finalText);
            sw.Close();

        }

        // update assets since we wrote to a file
        AssetDatabase.Refresh();

        Debug.Log("[Success] Scroll your inspector to view newest changes after saving InputManager.asset");
    }

    [Serializable]
    public class InputManagerData
    {
        // contains string with a bunch of text copy pasted basically from a default version of InputManager.asset
        public string initData =
            "%YAML 1.1" + "\n" +
            "%TAG !u! tag:unity3d.com,2011:" + "\n" +
            "--- !u!13 &1" + "\n" +
            "InputManager:" + "\n" +
            "  m_ObjectHideFlags: 0" + "\n" +
            "  serializedVersion: 2" + "\n" +
            "  m_Axes:";

        public List<InputManagerEntryData> axes = new List<InputManagerEntryData>();

        [Serializable]
        public class InputManagerEntryData
        {
            public string name = "Axis 0";
            public string serializedVersion = "3";
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;
            public float gravity = 3;
            public float dead = 0.001f;
            public float sensitivity = 3;
            public bool snap = false;
            public bool invert = false;
            public int type = 0;    // ENUMS
            public int axis = 0;    // ENUMS
            public int joyNum = 0;  // ENUMS

            public InputManagerEntryData Clone()
            {
                return (InputManagerEntryData)MemberwiseClone();
            }

            public static InputManagerEntryData CreateVRAxis(string name, int axisId,
                float dead = 0.001f, float gravity = 0f)
            {
                return new InputManagerEntryData()
                {
                    name = name,
                    type = 2, // hardcode axis type
                    axis = axisId,

                    snap = false,
                    invert = false,
                    gravity = gravity, // in case gravity is needed when device resting (not tested)
                    dead = dead, // in case the device is so fucked we can't use it at rest, we have to apply a death value (not tested)
                    sensitivity = 1f, // MUST be non-zero for axes. otherwise axis never updates (tested)

                };
            }

            public static InputManagerEntryData CreateVRButton(string name, int buttonId)
            {
                return new InputManagerEntryData()
                {
                    name = name,
                    type = 0, // hardcode button type
                    positiveButton = "joystick button " + buttonId, // hardcode joystick button, plus id
                    // all other shit doesn't matter for buttons
                };
            }
        }

        public static readonly string[] typeStrings = new string[]
        {
            "Key or Mouse Button",
            "Mouse Movement",
            "Joystick Axis",
        };

        public static readonly string[] axisStrings = new string[]
        {
            "X axis",
            "Y axis",
             "3rd axis (Joysticks and Scrollwheel)",
             "4th axis (Joysticks)",
             "5th axis (Joysticks)",
             "6th axis (Joysticks)",
             "7th axis (Joysticks)",
             "8th axis (Joysticks)",
             "9th axis (Joysticks)",
            "10th axis (Joysticks)",
            "11th axis (Joysticks)",
            "12th axis (Joysticks)",
            "13th axis (Joysticks)",
            "14th axis (Joysticks)",
            "15th axis (Joysticks)",
            "16th axis (Joysticks)",
            "17th axis (Joysticks)",
            "18th axis (Joysticks)",
            "19th axis (Joysticks)",
            "20th axis (Joysticks)",
            "21st axis (Joysticks)",
            "22nd axis (Joysticks)",
            "23rd axis (Joysticks)",
            "24th axis (Joysticks)",
            "25th axis (Joysticks)",
            "26th axis (Joysticks)",
            "27th axis (Joysticks)",
            "28th axis (Joysticks)",
        };

        public static readonly string[] joyNumStrings = new string[]
        {
            "Get Motion from all Joysticks",
             "Joystick 1",
             "Joystick 2",
             "Joystick 3",
             "Joystick 4",
             "Joystick 5",
             "Joystick 6",
             "Joystick 7",
             "Joystick 8",
             "Joystick 9",
            "Joystick 10",
            "Joystick 11",
            "Joystick 12",
            "Joystick 13",
            "Joystick 14",
            "Joystick 15",
            "Joystick 16",
        };

    }
}