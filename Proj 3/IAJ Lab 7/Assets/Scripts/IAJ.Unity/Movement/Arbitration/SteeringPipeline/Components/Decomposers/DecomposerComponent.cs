namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Decomposers
{
    //A decomposer takes a goal and decomposes it into a a sub-goal
    public abstract class DecomposerComponent : SteeringPipelineComponent
    {
        public abstract SteeringGoal DecomposeGoal(SteeringGoal goal);
    }
}
