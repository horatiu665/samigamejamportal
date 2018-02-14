namespace HhhNetwork.Build.Editor
{
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public sealed class BuildScriptWindow : EditorWindow
    {
        private BuildMode _currentMode = BuildMode.Singleplayer;

        private string _serverOnlyScene = string.Empty;
        private string _clientOnlyScene = string.Empty;
        private string _singleOnlyScene = string.Empty;

        private BuildTarget _buildTarget = BuildTarget.StandaloneWindows64;
        private string _buildFolder = string.Empty;
        private string _serverIp = string.Empty;
        private bool _buildAndRun = true;
        private bool _devBuild = false;
        private int _serverPort = -1;
        private bool _useLocal = true;
        private bool _useHetzner = false;
        private int _socketPort = -1;
        private bool _autoConnectOnEnable = false;

        private Vector2 _scroll;
        private ReorderableList _reorderableList;

        public SceneBuildManager buildManager
        {
            get
            {
                var manager = SceneBuildManager.instance;
                if (manager == null)
                {
                    var go = new GameObject("SceneBuildManager");
                    Undo.RegisterCreatedObjectUndo(go, "Created SceneBuildManager");

                    manager = Undo.AddComponent<SceneBuildManager>(go);
                    Undo.FlushUndoRecordObjects();

                    Debug.LogWarning("BUILD:: Creating new SceneBuildManager, since none were found in the scene!");
                    //EditorSceneManager.SaveOpenScenes();
                }

                return manager;
            }
        }

        [MenuItem("Build/Open Build Window")]
        public static void OpenBuildWindow()
        {
            // try to dock next to Game window as I like it -/H
            EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            var gameWindow = windows.FirstOrDefault(e => e.titleContent.text.Contains("Game"));

            if (gameWindow != null)
            {
                GetWindow<BuildScriptWindow>("Build Window", true, gameWindow.GetType());
            }
            else
            {
                GetWindow<BuildScriptWindow>("Build Window", true);
            }
        }

        private void OnEnable()
        {
            LoadPrefs();
            RefreshBuildScenes();
        }

        private void OnFocus()
        {
            LoadPrefs();
            RefreshBuildScenes();
        }

        public void LoadPrefs()
        {
            if (string.IsNullOrEmpty(_serverOnlyScene))
            {
                _serverOnlyScene = BuildScriptPrefs.GetServerOnlyScene();
            }

            if (string.IsNullOrEmpty(_clientOnlyScene))
            {
                _clientOnlyScene = BuildScriptPrefs.GetClientOnlyScene();
            }

            if (string.IsNullOrEmpty(_singleOnlyScene))
            {
                _singleOnlyScene = BuildScriptPrefs.GetSingleOnlyScene();
            }

            if (string.IsNullOrEmpty(_buildFolder))
            {
                _buildFolder = BuildScriptPrefs.GetBuildFolder();
            }

            if (string.IsNullOrEmpty(_serverIp))
            {
                _serverIp = BuildScriptPrefs.GetServerIP();
            }

            if (_serverPort == -1)
            {
                _serverPort = BuildScriptPrefs.GetServerPort();
            }

            if (_socketPort == -1)
            {
                _socketPort = BuildScriptPrefs.GetSocketPort();
            }

            _currentMode = BuildScriptPrefs.GetCurrentMode();
            _buildAndRun = BuildScriptPrefs.GetBuildAndRun();
            _useLocal = BuildScriptPrefs.GetUseLocal();
            _useHetzner = BuildScriptPrefs.GetUseHetzner();
            _devBuild = BuildScriptPrefs.GetDevelopmentBuild();
            _buildTarget = BuildScriptPrefs.GetBuildTarget();
            _autoConnectOnEnable = BuildScriptPrefs.GetAutoConnectOnEnable();
        }

        private void OnGUI()
        {
            _scroll = GUILayout.BeginScrollView(_scroll);

            // Tabs => choosing mode
            RenderModeToolbar();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // Build and run GUI for each mode
            RenderBuildAndRunning();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // Additively loaded scene settings
            RenderAdditivelyLoadedScene();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // Build settings scene setup => reorderable list
            RenderBuildSettings();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // Build folder
            RenderBuildFolder();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // Deployment
            RenderDeployment();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // Render the scene build manager
            RenderSceneBuildManager();

            GUILayout.EndScrollView();
        }

        private void RenderModeToolbar()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Singleplayer", "Switch to building for Singleplayer mode."), EditorStyles.miniButtonLeft))
            {
                SwitchMode(BuildMode.Singleplayer);
            }

            if (GUILayout.Button(new GUIContent("Client", "Switch to building for Client mode."), EditorStyles.miniButtonMid))
            {
                SwitchMode(BuildMode.Client);
            }

            if (GUILayout.Button(new GUIContent("Server", "Switch to building for Server mode."), EditorStyles.miniButtonRight))
            {
                SwitchMode(BuildMode.Server);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void RenderAdditivelyLoadedScene()
        {
            EditorGUILayout.LabelField(new GUIContent("Additively Loaded Scene Path", "In order to facilitate different modes of play (Multiplayer Client & Server + Singleplayer), a scene is additively loaded to the 'base' scene with the specifics for that mode of play. The path for that scene is found underneath."), EditorStyles.centeredGreyMiniLabel);

            if (_currentMode == BuildMode.Singleplayer)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                _singleOnlyScene = EditorGUILayout.TextField(new GUIContent("Singleplayer Only Scene:", "The path for the additively loaded scene used for the Singleplayer mode of play."), _singleOnlyScene, EditorStyles.textField);

                if (EditorGUI.EndChangeCheck())
                {
                    BuildScriptPrefs.SetSingleOnlyScene(_singleOnlyScene);
                }

                if (GUILayout.Button(new GUIContent("Browse", "Browse for scenes in the Unity project."), EditorStyles.miniButtonRight, GUILayout.Width(EditorGUIUtility.fieldWidth)))
                {
                    var sceneBrowse = BrowseScene();
                    if (!string.IsNullOrEmpty(sceneBrowse))
                    {
                        _singleOnlyScene = sceneBrowse;
                        BuildScriptPrefs.SetSingleOnlyScene(_singleOnlyScene);
                    }

                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.EndHorizontal();

                DrawLoadUnloadScene(_singleOnlyScene);
            }
            else if (_currentMode == BuildMode.Client)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                _clientOnlyScene = EditorGUILayout.TextField(new GUIContent("Client Only Scene:", "The path for the additively loaded scene used for the Client mode of play."), _clientOnlyScene, EditorStyles.textField);

                if (EditorGUI.EndChangeCheck())
                {
                    BuildScriptPrefs.SetClientOnlyScene(_clientOnlyScene);
                }

                if (GUILayout.Button(new GUIContent("Browse", "Browse for scenes in the Unity project."), EditorStyles.miniButtonRight, GUILayout.Width(EditorGUIUtility.fieldWidth)))
                {
                    var sceneBrowse = BrowseScene();
                    if (!string.IsNullOrEmpty(sceneBrowse))
                    {
                        _clientOnlyScene = sceneBrowse;
                        BuildScriptPrefs.SetClientOnlyScene(_clientOnlyScene);
                    }

                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.EndHorizontal();

                DrawLoadUnloadScene(_clientOnlyScene);
            }
            else if (_currentMode == BuildMode.Server)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                _serverOnlyScene = EditorGUILayout.TextField(new GUIContent("Server Only Scene:", "The path for the additively loaded scene used for the Server mode of play."), _serverOnlyScene, EditorStyles.textField);

                if (EditorGUI.EndChangeCheck())
                {
                    BuildScriptPrefs.SetServerOnlyScene(_serverOnlyScene);
                }

                if (GUILayout.Button(new GUIContent("Browse", "Browse for scenes in the Unity project."), EditorStyles.miniButtonRight, GUILayout.Width(EditorGUIUtility.fieldWidth)))
                {
                    var sceneBrowse = BrowseScene();
                    if (!string.IsNullOrEmpty(sceneBrowse))
                    {
                        _serverOnlyScene = sceneBrowse;
                        BuildScriptPrefs.SetServerOnlyScene(_serverOnlyScene);
                    }

                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.EndHorizontal();

                DrawLoadUnloadScene(_serverOnlyScene);
            }
        }

        private void DrawLoadUnloadScene(string scenePath)
        {
            if (Application.isPlaying)
            {
                return;
            }

            var existingScene = SceneManager.GetSceneByPath(scenePath);
            if (existingScene.IsValid() && existingScene.isLoaded)
            {
                if (GUILayout.Button(new GUIContent(string.Concat("Unload Scene \"", existingScene.name, "\""), "Press the button to unload the additively loaded scene now in the editor.")))
                {
                    EditorSceneManager.CloseScene(existingScene, true);
                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Additively Load Scene", "Press the button to additively load the set scene now in the editor.")))
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    GUIUtility.ExitGUI();
                }
            }
        }

        private void RenderBuildSettings()
        {
            if (_reorderableList == null)
            {
                _reorderableList = new ReorderableList(this.buildManager.scenes, typeof(SceneBuildItem), true, true, false, true);
                _reorderableList.drawHeaderCallback = ListHeaderCallback;
                _reorderableList.drawElementCallback = ListElementCallback;
                _reorderableList.onReorderCallback = ListReorderCallback;
                _reorderableList.onRemoveCallback = ListRemoveCallback;
                _reorderableList.onSelectCallback = ListSelectedCallback;
            }

            var buildScenes = EditorBuildSettings.scenes;
            if (this.buildManager.scenes.Count == 0 && buildScenes.Length > 0)
            {
                foreach (var scene in buildScenes)
                {
                    if (scene == null || string.IsNullOrEmpty(scene.path))
                    {
                        continue;
                    }

                    this.buildManager.scenes.Add(new SceneBuildItem()
                    {
                        path = scene.path
                    });
                }

                EditorUtility.SetDirty(this.buildManager);
            }

            EditorGUILayout.LabelField(new GUIContent("Build Settings Scenes", "This list replicates and modifies the Editor Build Settings automatically."), EditorStyles.centeredGreyMiniLabel);
            _reorderableList.DoLayoutList();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Add Current Scene", "Add the currently active main scene to the list of scenes, which in turn adds to the Editor Build Settings.")))
            {
                var scene = SceneManager.GetActiveScene();
                var scenePath = scene.path;
                if (string.IsNullOrEmpty(scenePath))
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    scene = SceneManager.GetActiveScene();
                    scenePath = scene.path;
                }

                AddScene(scenePath);
                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button(new GUIContent("Browse Scene", "Browse for scenes in the Unity project to add to the Editor Build Settings.")))
            {
                var path = BrowseScene();
                if (!string.IsNullOrEmpty(path))
                {
                    AddScene(path);
                }

                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button(new GUIContent("Refresh Scenes", "Refreshes the list of scenes shown here, and serialized in the SceneBuildManager, to match the Editor Build Settings' scenes list.")))
            {
                RefreshBuildScenes();
            }

            if (GUILayout.Button(new GUIContent("Clear All Scenes", "Clear the entire list of scenes here and in the Editor Build Settings.")))
            {
                Undo.RecordObject(this.buildManager, "Clearing scenes from build manager");
                this.buildManager.scenes.Clear();
                _reorderableList.list.Clear();
                EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];

                Undo.FlushUndoRecordObjects();
                EditorSceneManager.SaveOpenScenes();
                base.Repaint();
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void RefreshBuildScenes()
        {
            var buildManager = this.buildManager;
            buildManager.scenes.Clear();

            if (_reorderableList != null)
            {
                _reorderableList.list.Clear();
            }

            var buildScenes = EditorBuildSettings.scenes;
            foreach (var scene in buildScenes)
            {
                if (scene == null || string.IsNullOrEmpty(scene.path))
                {
                    continue;
                }

                buildManager.scenes.Add(new SceneBuildItem()
                {
                    path = scene.path
                });
            }
        }

        private void RenderBuildAndRunning()
        {
            EditorGUILayout.LabelField(new GUIContent("Building & Running (" + _currentMode.ToString() + ")", "Building and optionally auto-running builds."), EditorStyles.centeredGreyMiniLabel);

            EditorGUI.BeginChangeCheck();

            _buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(new GUIContent("Build Target", "Which platform to build for."), _buildTarget);

            if (EditorGUI.EndChangeCheck())
            {
                BuildScriptPrefs.SetBuildTarget(_buildTarget);
            }

            EditorGUI.BeginChangeCheck();

            _devBuild = EditorGUILayout.Toggle(new GUIContent("Development Build", "Toggle to build as development build."), _devBuild);

            if (EditorGUI.EndChangeCheck())
            {
                BuildScriptPrefs.SetDevelopmentBuild(_devBuild);
            }

            EditorGUI.BeginChangeCheck();
            _buildAndRun = EditorGUILayout.Toggle(new GUIContent("Build and Run?", "Whether to auto-run the game after building, if successful."), _buildAndRun);

            if (EditorGUI.EndChangeCheck())
            {
                BuildScriptPrefs.SetBuildAndRun(_buildAndRun);
            }

            // Actual mode pages
            switch (_currentMode)
            {
            case BuildMode.Singleplayer:
                {
                    OnSingleplayerGUI();
                    break;
                }

            case BuildMode.Client:
                {
                    OnClientGUI();
                    break;
                }

            case BuildMode.Server:
                {
                    OnServerGUI();
                    break;
                }
            }

            // vr mode
            using (var vrrow = new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = !Application.isPlaying;
                var vrSupport = EditorGUILayout.Toggle("VR Mode", PlayerSettings.virtualRealitySupported);
                if (!Application.isPlaying)
                {
                    PlayerSettings.virtualRealitySupported = vrSupport;
                }
                GUI.enabled = true;

                // buttons for VR platform definition - easier to access than player settings
                var supDevices = UnityEngine.XR.XRSettings.supportedDevices;
                var goodColor = Color.green;
                var badColor = Color.red;

                GUI.color = supDevices.Contains("OpenVR") ? goodColor : badColor;
                if (GUILayout.Button("OpenVR"))
                {
                    if (supDevices.Contains("OpenVR"))
                    {
                        // do nothing...
                    }
                    else
                    {
                        // add a new array containing only OpenVR
                        var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(this._buildTarget);
                        UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(buildTargetGroup, new string[] { "OpenVR" });
                    }
                }

                GUI.color = supDevices.Contains("Oculus") ? goodColor : badColor;
                if (GUILayout.Button("Oculus"))
                {
                    if (supDevices.Contains("Oculus"))
                    {
                        // do nothing...
                    }
                    else
                    {
                        // add a new array containing only Oculus
                        var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(this._buildTarget);
                        UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(buildTargetGroup, new string[] { "Oculus" });
                    }
                }

                GUI.color = Color.white;
            }

        }

        private void RenderBuildFolder()
        {
            EditorGUILayout.LabelField(new GUIContent("Build Folder", "The folder to place builds in."), EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            _buildFolder = EditorGUILayout.TextField(new GUIContent("Build Folder:", "The folder to place builds in."), _buildFolder);

            if (EditorGUI.EndChangeCheck())
            {
                BuildScriptPrefs.SetBuildFolder(_buildFolder);
            }

            if (GUILayout.Button(new GUIContent("Browse", "Browse for folders in the project."), EditorStyles.miniButtonRight, GUILayout.Width(EditorGUIUtility.fieldWidth)))
            {
                SetBuildFolder();
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void RenderDeployment()
        {
            EditorGUILayout.LabelField(new GUIContent("Server Deployment", "Automatic deploment to Hetzner server using WinSCP."), EditorStyles.centeredGreyMiniLabel);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset Online Server"))
            {
                DeployScripts.ResetServers();
                GUIUtility.ExitGUI();
            }

            if (DeployScripts.DeployServersValidation())
            {
                if (GUILayout.Button("Deploy to Hetzner"))
                {
                    DeployScripts.DeployServers();
                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Please make sure that you have built a server. Expecting to find server executable at path = " + DeployScripts.GetServerFilePath(), MessageType.Error);
            }

            if (GUILayout.Button("Build and Deploy"))
            {
                BuildScripts.BuildServer64(_serverOnlyScene, _buildFolder, false, BuildTarget.StandaloneLinux64, _socketPort);
                DeployScripts.DeployServers();
                GUIUtility.ExitGUI();
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Link to deployment google doc"))
            {
                Application.OpenURL(@"https://docs.google.com/document/d/1Ecy0n1JQWaICvW68l0zxcmgtMhjvU3e5twXNqi2jj_Q");
            }
        }

        private void RenderSceneBuildManager()
        {
            EditorGUILayout.LabelField(new GUIContent("Scene Build Manager", "The game object used to store (serialize) the list of Scene Build Items."), EditorStyles.centeredGreyMiniLabel);

            var instance = SceneBuildManager.instance;
            GUI.enabled = false;

            EditorGUILayout.ObjectField(new GUIContent("Scene Build Manager", "The current reference to the Scene Build Manager singleton, if it exists."), instance, typeof(SceneBuildManager), false);

            GUI.enabled = true;

            if (instance == null)
            {
                if (GUILayout.Button(new GUIContent("Generate", "Press this button to generate the Scene Build Manager now. It is also automatically generated when needed.")))
                {
                    var mgr = this.buildManager;
                    if (mgr == null)
                    {
                        Debug.LogError(this.ToString() + " generation of build manager somehow failed");
                    }

                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                var count = instance.scenes.Count;
                EditorGUILayout.LabelField("Scene Build Manager currently has " + count.ToString() + " registered " + (count == 1 ? "scene" : "scenes") + ".", EditorStyles.wordWrappedLabel);
            }
        }

        private void SwitchMode(BuildMode mode)
        {
            _currentMode = mode;
            BuildScriptPrefs.SetCurrentMode(_currentMode);

            base.Repaint();
            GUIUtility.ExitGUI();
        }

        private string SetBuildFolder()
        {
            var fullPath = EditorUtility.OpenFolderPanel("Build Folder", Application.dataPath.Replace("Assets", string.Empty), "bin");
            if (!string.IsNullOrEmpty(fullPath))
            {
                _buildFolder = fullPath;
                BuildScriptPrefs.SetBuildFolder(_buildFolder);
                base.Repaint();
            }

            return fullPath;
        }

        private string BrowseScene()
        {
            var fullPath = EditorUtility.OpenFilePanelWithFilters("Browse Scenes", Path.Combine(Application.dataPath, "Scenes"), new string[] { "Scene Files", "unity" });
            var path = fullPath.Substring(fullPath.IndexOf("Assets"));
            if (!string.IsNullOrEmpty(path))
            {
                var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (scene != null)
                {
                    if (!Path.HasExtension(path))
                    {
                        path += ".unity";
                    }

                    return path;
                }
                else
                {
                    Debug.LogWarning(this.ToString() + " please make sure you select a valid scene!");
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        private void ListHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Build Scenes", EditorStyles.boldLabel);
        }

        private void ListElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var scenes = this.buildManager.scenes;
            if (index < 0 || index >= scenes.Count)
            {
                return;
            }

            var scene = scenes[index];

            EditorGUI.LabelField(rect, new GUIContent(scene.path));

        }

        private void ListSelectedCallback(ReorderableList list)
        {
            var scenes = this.buildManager.scenes;
            var index = list.index;
            if (index < 0 || index >= scenes.Count)
            {
                return;
            }

            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenes[index].path);
            EditorGUIUtility.PingObject(scene);
        }

        private void ListReorderCallback(ReorderableList list)
        {
            UpdateEditorBuildSettings();
        }

        private void ListRemoveCallback(ReorderableList list)
        {
            var selectedIndex = list.index;
            if (selectedIndex < 0 || selectedIndex >= list.count)
            {
                return;
            }

            list.list.RemoveAt(selectedIndex);
            UpdateEditorBuildSettings();
        }

        private void UpdateEditorBuildSettings()
        {
            var scenes = this.buildManager.scenes;
            var count = scenes.Count;
            var buildScenes = new EditorBuildSettingsScene[count];
            for (int i = 0; i < count; i++)
            {
                var sceneData = scenes[i];
                if (sceneData == null)
                {
                    continue;
                }

                var scenePath = sceneData.path;
                if (string.IsNullOrEmpty(scenePath))
                {
                    continue;
                }

                buildScenes[i] = new EditorBuildSettingsScene(scenePath, true);
            }

            EditorBuildSettings.scenes = buildScenes;
        }

        private void AddScene(string path)
        {
            AddScene(new SceneBuildItem()
            {
                path = path
            });
        }

        private void AddScene(SceneBuildItem scene)
        {
            var scenes = this.buildManager.scenes;
            var count = scenes.Count;
            for (int i = 0; i < count; i++)
            {
                if (string.Equals(scenes[i].path, scene.path))
                {
                    Debug.LogWarning("BUILD:: cannot add duplicate scenes!");
                    return;
                }
            }

            _reorderableList.list.Add(scene);
            UpdateEditorBuildSettings();

            EditorUtility.SetDirty(this.buildManager);
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            base.Repaint();
        }

        private void OnSingleplayerGUI()
        {
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Build for Singleplayer"))
            {
                if (!Directory.Exists(_buildFolder))
                {
                    SetBuildFolder();
                }

                if (_buildAndRun)
                {
                    BuildScripts.BuildAndRunSingleplayer64(_singleOnlyScene, _buildFolder, _devBuild, _buildTarget);
                }
                else
                {
                    BuildScripts.BuildSingleplayer64(_singleOnlyScene, _buildFolder, _devBuild, _buildTarget);
                }

                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Run as Singleplayer"))
            {
                BuildScripts.PlayAsSingleplayer(_singleOnlyScene);
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OnClientGUI()
        {
            EditorGUI.BeginChangeCheck();
            _autoConnectOnEnable = EditorGUILayout.Toggle(new GUIContent("Auto Connect On Enable?", "Whether the ClientNetSender automatically connects on enable to the set IP and port."), _autoConnectOnEnable);

            if (EditorGUI.EndChangeCheck())
            {
                BuildScriptPrefs.SetAutoConnectOnEnable(_autoConnectOnEnable);
            }

            if (_autoConnectOnEnable)
            {
                EditorGUI.BeginChangeCheck();
                _useLocal = EditorGUILayout.Toggle(new GUIContent("Connect to Localhost?", "Whether the client should connect to a localhost (127.0.0.1), or another custom IP address."), _useLocal);

                if (EditorGUI.EndChangeCheck())
                {
                    if (_useHetzner && _useLocal)
                    {
                        _useHetzner = false;
                        BuildScriptPrefs.SetUseHetzner(_useHetzner);
                    }

                    BuildScriptPrefs.SetUseLocal(_useLocal);
                }

                EditorGUI.BeginChangeCheck();
                _useHetzner = EditorGUILayout.Toggle(new GUIContent("Connect to Hetzner Server?", "Whether the client should connect to the hetzner remote server (88.198.75.133), or another custom IP address."), _useHetzner);

                if (EditorGUI.EndChangeCheck())
                {
                    if (_useLocal && _useHetzner)
                    {
                        _useLocal = false;
                        BuildScriptPrefs.SetUseLocal(_useLocal);
                    }

                    BuildScriptPrefs.SetUseHetzner(_useHetzner);
                }

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(new GUIContent("Server Address:", "The IP + port address of the server, which the client should connect to."), GUILayout.Width(EditorGUIUtility.labelWidth));

                if (!_useLocal && !_useHetzner)
                {
                    EditorGUI.BeginChangeCheck();
                    _serverIp = EditorGUILayout.TextField(_serverIp);

                    if (EditorGUI.EndChangeCheck())
                    {
                        BuildScriptPrefs.SetServerIP(_serverIp);
                    }
                }
                else if (_useLocal)
                {
                    EditorGUILayout.SelectableLabel(BuildConstants.localhost);
                }
                else if (_useHetzner)
                {
                    EditorGUILayout.SelectableLabel(BuildConstants.hetznerIp);
                }

                EditorGUI.BeginChangeCheck();
                _serverPort = EditorGUILayout.IntField(_serverPort);

                if (EditorGUI.EndChangeCheck())
                {
                    BuildScriptPrefs.SetServerPort(_serverPort);
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                _serverPort = EditorGUILayout.IntField(new GUIContent("Server Port: ", "The server port to connect to. Should be the same port as on the server ;)"), _serverPort);

                if (EditorGUI.EndChangeCheck())
                {
                    BuildScriptPrefs.SetServerPort(_serverPort);
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Build for Client"))
            {
                if (!Directory.Exists(_buildFolder))
                {
                    SetBuildFolder();
                }

                if (_useLocal)
                {
                    if (_buildAndRun)
                    {
                        BuildScripts.BuildAndRunClientLocal(_clientOnlyScene, _buildFolder, _devBuild, _buildTarget, _serverPort, _autoConnectOnEnable);
                    }
                    else
                    {
                        BuildScripts.BuildClientLocal(_clientOnlyScene, _buildFolder, _devBuild, _buildTarget, _serverPort, _autoConnectOnEnable);
                    }
                }
                else if (_useHetzner)
                {
                    if (_buildAndRun)
                    {
                        BuildScripts.BuildAndRunForHetzner(_clientOnlyScene, _buildFolder, _devBuild, _buildTarget, _serverPort, _autoConnectOnEnable);
                    }
                    else
                    {
                        BuildScripts.BuildClientForHetzner(_clientOnlyScene, _buildFolder, _devBuild, _buildTarget, _serverPort, _autoConnectOnEnable);
                    }
                }
                else
                {
                    if (_buildAndRun)
                    {
                        BuildScripts.BuildAndRunClientForIP(_serverIp, _serverPort, _clientOnlyScene, _buildFolder, _devBuild, _buildTarget, _autoConnectOnEnable);
                    }
                    else
                    {
                        BuildScripts.BuildClientForIP(_serverIp, _serverPort, _clientOnlyScene, _buildFolder, _devBuild, _buildTarget, _autoConnectOnEnable);
                    }
                }

                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Run as Client"))
            {
                if (_useLocal)
                {
                    BuildScripts.PlayAsClientLocal(_serverPort, _clientOnlyScene, _autoConnectOnEnable);
                }
                else if (_useHetzner)
                {
                    BuildScripts.PlayAsHetzner(_serverPort, _clientOnlyScene, _autoConnectOnEnable);
                }
                else
                {
                    BuildScripts.PlayAsClientForIP(_serverIp, _serverPort, _clientOnlyScene, _autoConnectOnEnable);
                }

                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OnServerGUI()
        {
            EditorGUI.BeginChangeCheck();
            _socketPort = EditorGUILayout.IntField(new GUIContent("Socket Port", "The socket port used to host this server through."), _socketPort);

            if (EditorGUI.EndChangeCheck())
            {
                BuildScriptPrefs.SetSocketPort(_socketPort);
            }

            EditorGUILayout.Separator();

            var ip = IPUtils.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            if (!string.IsNullOrEmpty(ip))
            {
                EditorGUILayout.DelayedTextField(new GUIContent("Ethernet IP v4 Address:", "Automatically identified local IP V4 address for Ethernet (Read Only)."), ip);
            }

            ip = IPUtils.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            if (!string.IsNullOrEmpty(ip))
            {
                EditorGUILayout.DelayedTextField(new GUIContent("Wireless IP v4 Address:", "Automatically identified local IP V4 address for Wireless (Read Only)."), ip);
            }

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Build for Server"))
            {
                if (!Directory.Exists(_buildFolder))
                {
                    SetBuildFolder();
                }

                if (_buildAndRun)
                {
                    BuildScripts.BuildAndRunServer64(_serverOnlyScene, _buildFolder, _devBuild, _buildTarget, _socketPort);
                }
                else
                {
                    BuildScripts.BuildServer64(_serverOnlyScene, _buildFolder, _devBuild, _buildTarget, _socketPort);
                }

                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Run as Server"))
            {
                BuildScripts.PlayAsServer(_serverOnlyScene, _socketPort);
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}