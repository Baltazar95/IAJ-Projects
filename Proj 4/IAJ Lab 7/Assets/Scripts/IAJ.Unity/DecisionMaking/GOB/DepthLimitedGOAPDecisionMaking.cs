using Assets.Scripts.GameManager;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 3;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModel InitialWorldModel { get; set; }
        // List of original actions (not needed after all)
        // private List<Action> Actions { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth {  get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals, int maxActionCombinations)
        {
            this.ActionCombinationsProcessedPerFrame = maxActionCombinations;
            //this.Actions = actions;
            this.Goals = goals;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionPerLevel = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.MaxValue;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {
            var actionCombinations = 0;
            var startTime = Time.realtimeSinceStartup;

            while(this.CurrentDepth >= 0)
            {
                if(actionCombinations > this.ActionCombinationsProcessedPerFrame)
                {
                    this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
                    return null;
                }

                if(this.CurrentDepth >= MAX_DEPTH)
                {
                    var currentValue = Models[CurrentDepth].CalculateDiscontentment(this.Goals);

                    if(currentValue < this.BestDiscontentmentValue)
                    {
                        this.BestDiscontentmentValue = currentValue;
                        this.BestAction = this.ActionPerLevel[0];

                        for(int i = 0; i < this.ActionPerLevel.Length; i++)
                        {
                            this.BestActionSequence[i] = this.ActionPerLevel[i];
                        }
                    }
                    CurrentDepth -= 1;
                    actionCombinations += 1;
                    this.TotalActionCombinationsProcessed = actionCombinations;
                    continue;
                }

                Action nextAction = Models[CurrentDepth].GetNextAction();

                if(nextAction != null)
                {
                    Models[CurrentDepth + 1] = Models[CurrentDepth].GenerateChildWorldModel();
                    nextAction.ApplyActionEffects(Models[CurrentDepth + 1]);
                    Models[CurrentDepth + 1].CalculateNextPlayer();
                    ActionPerLevel[CurrentDepth] = nextAction;
                    CurrentDepth += 1;
                }
                else
                {
                    CurrentDepth -= 1;
                }
            }

            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            this.InProgress = false;

            return this.BestAction;
        }
    }
}
