using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Movement;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;

public class PriorityManager : MonoBehaviour
{
    public const float X_WORLD_SIZE = 55;
    public const float Z_WORLD_SIZE = 32.5f;
    public const float AVOID_MARGIN = 10.0f;
    public const float MAX_SPEED = 20.0f;
    public const float MAX_LOOK_AHEAD = 20.0f;
    public const float MAX_ACCELERATION = 40.0f;
    public const float DRAG = 0.1f;
    private DynamicAvoidObstacle avoidObstacleMovement;

    private DynamicStraightAhead straightAhead;

    private DynamicSeparation separation;

    private DynamicCohesion cohesion;

    private FlockVelocityMatch flockVelocity;

    private DynamicFlee DynamicFleeMovement;

    private DynamicCharacter BoidCharacter { get; set; }

    private Text BoidMovementText { get; set; }

    private BlendedMovement Blended { get; set; }

    private PriorityMovement Priority { get; set; }

    private List<DynamicCharacter> Characters { get; set; }

    private Vector3 click = new Vector3();


    // Use this for initialization
    void Start()
    {
        var textObj = GameObject.Find("InstructionsText");
        if (textObj != null)
        {
            textObj.GetComponent<Text>().text =
                "Instructions\n\n" +
                "B - Blended\n" +
                "P - Priority\n" +
                "Q - stop";
        }

        this.BoidMovementText = GameObject.Find("BoidMovements").GetComponent<Text>();
        var boidObj = GameObject.Find("Boid");

        this.BoidCharacter = new DynamicCharacter(boidObj)
        {
            Drag = DRAG,
            MaxSpeed = MAX_SPEED
        };

        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        this.Characters = this.CloneSecondaryCharacters(boidObj, 49, obstacles);
        this.Characters.Add(this.BoidCharacter);


        //initialize all but the last character (because it was already initialized as the main character)
        foreach (var character in this.Characters)
        {
            this.InitializeMainCharacter(character, obstacles);
        }
    }

    private void InitializeMainCharacter(DynamicCharacter character, GameObject[] obstacles)
    {
        this.Priority = new PriorityMovement
        {
            Character = character.KinematicData
        };

        this.Blended = new BlendedMovement
        {
            Character = character.KinematicData
        };

        //foreach (var obstacle in obstacles)
        //{

            avoidObstacleMovement = new DynamicAvoidObstacle(obstacles.FirstOrDefault())
            {
                MaxAcceleration = MAX_ACCELERATION,
                AvoidMargin = AVOID_MARGIN,
                MaxLookAhead = MAX_LOOK_AHEAD,
                Character = character.KinematicData,
                MovementDebugColor = Color.magenta
            };
            this.Blended.Movements.Add(new MovementWithWeight(avoidObstacleMovement, 0.5f));
            this.Priority.Movements.Add(avoidObstacleMovement);
        //}

        straightAhead = new DynamicStraightAhead
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.yellow
        };
      this.Blended.Movements.Add(new MovementWithWeight(straightAhead, 2f));
       // this.Priority.Movements.Add(straightAhead);

