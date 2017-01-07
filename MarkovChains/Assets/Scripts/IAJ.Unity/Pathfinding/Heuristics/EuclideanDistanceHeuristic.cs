using System;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class EuclideanDistanceHeuristic : IHeuristic
    {
        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            return (goalNode.Position - node.Position).magnitude;
        }

        public float H(Vector3 initial, Vector3 final)
        {
            throw new NotImplementedException();
        }
    }
}
