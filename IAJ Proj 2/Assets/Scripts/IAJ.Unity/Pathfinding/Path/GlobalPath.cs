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
        private float offset = 5.0f;

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
           
            var closestParam = this.LocalPaths[(int)previousParam].GetParam(position, previousParam);

            if (PathEnd(previousParam))
            {
                return LocalPaths.Count;
            }
            else
            {
                return (int)previousParam + closestParam;
            }
            //Vector3 pos;
            //float newParam = 0;
            //float returnParam = previousParam;
            //float distance = (position - this.LocalPaths[(int)(previousParam)].GetPosition(previousParam - (int)(previousParam))).sqrMagnitude;

            //for (int paramTest = (int)previousParam; paramTest < this.LocalPaths.Count; paramTest++)
            //{
            //    newParam = this.LocalPaths[paramTest].GetParam(position, previousParam);
            //    pos = this.LocalPaths[paramTest].GetPosition(newParam);

            //    if (newParam + paramTest > previousParam)
            //    {
            //        returnParam = newParam + paramTest;
            //    }

            //}
            //return returnParam;
        }

        public override Vector3 GetPosition(float param)
        {
            if(PathEnd((int)param))
            {
                return this.LocalPaths[this.LocalPaths.Count-1].EndPosition;
            }
            
            return this.LocalPaths[(int)param].GetPosition(param - (int)param);
        }

        public override bool PathEnd(float param)
        {
           if (param >= (this.LocalPaths.Count - 1))
                return true;
            else
            {
                return false;
            }
        }
    }
}
