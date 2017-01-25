using UnityEngine;
using UnityEditor;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Utils;
using RAIN.Navigation.NavMesh;
using System.Collections.Generic;
using RAIN.Navigation.Graph;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using System;
using Assets.Scripts.IAJ.Unity.DecisionMaking.DataStructures.HPStructures;

public class IAJMenuItems  {

    [MenuItem("IAJ/Create Cluster Graph")]
    private static void CreateClusterGraph()
    {
        Cluster cluster;
        Gateway gateway;
        GlobalPath solution = null;
        GatewayDistanceTableRow row;
        GatewayDistanceTableEntry entry;

        //get cluster game objects
        var clusters = GameObject.FindGameObjectsWithTag("Cluster");

        //get gateway game objects
        var gateways = GameObject.FindGameObjectsWithTag("Gateway");

        //get the NavMeshGraph from the current scene
        NavMeshPathGraph navMesh = GameObject.Find("Navigation Mesh").GetComponent<NavMeshRig>().NavMesh.Graph;

        ClusterGraph clusterGraph = ScriptableObject.CreateInstance<ClusterGraph>();

        //create gateway instances for each gateway game object
        for (int i = 0; i < gateways.Length; i++)
            {
                var gatewayGO = gateways[i];
                gateway = ScriptableObject.CreateInstance<Gateway>();
                //i for id
                gateway.Initialize(i, gatewayGO);
                clusterGraph.gateways.Add(gateway);
            }

        //create cluster instances for each cluster game object and check for connections through gateways
        foreach (var clusterGO in clusters)
                {

                    cluster = ScriptableObject.CreateInstance<Cluster>();
                    cluster.Initialize(clusterGO);
                    clusterGraph.clusters.Add(cluster);

                    //determine intersection between cluster and gateways and add connections when they intersect
                    foreach (var gate in clusterGraph.gateways)
                    {
                        if (MathHelper.BoundingBoxIntersection(cluster.min, cluster.max, gate.min, gate.max))
                        {
                            cluster.gateways.Add(gate);
                            gate.clusters.Add(cluster);
                        }
                    }
                }

        //precimpute cluster indices for all nodes
        var pathfindingAlgorithm = new NodeArrayAStarPathFinding(navMesh, new EuclideanDistanceHeuristic());

        var nodes = GetNodesHack(pathfindingAlgorithm.NavMeshGraph);

        //create the list of smellyNodes
        int counter = 0;
        foreach (var node in nodes)
        {
            clusterGraph.smellyNodes.Add(new SmellyNode());
            counter++;
        }
        //var node = pathfindingAlgorithm.NavMeshGraph.QuantizeToNode(Vector3.zero,0.5f);

        //node.EdgeOut(0)

        var nodesInCluster = new List<Cluster>();
        foreach(var node in nodes)
        {
            foreach (var _cluster in clusterGraph.clusters)
            {

                if (MathHelper.PointInsideBoundingBox(node.Position, _cluster.min, _cluster.max))
                {
                    cluster = ScriptableObject.CreateInstance<Cluster>();
                    cluster.center = _cluster.center;
                    cluster.gateways = _cluster.gateways;
                    cluster.max = _cluster.max;
                    cluster.min = _cluster.min;
                    clusterGraph.AddNode(node, cluster);
                }
            }
        }

        // Second stage of the algorithm, calculation of the Gateway table
        clusterGraph.gatewayDistanceTable = new GatewayDistanceTableRow[gateways.Length];

        //TODO implement the rest of the algorithm here, i.e. build the GatewayDistanceTable

        foreach (var beginGate in clusterGraph.gateways)
        {
            row = ScriptableObject.CreateInstance<GatewayDistanceTableRow>();
            row.entries = new GatewayDistanceTableEntry[clusterGraph.gateways.Count];
            foreach (var combinationGate in clusterGraph.gateways)
            {
                entry = ScriptableObject.CreateInstance<GatewayDistanceTableEntry>();
                entry.Initialize(beginGate, combinationGate);

                if (combinationGate.id != beginGate.id)
                {
                    pathfindingAlgorithm.InitializePathfindingSearch(beginGate.center, combinationGate.center);
                    var finished = pathfindingAlgorithm.Search(out solution, false);

                    if (finished && solution != null)
                    {
                        entry.shortestDistance = solution.Length;
                    }
                    else
                    {
                        entry.shortestDistance = float.MaxValue;
                    }


                }
                else entry.Initialize(0);
                row.entries[combinationGate.id] = entry;
            }

            clusterGraph.gatewayDistanceTable[beginGate.id] = row;
        }

        //Debug.Log("Distance table with: " + clusterGraph.gatewayDistanceTable.Length + " rows and " + clusterGraph.gatewayDistanceTable[0].entries.Length + " columns.");
        //string print = "[";
        //for (int i = 0; i < clusterGraph.gatewayDistanceTable.Length; i++)
        //{
        //    print += "[";
        //    for (int j = 0; j < clusterGraph.gatewayDistanceTable[i].entries.Length; j++)
        //    {
        //        print += clusterGraph.gatewayDistanceTable[i].entries[j].shortestDistance + ", ";
        //    }
        //    print += "]\n";
        //}
        //print += "]";
        //Debug.Log(print);

        //precompute dijkstra and populate smellyNode list
        int listIndex = 0;
        NavigationGraphNode sNode;
        foreach (var chest in GameObject.FindGameObjectsWithTag("Chest"))
        {
            sNode = pathfindingAlgorithm.Quantize(chest.transform.position);
            //I need to do this because in Recast NavMesh graph, the edges of polygons are considered to be nodes and not the connections.
            //Theoretically the Quantize method should then return the appropriate edge, but instead it returns a polygon
            //Therefore, we need to create one explicit connection between the polygon and each edge of the corresponding polygon for the search algorithm to work
            ((NavMeshPoly)sNode).AddConnectedPoly(sNode.Position);
            dijkstra(clusterGraph, sNode, sNode, listIndex, 100000, chest.name);
            listIndex++;
        }

        foreach(var skeleton in GameObject.FindGameObjectsWithTag("Skeleton"))
        {
            sNode = pathfindingAlgorithm.Quantize(skeleton.transform.position);
            //I need to do this because in Recast NavMesh graph, the edges of polygons are considered to be nodes and not the connections.
            //Theoretically the Quantize method should then return the appropriate edge, but instead it returns a polygon
            //Therefore, we need to create one explicit connection between the polygon and each edge of the corresponding polygon for the search algorithm to work
            ((NavMeshPoly)sNode).AddConnectedPoly(sNode.Position);
            dijkstra(clusterGraph, sNode, sNode, listIndex, 100000, skeleton.name);
            listIndex++;
        }

        foreach (var orc in GameObject.FindGameObjectsWithTag("Orc"))
        {
            sNode = pathfindingAlgorithm.Quantize(orc.transform.position);
            //I need to do this because in Recast NavMesh graph, the edges of polygons are considered to be nodes and not the connections.
            //Theoretically the Quantize method should then return the appropriate edge, but instead it returns a polygon
            //Therefore, we need to create one explicit connection between the polygon and each edge of the corresponding polygon for the search algorithm to work
            ((NavMeshPoly)sNode).AddConnectedPoly(sNode.Position);
            dijkstra(clusterGraph, sNode, sNode, listIndex, 100000, orc.name);
            listIndex++;
        }

        foreach (var dragon in GameObject.FindGameObjectsWithTag("Dragon"))
        {
            sNode = pathfindingAlgorithm.Quantize(dragon.transform.position);
            //I need to do this because in Recast NavMesh graph, the edges of polygons are considered to be nodes and not the connections.
            //Theoretically the Quantize method should then return the appropriate edge, but instead it returns a polygon
            //Therefore, we need to create one explicit connection between the polygon and each edge of the corresponding polygon for the search algorithm to work
            ((NavMeshPoly)sNode).AddConnectedPoly(sNode.Position);
            dijkstra(clusterGraph, sNode, sNode, listIndex, 100000, dragon.name);
            listIndex++;
        }

        foreach (var health in GameObject.FindGameObjectsWithTag("HealthPotion"))
        {
            sNode = pathfindingAlgorithm.Quantize(health.transform.position);
            //I need to do this because in Recast NavMesh graph, the edges of polygons are considered to be nodes and not the connections.
            //Theoretically the Quantize method should then return the appropriate edge, but instead it returns a polygon
            //Therefore, we need to create one explicit connection between the polygon and each edge of the corresponding polygon for the search algorithm to work
            ((NavMeshPoly)sNode).AddConnectedPoly(sNode.Position);
            dijkstra(clusterGraph, sNode, sNode, listIndex, 100000, health.name);
            listIndex++;
        }

        foreach (var mana in GameObject.FindGameObjectsWithTag("ManaPotion"))
        {
            sNode = pathfindingAlgorithm.Quantize(mana.transform.position);
            //I need to do this because in Recast NavMesh graph, the edges of polygons are considered to be nodes and not the connections.
            //Theoretically the Quantize method should then return the appropriate edge, but instead it returns a polygon
            //Therefore, we need to create one explicit connection between the polygon and each edge of the corresponding polygon for the search algorithm to work
            ((NavMeshPoly)sNode).AddConnectedPoly(sNode.Position);
            dijkstra(clusterGraph, sNode, sNode, listIndex, 100000, mana.name);
            listIndex++;
        }

        //create a new asset that will contain the ClusterGraph and save it to disk (DO NOT REMOVE THIS LINE)
        clusterGraph.SaveToAssetDatabase();
    }


