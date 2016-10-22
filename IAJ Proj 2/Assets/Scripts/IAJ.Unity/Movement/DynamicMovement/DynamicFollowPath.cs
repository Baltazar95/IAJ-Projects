using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicFollowPath : DynamicArrive
    {
        public Path Path { get; set; }
        public float PathOffset { get; set; }

        public float CurrentParam { get; set; }

        private MovementOutput EmptyMovementOutput { get; set; }


        public DynamicFollowPath(KinematicData character, Path path) 
        {
            this.Target = new KinematicData();
            this.Character = character;
            this.Path = path;
            this.EmptyMovementOutput = new MovementOutput();
            //don't forget to set all properties
            //arrive properties
            
            
        }

        public override MovementOutput GetMovement()
        {
            //TODO implement the GetMovement 
            throw new NotImplementedException();
        }
    }
}
