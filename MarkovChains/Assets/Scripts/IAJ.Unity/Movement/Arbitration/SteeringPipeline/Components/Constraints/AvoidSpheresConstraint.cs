using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Constraints
{
    public class AvoidSpheresConstraint : ConstraintComponent
    {
        public SteeringGoal Suggestion { get; protected set; }

        public float AvoidMargin { get; protected set; }

        public List<Transform> Obstacles { get; protected set; }

        public AvoidSpheresConstraint(SteeringGoal suggestion, float avoidMargin)
        {
            this.Suggestion = suggestion;
            this.Obstacles = new List<Transform>();
            this.AvoidMargin = avoidMargin;
        }

        public override float WillViolate(Path path, float maxPriority)
        {
            var priority = float.MaxValue;
            float tempPriority;

            foreach (var obstacle in this.Obstacles)
            {
                tempPriority = this.WillViolate(path, priority, obstacle);
                if (tempPriority < priority) priority = tempPriority;
            }

            return priority;
        }

        protected float WillViolate(Path path, float maxPriority, Transform obstacle)
        {
            if (!path.Goal.PositionSet) return float.MaxValue;

            var direction = path.Goal.Position - this.Pipeline.Character.position;

            if (direction.sqrMagnitude > 0)
            {
                var normalizedDirection = direction.normalized;
                var selfToObstacle = obstacle.position - this.Pipeline.Character.position;

                var dotProduct = Vector3.Dot(selfToObstacle, normalizedDirection);
                var distanceSquared = selfToObstacle.sqrMagnitude - dotProduct * dotProduct;

                float radius = obstacle.localScale.x/2 + this.AvoidMargin;

                if (distanceSquared < radius * radius)
                {
                    if (dotProduct > 0 && dotProduct < maxPriority)
                    {
                        var closestPoint = this.Pipeline.Character.position + normalizedDirection * dotProduct;

                        this.Suggestion.Position = obstacle.position + (closestPoint - obstacle.position).normalized*radius;

                        return dotProduct;
                    }
                }
            }

            return float.MaxValue;
        }

        public override SteeringGoal Suggest(Path path)
        {
            return this.Suggestion;
        }
    }
}
