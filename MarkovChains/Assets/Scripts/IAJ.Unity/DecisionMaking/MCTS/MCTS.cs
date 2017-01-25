using Assets.Scripts.DecisionMakingActions;
using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTS
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int MaxIterationsProcessedPerFrame { get; set; }
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<GOB.Action> BestActionSequence { get; private set; }
		public GOB.Action previousAction;


        protected int CurrentIterations { get; set; }
        protected int CurrentIterationsInFrame { get; set; }
        protected int CurrentDepth { get; set; }

        protected CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        protected MCTSNode InitialNode { get; set; }

        protected System.Random RandomGenerator { get; set; }
        
        

        public MCTS(CurrentStateWorldModel currentStateWorldModel)
        {
            this.InProgress = false;
            this.CurrentStateWorldModel = currentStateWorldModel;
            this.MaxIterations = 100;
            this.MaxIterationsProcessedPerFrame = 10;
            this.RandomGenerator = new System.Random();
        }


        public void InitializeMCTSearch()
        {
            this.MaxPlayoutDepthReached = 0;
            this.MaxSelectionDepthReached = 0;
            this.CurrentIterations = 0;
            this.CurrentIterationsInFrame = 0;
            this.TotalProcessingTime = 0.0f;
            this.CurrentStateWorldModel.Initialize();
            this.InitialNode = new MCTSNode(this.CurrentStateWorldModel)
            {
                Action = null,
                Parent = null,
                PlayerID = 0
            };
            this.InProgress = true;
            this.BestFirstChild = null;
            this.BestActionSequence = new List<GOB.Action>();
        }

        public GOB.Action Run()
        {
            MCTSNode selectedNode;
            Reward reward;

            var startTime = Time.realtimeSinceStartup;
            this.CurrentIterationsInFrame = 0;

            while (this.CurrentIterationsInFrame < this.MaxIterationsProcessedPerFrame && this.CurrentIterations < this.MaxIterations)
            {
                this.CurrentDepth = 0;
                selectedNode = Selection(this.InitialNode);
				previousAction = selectedNode.Action;
                reward = Playout(selectedNode.State);

                Backpropagate(selectedNode, reward);
                this.CurrentIterationsInFrame++;
                this.CurrentIterations++;
            }

            if (this.CurrentIterations >= this.MaxIterations)
            {
                this.InProgress = false;
            }

            this.BestFirstChild = BestChild(InitialNode);

            this.BestActionSequence.Clear();
            var auxNode = this.BestFirstChild;
            while (true)
            {
                if (auxNode == null)
                {
                    break;
                }
                this.BestActionSequence.Add(auxNode.Action);
                auxNode = BestChild(auxNode);
            }

            if (this.BestFirstChild == null)
            {
                return null;
            }
            this.TotalProcessingTime = Time.realtimeSinceStartup - startTime;
            return this.BestFirstChild.Action;
    }

		public virtual void SaveAssets()
		{

		}
        protected MCTSNode Selection(MCTSNode initialNode)
        {
            GOB.Action nextAction;
            MCTSNode currentNode = initialNode;
            MCTSNode previous;

            while (!currentNode.State.IsTerminal())
            {
                nextAction = currentNode.State.GetNextAction();
                if (nextAction != null)
                {
                      
                    return Expand(currentNode, nextAction);
                }
                else
                {
                    previous = currentNode;
                    currentNode = BestUCTChild(currentNode);
                    if (currentNode == null)
                    {
                        return previous;
                    }
                    this.CurrentDepth++;
                }
            }
            return currentNode;
        }


		protected virtual MCTSNode Expand(MCTSNode parent, GOB.Action action)
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

        protected virtual Reward Playout(WorldModel initialPlayoutState)
        {
            GOB.Action action;
            GOB.Action[] actions;
            Reward reward = new Reward();
            WorldModel current = initialPlayoutState;
            int random;
            actions = current.GetExecutableActions();
            if (actions.Length == 0)
            {
                reward.PlayerID = current.GetNextPlayer();
                reward.Value = 0;
				return reward;
            }

            while (!current.IsTerminal())
            {
                current = current.GenerateChildWorldModel();
                random = RandomGenerator.Next(0, actions.Length);
                action = actions[random];
                action.ApplyActionEffects(current);
                current.CalculateNextPlayer();
            }

            reward.PlayerID = current.GetNextPlayer();
            reward.Value = current.GetScore();
            return reward;
        }

        protected virtual void Backpropagate(MCTSNode node, Reward reward)
        {
            while (node != null)
            {
                node.N++;
                node.Q += reward.GetRewardForNode(node);
                node = node.Parent;

            }
        }


        protected virtual MCTSNode BestUCTChild(MCTSNode node)
        {
            List<MCTSNode> children = node.ChildNodes;
            MCTSNode best, currentChild;
            float ui;
            double uct;
            double BestUCT = -1;
            best = null;
            for (int count = 0; count < children.Count; count++)
            {
                currentChild = children[count];
                ui = currentChild.Q / currentChild.N;
                uct = ui + C * Math.Sqrt(Math.Log(currentChild.Parent.N) / currentChild.N);
                if (uct > BestUCT)
                {
                    BestUCT = uct;
                    best = currentChild;

                }
            }
            return best;
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        protected MCTSNode BestChild(MCTSNode node)
        {
            List<MCTSNode> children = node.ChildNodes;
            MCTSNode best, currentChild;
            float ui;
            double uct;
            double BestUCT = -1;
            best = null;
            for (int count = 0; count < children.Count; count++)
            {
                currentChild = children[count];
                ui = currentChild.Q / currentChild.N;
                uct = ui + Math.Sqrt(Math.Log(currentChild.Parent.N) / currentChild.N);
                if (uct > BestUCT)
                {
                    BestUCT = uct;
                    best = currentChild;
                }
                else if(uct == BestUCT)
                {
                    var random = RandomGenerator.Next(0, 2);
                    if(random == 1)
                    {
                        BestUCT = uct;
                        best = currentChild;
                    }
                }
            }
            return best;
        }

        
        protected GOB.Action BestFinalAction(MCTSNode node)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

    }
}
