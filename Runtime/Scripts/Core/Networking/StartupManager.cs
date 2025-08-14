using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Playmode;
using UnityEngine;

namespace Core.Networking
{
    public class StartupManager : MonoBehaviour
    {
        [Tooltip("Use Platform for builds, use PlayModeTag for editor (multiplayer center)")]
        private StartupMode _startupMode;
        private string[] _playModeTags;

        private ConnectionAndSpawningSO _connectionAndSpawningSO => ConnectionAndSpawningSO.Instance;

        private enum StartupMode
        {
            Platform,
            PlayModeTag
        }

        private void Awake()
        {
            // use tags if in editor, use platform if in build

            if (Application.isEditor)
            {
                _startupMode = StartupMode.PlayModeTag;
            }
            else
            {
                _startupMode = StartupMode.Platform;
            }

            switch (_startupMode)
            {
                case StartupMode.Platform:
                    PlatformStartup();
                    break;
                case StartupMode.PlayModeTag:
                    TagStartup();
                    break;
            }
        }

        private void TagStartup()
        {
            _playModeTags = CurrentPlayer.ReadOnlyTags();
            if (_playModeTags.Contains("PC"))
            {
                StartPCStartup();
            }
            else if (_playModeTags.Contains("VR"))
            {
                StartVRStartup();
            }
            else
            {
                Debug.LogError("Play mode tag not supported! Please go to the multiplayer center and add the tags");
            }
        }

        private void PlatformStartup()
        {
            if (_connectionAndSpawningSO.PCPlatforms.Contains(Application.platform))
            {
                StartPCStartup();
            }
            else if (_connectionAndSpawningSO.VRPlatforms.Contains(Application.platform))
            {
                StartVRStartup();
            }
            else
            {
                Debug.LogError("Platform not supported");
            }
        }

        private void StartPCStartup()
        {
            Instantiate(_connectionAndSpawningSO.PCStartupPrefab);
            Destroy(this);
        }

        private void StartVRStartup()
        {
            Instantiate(_connectionAndSpawningSO.VRStartupPrefab);
            Destroy(this);
        }
    }
}
