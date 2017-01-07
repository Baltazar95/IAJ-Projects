using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {
        public float MaxSpeed { get; set; }

        public float TargetRadius { get; set; }

        public float SlowRadius { get; set; }

        public override string Name
        {
            get { return "Arrive"; }
        }

        public DynamicArrive()
        {
            this.TimeToTargetSpeed = 0.5f;
            this.TargetRadius = 1;
            this.SlowRadius = 5.0f;
        }
        
        
        public override MovementOutput GetMovement()
        {
            float targetSpeed;
            MovementOutput output = new MovementOutput();

            var direction = this.Target.position - this.Character.position;
            var distance = direction.magnitude;

            if (distance < this.TargetRadius)
            {
                targetSpeed = 0.0f;
                direction = this.Character.position;
            }

            if (distance > this.SlowRadius)
            {
                //maximum speed
                targetSpeed = this.MaxSpeed;
            }
            else
            {
                targetSpeed = this.MaxSpeed*distance/this.SlowRadius;
            }

            direction.Normalize();
            this.TargetVelocity.velocity = direction.normalized * targetSpeed;

            return base.GetMovement();
        }
    }
}
