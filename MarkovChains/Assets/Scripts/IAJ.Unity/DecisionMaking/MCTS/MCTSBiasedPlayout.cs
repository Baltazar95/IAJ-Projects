using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

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
            Reward reward = new Reward();
            double random;


            current = current.GenerateChildWorldModel();
            actions = current.GetExecutableActions();
            if(actions.Length == 0)
            {
                reward.Value = 0;
                reward.PlayerID = current.GetNextPlayer();
                return reward;
            }
            while (!current.IsTerminal())
            {
                accumulate = 0;
                interval.Clear();
                //if (actions.Length == 0)
                //    break;

                foreach (var a in actions)
                {
                    var child = current.GenerateChildWorldModel();
                    var gameMan = CurrentStateWorldModel.GetGameManager();
                    var character = gameMan.autonomousCharacter;
                    //var h = Math.Pow(Math.E, character.BeQuickGoal.InsistenceValue * 2 + character.SurviveGoal.InsistenceValue * 2 + character.GainXPGoal.InsistenceValue * 1 + character.GetRichGoal.InsistenceValue * 3 + 1 / (float)current.GetProperty(Properties.TIME))
                    //var h = Math.Pow(Math.E, child.CalculateDiscontentment(character.Goals) *(float)current.GetProperty(Properties.TIME));
                    var h = Math.Pow(Math.E,
                        CurrentStateWorldModel.GetGoalValue("BeQuick") * 2
                        + CurrentStateWorldModel.GetGoalValue("Survive") * 2
                        + 1 / CurrentStateWorldModel.GetGoalValue("GainXP") * 1
                        + CurrentStateWorldModel.GetGoalValue("GetRich") * 2
                        );

                    //Debug.Log((float)current.GetProperty(Properties.TIME));
                    accumulate += h;
                    interval.Add(accumulate);
                }
                //Debug.Log(accumulate);
                //Debug.Log(RandomGenerator.NextDouble());
                random = RandomGenerator.NextDouble() * accumulate;
                for (int j = 0; j < interval.Count; j++)
                {
                    //maybe it gets stuck here

                    if (random <= interval[j])
                    {
                        action = actions[j];
                        current = current.GenerateChildWorldModel();
                        action.ApplyActionEffects(current);
                        current.CalculateNextPlayer();
                        break;
                        
                    }

                    if (j == interval.Count - 1)
                    {
                        current = current.GenerateChildWorldModel();
                        reward.Value = 0;
                        reward.PlayerID = current.GetNextPlayer();
                        return reward;
                    }
                }
            }
            
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
