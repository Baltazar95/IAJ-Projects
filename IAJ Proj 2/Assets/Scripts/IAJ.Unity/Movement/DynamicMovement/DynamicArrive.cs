
namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {
        public float maxSpeed { get; private set; }
        public float stopRadius { get; private set; }
        public float slowRadius { get; private set; }

        public DynamicArrive()
        {
            this.MovingTarget = new KinematicData();
        }

        public override string Name
        {
            get { return "Arrive"; }
        }

        public override MovementOutput GetMovement()
        {
            maxSpeed = 20.0f;
            stopRadius = 2.0f;
            slowRadius = 5.0f;
            float targetSpeed;
            var direction = this.Target.position - this.Character.position;
            var distance = direction.magnitude;

            if (distance < stopRadius)
            {
                targetSpeed = 0;
            }

            else if (distance > slowRadius)
            {
                targetSpeed = maxSpeed;
            }
            else
            {
                targetSpeed = maxSpeed * (distance / slowRadius);
            }

            this.MovingTarget.velocity = direction.normalized * targetSpeed;

            return base.GetMovement();
        }
    }
}

