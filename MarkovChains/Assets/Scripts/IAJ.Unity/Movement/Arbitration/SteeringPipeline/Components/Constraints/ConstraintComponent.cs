namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Constraints
{
    //A Constraint takes a Path to a goal, determines if it violates the constraint, then returns an action to avoid the constraint.
    public abstract class ConstraintComponent : SteeringPipelineComponent
    {
        public bool SuggestionUsed { get; set; }
        public abstract float WillViolate(Path path, float maxPriority);

        public abstract SteeringGoal Suggest(Path path);

        protected ConstraintComponent()
        {
            this.SuggestionUsed = false;
        }
    }
}
