using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicFlee : DynamicSeek
    {
        public override string Name
        {
            get { return "Flee"; }
        }

        public Vector3 point { get; set; }
        public float radius { get; set; }

        public DynamicFlee()
        {

        }

        public override MovementOutput GetMovement()
        {
            var output = new MovementOutput();

            output.linear = this.Character.position - this.Target.position;
                if (output.linear.sqrMagnitude > 0)
                {
                    output.linear.Normalize();
                    output.linear *= this.MaxAcceleration;
                }
                Debug.DrawRay(this.Character.position, this.Target.position, Color.cyan);
   
            
            return output;
        }
    }
}
