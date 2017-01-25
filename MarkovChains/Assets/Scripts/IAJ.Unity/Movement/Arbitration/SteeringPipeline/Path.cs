namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline
{
    public class Path
    {
        public KinematicData KinematicData { get; set; }

        public SteeringGoal Goal { get; set; }

        public virtual float GetMaxPriority()
        {
            return (this.KinematicData.position - this.Goal.Position).magnitude;
        }
    }
}
