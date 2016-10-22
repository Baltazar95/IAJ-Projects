using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class LineSegmentPath : LocalPath
    {
        protected Vector3 LineVector;
        public LineSegmentPath(Vector3 start, Vector3 end)
        {
            this.StartPosition = start;
            this.EndPosition = end;
            this.LineVector = end - start;
        }

        public override Vector3 GetPosition(float param)
        {
            //TODO implement
            throw new System.NotImplementedException();
        }

        public override bool PathEnd(float param)
        {
            //TODO implement
            throw new System.NotImplementedException();
        }

        public override float GetParam(Vector3 position, float lastParam)
        {
            //TODO implement
            throw new System.NotImplementedException();
        }
    }
}
