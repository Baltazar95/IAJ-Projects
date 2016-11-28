using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedPlayout : MCTS
    {
        public MCTSBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
        }

        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            GOB.Action action;
            GOB.Action[] actions;
            List<double> interval = new List<double>();
            double accumulate = 0;
            WorldModel current = initialPlayoutState;
            double random;

            while (!current.IsTerminal())
            {
                accumulate = 0;
                interval.Clear();
                actions = current.GetExecutableActions();
                if (actions.Length == 0)
                    continue;

                foreach (var a in actions)
                {
                    var child = current.GenerateChildWorldModel();

                    var h = Math.Pow(Math.E, -child.CalculateDiscontentment(CurrentStateWorldModel.GetGameManager().autonomousCharacter.Goals));
                    accumulate += h;
                    interval.Add(accumulate);
                }

                random = RandomGenerator.NextDouble() * accumulate;
                for(int j = 0; j < interval.Count; j++)
                {
                    if(random < interval[j])
                    {
                        action = actions[j];
                        current = current.GenerateChildWorldModel();
                        action.ApplyActionEffects(current);
                        current.CalculateNextPlayer();
                        break;
                    }
                }
            }

            Reward reward = new Reward();
            reward.PlayerID = current.GetNextPlayer();
            reward.Value = current.GetScore();
            return reward;
        }

        protected override MCTSNode Expand(MCTSNode parent, GOB.Action action)
        {
            WorldModel state = parent.State.GenerateChildWorldModel();
            MCTSNode child = new MCTSNode(state);

            child.Parent = parent;
            action.ApplyActionEffects(state);
            state.CalculateNextPlayer();
           
           

            child.Action = action;
            parent.ChildNodes.Add(child);

            return child;
        }
    }
}
