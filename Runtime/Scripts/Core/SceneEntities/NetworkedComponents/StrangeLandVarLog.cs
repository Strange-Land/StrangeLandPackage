using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.SceneEntities
{
    public class StrangeLandVarLog : MonoBehaviour
    {
        [Serializable]
        public class Binding
        {
            public Component target;     // e.g., SomeMonoBehaviour
            public string memberName;    // chosen field/property name
            public string label;         // optional CSV column label
        }

        public List<Binding> bindings = new();
    }
}