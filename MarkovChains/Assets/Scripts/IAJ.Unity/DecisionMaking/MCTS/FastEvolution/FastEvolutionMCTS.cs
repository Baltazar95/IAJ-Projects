using Assets.Scripts.GameManager;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS.FastEvolutionMCTS
{
	public class FastEvolutionMCTS : MCTS
	{
		RandomWalkMarkovChain MC;
		Genetics evo;
		public FastEvolutionMCTS(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
		{
			evo = new Genetics ();
		}

		public override void SaveAssets(){
			evo.CrossOver ();
			evo.SaveAsset ();
//			MC.SaveTable ();	
		}

		protected override Reward Playout(WorldModel initialPlayoutState)
		{ 
			Reward reward = new Reward();
			WorldModel current = initialPlayoutState;


			MC = new RandomWalkMarkovChain (current, previousAction);
			if (MC.ExecutableActions.Length == 0)
			{
				reward.PlayerID = current.GetNextPlayer();
				reward.Value = 0;
				return reward;
			}

			MC.setGoals (CurrentStateWorldModel.GetGameManager ().autonomousCharacter.Goals);
			MC.Init ();

//			Debug.Log ("BUgs");
			while (!current.IsTerminal())
			{
				current = MC.DoRandomTransition (current);
				current.CalculateNextPlayer();
			}
			evo.AddTable (MC, current);
			MC = evo.Deviate (MC);
			reward.PlayerID = current.GetNextPlayer();
			reward.Value = (float)MC.GetScore();
			return reward;


		}

		protected override void Backpropagate (MCTSNode node, Reward reward)
		{
			while (node != null)
			{
				node.N++;
				node.Q += reward.GetRewardForNode(node);
				node = node.Parent;

			}
		}

	}
}
