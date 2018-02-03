namespace HhhNetwork.Build.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using HhhNetwork;

    public static class BuildScripts
    {
        public static string gameName
        {
            get
            {
                return PlayerSettings.productName;
            }
        }
        public static string singlePlayerBuildName { get { return gameName + "Singleplayer"; } }
        public static string clientBuildName { get { return gameName + "Client"; } }
        public static string serverBuildName { get { return gameName + "Server"; } }

        private const int _defaultPort = 8080;

        /* SINGLEPLAYER */

        public static void BuildSingleplayer64(string singleOnlyScene, string buildFolder, bool devBuild, BuildTarget target)
        {
            BuildSingleplayer(singleOnlyScene, buildFolder, target, BuildOptions.ShowBuiltPlayer, devBuild);
        }

        public static void BuildAndRunSingleplayer64(string singleOnlyScene, string buildFolder, bool devBuild, BuildTarget target)
        {
            BuildSingleplayer(singleOnlyScene, buildFolder, target, BuildOptions.ShowBuiltPlayer | BuildOptions.AutoRunPlayer, devBuild);
        }

        public static void PlayAsSingleplayer(string singleOnlyScene)
        {
            Run(singleOnlyScene);
        }

        /* SERVER */

        public static void BuildServer64(string serverOnlyScene, string buildFolder, bool devBuild, BuildTarget target, int socketPort)
        {
            BuildServer(serverOnlyScene, buildFolder, target, BuildOptions.ShowBuiltPlayer, devBuild, socketPort);
        }

        public static void BuildAndRunServer64(string serverOnlyScene, string buildFolder, bool devBuild, BuildTarget target, int socketPort)
        {
            BuildServer(serverOnlyScene, buildFolder, target, BuildOptions.ShowBuiltPlayer | BuildOptions.AutoRunPlayer, devBuild, socketPort);
        }

        public static void PlayAsServer(string serverOnlyScene, int socketPort)
        {
            SetServerNetSenderSocketAddress(serverOnlyScene, socketPort);
            Run(serverOnlyScene);
        }

        /* CLIENT */

        public static void BuildAndRunForHetzner(string clientOnlyScene, string buildFolder, bool devBuild, BuildTarget target, int port, bool autoConnect)
        {
            BuildClient(clientOnlyScene, buildFolder, target, BuildOptions.ShowBuiltPlayer | BuildOptions.AutoRunPlayer, BuildConstants.hetznerIp, port, devBuild, autoConnect);
        }

        public static void BuildClientForHetzner(string clientOnlyScene, string buildFolder, bool devBuild, BuildTarget target, int port, bool autoConnect)
        {
            BuildClient(clientOnlyScene, buildFolder, target, BuildOptions.ShowBuiltPlayer, BuildConstants.hetznerIp, port, devBuild, autoConnect);
        }

        public static void BuildAndRunClientLocal(string clientOnlyScene, string buildFolder, bool devBuild, BuildTarget target, int port, bool autoConnect)
        {
            BuildClient(clientOnlyScene, buildFolder, target, BuildOptions.ShowBuiltPlayer | BuildOptions.AutoRunPlayer, BuildConstants.localhost, port, devBuild, autoConnect);
        }

        public static void BuildClientLocal(string clientOnlyScene, string buildFolder, bool devBuild, BuildTarget target, int port, bool autoConnect)
        {
            BuildClient(clientOnlyScene, buildFolder, target, BuildOptions.ShowBuiltPlayer, BuildConstants.localhost, port, devBuild, autoConnect);
        }

        public static void BuildAndRunClientForIP(string ip, int port, string clientOnlyScene, string buildFolder, bool devBuild, BuildTarget target, bool autoConnect)
        {
            BuildClient(clientOnlyScene, buildFolder, target, BuildOptions.ShowBuiltPlayer | BuildOptions.AutoRunPlayer, ip, port, devBuild, autoConnect);
        }

        public static void BuildClientForIP(string ip, int port, string clientOnlyScene, string buildFolder, bool devBuild, BuildTarget target, bool autoConnect)
        {
            BuildClient(clientOnlyScene, buildFolder, target, BuildOptions.ShowBuiltPlayer, ip, port, devBuild, autoConnect);
        }

        public static void PlayAsHetzner(int port, string clientOnlyScene, bool autoConnect)
        {
            RunClient(clientOnlyScene, BuildConstants.hetznerIp, port, autoConnect);
        }

        public static void PlayAsClientLocal(int port, string clientOnlyScene, bool autoConnect)
        {
            RunClient(clientOnlyScene, BuildConstants.localhost, port, autoConnect);
        }

        public static void PlayAsClientForIP(string ip, int port, string clientOnlyScene, bool autoConnect)
        {
            RunClient(clientOnlyScene, ip, port, autoConnect);
        }

        /* HELPERS */

        private static void BuildSingleplayer(string singleOnlyScene, string buildFolder, BuildTarget target, BuildOptions options, bool devBuild)
        {
            Build(singleOnlyScene, singlePlayerBuildName, buildFolder, target, options, ForceVRSupportedModes.NotForced, devBuild);
        }

        private static void BuildServer(string serverOnlyScene, string buildFolder, BuildTarget target, BuildOptions options, bool devBuild, int socketPort)
        {
            Build(serverOnlyScene, serverBuildName, buildFolder, target, options, ForceVRSupportedModes.ForcedFalse, devBuild, socketPort);
        }

        private static void BuildClient(string clientOnlyScene, string buildFolder, BuildTarget target, BuildOptions options, string ip, int port, bool devBuild, bool autoConnect)
        {
            SetAutoConnectOnEnable(clientOnlyScene, autoConnect);
            Build(clientOnlyScene, clientBuildName, buildFolder, target, options, ForceVRSupportedModes.NotForced, devBuild, -1, ip, port);
        }

        private static void RunClient(string extraScenePath, string ip, int port, bool autoConnect)
        {
            AddExtraScene(extraScenePath);

            SetAutoConnectOnEnable(extraScenePath, autoConnect);
            SetClientNetSenderServerAddress(extraScenePath, ip, port);
            SetLevelLoaderSceneName(EditorSceneManager.GetActiveScene().path, extraScenePath);
            EditorApplication.isPlaying = true;

            Debug.LogWarning("BUILD:: Be aware that currently the scene: " + extraScenePath + " - exists in Editor Build Settings as well, which is not a problem, as long as it is not the 0th index scene.");
        }

        private static void Run(string extraScenePath)
        {
            AddExtraScene(extraScenePath);

            SetLevelLoaderSceneName(EditorSceneManager.GetActiveScene().path, extraScenePath);
            EditorApplication.isPlaying = true;

            Debug.LogWarning("BUILD:: Be aware that currently the scene: " + extraScenePath + " - exists in Editor Build Settings as well, which is not a problem, as long as it is not the 0th index scene.");
        }

        private static void Build(string extraScenePath, string pathFolder, string buildFolder, BuildTarget target, BuildOptions options, ForceVRSupportedModes forceVRSupported, bool devBuild, int socketPort = -1, string ip = "", int port = -1)
        {
            AddExtraScene(extraScenePath);

            SetLevelLoaderSceneName(EditorSceneManager.GetActiveScene().path, extraScenePath);
            if (forceVRSupported == ForceVRSupportedModes.NotForced)
            {
                // do not force on or off.
            }
            else
            {
                // force true or false based on mode
                PlayerSettings.virtualRealitySupported = forceVRSupported == ForceVRSupportedModes.ForcedTrue;
                Debug.Log("BUILD:: Virtual reality supported == " + PlayerSettings.virtualRealitySupported.ToString());
            }

            if (!string.IsNullOrEmpty(ip) && port != -1)
            {
                SetClientNetSenderServerAddress(extraScenePath, ip, port);
            }

            if (socketPort >= 0)
            {
                SetServerNetSenderSocketAddress(extraScenePath, socketPort);
            }

            if (devBuild)
            {
                options |= BuildOptions.Development;
            }
            else if (target == BuildTarget.StandaloneLinux || target == BuildTarget.StandaloneLinux64 || target == BuildTarget.StandaloneLinuxUniversal)
            {
                // Linux builds always headless, hardcoded - but only if it is not development build, otherwise Unity crashes (Development cannot work with Headless)
                options |= BuildOptions.EnableHeadlessMode;
            }

            var path = GetPath(pathFolder, buildFolder, target);
            BuildPipeline.BuildPlayer(new BuildPlayerOptions()
            {
                scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray(),
                locationPathName = path,
                target = target,
                options = options
            });

            RemoveScene(extraScenePath);
            Debug.Log("BUILD:: " + pathFolder + " for " + target.ToString() + " " + (devBuild ? "(Development Build)" : string.Empty) + ", built at == " + path);
        }

        private static void AddExtraScene(string extraScenePath)
        {
            var scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (string.Equals(scenes[i].path, extraScenePath))
                {
                    // scene already exists in build settings
                    return;
                }
            }

            var extraScene = new EditorBuildSettingsScene()
            {
                path = extraScenePath,
                enabled = true
            };

            EditorBuildSettings.scenes = scenes.Concat(new EditorBuildSettingsScene[] { extraScene });
            Debug.Log("BUILD:: Added new scene to editor build settings, scene path == " + extraScenePath);
        }

        private static void RemoveScene(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes;
            var newScenes = new List<EditorBuildSettingsScene>(scenes.Length);
            for (int i = 0; i < scenes.Length; i++)
            {
                var scene = scenes[i];
                if (!string.Equals(scene.path, scenePath))
                {
                    // only add scenes that do not match the removed scene
                    newScenes.Add(scene);
                }
            }

            EditorBuildSettings.scenes = newScenes.ToArray();
        }

        private static string GetPath(string serverOrClient, string buildFolder, BuildTarget buildTarget)
        {
            return Path.Combine(Path.Combine(buildFolder, serverOrClient), Path.Combine(buildTarget.ToString(), string.Concat(serverOrClient, ".", GetExtension(buildTarget))));
        }

        private static string GetExtension(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                {
                    return "exe";
                }

            case BuildTarget.StandaloneLinux:
                {
                    return "x86";
                }

            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                {
                    return "x86_64";
                }
            }

            return string.Empty;
        }

        private static void SetAutoConnectOnEnable(string scenePath, bool autoConnect)
        {
            var modified = ModifyRootGameObject<Client.ClientNetSender>(scenePath, (clientNetSender) =>
            {
                if (clientNetSender.autoConnectOnEnable != autoConnect)
                {
                    clientNetSender.autoConnectOnEnable = autoConnect;
                }
            });

            if (!modified)
            {
                Debug.LogError("BUILD:: Could not identify a ClientNetSender in any of the scenes in the Editor Build Settings currently. Thus, cannot modify AutoConnect on enable setting.");
            }
        }

        private static void SetServerNetSenderSocketAddress(string scenePath, int socketPort)
        {
            var modified = ModifyRootGameObject<Server.ServerNetSender>(scenePath, (serverNetSender) =>
            {
                if (serverNetSender.socketPort != socketPort)
                {
                    serverNetSender.socketPort = socketPort;
                    Debug.Log("BUILD:: Server server socket port on ServerNetSender (" + serverNetSender.ToString() + ") == " + serverNetSender.socketPort.ToString());
                }
            });

            if (!modified)
            {
                Debug.LogError("BUILD:: Could not identify a ServerNetSender in any of the scenes in the Editor Build Settings currently. Thus, server may not work! Please ensure the additively loaded 'Server Only' scene is setup and is valid.");
            }
        }

        private static void SetClientNetSenderServerAddress(string scenePath, string serverIp, int serverPort)
        {
            var modified = ModifyRootGameObject<Client.ClientNetSender>(scenePath, (clientNetSender) =>
            {
                if (!string.Equals(clientNetSender.serverIp, serverIp) || clientNetSender.serverPort != serverPort)
                {
                    clientNetSender.serverIp = serverIp;
                    clientNetSender.serverPort = serverPort;
                    Debug.Log("BUILD:: Set server IP and port on ClientNetSender (" + clientNetSender.ToString() + ") == " + clientNetSender.serverIp + " : " + clientNetSender.serverPort.ToString());
                }
            });

            if (!modified)
            {
                Debug.LogError("BUILD:: Could not identify a ClientNetSender in any of the scenes in the Editor Build Settings currently. Thus, client may not work! Please ensure that the additively loaded 'Client Only' scene is setup and is valid.");
            }
        }

        private static void SetLevelLoaderSceneName(string scenePath, string extraScenePath)
        {
            var sceneName = Path.GetFileNameWithoutExtension(extraScenePath);
            var modified = ModifyRootGameObject<AdditiveLevelLoader>(scenePath, (levelLoader) =>
            {
                if (!string.Equals(levelLoader.sceneName, sceneName))
                {
                    levelLoader.sceneName = sceneName;
                    Debug.Log("BUILD:: Set scene name on additive level loader (" + levelLoader.ToString() + ") == " + levelLoader.sceneName);
                }
            });

            if (!modified)
            {
                var go = new GameObject("AdditiveLevelLoader");
                Undo.RegisterCreatedObjectUndo(go, "Created additive level loader");

                var levelLoader = Undo.AddComponent<AdditiveLevelLoader>(go);

                Undo.RecordObject(levelLoader, "Set additive level loader scene name");
                levelLoader.sceneName = sceneName;

                Undo.FlushUndoRecordObjects();
                EditorSceneManager.SaveOpenScenes();
                Debug.Log("BUILD:: Create an additive level loader & set scene name on it (" + levelLoader.ToString() + ") == " + levelLoader.sceneName);
            }
        }

        private static bool ModifyRootGameObject<T>(string scenePath, Action<T> modification) where T : Component
        {
            var success = false;
            var openedScene = false;
            var scene = EditorSceneManager.GetSceneByPath(scenePath);
            if (!scene.IsValid() || !scene.isLoaded)
            {
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                openedScene = true;
            }

            var root = scene.GetRootGameObjects();
            foreach (var go in root)
            {
                var component = go.GetComponent<T>();
                if (component != null)
                {
                    Undo.RecordObject(component, "Modified root game object as part of BuildScripts");
                    modification(component);
                    Undo.FlushUndoRecordObjects();

                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);

                    success = true;
                    break;
                }
            }

            if (openedScene)
            {
                EditorSceneManager.CloseScene(scene, true);
            }

            return success;
        }

        private enum ForceVRSupportedModes
        {
            NotForced,
            ForcedTrue,
            ForcedFalse
        }
    }
}