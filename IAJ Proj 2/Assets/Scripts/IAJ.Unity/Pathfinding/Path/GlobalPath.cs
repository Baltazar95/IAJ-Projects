using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Utils;
using RAIN.Navigation.Graph;
using UnityEngine;
using System;
namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class GlobalPath : Path
    {
        public List<NavigationGraphNode> PathNodes { get; protected set; }
        public List<Vector3> PathPositions { get; protected set; } 
        public bool IsPartial { get; set; }
        public float Length { get; set; }
        public List<LocalPath> LocalPaths { get; protected set; } 


        public GlobalPath()
        {
            this.PathNodes = new List<NavigationGraphNode>();
            this.PathPositions = new List<Vector3>();
            this.LocalPaths = new List<LocalPath>();
        }

        public void CalculateLocalPathsFromPathPositions(Vector3 initialPosition)
        {
            Vector3 previousPosition = initialPosition;

           
            for (int i = 0; i < this.PathPositions.Count; i++)
            {
                if (!previousPosition.Equals(this.PathPositions[i]))
                {
                    this.LocalPaths.Add(new LineSegmentPath(previousPosition, this.PathPositions[i]));
                    previousPosition = this.PathPositions[i];
                }
            }
        }

        public override float GetParam(Vector3 position, float previousParam)
        {
            Vector3 pos;
            float newParam = 0;
            float distance = (position - this.LocalPaths[(int)Math.Truncate(previousParam)].GetPosition(previousParam)).sqrMagnitude;
            for (int paramTest = (int)Math.Truncate(previousParam); paramTest < this.LocalPaths.Count; paramTest++) {
                newParam = this.LocalPaths[paramTest].GetParam(position, previousParam);
                Debug.Log("new param: " + newParam);
                pos = this.LocalPaths[paramTest].GetPosition(newParam);
                if ((position - pos).sqrMagnitude < distance) {
                    distance = (position - pos).sqrMagnitude;
                }
            }
            return newParam;
        }

        public override Vector3 GetPosition(float param)
        {
            return LocalPaths[0].GetPosition(param);
        }

        public override bool PathEnd(float param)
        {
            //TODO implement
            throw new System.NotImplementedException();
        }
    }
}
