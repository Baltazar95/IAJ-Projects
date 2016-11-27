using UnityEngine;
using System.Collections;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Movement;
using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Util;

public class DynamicCohesion : DynamicArrive
{
    protected float radius { get; set; }
    protected float fanAngle { get; set; }
    protected List<DynamicCharacter> Flock { get; set; }

    public DynamicCohesion(List<DynamicCharacter> flock)
    {
        Flock = flock;
        this.Target = new KinematicData();
        //this.MovingTarget = new KinematicData();
    }

    public override KinematicData Target { get; set; }

    public override MovementOutput GetMovement()
    {
        var massCenter = new Vector3();
        var closeBoids = 0;
        radius = 25f;
       //Debug.DrawRay(massCenter, this.Character.velocity.normalized * radius, Color.cyan);

        fanAngle = MathConstants.MATH_PI * 3/2;

        foreach (var boid in Flock)
        {
            if (this.Character != boid.KinematicData)
            {
                var direction = boid.KinematicData.position - this.Character.position;
                if (direction.magnitude <= radius)
                {
                    var angle = MathHelper.ConvertVectorToOrientation(direction);
                    var deltaAngle = ShortestAngleDifference(this.Character.orientation, angle);

                    if (Math.Abs(deltaAngle) <= fanAngle)
                    {
                        massCenter += boid.KinematicData.position;
                        closeBoids++;
                    }

                }
            }
        }

        if (closeBoids == 0)
        {
            return new MovementOutput();
        }
        massCenter /= closeBoids;
        this.Target.position = massCenter;

        Debug.DrawLine(this.Character.position, this.Target.position, Color.blue);
        return base.GetMovement();
    }

    public float ShortestAngleDifference(float angle1, float angle2)
    {
        var deltaAngle = angle2 - angle1;

        if (deltaAngle > MathConstants.MATH_PI)
        {
            deltaAngle -= MathConstants.MATH_2PI;
        }
        else if (deltaAngle < -MathConstants.MATH_PI)
        {
            deltaAngle += MathConstants.MATH_2PI;
        }
        return deltaAngle;
    }
}
