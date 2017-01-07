using System;
using UnityEngine;
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

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS.FastEvolutionMCTS
{
	public class Table : ScriptableObject
	{
		public EdgeRow[] edgeRow;

		public Table(){

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

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + typeof(Table).Name.ToString() + ".asset");

			AssetDatabase.CreateAsset(this, assetPathAndName);
			EditorUtility.SetDirty(this);

			//save the gatewayTableRows and tableEntries
//			for(int i = 0; i< 6; i++)
//			{
//				AssetDatabase.AddObjectToAsset(edgeRow[i], assetPathAndName);
//
//				for (int j = 0; j < 6; j++)
//				{
//					EdgeRow e = edgeRow [i];
//					Edge edgy = e.edges [j];
//					//Debug.Log(tableEntry.startGatewayPosition);
//					Debug.Log (edgy.probability);
//					AssetDatabase.AddObjectToAsset(edgy, assetPathAndName);
//				}
//			}

			foreach (var row in edgeRow) {
				AssetDatabase.AddObjectToAsset(row, assetPathAndName);
				foreach(var edgy in row.edges){
//					Debug.Log (edgy);
					AssetDatabase.AddObjectToAsset(edgy, assetPathAndName);

				}
					
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = this;
			Debug.Log ("All clear");
		}
}
}