    private static List<NavigationGraphNode> GetNodesHack(NavMeshPathGraph graph)
    {
        //this hack is needed because in order to implement NodeArrayA* you need to have full acess to all the nodes in the navigation graph in the beginning of the search
        //unfortunately in RAINNavigationGraph class the field which contains the full List of Nodes is private
        //I cannot change the field to public, however there is a trick in C#. If you know the name of the field, you can access it using reflection (even if it is private)
        //using reflection is not very efficient, but it is ok because this is only called once in the creation of the class
        //by the way, NavMeshPathGraph is a derived class from RAINNavigationGraph class and the _pathNodes field is defined in the base class,
        //that's why we're using the type of the base class in the reflection call
        return (List<NavigationGraphNode>)Assets.Scripts.IAJ.Unity.Utils.Reflection.GetInstanceField(typeof(RAINNavigationGraph), graph, "_pathNodes");
    }

    private static void dijkstra(ClusterGraph clusterGraph, NavigationGraphNode node, NavigationGraphNode parent, int index, int intensity, string name)
    {
        NavigationGraphNode currentNode, newNode, nodeInList;
        List<NavigationGraphNode> open = new List<NavigationGraphNode>();
        List<NavigationGraphNode> closed = new List<NavigationGraphNode>();

        //clusterGraph.smellyNodes.Insert(node.NodeIndex, new SmellyNode());
        clusterGraph.smellyNodes[node.NodeIndex].smellyIntensity[index] = intensity;
        clusterGraph.smellyNodes[node.NodeIndex].smellyNodeIndex[index] = parent.NodeIndex;
        clusterGraph.smellyNodes[node.NodeIndex].objectNames[index] = name;
        open.Add(node);

        while(open.Count != 0)
        {
            currentNode = open[0];
            open.Remove(currentNode);
            closed.Add(currentNode);

            var outconnections = currentNode.OutEdgeCount;
            for (int i = 0; i < outconnections; i++)
            {
                intensity = clusterGraph.smellyNodes[currentNode.NodeIndex].smellyIntensity[index] - 1;
                newNode = currentNode.EdgeOut(i).ToNode;

                nodeInList = closed.Find(x => x.NodeIndex == newNode.NodeIndex);
                if(nodeInList != null)
                {
                    if(clusterGraph.smellyNodes[nodeInList.NodeIndex].smellyIntensity[index] >= intensity)
                    {
                        continue;
                    }
                    else
                    {
                        closed.Remove(nodeInList);
                    }
                }
                else
                {
                    nodeInList = open.Find(x => x.NodeIndex == newNode.NodeIndex);

                    if(nodeInList != null)
                    {
                        if(clusterGraph.smellyNodes[nodeInList.NodeIndex].smellyIntensity[index] < intensity)
                        {
                            clusterGraph.smellyNodes[nodeInList.NodeIndex].smellyIntensity[index] = intensity;
                            clusterGraph.smellyNodes[nodeInList.NodeIndex].smellyNodeIndex[index] = currentNode.NodeIndex;
                            clusterGraph.smellyNodes[nodeInList.NodeIndex].objectNames[index] = name;
                        }
                        continue;
                    }
                }

                //clusterGraph.smellyNodes.Insert(nodeInList.NodeIndex, new SmellyNode());
                clusterGraph.smellyNodes[newNode.NodeIndex].smellyIntensity[index] = intensity;
                clusterGraph.smellyNodes[newNode.NodeIndex].smellyNodeIndex[index] = currentNode.NodeIndex;
                clusterGraph.smellyNodes[newNode.NodeIndex].objectNames[index] = name;
                open.Add(newNode);
            }
        }
    }
}