        separation = new DynamicSeparation(this.Characters)
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.black
        };
        this.Blended.Movements.Add(new MovementWithWeight(separation, 5f));

        cohesion = new DynamicCohesion(this.Characters)
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.blue
        };
        this.Blended.Movements.Add(new MovementWithWeight(cohesion, 1.0f));

        flockVelocity = new FlockVelocityMatch(this.Characters)
        {
            Character = character.KinematicData,
            MovementDebugColor = Color.red
        };
        this.Blended.Movements.Add(new MovementWithWeight(flockVelocity, 3.0f));

        character.Movement = this.Blended;

    }

    //private void InitializeSecondaryCharacter(DynamicCharacter character, GameObject[] obstacles)
    //{
    //    var priority = new PriorityMovement
    //    {
    //        Character = character.KinematicData
    //    };

    //    //new to test
    //    var blended = new BlendedMovement
    //    {
    //        Character = character.KinematicData
    //    };
    //    //end 

    //    foreach (var obstacle in obstacles)
    //    {
    //        avoidObstacleMovement = new DynamicAvoidObstacle(obstacle)
    //        {
    //            MaxAcceleration = MAX_ACCELERATION,
    //            AvoidMargin = AVOID_MARGIN,
    //            MaxLookAhead = MAX_LOOK_AHEAD,
    //            Character = character.KinematicData,
    //            MovementDebugColor = Color.magenta
    //        };
    //        blended.Movements.Add(new MovementWithWeight(avoidObstacleMovement, 1.0f));
    //        priority.Movements.Add(avoidObstacleMovement);
    //    }

    //    var straightAhead = new DynamicStraightAhead
    //    {
    //        Character = character.KinematicData,
    //        MaxAcceleration = MAX_ACCELERATION,
    //        MovementDebugColor = Color.yellow
    //    };
    //    blended.Movements.Add(new MovementWithWeight(straightAhead, 1.9f));

    //    var separation = new DynamicSeparation(this.Characters)
    //    {
    //        Character = character.KinematicData,
    //        MovementDebugColor = Color.black
    //    };
    //    blended.Movements.Add(new MovementWithWeight(separation, 1.0f));

    //    var cohesion = new DynamicCohesion(this.Characters)
    //    {
    //        Character = character.KinematicData,
    //        MovementDebugColor = Color.blue,
    //        MaxAcceleration = 40.0f
    //    };
    //    blended.Movements.Add(new MovementWithWeight(cohesion, 1.0f));

    //    var flockVelocity = new FlockVelocityMatch(this.Characters)
    //    {
    //        Character = character.KinematicData,
    //        MovementDebugColor = Color.red
    //    };
    //    blended.Movements.Add(new MovementWithWeight(flockVelocity, 2.0f));



    //    character.Movement = blended;
    //}

    private List<DynamicCharacter> CloneSecondaryCharacters(GameObject objectToClone, int numberOfCharacters, GameObject[] obstacles)
    {
        var characters = new List<DynamicCharacter>();
        for (int i = 0; i < numberOfCharacters; i++)
        {
            var clone = GameObject.Instantiate(objectToClone);
            //clone.transform.position = new Vector3(30, 0, i * 20);
            clone.transform.position = this.GenerateRandomClearPosition(obstacles);
            var character = new DynamicCharacter(clone)
            {
                MaxSpeed = MAX_SPEED,
                Drag = DRAG
            };
            character.KinematicData.orientation = (float)Math.PI * i;
            characters.Add(character);
        }

        return characters;
    }


    private Vector3 GenerateRandomClearPosition(GameObject[] obstacles)
    {
        Vector3 position = new Vector3();
        var ok = false;
        while (!ok)
        {
            ok = true;

            position = new Vector3(Random.Range(-X_WORLD_SIZE, X_WORLD_SIZE), 0, Random.Range(-Z_WORLD_SIZE, Z_WORLD_SIZE));

            //foreach (var obstacle in obstacles)
            //{
            //    var distance = (position - obstacle.transform.position).magnitude;

            //    //assuming obstacle is a sphere just to simplify the point selection
            //    if (distance < obstacle.transform.localScale.x + AVOID_MARGIN)
            //    {
            //        ok = false;
            //        break;
            //    }
            //}
        }

        return position;
    }

    void Update()
    {
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        bool buttonPressed = false;
        float fleeRadius = 5.0f;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            this.BoidCharacter.Movement = null;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            this.BoidCharacter.Movement = this.Blended;
        }
        if (Input.GetMouseButton(0))
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 55.8f;
            click = camera.ScreenToWorldPoint(mousePos);
            buttonPressed = true;
        }

        foreach (var character in this.Characters)
        {
            BlendedMovement movement = (BlendedMovement)character.Movement;
            MovementWithWeight fleeSearch = movement.Movements.Find(x => x.Movement.Name == "Flee");

            MovementWithWeight straigthSearch = movement.Movements.Find(x => x.Movement.Name == "StraightAhead");

            var clickToCharacter = character.KinematicData.position - click;

            if (buttonPressed)
            {
                if (clickToCharacter.magnitude <= fleeRadius)
                {
                    if (straigthSearch != null)
                    {
                        movement.Movements.Remove(straigthSearch);
                    }

                    var DynamicFleeMovement = new DynamicFlee()
                    {
                        Character = character.KinematicData,
                        MaxAcceleration = MAX_ACCELERATION,
                        MovementDebugColor = Color.blue,
                        Target = new KinematicData()
                    };

                    if (fleeSearch != null)
                    {
                        movement.Movements.Remove(fleeSearch);
                    }

                    DynamicFleeMovement.Target.position = click;
                    DynamicFleeMovement.Target.position.y = character.KinematicData.position.y;
                    movement.Movements.Add(new MovementWithWeight(DynamicFleeMovement, 5.0f));

                    character.Movement = movement;
                }

            }
            else 
            {
                if(fleeSearch != null)
                {
                   
                    if (clickToCharacter.magnitude > fleeRadius)
                    {
                        movement.Movements.Remove(fleeSearch);
                        var straight = new DynamicStraightAhead
                        {
                            Character = this.BoidCharacter.KinematicData,
                            MaxAcceleration = MAX_ACCELERATION,
                         };
                         movement.Movements.Add(new MovementWithWeight(straight, 1.9f));
                        }

                    }
                }
              
            this.UpdateMovingGameObject(character);
        }

        this.UpdateMovementText();
    }

    private void UpdateMovingGameObject(DynamicCharacter movingCharacter)
    {
        if (movingCharacter.Movement != null)
        {
            movingCharacter.Update();
            movingCharacter.KinematicData.ApplyWorldLimit(X_WORLD_SIZE, Z_WORLD_SIZE);
            movingCharacter.GameObject.transform.position = movingCharacter.Movement.Character.position;
        }
    }

    private void UpdateMovementText()
    {
        if (this.BoidCharacter.Movement == null)
        {
            this.BoidMovementText.text = "Red Movement: Stationary";
        }
        else
        {
            this.BoidMovementText.text = "Red Movement: " + this.BoidCharacter.Movement.Name;
        }
    }
}
