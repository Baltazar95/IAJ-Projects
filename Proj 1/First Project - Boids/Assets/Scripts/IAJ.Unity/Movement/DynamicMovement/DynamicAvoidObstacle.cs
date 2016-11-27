using Assets.Scripts.IAJ.Unity.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicAvoidObstacle : DynamicSeek
    {
        private Collider collider;
        public float AvoidMargin { get; set; }
        public float MaxLookAhead { get; set; }
        public GameObject gameObject { get; set; }



        public DynamicAvoidObstacle(GameObject ob) {
            this.gameObject = ob;
            this.Target = new KinematicData();
        }

        public override MovementOutput GetMovement()
        {
            int layerMask = 1 << 8;
            layerMask = ~layerMask;
            RaycastHit hit;
            RaycastHit hitw1;
            RaycastHit hitw2;
            this.Character.SetOrientationFromVelocity();
            var angle = this.Character.orientation - MathConstants.MATH_PI/8;

            var rayVector = Physics.Raycast(this.Character.position,this.Character.velocity,out hit, MaxLookAhead, layerMask);


            //Debug.DrawRay(this.Character.position, this.Character.velocity.normalized*MaxLookAhead,Color.green);
            //Debug.DrawRay(this.Character.position, MathHelper.ConvertOrientationToVector(angle)* MaxLookAhead / 2, Color.red);
            //Debug.DrawRay(this.Character.position, MathHelper.ConvertOrientationToVector(angle + MathConstants.MATH_PI / 4)* MaxLookAhead / 2, Color.yellow);

            var w1 = Physics.Raycast(this.Character.position, MathHelper.ConvertOrientationToVector(angle) , out hitw1, 5.0f, layerMask);
            var w2 = Physics.Raycast(this.Character.position, MathHelper.ConvertOrientationToVector(angle + MathConstants.MATH_PI / 4) , out hitw2, 5.0f, layerMask);

            if (rayVector == false && w1 == false && w2 == false)
            {
                return new MovementOutput();
            }
            else if (rayVector == false && w1 == true && w2 == false)
            {
                this.Target.position = hitw1.point + hitw1.normal * AvoidMargin;
                return base.GetMovement();
            }

            else if (rayVector == false && w2 == true && w1 == false)
            {
                this.Target.position = hitw2.point + hitw2.normal * AvoidMargin;
                return base.GetMovement();
            }

            //if(rayVector == false)
            //{
            //    this.Target.position = this.Character.position * MaxLookAhead;
            //    return base.GetMovement();
            //}

            this.Target.position = hit.point + hit.normal * AvoidMargin; 

            return base.GetMovement();

        }


    }
}
