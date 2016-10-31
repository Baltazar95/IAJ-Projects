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
            Vector3 vec, n;

            n.x = EndPosition.x - StartPosition.x ;
            
            n.z = EndPosition.z - StartPosition.z;



            vec.x = StartPosition.x + n.x * param;
            vec.y = StartPosition.y;
            vec.z = StartPosition.z + n.z * param;

         

            return vec;
        }

        public override bool PathEnd(float param)
        {
            float p = param - (int)param;
            if (p < 1.0f)
                return false;
            else
                return true;
        }

        public override float GetParam(Vector3 position, float lastParam)
        {
            return MathHelper.closestParamInLineSegmentToPoint(this.StartPosition, this.EndPosition, position);
           
        }
    }
}
