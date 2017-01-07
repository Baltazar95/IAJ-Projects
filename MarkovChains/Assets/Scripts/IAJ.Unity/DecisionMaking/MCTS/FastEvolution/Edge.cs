using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS.FastEvolutionMCTS
{
	public class Edge : ScriptableObject
	{
		public double probability = 0;


		public Edge(){
		}

		public void Init (double v)
		{
			this.probability = v;
		}

	}
}


