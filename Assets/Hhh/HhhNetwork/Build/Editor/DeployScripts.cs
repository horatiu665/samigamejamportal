namespace HhhNetwork.Build.Editor
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using WinSCP;

    public class DeployScripts
    {
        private const string _winSCPExePath = "Plugins/Editor/WinSCP/WinSCP.exe";
        private const string _remotePath = "/root/";

        private static string[] _sshPrivateKeyPaths;
        private static float _progress;

        private readonly static string _applicationDataPath = Application.dataPath;

        private static SessionOptions[] _serverSessions;

        private static void SetServerSessionsDefaultValues()
        {
            _serverSessions = new SessionOptions[]
            {
                new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = "88.198.75.133",
                    UserName = "root",
                    SshHostKeyFingerprint = "ssh-ed25519 256 82:13:a5:b2:c1:91:57:3f:4a:7b:95:a4:57:2e:90:fd",
                },
            };
        }

        private static void PrepareSSH()
        {
            _sshPrivateKeyPaths = new string[_serverSessions.Length];

            for (int i = 0; i < _serverSessions.Length; i++)
            {
                var so = _serverSessions[i];
                var path = GetSSHKeyPath(so.HostName);
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogError("DEPLOY:: Must have valid path for SSH private keys");
                    return;
                }

                SetSSHKeyPref(so.HostName, path);
                _sshPrivateKeyPaths[i] = path;
            }
        }

        [MenuItem("Build/Deploy/Reset Servers", false, 12)]
        public static void ResetServers()
        {
            SetServerSessionsDefaultValues();

            PrepareSSH();

            Debug.Log("DEPLOY:: Starting reset of servers");
            EditorUtility.DisplayProgressBar("RESETTING", "Resetting server(s)...", (_progress = 0f));

            for (int i = 0; i < _serverSessions.Length; i++)
            {
                ResetServersWork(_serverSessions[i], _sshPrivateKeyPaths[i]);
            }

            EditorUtility.ClearProgressBar();
            Debug.Log("DEPLOY:: Finished resetting servers");
        }

        private static void ResetServersWork(SessionOptions sessionOptions, string sshPrivateKeyPath)
        {
            Session session = new Session();
            try
            {
                sessionOptions.SshPrivateKeyPath = sshPrivateKeyPath;
                sessionOptions.AddRawSettings("AuthGSSAPI", "1");
                sessionOptions.AddRawSettings("TcpNoDelay", "1");

                session.ExecutablePath = Path.Combine(_applicationDataPath, _winSCPExePath);
                session.DisableVersionCheck = true;

                // Connect
                EditorUtility.DisplayProgressBar("RESETTING", "Opening session with: " + sessionOptions.HostName, IncreaseProgress(0.05f)); // 0.05
                session.Open(sessionOptions);

                // Terminate active session
                var killCmdText = "screen -S server -X quit";
                EditorUtility.DisplayProgressBar("RESETTING", "Terminating previous server screen with command: " + killCmdText, IncreaseProgress(0.15f)); // 0.2
                ExecuteCommand(session, killCmdText, "Terminate screen command.", "Terminated active sessions successfully.");

                // Start screen
                var newScreenCmd = "screen -S server -dm";
                EditorUtility.DisplayProgressBar("RESETTING", "Starting new server screen with command:" + newScreenCmd, IncreaseProgress(0.4f)); // 0.9
                ExecuteCommand(session, newScreenCmd, "Start screen command error", "Start screen command successful");

                // Run server in screen
                var filepath = GetServerFilePath();
                var filename = Path.GetFileName(filepath);
                var startServerCmd = string.Concat("screen -S server -p 0 -X stuff './", filename, " -batchmode -nographics -headlessmode\n'");
                EditorUtility.DisplayProgressBar("RESETTING", "Starting server in new screen with command: " + startServerCmd, IncreaseProgress(0.05f)); // 0.95
                ExecuteCommand(session, startServerCmd, "Start server command error", "Start server command succesful");
            }
            catch (Exception e)
            {
                Debug.LogError("DEPLOY:: WINSCP Session (" + sessionOptions.HostName + ") exception: " + e.ToString());
            }
            finally
            {
                session.Dispose();
            }
        }

        [MenuItem("Build/Deploy/Deploy Servers", true)]
        public static bool DeployServersValidation()
        {
            return Directory.Exists(GetServerDirectoryPath()) && File.Exists(GetServerFilePath());
        }

        [MenuItem("Build/Deploy/Deploy Servers", false, 11)]
        public static void DeployServers()
        {
            SetServerSessionsDefaultValues();

            PrepareSSH();

            Debug.Log("DEPLOY:: Starting deployment");
            EditorUtility.DisplayProgressBar("DEPLOYING", "Deploying to server(s)...", (_progress = 0f));

            for (int i = 0; i < _serverSessions.Length; i++)
            {
                DeployServersWork(_serverSessions[i], _sshPrivateKeyPaths[i]);
            }

            EditorUtility.ClearProgressBar();
            Debug.Log("DEPLOY:: Finished running deployment");
        }

        private static void DeployServersWork(SessionOptions sessionOptions, string sshPrivateKeyPath)
        {
            Session session = new Session();
            try
            {
                sessionOptions.SshPrivateKeyPath = sshPrivateKeyPath;
                sessionOptions.AddRawSettings("AuthGSSAPI", "1");
                sessionOptions.AddRawSettings("TcpNoDelay", "1");

                //Debug.Log("DEPLOY:: Opening session with server at == " + sessionOptions.HostName);

                session.ExecutablePath = Path.Combine(_applicationDataPath, _winSCPExePath);
                session.DisableVersionCheck = true;

                // Connect
                EditorUtility.DisplayProgressBar("DEPLOYING", "Opening session with: " + sessionOptions.HostName, IncreaseProgress(0.05f)); // 0.05
                session.Open(sessionOptions);

                // Terminate active session
                var killCmdText = "screen -S server -X quit";
                EditorUtility.DisplayProgressBar("DEPLOYING", "Terminating previous server screen with command: " + killCmdText, IncreaseProgress(0.15f)); // 0.2
                ExecuteCommand(session, killCmdText, "Terminate screen command.", "Terminated active sessions successfully.");

                // Upload server build
                var localPath = GetServerDirectoryPath();
                var remotePath = _remotePath;
                var filepath = GetServerFilePath();
                //Debug.Log("DEPLOY:: Uploading files from local == " + localPath + " to remote == " + remotePath);

                // Upload actual executable file
                EditorUtility.DisplayProgressBar("DEPLOYING", "Uploading executable file from: " + filepath, IncreaseProgress(0.05f)); // 0.25
                UploadFiles(session, filepath, remotePath, new TransferOptions()
                {
                    OverwriteMode = OverwriteMode.Overwrite,
                    PreserveTimestamp = false,
                    FilePermissions = new FilePermissions() { GroupExecute = true, OtherExecute = true, UserExecute = true }
                });

                // Upload Server_Data folder & contents
                var dataFolder = Path.Combine(localPath, BuildScripts.serverBuildName + "_Data");
                EditorUtility.DisplayProgressBar("DEPLOYING", "Uploading Data folder from: " + dataFolder, IncreaseProgress(0.25f)); // 0.5
                UploadFiles(session, dataFolder, remotePath);

                // Start screen
                var newScreenCmd = "screen -S server -dm";
                EditorUtility.DisplayProgressBar("DEPLOYING", "Starting new server screen with command:" + newScreenCmd, IncreaseProgress(0.4f)); // 0.9
                ExecuteCommand(session, newScreenCmd, "Start screen command error", "Start screen command successful");

                // Run server in screen
                var filename = Path.GetFileName(filepath);
                var startServerCmd = string.Concat("screen -S server -p 0 -X stuff './", filename, " -batchmode -nographics -headlessmode\n'");
                EditorUtility.DisplayProgressBar("DEPLOYING", "Starting server in new screen with command: " + startServerCmd, IncreaseProgress(0.05f)); // 0.95
                ExecuteCommand(session, startServerCmd, "Start server command error", "Start server command succesful");

                //Debug.Log("DEPLOY:: Finished uploading and starting server at hostname == " + sessionOptions.HostName);
            }
            catch (Exception e)
            {
                Debug.LogError("DEPLOY:: WINSCP Session (" + sessionOptions.HostName + ") exception: " + e.ToString());
            }
            finally
            {
                session.Dispose();
            }
        }

        private static void ExecuteCommand(Session session, string cmdText, string errorMsg, string successMsg)
        {
            //Debug.Log("DEPLOY:: Executing command == " + cmdText);
            var cmd = session.ExecuteCommand(cmdText);
            if (cmd.IsSuccess)
            {
                //Debug.Log("DEPLOY:: " + successMsg + (!string.IsNullOrEmpty(cmd.Output) ? "\nOutput == " + cmd.Output : " (No Output)"));
            }
            else
            {
                var failures = cmd.Failures;
                for (int i = 0; i < failures.Count; i++)
                {
                    var failure = failures[i];
                    Debug.LogError("DEPLOY:: " + errorMsg + ". Error Output == " + cmd.ErrorOutput + ".\n Error == " + failure.Message + "\nHelp link: " + failure.HelpLink + "\n" + failure.StackTrace);
                }
            }

            cmd.Check();
        }

        private static void UploadFiles(Session session, string localPath, string remotePath)
        {
            var options = new TransferOptions()
            {
                OverwriteMode = OverwriteMode.Overwrite,
                PreserveTimestamp = false
            };

            UploadFiles(session, localPath, remotePath, options);
        }

        private static void UploadFiles(Session session, string localPath, string remotePath, TransferOptions options)
        {
            try
            {
                var putResult = session.PutFiles(localPath, remotePath, false, options);
                if (putResult.IsSuccess)
                {
                    foreach (TransferEventArgs transfer in putResult.Transfers)
                    {
                        //Debug.Log("DEPLOY:: Uploaded file successfully to == " + remotePath + " from " + localPath + ", filename == " + transfer.FileName);
                    }
                }
                else
                {
                    var failures = putResult.Failures;
                    for (int i = 0; i < failures.Count; i++)
                    {
                        var failure = failures[i];
                        Debug.LogError("DEPLOY:: Upload file error == " + failure.Message + "\nHelp link:" + failure.HelpLink + "\n" + failure.StackTrace);
                    }
                }

                putResult.Check();
            }
            catch (Exception e)
            {
                Debug.LogError("DEPLOY:: Upload file exception == " + e.Message + "\n" + e.StackTrace);
            }
        }

        public static string GetServerDirectoryPath(BuildTarget target = BuildTarget.StandaloneLinux64)
        {
            return Path.Combine(Path.Combine(BuildScriptPrefs.GetBuildFolder(), BuildScripts.serverBuildName), target.ToString());
        }

        public static string GetServerFilePath(BuildTarget target = BuildTarget.StandaloneLinux64)
        {
            return Path.Combine(GetServerDirectoryPath(target), BuildScripts.serverBuildName + ".x86_64");
        }

        private static float IncreaseProgress(float increase)
        {
            return _progress += (increase / _serverSessions.Length);
        }

        /* SSH */

        private const string _deployKeyPathsKey = "DEPLOY_KEY_PATHS";
        private const string _deployKeyPathKeyPrefix = "DEPLOY_KEY_PATH_";

        private static string GetSSHKeyPath(string hostName)
        {
            var prefsKey = string.Concat(_deployKeyPathKeyPrefix, hostName);
            var path = EditorPrefs.GetString(prefsKey, string.Empty);
            if (string.IsNullOrEmpty(path))
            {
                path = EditorUtility.OpenFilePanel("Find SSH Private Key (" + hostName + ")", "Please locate the SSH private key file (.ppk) for the host at == " + hostName, "ppk");
            }

            return path;
        }

        private static void SetSSHKeyPref(string hostName, string path)
        {
            var prefsKey = string.Concat(_deployKeyPathKeyPrefix, hostName);
            EditorPrefs.SetString(prefsKey, path);

            var keys = EditorPrefs.GetString(_deployKeyPathsKey, string.Empty);
            keys = string.IsNullOrEmpty(keys) ? prefsKey : string.Concat(keys, "|", prefsKey);
            EditorPrefs.SetString(_deployKeyPathsKey, keys);
        }

        [MenuItem("Build/Deploy/Clear SSH Key Paths", false, 90)]
        public static void ClearSSHKeyPaths()
        {
            var keys = EditorPrefs.GetString(_deployKeyPathsKey, string.Empty);
            if (string.IsNullOrEmpty(keys))
            {
                return;
            }

            foreach (var key in keys.Split('|'))
            {
                EditorPrefs.DeleteKey(key);
            }

            EditorPrefs.DeleteKey(_deployKeyPathsKey);
        }

        [MenuItem("Build/Deploy/Clear SSH Key Paths", true, 90)]
        public static bool ClearSSHKeyPathsValidation()
        {
            return !string.IsNullOrEmpty(EditorPrefs.GetString(_deployKeyPathsKey, string.Empty));
        }
    }
}