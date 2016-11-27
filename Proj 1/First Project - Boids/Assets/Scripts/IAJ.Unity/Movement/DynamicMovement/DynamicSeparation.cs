using UnityEngine;
using System.Collections;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Movement;
using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Util;

public class DynamicSeparation: DynamicMovement
{
    public override string Name
    {
        get { return "Separation"; }
    }

    public override KinematicData Target { get; set; }
    public List<DynamicCharacter> flock { get; set; }
    public float separationFactor { get; set; }
    public float radius { get; set; }
    public float maxAccelaration { get; set; }

    public DynamicSeparation(List<DynamicCharacter> flock)
    {
        this.flock = flock;

        separationFactor = 30.0f;
        radius = 2.0f;
        maxAccelaration = 25.0f;
    }

    public override MovementOutput GetMovement()
    {
        var output = new MovementOutput();

        foreach (var boid in this.flock)
        {
            if(boid.KinematicData != this.Character)
            {
                var direction = this.Character.position - boid.KinematicData.position;
                if (direction.magnitude < radius)
                {
                    var separationStrength = Mathf.Min(separationFactor / (direction.magnitude * direction.magnitude), maxAccelaration);
                    output.linear += direction.normalized * separationStrength;

                }
            }
        }

        if (output.linear.magnitude > maxAccelaration)
        {
            output.linear.Normalize();
            output.linear *= maxAccelaration;
        }
        Debug.DrawRay(this.Character.position, output.linear, Color.red);

        return output;
    }
}
