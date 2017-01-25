using Assets.Scripts.IAJ.Unity.DecisionMaking.DataStructures.HPStructures;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.Smelly_GOB
{
    public class SmellyGOB
    {
        public bool InProgress { get; set; }
        public float TotalProcessingTime { get; set; }
        public int Smelliest { get; set; }
        private GOB.Action[] Actions { get; set; }
        private WorldModel WorldModel { get; set; }
        private NavMeshPathGraph navMesh { get; set; }
        private ClusterGraph klausTheCluster { get; set; }
        public float BestDiscontentmentValue { get; private set; }
        private string smellyName;
        private List<Goal> Goals { get; set; }
        //lista de intensidades para cada no
        private List<int> Intensities { get; set; }

        public SmellyGOB(List<Goal> goals, ClusterGraph klausTheCluster, NavMeshPathGraph navMesh, WorldModel wm)
        {
            this.Goals = goals;
            this.klausTheCluster = klausTheCluster;
            this.navMesh = navMesh;
            this.WorldModel = wm;
        }

        public void InitializeSmellyGOB()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
        }

        public GOB.Action Run(Vector3 position)
        {
            SmellyNode klaus = getSmellyNode(position);
            float biggestWeight = 0.0f;
            int count = 0;
            Goal Goal = null;
            this.Actions = this.WorldModel.GetExecutableActions();
            string name = "";
            int xp = (int)this.WorldModel.GetProperty(GameManager.Properties.XP);

            for (int i = 0; i < this.Actions.Count(); i++)
            {
                if (this.Actions[i].Name.Equals("LevelUp") && xp >= 10)
                {
                    return this.Actions[i];
                }
            }


            foreach (var goal in this.Goals)
            {

                if (biggestWeight < goal.InsistenceValue)
                {
                    biggestWeight = goal.InsistenceValue;
                    Goal = goal;
                }
            }

            if (Goal.Name.Equals("GetRich") || Goal.Name.Equals("BeQuick"))
            {

                for (int i = 5; i < 10; i++)
                {
                    if (klaus.smellyIntensity[i] >= 999)
                    {
                        if (i == 5 && GameObject.Find("Skeleton (3)"))
                        {
                            for (int j = 0; j < this.Actions.Count(); j++)
                            {
                                if (this.Actions[j].Name.Equals("SwordAttack(Skeleton (3))"))
                                {

                                    this.Actions[j].ApplyActionEffects(this.WorldModel);
                                    return this.Actions[j];

                                }
                            }

                        }
                        else if (i == 6 && GameObject.Find("Skeleton (2)"))
                        {
                            for (int j = 0; j < this.Actions.Count(); j++)
                            {
                                if (this.Actions[j].Name.Equals("SwordAttack(Skeleton (2))"))
                                {

                                    this.Actions[j].ApplyActionEffects(this.WorldModel);
                                    return this.Actions[j];

                                }
                            }
                        }
                        else if (i == 7 && GameObject.Find("Orc (1)"))
                        {
                            for (int j = 0; j < this.Actions.Count(); j++)
                            {
                                if (this.Actions[j].Name.Equals("SwordAttack(Orc (1))"))
                                {

                                    this.Actions[j].ApplyActionEffects(this.WorldModel);
                                    return this.Actions[j];

                                }
                            }
                        }
                        else if (i == 8 && GameObject.Find("Orc"))
                        {
                            for (int j = 0; j < this.Actions.Count(); j++)
                            {
                                if (this.Actions[j].Name.Equals("SwordAttack(Orc)"))
                                {

                                    this.Actions[j].ApplyActionEffects(this.WorldModel);
                                    return this.Actions[j];

                                }
                            }
                        }
                        else if (i == 9 && GameObject.Find("Dragon"))
                        {
                            for (int j = 0; j < this.Actions.Count(); j++)
                            {
                                if (this.Actions[j].Name.Equals("SwordAttack(Dragon)"))
                                {

                                    this.Actions[j].ApplyActionEffects(this.WorldModel);
                                    return this.Actions[j];

                                }
                            }
                        }
                    }
                }

                while (count < 5)
                {
                    if (this.Smelliest < klaus.smellyIntensity[count] && GameObject.Find(klaus.objectNames[count]))
                    {
                        this.Smelliest = klaus.smellyIntensity[count];
                        smellyName = klaus.objectNames[count];
                    }
                    count++;
                }
                if (smellyName.Equals("Chest") && GameObject.Find("Chest"))
                {
                    for (int j = 0; j < this.Actions.Count(); j++)
                    {
                        if (this.Actions[j].Name.Equals("PickUpChest(Chest)"))
                        {

                            this.Actions[j].ApplyActionEffects(this.WorldModel);
                            this.Smelliest = 0;
                            return this.Actions[j];

                        }
                    }
                }

                else if (smellyName.Equals("Chest (1)") && GameObject.Find("Chest (1)"))
                {
                    for (int j = 0; j < this.Actions.Count(); j++)
                    {
                        if (this.Actions[j].Name.Equals("PickUpChest(Chest (1))"))
                        {

                            this.Actions[j].ApplyActionEffects(this.WorldModel);
                            this.Smelliest = 0;

                            return this.Actions[j];

                        }
                    }
                }

                else if (smellyName.Equals("Chest (2)") && GameObject.Find("Chest (2)"))
                {
                    for (int j = 0; j < this.Actions.Count(); j++)
                    {
                        if (this.Actions[j].Name.Equals("PickUpChest(Chest (2))"))
                        {

                            this.Actions[j].ApplyActionEffects(this.WorldModel);
                            this.Smelliest = 0;

                            return this.Actions[j];

                        }
                    }
                }
                else if (smellyName.Equals("Chest (3)") && GameObject.Find("Chest (3)"))
                {
                    for (int j = 0; j < this.Actions.Count(); j++)
                    {
                        if (this.Actions[j].Name.Equals("PickUpChest(Chest (3))"))
                        {

                            this.Actions[j].ApplyActionEffects(this.WorldModel);
                            this.Smelliest = 0;

                            return this.Actions[j];

                        }
                    }
                }

                else if (smellyName.Equals("Chest (4)") && GameObject.Find("Chest (4)"))
                {
                    for (int j = 0; j < this.Actions.Count(); j++)
                    {
                        if (this.Actions[j].Name.Equals("PickUpChest(Chest (4))"))
                        {

                            this.Actions[j].ApplyActionEffects(this.WorldModel);
                            this.Smelliest = 0;

                            return this.Actions[j];

                        }
                    }
                }
            }

            if (Goal.Name.Equals("Survive"))
            {
                if (klaus.smellyIntensity[10] > klaus.smellyIntensity[11])
                {
                    name = "GetHealthPotion" + "(" + klaus.objectNames[10] + ")";
                    for (int j = 0; j < this.Actions.Count(); j++)
                    {
                        if (this.Actions[j].Name.Equals(name))
                        {

                            this.Actions[j].ApplyActionEffects(this.WorldModel);
                            return this.Actions[j];

                        }
                    }
                }
                else
                {
                    name = "GetHealthPotion" + "(" + klaus.objectNames[11] + ")";
                    for (int j = 0; j < this.Actions.Count(); j++)
                    {
                        if (this.Actions[j].Name.Equals(name))
                        {

                            this.Actions[j].ApplyActionEffects(this.WorldModel);
                            return this.Actions[j];

                        }
                    }
                }


            }



            return null;
        }


        private SmellyNode getSmellyNode(Vector3 position)
        {
            NavigationGraphNode smellyNode = navMesh.QuantizeToNode(position, 1.0f);
            ((NavMeshPoly)smellyNode).AddConnectedPoly(smellyNode.Position);
            int smellyIndex = smellyNode.NodeIndex;
            return this.klausTheCluster.smellyNodes[smellyIndex];
        }

    }
}
