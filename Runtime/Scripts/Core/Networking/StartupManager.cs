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
            _startupMode = Application.isEditor ? StartupMode.PlayModeTag : StartupMode.Platform;

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

            RuntimePlatform platformToLookFor = RuntimePlatform.WindowsPlayer; 
            bool platformTagFound = false;

            if (_playModeTags.Contains("PC"))
            {
                platformToLookFor = RuntimePlatform.WindowsPlayer;
                platformTagFound = true;
            }
            else if (_playModeTags.Contains("VR"))
            {
                platformToLookFor = RuntimePlatform.Android;
                platformTagFound = true;
            }

            if (platformTagFound)
            {
                foreach (var platformDef in _connectionAndSpawningSO.PlatformDefinitions)
                {
                    if (platformDef.Platforms.Contains(platformToLookFor))
                    {
                        Instantiate(platformDef.StartupPrefab);
                        Destroy(gameObject);
                        return;
                    }
                }
            }
            
            Debug.LogError("Play mode tag not supported or no matching platform definition found! Please go to the multiplayer center and add the tags");
        }

        private void PlatformStartup()
        {
            foreach (var platformDef in _connectionAndSpawningSO.PlatformDefinitions)
            {
                if (platformDef.Platforms.Contains(Application.platform))
                {
                    Instantiate(platformDef.StartupPrefab);
                    Destroy(gameObject);
                    return;
                }
            }
            Debug.LogError("Platform not supported");
        }
    }
}