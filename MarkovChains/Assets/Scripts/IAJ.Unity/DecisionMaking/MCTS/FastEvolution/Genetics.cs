using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Utils;
using Assets.Scripts.GameManager;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS.FastEvolutionMCTS
{
	public class Genetics
	{
		List<RandomWalkMarkovChain> Pool;
		List<RandomWalkMarkovChain> FinalTables;
		RandomWalkMarkovChain[] arrayPool;

		public Genetics ()
		{
			arrayPool = new RandomWalkMarkovChain[25];
			Pool = new List<RandomWalkMarkovChain> ();
			FinalTables = new List<RandomWalkMarkovChain> ();

		}
		public void AddTable(RandomWalkMarkovChain mc, WorldModel model){
			mc.fitness = Fitness (model);
			Pool.Add (mc);
			Pool.OrderBy (o => o.fitness).ToList ();
			if (Pool.Count > 25)
				Pool.RemoveAt (0);

		}
		public void Cross(int crossPoint, RandomWalkMarkovChain mc1, RandomWalkMarkovChain mc2){
			RandomWalkMarkovChain result1 = new RandomWalkMarkovChain ();
			RandomWalkMarkovChain result2 = new RandomWalkMarkovChain();
			result1.RandomTable();
			result2.RandomTable();
			result1.fitness = crossPoint * 0.166 * mc1.fitness + (6-crossPoint)*0.166*mc2.fitness;
			result2.fitness = crossPoint * 0.166 * mc2.fitness + (6-crossPoint)*0.166*mc1.fitness;
			for (int i = 0; i < 6; i++) {
				if (i >= crossPoint) {
					result1.Transitions [i] = mc1.Transitions [i];
					result2.Transitions [i] = mc2.Transitions [i];

				} else {
					//here
					result1.Transitions [i] = mc2.Transitions [i];
					result2.Transitions [i] = mc1.Transitions [i];
				}
			}
			FinalTables.Add (result1);
			FinalTables.Add (result2);
			string print = "[";
			foreach (var row in result1.Transitions) {
				foreach (var edgy in row.TableEntry) {
					print += edgy.ToString ();
				}
			}
//			Debug.Log ("result1: " + print);
			print = "[";
			foreach (var row in result2.Transitions) {
				foreach (var edgy in row.TableEntry) {
					print += edgy.ToString ();
				}
			}
//			Debug.Log ("result2: " + print);

		}

		public double Fitness(WorldModel model){
			return (100 - (float)model.GetProperty (Properties.TIME)) / 100;
			
		}

		public void CrossOver(){
			Debug.Log ("Crossing");
			List<RandomWalkMarkovChain> SortedList = Pool.OrderBy(o => o.fitness).ToList();
			var rnd = new System.Random ();
			int sample;
			sample = rnd.Next (1, 5);


//			Debug.Log (SortedList.Count);
//			Debug.Log (SortedList [22].fitness + " " + SortedList [23].fitness + " " + SortedList [24].fitness);
			//here
//			if( SortedList [22] != null && SortedList [23] != null)
			Cross (sample, SortedList [22], SortedList [23]);
			sample = rnd.Next (1, 5);
//			if( SortedList [22] != null && SortedList [24] != null)
			Cross (sample, SortedList [22], SortedList [24]);
			sample = rnd.Next (1, 5);
//			if( SortedList [24] != null && SortedList [23] != null)
			Cross (sample, SortedList [24], SortedList [23]);

		}

		public RandomWalkMarkovChain Deviate(RandomWalkMarkovChain MC){
//			Debug.Log ("Deviating");
			var sample = RandomHelper.RandomBinomial (0.1f);
			for (int i = 0; i < 6; i++) {
				for (int j = 0; j < 6; j++) {
					if (j % 2 == 0) {
						MC.Transitions [i].TableEntry [j] += sample;
//						Debug.Log (MC.Transitions [i].TableEntry [j]);
					} else {
						MC.Transitions [i].TableEntry [j] -= sample;
//						Debug.Log (MC.Transitions [i].TableEntry [j]);

					}

				}
			}
			return MC;
		}
		public void SaveAsset(){
			List<RandomWalkMarkovChain> SortedList = FinalTables.OrderBy(o=>o.fitness).ToList();
			Table asset = ScriptableObject.CreateInstance<Table>();
			asset.edgeRow = new EdgeRow[6];
			EdgeRow column;
				Edge edge;
			for (int i = 0; i < 6; i++) {
				column = ScriptableObject.CreateInstance<EdgeRow>();
				column.edges = new Edge[6];
				for(int j = 0 ; j<6 ; j++){
					edge = ScriptableObject.CreateInstance<Edge> ();
//					edge = new Edge ();
					edge.probability = SortedList.Last().Transitions [i].TableEntry [j];
//					Debug.Log (SortedList.Last ().Transitions [i].TableEntry [j]);
					edge.Init (SortedList.Last().Transitions [i].TableEntry [j]);
//					Debug.Log ("my prob is: " + edge.probability);
//					Debug.Log (i + " " + j);
					column.edges[i] = edge;
//					Debug.Log (column.edges [i].probability);

				}
				asset.edgeRow[i] = column;
			}

			for(int j=0; j<6;j++) {
				for(int i = 0; i<6;i++) {
					Debug.Log (i + ": ");
					Debug.Log( asset.edgeRow[j].edges[i].probability);
				}
			}
			Debug.Log ("im so edy");
			asset.SaveToAssetDatabase ();
		}
	}
}

