using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicFollowPath : DynamicArrive
    {
        public Path Path { get; set; }
        public float PathOffset { get; set; }

        public float CurrentParam { get; set; }

        public KinematicData Target { get; set; }
        public KinematicData Character { get; set; }

        private MovementOutput EmptyMovementOutput { get; set; }


        public DynamicFollowPath(KinematicData character, Path path) 
        {
            this.Target = new KinematicData();
            this.Character = character;
            this.Path = path;
            this.EmptyMovementOutput = new MovementOutput();
            this.PathOffset = 0.3f;
            this.CurrentParam = 0.0f;
            //don't forget to set all properties
            //arrive properties
            
            
        }

        public override MovementOutput GetMovement()
        {

            this.CurrentParam = this.Path.GetParam(this.Character.position, CurrentParam);
            float targetParam = this.CurrentParam + this.PathOffset;
            this.Target.position = this.Path.GetPosition(targetParam);
            this.CurrentParam = targetParam;
            return base.GetMovement();
           // throw new NotImplementedException();
        }
    }
}
