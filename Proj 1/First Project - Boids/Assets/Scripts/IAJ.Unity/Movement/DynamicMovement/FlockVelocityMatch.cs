using UnityEngine;
using System.Collections;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Movement;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Util;
using System;

public class FlockVelocityMatch : DynamicVelocityMatch
{
    public float radius { get; set; }
    public float fanAngle { get; set; }
    public List<DynamicCharacter> flock { get; set; }
   

    public FlockVelocityMatch(List<DynamicCharacter> flock) {
        this.flock = flock;
        this.MovingTarget = new KinematicData();
        this.Target = new KinematicData();
    }

    public float ShortestAngleDifference(float angle1, float angle2) {

        var minDif = angle2 - angle1;

        if (minDif > MathConstants.MATH_PI) {
            minDif -= MathConstants.MATH_2PI;
        }
        else if (minDif < -MathConstants.MATH_PI) {
            minDif += MathConstants.MATH_2PI;;
        }

        return minDif;
    }

    public override MovementOutput GetMovement()
    {

        radius = 50.0f;
        fanAngle = MathConstants.MATH_PI/2;
        var avgVelocity = new Vector3();
        var closeBoids = 0;

        foreach (var boid in this.flock) {
            if (this.Character != boid.KinematicData){
                var direction = boid.KinematicData.position - this.Character.position;
                


                if (direction.magnitude <= radius){

                    var angle = MathHelper.ConvertVectorToOrientation(direction);
                    var angleDifference = ShortestAngleDifference(this.Character.orientation,angle);
               

                    if (Math.Abs(angleDifference) <= fanAngle) {
                        avgVelocity += boid.KinematicData.velocity;
                        closeBoids++;         
                    }
                        
                }
            }
        }

        if (closeBoids == 0) {
            return new MovementOutput();
        }

        avgVelocity /= closeBoids;
        this.MovingTarget.velocity = avgVelocity;

        Debug.DrawRay(this.Character.position, this.MovingTarget.velocity, Color.green);

        return base.GetMovement();
    }
}
