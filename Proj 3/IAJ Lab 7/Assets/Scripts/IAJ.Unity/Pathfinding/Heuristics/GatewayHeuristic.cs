using RAIN.Navigation.Graph;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class GatewayHeuristic : IHeuristic
    {
        private ClusterGraph ClusterGraph { get; set; }

        public GatewayHeuristic(ClusterGraph clusterGraph)
        {
            this.ClusterGraph = clusterGraph;
        }

        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            //    Cluster cluster = ClusterGraph.Quantize(node);
            //    Debug.Log("Being: " + cluster.ToString());
            //    ClusterGraph.Quantize(goalNode);
            //    Debug.Log("End: " + cluster.ToString());

            //for now just returns the euclidean distance
            //return EuclideanDistance(node.LocalPosition, goalNode.LocalPosition);
            //TODO implement this properly



            Cluster startCluster = ClusterGraph.Quantize(node);
            Cluster endCluster = ClusterGraph.Quantize(goalNode);
        

            if(object.ReferenceEquals(null, startCluster) || object.ReferenceEquals(null, endCluster) || object.ReferenceEquals(startCluster, endCluster))
            {
               // Debug.Log("start null");
                return EuclideanDistance(node.LocalPosition, goalNode.LocalPosition);
            }

            //optimization - remove these news
            //these vectors are gonna be used to determine the position of the gateways the heuristic chose, 
            //and then calculate the distance between the start node and gatewayA, and endnode and gatewayB
           // Debug.Log("yo");
            //this is gonna be the minimum cost found for all the combinations found
            float min = float.MaxValue;
            //just a temporary variable
            float graphCost, component1, component2, sum;
            //iterate all gateways in the start cluster
            foreach (var startGate in startCluster.gateways)
            {
                //iterate all gateways in the end cluster
                foreach (var endGate in endCluster.gateways)
                {
                    //this is the cost from going from startGate to endGate
                    graphCost = ClusterGraph.gatewayDistanceTable[startGate.id].entries[endGate.id].shortestDistance;
                    //distance from nodes to gateways
                    component1 = EuclideanDistance(node.LocalPosition, startGate.center);
                    component2 = EuclideanDistance(endGate.center, goalNode.LocalPosition);
                    sum = graphCost + component1 + component2;
                    if (sum < min)
                    {
                        //asign variables with best values
                        //Debug.Log(graphCost);
                        min = sum;
                    }
                }
            }

            //weighted

            //sum all the component to give the full cost from start to end
            return min; //+ EuclideanDistance(node.Position,goalNode.Position);
        }

        public float H(Vector3 initial, Vector3 final)
        {
            Cluster startCluster = ClusterGraph.Quantize(initial);
            Cluster endCluster = ClusterGraph.Quantize(final);


            if (object.ReferenceEquals(null, startCluster) || object.ReferenceEquals(null, endCluster) || object.ReferenceEquals(startCluster, endCluster))
            {
                return EuclideanDistance(initial, final);
            }

            float min = float.MaxValue;
            float graphCost, component1, component2, sum;
            foreach (var startGate in startCluster.gateways)
            {
                foreach (var endGate in endCluster.gateways)
                {
                    graphCost = ClusterGraph.gatewayDistanceTable[startGate.id].entries[endGate.id].shortestDistance;
                    component1 = EuclideanDistance(initial, startGate.center);
                    component2 = EuclideanDistance(endGate.center, final);
                    sum = graphCost + component1 + component2;
                    if (sum < min)
                    {
                        min = sum;
                    }
                }
            }
            return min;
        }

        public float EuclideanDistance(Vector3 startPosition, Vector3 endPosition)
        {
            return (endPosition - startPosition).magnitude;
        }
    }
}
