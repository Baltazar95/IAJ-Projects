using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures
{
    public class GatewayDistanceTableEntry : ScriptableObject
    {
        public Vector3 startGatewayPosition;
        public Vector3 endGatewayPosition;
        public float shortestDistance;

        public void Initialize(Gateway start, Gateway end)
        {
            this.startGatewayPosition = start.center;
            //clusters have a size of 10 multipled by the scale
            this.endGatewayPosition = end.center;
        }
        public void Initialize(float dist)
        {
            this.shortestDistance = dist;
        }
    }
}
