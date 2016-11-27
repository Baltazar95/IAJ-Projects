using Assets.Scripts.IAJ.Unity.Util;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicWander : DynamicSeek
    {
        public DynamicWander()
        {
            this.Target = new KinematicData();
        }
        public override string Name
        {
            get { return "Wander"; }
        }
        public float TurnAngle { get; private set; }

        public float WanderOffset { get; private set; }
        public float WanderRadius { get; private set; }

        protected float WanderOrientation { get; set; }

        /* WanderRate defines the maximum reachable angle in each frame*/
        public float WanderRate { get; set; }

        public override MovementOutput GetMovement()
        {
            WanderRate = MathConstants.MATH_PI / 8;
            WanderOffset = 0.25f;
            WanderRadius = 0.1f;
            this.MaxAcceleration = 100.0f;

            WanderOrientation += RandomHelper.RandomBinomial() * WanderRate;

            var targetOrientation = WanderOrientation + this.Character.orientation;

            var circleCenter = this.Character.position + WanderOffset * this.Character.GetOrientationAsVector();

            this.Target.position = circleCenter + WanderRadius * MathHelper.ConvertOrientationToVector(targetOrientation);

            return base.GetMovement();
        }
    }
}
