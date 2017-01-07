using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Actuators;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Constraints;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Decomposers;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Targeters;

namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline
{
    public class SteeringPipeline : DynamicMovement.DynamicMovement
    {
        public ActuatorComponent Actuator { get; set; }
        public List<TargeterComponent> Targeters { get; protected set; }
        public List<DecomposerComponent> Decomposers { get; protected set; }
        public List<ConstraintComponent> Constraints { get; protected set; }

        public uint MaxConstraintSteps { get; set; }

        public DynamicMovement.DynamicMovement FallBackMovement { get; set; }

        public Path Path { get; protected set; }

        public SteeringGoal Goal { get; set; }

        public override KinematicData Target { get; set; }

        public override string Name
        {
            get { return "Pipeline"; }
        }

        public SteeringPipeline()
        {
            this.Targeters = new List<TargeterComponent>();
            this.Decomposers = new List<DecomposerComponent>();
            this.Constraints = new List<ConstraintComponent>();
        }

        public void RegisterComponents()
        {
            this.Actuator.Pipeline = this;

            foreach (var component in this.Targeters)
            {
                component.Pipeline = this;
            }
            foreach (var component in this.Decomposers)
            {
                component.Pipeline = this;
            }
            foreach (var component in this.Constraints)
            {
                component.Pipeline = this;
            }
        }

        public override MovementOutput GetMovement()
        {
            var goal = this.Goal;
            goal.Clear();

            foreach (var targeter in this.Targeters)
            {
                if (goal.CanMergeGoals(targeter.GetGoal()))
                {
                    goal.UpdateGoal(targeter.GetGoal());
                }
            }

            foreach (var decomposer in this.Decomposers)
            {
                goal = decomposer.DecomposeGoal(goal);
            }

            var constraintSteps = 0;
            float currentViolation, minViolation;
            ConstraintComponent violationConstraint;

            while (constraintSteps < this.MaxConstraintSteps)
            {
                this.Path = this.Actuator.GetPath(goal);

                minViolation = this.Path.GetMaxPriority();
                violationConstraint = null;

                foreach (var constraint in this.Constraints)
                {
                    currentViolation = constraint.WillViolate(this.Path, minViolation);
                    if (currentViolation > 0 && currentViolation < minViolation)
                    {
                        minViolation = currentViolation;
                        violationConstraint = constraint;
                    }
                }

                if (violationConstraint != null)
                {
                    goal = violationConstraint.Suggest(this.Path);
                }
                else
                {
                     return this.Actuator.GetSteering(this.Path);
                }
                constraintSteps++;
            }

            if (this.FallBackMovement != null)
            {
                return this.FallBackMovement.GetMovement();
            }
            else return new MovementOutput();
        }
    }
}
