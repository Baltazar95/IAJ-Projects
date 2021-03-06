﻿
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {
        public float maxSpeed { get; private set; }
        public float stopRadius { get; private set; }
        public float slowRadius { get; private set; }

        public DynamicArrive()
        {
            this.MovingTarget = new KinematicData();
        }

        public override string Name
        {
            get { return "Arrive"; }
        }

        public override MovementOutput GetMovement()
        {
            this.MaxAcceleration = 20.0f;
            maxSpeed = 20.0f;
            stopRadius = 0.5f;
            slowRadius = 0.8f;
            float targetSpeed;
            var direction = this.Target.position - this.Character.position;
            var distance = direction.magnitude;

            if (distance  < stopRadius)
            {
                targetSpeed = 0;
            }

            else if (distance > slowRadius)
            {
                targetSpeed = maxSpeed;
            }
            else
            {
               
                targetSpeed = maxSpeed * (distance / (slowRadius));
                //this.Character.velocity = Vector3.zero;
            }
            
            this.Character.velocity = direction.normalized * targetSpeed;

            return base.GetMovement();
        }
    }
}

