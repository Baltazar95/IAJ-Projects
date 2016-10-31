using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicFollowPath : DynamicArrive
    {
        public GlobalPath Path { get; set; }
        public float PathOffset { get; set; }

        public float CurrentParam { get; set; }

        //public KinematicData Target { get; set; }
        //public KinematicData Character { get; set; }

        private MovementOutput EmptyMovementOutput { get; set; }


        public DynamicFollowPath(KinematicData character, GlobalPath path) 
        {
           // this.MovingTarget = new KinematicData();
            this.Target = new KinematicData();
            this.Character = character;
            this.Path = path;
            this.EmptyMovementOutput = new MovementOutput();
            this.PathOffset = 3.0f;
            this.CurrentParam = 0.0f;
            //don't forget to set all properties
            //arrive properties
            
            
        }

        public override MovementOutput GetMovement()
        {
            
            this.MaxAcceleration = 20.0f;
            this.CurrentParam = this.Path.GetParam(Character.position + Character.velocity, CurrentParam);
            float targetParam = this.CurrentParam + this.PathOffset;

            var distance = (this.Target.position - this.Character.position).magnitude;
            Debug.Log("DISTANCE " + distance);
            if (distance > 20.0f) {
                Debug.Log("I AM HEREHRERIHEHREHR");
                this.PathOffset /= 3;
            }
            if (this.Path.PathEnd(targetParam))
            {

                this.PathOffset = 0.2f;
                targetParam = this.CurrentParam + this.PathOffset;
            }

            Debug.Log("target param: " + targetParam);

            this.Target.position = this.Path.GetPosition(targetParam);
            Debug.Log("target " + this.Target.position);

            return base.GetMovement();
        }
    }
}
