namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Actuators
{
    //An actuator turns a goal into a Path: taking the character's capabilities into account.
    public abstract class ActuatorComponent : SteeringPipelineComponent
    {
        public abstract Path GetPath(SteeringGoal goal);

        public abstract MovementOutput GetSteering(Path path);
    }
}
