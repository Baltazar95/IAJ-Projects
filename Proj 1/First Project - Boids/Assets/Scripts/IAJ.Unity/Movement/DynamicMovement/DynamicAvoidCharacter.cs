using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicAvoidCharacter : DynamicSeek
    {
        public float AvoidMargin;
        private KinematicData kinematicData;

        public float maxTimeLookAhead { get; private set; }
        public float collisionRadius { get; private set; }


        public DynamicAvoidCharacter(KinematicData kinematicData)
        {
            this.Target = kinematicData;
        }

        public override MovementOutput GetMovement()
        {
            MovementOutput output = new MovementOutput();

            var deltaPos = this.Target.position - this.Character.position;
            var deltaVel = this.Target.velocity - this.Character.velocity;
            var deltaSpeed = deltaVel.magnitude;

            if (deltaSpeed == 0) {

                return output;
            }

            var timeToClosest = -Vector3.Dot(deltaPos, deltaVel)/(deltaSpeed*deltaSpeed);
            if (timeToClosest > maxTimeLookAhead)
            {

                return output;
            }

            var futureDeltaPos = deltaPos + deltaVel * timeToClosest;
            var futureDistance = futureDeltaPos.magnitude;
            if (futureDistance > 2 * collisionRadius)
            {
                return new MovementOutput();
            }

            if (futureDistance <= 0 || deltaPos.magnitude < 2 * collisionRadius)
            {
                output.linear = this.Character.position - this.Target.position;
            }
            else
            {
                output.linear = futureDeltaPos * -1;
            }



            output.linear = output.linear.normalized * MaxAcceleration;

            return output;
        }
    }
}
