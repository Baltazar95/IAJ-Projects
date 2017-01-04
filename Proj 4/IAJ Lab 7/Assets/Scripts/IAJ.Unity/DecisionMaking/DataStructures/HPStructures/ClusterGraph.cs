﻿using RAIN.Navigation.Graph;
using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures
{
    public class ClusterGraph : ScriptableObject
    {
        public List<Cluster> clusters;
        public List<Gateway> gateways;
        public GatewayDistanceTableRow[] gatewayDistanceTable;
        public List<Cluster> nodeInCluster;


        public ClusterGraph()
        {
            this.clusters = new List<Cluster>();
            this.gateways = new List<Gateway>();
            this.nodeInCluster = new List<Cluster>();
        }

        public Cluster Quantize(NavigationGraphNode node)
        {
            //optimization - do a dictionary
            //optimization store nodes in cluster, while preprocessing

            //foreach (var cluster in clusters)
            //{

            //    if (MathHelper.PointInsideBoundingBox(node.Position, cluster.min, cluster.max))
            //    {
            //        //Debug.Log("in the matrix");
            //        return cluster;
            //    }
            //}
            //    return null;

            return nodeInCluster[node.NodeIndex];
        }

        public Cluster Quantize(Vector3 pos)
        {
            foreach (var cluster in clusters)
            {

                if (MathHelper.PointInsideBoundingBox(pos, cluster.min, cluster.max))
                {
                    return cluster;
                }
            }
            return null;
        }

        public void AddNode(NavigationGraphNode node, Cluster KlausTheCluster)
        {
            //Debug.Log(node.ToString());
         
            nodeInCluster.Insert(node.NodeIndex, KlausTheCluster);

        }

   

        public void SaveToAssetDatabase()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (System.IO.Path.GetExtension(path) != "")
            {
                path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + typeof(ClusterGraph).Name.ToString() + ".asset");

            AssetDatabase.CreateAsset(this, assetPathAndName);
            EditorUtility.SetDirty(this);
            
            //save the clusters
            foreach(var cluster in this.clusters)
            {
                AssetDatabase.AddObjectToAsset(cluster, assetPathAndName);
            }

            //save the gateways
            foreach (var gateway in this.gateways)
            {
                AssetDatabase.AddObjectToAsset(gateway, assetPathAndName);
            }

            //save the nodeinclusters
            foreach (var clust in this.nodeInCluster)
            {
                AssetDatabase.AddObjectToAsset(clust, assetPathAndName);
            }


            //save the gatewayTableRows and tableEntries
            foreach (var tableRow in this.gatewayDistanceTable)
            {
                AssetDatabase.AddObjectToAsset(tableRow, assetPathAndName);

                foreach (var tableEntry in tableRow.entries)
                {
                   //Debug.Log(tableEntry.startGatewayPosition);
                    AssetDatabase.AddObjectToAsset(tableEntry, assetPathAndName);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = this;
        }
    }
}
