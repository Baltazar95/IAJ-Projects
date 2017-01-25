using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameManager;
using System;
using Assets.Scripts.IAJ.Unity.Utils;
using Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS.FastEvolutionMCTS;
using System.Text;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine; 
using UnityEditor;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS.FastEvolutionMCTS
{
	public class MCNode
	{
		//maybe the action is not needed after all
		public string actionName = "";
		public bool enabled = true;
		public double actionReward { get; set; }
		public int rowNumber { get; set; }
		public double[] TableEntry;

		public MCNode(){
			TableEntry = new double[6] { 0, 0.166,0.166,0.166,0.166,0.333 };
		}

	}

	public class RandomWalkMarkovChain
	{
		
		public double FinalReward;
		public int iterations = 0;
		public const double DISCOUNT = 0.3; 
		public MCNode currentNode;
		public double fitness { get; set; }
		public WorldModel initialState;
		public GOB.Action[] ExecutableActions;
		public GOB.Action previousAction;
		// FB LVLUP MANA HP PICKUP SA
		public MCNode[] Transitions { get; private set; }
		public List<GOB.Action>[] AllActions;
		public List<Goal> Goals;

		public RandomWalkMarkovChain ()
		{
			Transitions = new MCNode[6];
			Goals = new List<Goal> ();
			//LOAD TABLE HERE
			AllActions = new List<GOB.Action>[6];

			for (int i = 0; i < 6; i++) {
				Transitions [i] = new MCNode ();

				Transitions [(i)].actionName = ParseIndex (i);
				Transitions [i].TableEntry = new double[6] { 0, 0.166, 0.166, 0.166, 0.166, 0.333 };
				//				Debug.Log (Transitions [0].TableEntry [1]);
				Transitions [(i)].enabled = true;
				Transitions [(i)].rowNumber = i;
				Transitions [(i)].actionReward = 0;
			}
		}

		public RandomWalkMarkovChain(WorldModel model, GOB.Action action){
			// I have not yet created an entire table and therefore its needed to have something to test on
			//HOLY SHIT
			//If actions are not executable, the probabilities are nonexistent
			//so they need to be reweighted accordingly
			//fuck
			Transitions = new MCNode[6];
			Goals = new List<Goal> ();
			initialState = model;
			previousAction = action;
			//LOAD TABLE HERE
			AllActions = new List<GOB.Action>[6];
			for (int i = 0; i < 6; i++) {
				AllActions [i] = new List<GOB.Action> ();
			}
			for (int i = 0; i < 6; i++) {
				Transitions [i] = new MCNode ();

				Transitions [(i)].actionName = ParseIndex (i);
				Transitions [i].TableEntry = new double[6] { 0, 0.166, 0.166, 0.166, 0.166, 0.333 };
				//				Debug.Log (Transitions [0].TableEntry [1]);
				Transitions [(i)].enabled = true;
				Transitions [(i)].rowNumber = i;
				Transitions [(i)].actionReward = 0;
			}
			ExecutableActions = model.GetExecutableActions();

		}

		public void setTransition(Table table){
			for (int i = 0; i < 6; i++) {
				for(int j = 0 ; j<6 ; j++){
					Transitions [i].TableEntry [j] = table.edgeRow [i].edges [j].probability;
				}
			}
		}

		public void setTransition(RandomWalkMarkovChain mc){
			for (int i = 0; i < 6; i++) {
				for(int j = 0 ; j<6 ; j++){
					Transitions [i].TableEntry [j] = mc.Transitions[i].TableEntry[j];
				}
			}
		}

		public void setGoals(List<Goal> goals){ Goals = goals; }

		public int ParseActionName(string name){
			if (name.Contains("Fireball")) {
				return 0;
			}
			if (name.Contains("SwordAttack")) {
				return 5;
			}
			if (name.Contains("PickUpChest")) {
				return 4;
			}
			if (name.Contains("GetManaPotion")) {
				return 2;
			}
			if (name.Contains("GetHealthPotion")) {
				return 3;
			}
			if (name.Contains("LevelUp")) {
				return 1;
			}
			return -1;
		}

		public string ParseIndex(int index){
			if (index == 0) {
				return "Fireball";
			}
			if (index == 1) {
				return "LevelUp";
			}
			if (index == 2) {
				return "GetManaPotion";
			}
			if (index == 3) {
				return "GetHealthPotion";
			}
			if (index == 4) {
				return "PickUpChest";
			}
			if (index == 5) {
				return "SwordAttack";
			}
			return null;
		}

		public double GetScore ()
		{
			return this.FinalReward;
		}
		public void RandomTable(){
			foreach (var row in Transitions) {
				for(int i = 0 ; i < 6; i++){
					
					row.TableEntry = new double[6]{ 0, 0, 0, 0, 0, 0 };
				}
			}
		}


		public void Init(){
			//assign the nodes the actions, enable them, reweight the tree, and finally calculate each action reward

			foreach(var entry in ExecutableActions){
				//ADDING
//				Debug.Log("ADDING");
				if(entry.Name.Contains("Fireball")) {
					AllActions [0].Add (entry);
//					Debug.Log (entry.Name);
			}
				if (entry.Name.Contains("SwordAttack")) {
					AllActions [5].Add (entry);
//					Debug.Log (entry.Name);
			}
					if (entry.Name.Contains("PickUpChest")) {
					AllActions [4].Add (entry);
//					Debug.Log (entry.Name);
			}
					if (entry.Name.Contains("GetManaPotion")) {
					AllActions [2].Add (entry);
//					Debug.Log (entry.Name);
			}
					if (entry.Name.Contains("GetHealthPotion")) {
					AllActions [3].Add (entry);
//					Debug.Log (entry.Name);
			}
					if (entry.Name.Contains("LevelUp")) {
					AllActions [1].Add (entry);
//					Debug.Log (entry.Name);
			}
				previousAction = entry;
			}

			for (int i = 0; i < 6; i++) {
				//INITIALIZE TABLE
//				Debug.Log("INITIALIZING");
				Transitions[i] = new MCNode();

				Transitions [(i)].actionName = ParseIndex(i);
				Transitions [i].TableEntry = new double[6] { 0, 0.166,0.166,0.166,0.166,0.333 };
//				Debug.Log (Transitions [0].TableEntry [1]);
				Transitions [(i)].enabled = true;
				Transitions [(i)].rowNumber = i;
				Transitions [(i)].actionReward = 0;
				if(AllActions[i].Count != 0){
				foreach (var entry in AllActions[i]) {
						CalculateReward (initialState, entry);
//						Debug.Log ("Inital rewards" + entry.Name);
				}
				}
//				Debug.Log ("TABLE INITIALIZED");

			}

			this.currentNode = Transitions [ParseActionName (previousAction.Name)];
//			Debug.Log ("MC DEBUG");
			string print = "";
			foreach (var row in Transitions) {
				foreach(var edgy in row.TableEntry){

					print += edgy.ToString ()+ " ";
				}
			}
//			Debug.Log (print);
			//TODO
			//REWEIGHT THE DAMN TREE


		}
		public void CalculateReward(WorldModel newState, GOB.Action entry){

//				Debug.Log (entry.Name);
				//var entry = ExecutableActions[0];
				if((float)newState.GetProperty("Time") >= 100 ||  (int)newState.GetProperty("HP") <= 0)
					Transitions [ParseActionName (entry.Name)].actionReward = -1;
				else if((int)newState.GetProperty("Money") == 25)
					Transitions [ParseActionName (entry.Name)].actionReward = 1;
				else
					Transitions [ParseActionName (entry.Name)].actionReward = 1/newState.CalculateDiscontentment(Goals);
		}




		public WorldModel DoRandomTransition(WorldModel current){
			//on the beggining the only executable actions are sword attack
			//so we have to create the chain on the fly 
			//as in previous action is x, which reward is already calculated
			double accumulator = 0;
			int index = -1;
			//sample from binomial
			float sample = RandomHelper.RandomBinomial();
			//choose transition according to table order, accumulate and see what path to choose
			for (int i = 0; i < 6; i++) {
				if (Transitions [i].enabled) {
					if (sample <= Transitions [currentNode.rowNumber].TableEntry[i] + accumulator) {
						index = i;
						break;
					} else
						accumulator += Transitions [currentNode.rowNumber].TableEntry[i];	
				}
			}
			//switch currentState
			currentNode = Transitions[index];
//			Debug.Log ("I chose " + index + " as " + Transitions [index].actionName);
			//here


			//return state.action
			current = current.GenerateChildWorldModel ();
			GOB.Action action;
			//here cycle here
			if (AllActions [ParseActionName (currentNode.actionName)].Count == 0) {
				action = previousAction;
			} else {
				action = AllActions [ParseActionName (currentNode.actionName)] [0];
				AllActions [ParseActionName (currentNode.actionName)].RemoveAt (0);
			}
				action.ApplyActionEffects (current);
			if (currentNode.actionReward == 0)
				CalculateReward (current, action);
			FinalReward += currentNode.actionReward * Math.Pow(DISCOUNT, iterations);
			iterations++;
			previousAction = action;
			return current;
		}

		public void SaveTable(){
			Table asset = ScriptableObject.CreateInstance<Table>();
			asset.edgeRow = new EdgeRow[6];
			for (int i = 0; i < 6; i++) {
				EdgeRow column = ScriptableObject.CreateInstance<EdgeRow>();
				for(int j = 0 ; j<6 ; j++){
					column.edges = new Edge[6];
					Debug.Log (i + " " + j);
					column.edges [j].probability = Transitions [i].TableEntry [j];
				}
				asset.edgeRow[i] = column;
			}
			asset.SaveToAssetDatabase ();
			Debug.Log ("Saved to Database");
		}
	}
}