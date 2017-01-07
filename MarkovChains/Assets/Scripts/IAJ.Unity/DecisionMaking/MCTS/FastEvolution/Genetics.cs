using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS.FastEvolutionMCTS
{
	public class Genetics
	{
		Dictionary<RandomWalkMarkovChain, WorldModel> Pool;

		public Genetics ()
		{
			Pool = new Dictionary<RandomWalkMarkovChain, WorldModel> ();
		}

		public void Cross(RandomWalkMarkovChain mc1, RandomWalkMarkovChain mc2){
			//
			throw new NotImplementedException();
		}

		public double Fitness(RandomWalkMarkovChain mc){

			throw new NotImplementedException ();
		}

		public void CrossOver(){
			throw new NotImplementedException ();	
		}

		public RandomWalkMarkovChain Deviate(){
			throw new NotImplementedException ();
		}
	}
}

