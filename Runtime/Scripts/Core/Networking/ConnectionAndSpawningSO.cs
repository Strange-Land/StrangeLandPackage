using System.Collections.Generic;
using Core.Utilities;
using UnityEngine;

namespace Core.Networking
{
    public class ConnectionAndSpawningSO : SingletonSO<ConnectionAndSpawningSO>
    {
        public List<GameObject> ResearcherPrefabs;
        public List<GameObject> ServerPrefabs;
        
        public SceneField WaitingRoomScene;
        public List<SceneField> ScenarioScenes = new List<SceneField>();
        
        [System.Serializable]
        public class PlatformDefinition
        {
            public List<RuntimePlatform> Platforms;
            public GameObject StartupPrefab;
            public List<GameObject> LocalPlayerPrefabs = new List<GameObject>();
        }
        
        public List<PlatformDefinition> PlatformDefinitions = new List<PlatformDefinition>();
    }
}