using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;

namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Actuators 
{
    public class BasicActuator : ActuatorComponent
    {

        public DynamicSeek Seek { get; set; }

        public override Path GetPath(SteeringGoal goal)
        {
            return new Path
            {
                Goal = goal,
                KinematicData = this.Pipeline.Character
            };
        }

        public override MovementOutput GetSteering(Path path)
        {
            if (path.Goal.PositionSet)
            {
                this.Seek.Character = this.Pipeline.Character;
                this.Seek.Target = path.Goal.ToKinematicData();
                this.Seek.MaxAcceleration = this.Pipeline.MaxAcceleration;

                return this.Seek.GetMovement();
            }
            else
            {
                return new MovementOutput();
            }
        }
    }
}
