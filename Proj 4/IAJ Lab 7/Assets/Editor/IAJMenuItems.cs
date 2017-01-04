﻿using UnityEngine;
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
        for(int i = 0; i < gateways.Length; i++)
        {
            var gatewayGO = gateways[i];
            gateway = ScriptableObject.CreateInstance<Gateway>();
            //i for id
            gateway.Initialize(i,gatewayGO);
            clusterGraph.gateways.Add(gateway);
        }

        //create cluster instances for each cluster game object and check for connections through gateways
        foreach (var clusterGO in clusters)
        {

            cluster = ScriptableObject.CreateInstance<Cluster>();
            cluster.Initialize(clusterGO);
            clusterGraph.clusters.Add(cluster);

            //determine intersection between cluster and gateways and add connections when they intersect
            foreach(var gate in clusterGraph.gateways)
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

        Debug.Log("Distance table with: " + clusterGraph.gatewayDistanceTable.Length + " rows and " + clusterGraph.gatewayDistanceTable[0].entries.Length + " columns.");
        string print = "[";
        for (int i = 0; i < clusterGraph.gatewayDistanceTable.Length; i++)
        {
            print += "[";
            for (int j = 0; j < clusterGraph.gatewayDistanceTable[i].entries.Length; j++)
            {
                print += clusterGraph.gatewayDistanceTable[i].entries[j].shortestDistance + ", ";
            }
            print += "]\n";
        }
        print += "]";
        Debug.Log(print);

        //precompute dijkstra and populate smellyNode list
        int listIndex = 0;
        foreach (var chest in GameObject.FindGameObjectsWithTag("Chest"))
        {
            Cluster klaus = clusterGraph.Quantize(chest.transform.position);
            dijkstra(clusterGraph, klaus, klaus, listIndex, 1000);
            listIndex++;
        }

        foreach(var skeleton in GameObject.FindGameObjectsWithTag("Skeleton"))
        {
            Cluster klaus = clusterGraph.Quantize(skeleton.transform.position);
            dijkstra(clusterGraph, klaus, klaus, listIndex, 1000);
            listIndex++;
        }

        foreach (var orc in GameObject.FindGameObjectsWithTag("Orc"))
        {
            Cluster klaus = clusterGraph.Quantize(orc.transform.position);
            dijkstra(clusterGraph, klaus, klaus, listIndex, 1000);
            listIndex++;
        }

        foreach (var dragon in GameObject.FindGameObjectsWithTag("Dragon"))
        {
            Cluster klaus = clusterGraph.Quantize(dragon.transform.position);
            dijkstra(clusterGraph, klaus, klaus, listIndex, 1000);
            listIndex++;
        }

        foreach (var health in GameObject.FindGameObjectsWithTag("HealthPotion"))
        {
            Cluster klaus = clusterGraph.Quantize(health.transform.position);
            dijkstra(clusterGraph, klaus, klaus, listIndex, 1000);
            listIndex++;
        }

        foreach (var mana in GameObject.FindGameObjectsWithTag("ManaPotion"))
        {
            Cluster klaus = clusterGraph.Quantize(mana.transform.position);
            dijkstra(clusterGraph, klaus, klaus, listIndex, 1000);
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

    private static void dijkstra(ClusterGraph clusterGraph, Cluster klaus, Cluster parent, int index, int intensity)
    {
        klaus.smellyIntensity[index] = intensity;
        klaus.smellyClusterIndex[index] = parent.center;

        List<Gateway> gates = klaus.gateways;
        
        for(int i = 0; i < gates.Count; i++)
        {
            foreach(var cluster in gates[i].clusters)
            {
                if(!cluster.Equals(klaus))
                {
                    if(cluster.smellyIntensity[index] < intensity-1)
                    {
                        dijkstra(clusterGraph, cluster, klaus, index, intensity-1);
                    }
                }
            }
        }
    }
}
