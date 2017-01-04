using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures
{
    public class Cluster : ScriptableObject
    {
        public Vector3 center;
        public Vector3 min;
        public Vector3 max;
        public List<Gateway> gateways;
        public Vector3[] smellyClusterIndex;
        public int[] smellyIntensity;

        public Cluster()
        {
            this.gateways = new List<Gateway>();
            this.smellyClusterIndex = new Vector3[14];
            this.smellyIntensity = new int[14];
        }

        public void Initialize(GameObject clusterObject)
        {
            this.center = clusterObject.transform.position;
            //clusters have a size of 10 multipled by the scale
            this.min = new Vector3(this.center.x - clusterObject.transform.localScale.x * 10/2, 0, this.center.z - clusterObject.transform.localScale.z * 10/2);
            this.max = new Vector3(this.center.x + clusterObject.transform.localScale.x * 10/2, 0, this.center.z + clusterObject.transform.localScale.z * 10/2);
        }

        public Vector3 Localize()
        {
            return this.center;
        }
    }
}
