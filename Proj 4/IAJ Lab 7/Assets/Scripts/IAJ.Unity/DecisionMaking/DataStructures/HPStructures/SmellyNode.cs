using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.DataStructures.HPStructures
{
    public class SmellyNode : ScriptableObject
    {
        public int[] smellyNodeIndex;
        public int[] smellyIntensity;
        public string[] objectNames;

        public SmellyNode()
        {
            this.smellyNodeIndex = new int[14];
            this.smellyIntensity = new int[14];
            this.objectNames = new string[14];
        }

    }
}
