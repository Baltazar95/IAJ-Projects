using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class WorldModel
    {
        private Dictionary<string, object> Properties { get; set; }
        //mana, hp, maxhp, xp, time, money, level, position
        private object[] PropertiesArray { get; set; }
        private List<Action> Actions { get; set; }
        protected IEnumerator<Action> ActionEnumerator { get; set; }

        private Dictionary<string, float> GoalValues { get; set; }
        //Survive, xp, money, be quick
        private float[] GoalValuesArray { get; set; }

        //chest
        //chest 1
        //    chset 2
        //    chest 3
        //    chset 4
        //    orc
        //    orc 1
        //    skeleton
        //    skeleton 1
        //    skeleton 2
        //    skeleteon 3
        //    dragon
        //    health potion
        //    health potion 1
        //    mana potion
        //    mana potion 1 
        //private bool[] ResourcesArray { get; set; }
        protected WorldModel Parent { get; set; }

        public WorldModel(List<Action> actions)
        {
            this.Properties = new Dictionary<string, object>();
            this.GoalValues = new Dictionary<string, float>();
            this.Actions = actions;
            this.ActionEnumerator = actions.GetEnumerator();
        }

        public WorldModel(WorldModel parent)
        {
            this.Properties = new Dictionary<string, object>();
            this.GoalValues = new Dictionary<string, float>();
            this.GoalValuesArray = new float[4];
            this.PropertiesArray = new object[24];
            //this.ResourcesArray = new bool[16];
            this.Actions = parent.Actions;
            this.Parent = parent;
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }
        public bool checkEnemiesDead()
        {
            for(int i = 5; i<11; i++)
            {
                if ((bool)PropertiesArray[i] == true)
                    return false;
            }
            return true;
        }
        public int parseProperty(string propertyName)
        {
            //Debug.Log(propertyName);
            switch (propertyName)
            {
                case "Mana":
                    return 0;
                case "HP":
                    return 1;
                case "MAXHP":
                    return 2;
                case "XP":
                    return 3;
                case "Time":
                    return 4;
                case "Money":
                    return 5;
                case "Level":
                    return 6;
                case "Position":
                    return 7;
                case "Chest":
                    return 8;
                case "Chest (1)":
                    return 9;
                case "Chest (2)":
                    return 10;
                case "Chest (3)":
                    return 11;
                case "Chest (4)":
                    return 12;
                case "Orc":
                    return 13;
                case "Orc (1)":
                    return 14;
                case "Skeleton":
                    return 15;
                case "Skeleton (1)":
                    return 16;
                case "Skeleton (2)":
                    return 17;
                case "Skeleton (3)":
                    return 18;
                case "Dragon":
                    return 19;
                case "HealthPotion":
                    return 20;
                case "HealthPotion (1)":
                    return 21;
                case "ManaPotion":
                    return 22;
                case "ManaPotion (1)":
                    return 23;
            }
            return -1;
        }
        public int parseGoal(string goalName)
        {
            //Debug.Log(goalName);
            switch (goalName)
            {
                case "Survive":
                    return 0;
                case "GainXP":
                    return 1;
                case "BeQuick":
                    return 2;
                case "GetRich":
                    return 3;
            }
            return -1;
        }

        public int parseResource(string resourceName)
        {
            Debug.Log(resourceName);
            switch (resourceName)
            {
                case "Chest":
                    return 0;
                case "Chest (1)":
                    return 1;
                case "Chest (2)":
                    return 2;
                case "Chest (3)":
                    return 3;
                case "Chest (4)":
                    return 4;
                case "Orc":
                    return 5;
                case "Orc (1)":
                    return 6;
                case "Skeleton":
                    return 7;
                case "Skeleton (1)":
                    return 8;
                case "Skeleton (2)":
                    return 9;
                case "Skeleton (3)":
                    return 10;
                case "Dragon":
                    return 11;
                case "HealthPotion":
                    return 12;
                case "HealthPotion (1)":
                    return 13;
                case "ManaPotion":
                    return 14;
                case "ManaPotion (1)":
                    return 15;
            }
            return -1;
        }
        public virtual object GetProperty(string propertyName)
        {
            //recursive implementation of WorldModel
            //if (this.Properties.ContainsKey(propertyName))
            //{
            //    return this.Properties[propertyName];
            //}
            //else if (this.Parent != null)
            //{
            //    return this.Parent.GetProperty(propertyName);
            //}
            //else
            //{
            //    return null;
            //}

            //array implementation
            object value;
            if ((value = this.PropertiesArray[parseProperty(propertyName)]) != null)
            {
                return value;
            }
            else if (this.Parent != null)
            {
                return this.Parent.GetProperty(propertyName);
            }
            else
            {
                return null;
            }
        }

        public virtual void SetProperty(string propertyName, object value)
        {
            this.Properties[propertyName] = value;

            //array implementation
            this.PropertiesArray[parseProperty(propertyName)] = value;
        }

        public virtual float GetGoalValue(string goalName)
        {
            //recursive implementation of WorldModel
            //if (this.GoalValues.ContainsKey(goalName))
            //{
            //    return this.GoalValues[goalName];
            //}
            //else if (this.Parent != null)
            //{
            //    return this.Parent.GetGoalValue(goalName);
            //}
            //else
            //{
            //    return 0;
            //}

            //array implementation
            //mb do check index function
			float value = this.GoalValuesArray[parseGoal(goalName)];
            if (value != null)
            {
                return value;
            }
            else if (this.Parent != null)
            {
                return this.Parent.GetGoalValue(goalName);
            }
            else
            {
                return 0;
            }
        }

        public virtual void SetGoalValue(string goalName, float value)
        {
            var limitedValue = value;
            if (value > 10.0f)
            {
                limitedValue = 10.0f;
            }

            else if (value < 0.0f)
            {
                limitedValue = 0.0f;
            }

            //this.GoalValues[goalName] = limitedValue;

            //array implementation
            this.GoalValuesArray[parseGoal(goalName)] = limitedValue;
        }

        //public virtual bool GetResource(string resourceName)
        //{
        //    return this.ResourcesArray[parseResource(resourceName)];
        //}

        //public virtual void SetResource(string resourceName, bool value)
        //{
        //    this.ResourcesArray[parseResource(resourceName)] = value;
        //}

        public virtual WorldModel GenerateChildWorldModel()
        {
            return new WorldModel(this);
        }

        public float CalculateDiscontentment(List<Goal> goals)
        {
            var discontentment = 0.0f;

            foreach (var goal in goals)
            {
                var newValue = this.GetGoalValue(goal.Name);

                discontentment += goal.GetDiscontentment(newValue);
            }

            return discontentment;
        }

        public virtual Action GetNextAction()
        {
            Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ActionEnumerator.MoveNext())
            {
                action = this.ActionEnumerator.Current;
            }

            while (action != null && !action.CanExecute(this))
            {
                if (this.ActionEnumerator.MoveNext())
                {
                    action = this.ActionEnumerator.Current;
                }
                else
                {
                    action = null;
                }
            }

            return action;
        }

        public virtual Action[] GetExecutableActions()
        {
            return this.Actions.Where(a => a.CanExecute(this)).ToArray();
        }

        public virtual bool IsTerminal()
        {
            return true;
        }


        public virtual float GetScore()
        {
            return 0.0f;
        }

        public virtual int GetNextPlayer()
        {
            return 0;
        }

        public virtual void CalculateNextPlayer()
        {
        }
    }
}
