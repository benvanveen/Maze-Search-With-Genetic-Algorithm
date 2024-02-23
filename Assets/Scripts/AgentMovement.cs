using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UI;

public class AgentMovement : MonoBehaviour
{

    // PUBLIC VARIABLES FOR AGENTS TO CHANGE

    public float AgentSpeed = 10f;
    public float OriginalAgentSpeed;

    public float CheckRadius;
    public int MaxCheckpointTargetRepetition = 20;
    public float GoalToCheckpointPriority;
    public int RequiredMovement10Steps;

    // PUBLIC VARIABLES REGARDING THE MAZE TO CHANGE

    public float PenaltyPerSpace;
    public float PointsPerCheckpoint;

    // PUBLIC VARIABLES FOR AGENTS TO SET

    public Transform UpCheck;
    public Transform DownCheck;
    public Transform LeftCheck;
    public Transform RightCheck;
    public LayerMask whatIsWall;
    private List<Vector3> CheckPointPositionsList = new List<Vector3>();
    private Vector3 EndGoalPosition;
    public Checkpoints CheckpointsScript;

    // PRIVATE CHECKPOINT LOCATION VARIABLES

    private Vector3 TempHiddenCheckpoint;
    private Vector3 CurrentTargetedCheckpoint;
    private Vector3 LastTargetedCheckpoint;
    private Vector3 CurrentTarget;
    private float CurrentTargetDist;

    // AGENT POSITION VECTORS

    private Vector3 LastPosition;
    private List<Vector3> CornersTaken = new List<Vector3>();
    private Vector3 TenSpaceIntervalLocation;

    // AGENT VARIABLES OTHER
    
    private bool UpBlocked;
    private bool DownBlocked;
    private bool LeftBlocked;
    private bool RightBlocked;

    private int CheckpointTargetRepetition;
    private int LastMove;
    private int LastMoveTrue;
    private int BestNextMove;

    private float BestNextMoveDist;
    private int SecondBestNextMove;
    private float SecondBestNextMoveDist;
    private float CheckPointsPassed;
    private int trueCount;
    private bool FirstMove;
    private float SpacesMoved;
    private bool CornerAlert;

    private float CurrentTime;
    
    private bool recentered = false;

    public Slider CheckPointValue;
    public Slider TimeValue;

    public float CheckPointValueFloat;
    public float TimeValueFloat;
    
    public float timerDuration = 20f;
    public float timer = 20f;

    public int generation = 0;

    void CustomStart(){
        CheckPointsPassed = 0;
        SpacesMoved = 0;
        CheckpointTargetRepetition = 0;
        CurrentTime = 1/AgentSpeed;
        Debug.Log(CheckPointPositionsList.Count());
        CheckPointPositionsList = CheckpointsScript.GetCheckPoints();
        Debug.Log(CheckPointPositionsList.Count());
        BestNextMove = 0;
        LastMove = 0;
        LastMoveTrue = 0;
        FirstMove = true;
        LastPosition = Vector3.zero;
        CurrentTarget = EndGoalPosition;
        recentered = false;
        AgentSpeed = 136.7f;
        OriginalAgentSpeed = AgentSpeed;
        CornersTaken.Clear();

        CheckPointValueFloat = CheckPointValue.value;
        TimeValueFloat = TimeValue.value;
        //InvokeRepeating("CustomStart", timerDuration, timerDuration);
        timer = 0;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if(timer > timerDuration){
            CustomStart();
            generation++;
        }
        // moves the agents to the beggining of the maze once the maze has been generated
        if(!recentered && CheckpointsScript.checkPointsChecked){
            transform.position = CheckpointsScript.StartPosition;
            CustomStart();
            // locates the end of the maze once it has been randomly generated
            EndGoalPosition = CheckpointsScript.EndGoalPosition;
            recentered = true;
        }

        CornerAlert = false;


        // if the agent appears to be "stuck" heading towards a certain checkpoint, it "forgets" the checkpoint temporarily to allow for alternate movement
        if(MaxCheckpointTargetRepetition < CheckpointTargetRepetition && CurrentTarget != EndGoalPosition){
            TempHiddenCheckpoint = CurrentTarget;
            CheckPointPositionsList.Remove(CurrentTarget);
            CheckpointTargetRepetition = 0;
        }

        // if the agent has not moved far enough in the lastt 12 steps, it will forget the corners it has previously taken to stop bias towards certain paths occurring
        
        if (SpacesMoved % 12 == 0 ){
            if(EuclidianDistance(TenSpaceIntervalLocation, transform.position) < RequiredMovement10Steps){
                CornersTaken.Clear();
            }
            TenSpaceIntervalLocation = transform.position;
        }

        // once the end goal has been reached, post the vistory message and stop movement
        if(transform.position == EndGoalPosition){
            Debug.Log("Agent With Chromosome: " + GoalToCheckpointPriority + " Reached The Target! Steps Taken: " + SpacesMoved + ", Checkpoints Hit: " + CheckPointsPassed + ", Fitness Score: " + FitnessFunction());
            OriginalAgentSpeed = AgentSpeed;
            AgentSpeed = 0;

        }

        // once a checkpoint has been reached, increase the checkpoints passed by one and remove it from the checkpoint list
        if(CheckPointPositionsList.Contains(transform.position)){
            CheckPointsPassed = CheckPointsPassed + 1f;
            CheckPointPositionsList.Remove(transform.position);
        }

        // MOVEMENT SCRIPTING BEGINS HERE

        // set the default direction to move to be heading towards the goal
        CurrentTargetDist = EuclidianDistance(EndGoalPosition, transform.position);
        CurrentTarget = EndGoalPosition;

        // check if any of the checkpoints are closer than the current target than the object and also close enough for the agent to deviate from heading towards the main goal
        for (int i = 0; i < CheckPointPositionsList.Count; i++)
        {
            if(EuclidianDistance(CheckPointPositionsList[i], transform.position) < CurrentTargetDist && EuclidianDistance(CheckPointPositionsList[i], transform.position) < GoalToCheckpointPriority){
                CurrentTarget = CheckPointPositionsList[i];
                CurrentTargetDist = EuclidianDistance(CheckPointPositionsList[i], transform.position);
            }
        }
        
        // this checks if the agents is stuck between two checkpoints heading backwards and forweards betweeen them
        if(CurrentTarget == LastTargetedCheckpoint && CurrentTarget != EndGoalPosition){
            CheckpointTargetRepetition++;
        }

        if(CurrentTarget != CurrentTargetedCheckpoint){
            LastTargetedCheckpoint = CurrentTargetedCheckpoint;
            CurrentTargetedCheckpoint = CurrentTarget;
        }



        BestNextMoveDist = 999999f;

        // 4 detectors on each side of each agents which can detect walls
        UpBlocked = Physics2D.OverlapCircle(UpCheck.position, CheckRadius, whatIsWall);
        DownBlocked = Physics2D.OverlapCircle(DownCheck.position, CheckRadius, whatIsWall);
        LeftBlocked = Physics2D.OverlapCircle(LeftCheck.position, CheckRadius, whatIsWall);
        RightBlocked = Physics2D.OverlapCircle(RightCheck.position, CheckRadius, whatIsWall);

        CurrentTime -= Time.deltaTime;

        //  Used for debugging by manually moving an agent, Checks for arrow key presses
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        trueCount = 0;
        
        // Check each boolean and increment trueCount if it's true, if true count is 3 that means it has reached a deadend 
        
        if(UpBlocked){
            trueCount++;
        }
        if(DownBlocked){
            trueCount++;
        }
        if(LeftBlocked){
            trueCount++;
        }
        if(RightBlocked){
            trueCount++;
        }
        if(trueCount == 3){
            LastMove = 0;
        }

        // here we will use the euclidian distance to the final checkpoint to find the optimal direction to move
        if((EuclidianDistance(CurrentTarget, UpCheck.position) < BestNextMoveDist) && !UpBlocked && LastMove != 1 || FirstMove){
            SecondBestNextMoveDist = BestNextMoveDist;
            BestNextMoveDist = EuclidianDistance(CurrentTarget, UpCheck.position);
            SecondBestNextMove = BestNextMove;
            BestNextMove = 1;
            FirstMove = false;
        }
        if(EuclidianDistance(CurrentTarget, DownCheck.position) < BestNextMoveDist && !DownBlocked && LastMove != 2 || FirstMove){
            SecondBestNextMoveDist = BestNextMoveDist;
            BestNextMoveDist = EuclidianDistance(CurrentTarget, DownCheck.position);
            SecondBestNextMove = BestNextMove;
            BestNextMove = 2;
            FirstMove = false;
        }
        if(EuclidianDistance(CurrentTarget, LeftCheck.position) < BestNextMoveDist && !LeftBlocked && LastMove != 3 || FirstMove ){
            SecondBestNextMoveDist = BestNextMoveDist;
            BestNextMoveDist = EuclidianDistance(CurrentTarget, LeftCheck.position);
            SecondBestNextMove = BestNextMove;
            BestNextMove = 3;
            FirstMove = false;
        }
        if(EuclidianDistance(CurrentTarget, RightCheck.position) < BestNextMoveDist  && !RightBlocked && LastMove != 4 || FirstMove ){
            BestNextMoveDist = EuclidianDistance(CurrentTarget, RightCheck.position);
            SecondBestNextMove = BestNextMove;
            BestNextMove = 4;
            FirstMove = false;
        }
        if(BestNextMove != 4 && EuclidianDistance(CurrentTarget, RightCheck.position) < SecondBestNextMoveDist && !RightBlocked && LastMove != 4){
            SecondBestNextMove = 4;
            SecondBestNextMoveDist = EuclidianDistance(CurrentTarget, RightCheck.position);
        }


        
        
        CornerAlert = LastMoveTrue != BestNextMove|| trueCount < 2;
        
        // this allows for alternate paths to be taken by remembering corners already taken
        if(CurrentTime <= 0f && CornerAlert && SecondBestNextMove != 0){
            // if corner already in list then use sewcond best move instead
            if (CornersTaken.Contains(transform.position)){
                BestNextMove = SecondBestNextMove;
                CornersTaken.Add(transform.position);

            } else{
                // add the corner to list here
                CornersTaken.Add(transform.position);
            }
        }

        // moves the agent based on the best movement decided
        if(CurrentTime <= 0f && BestNextMove == 1){
            LastPosition = transform.position;
            MoveUp();
            LastMove = 2;
            LastMoveTrue = 1;
            SpacesMoved = SpacesMoved + 1f;
        }
        if(CurrentTime <= 0f && BestNextMove == 2 && LastMove != 2){
            LastPosition = transform.position;
            MoveDown();
            LastMove = 1;
            LastMoveTrue = 2;
            SpacesMoved = SpacesMoved + 1f;
        }
        if(CurrentTime <= 0f && BestNextMove == 4 && LastMove != 4){
            LastPosition = transform.position;
            MoveRight();
            LastMove = 3;
            LastMoveTrue = 4;
            SpacesMoved = SpacesMoved + 1f;
        }
        if(CurrentTime <= 0f && BestNextMove == 3 && LastMove != 3){
            LastPosition = transform.position;
            MoveLeft();
            LastMove = 4;
            LastMoveTrue = 3;
            SpacesMoved = SpacesMoved + 1f;
        }
    }

    // euclidian distance function
    float EuclidianDistance(Vector3 Goal, Vector3 CurrentPosition){
        return Mathf.Pow(Mathf.Pow(Goal.x - CurrentPosition.x,2f) + Mathf.Pow(Goal.y - CurrentPosition.y,2f),0.5f);
    }

    void MoveUp()
    {
        // Move the GameObject up by 1 unit instantly
        if(!UpBlocked){
            transform.Translate(Vector3.up);
            CurrentTime = 1/AgentSpeed;
        }
    }
    void MoveDown()
    {
        // Move the GameObject up by 1 unit instantly
        
        if(!DownBlocked){
            transform.Translate(Vector3.down);
            CurrentTime = 1/AgentSpeed;
        }
    }
    void MoveLeft()
    {
        // Move the GameObject up by 1 unit instantly
        
        if(!LeftBlocked){
            transform.Translate(Vector3.left);
            CurrentTime = 1/AgentSpeed;
        }

    }
    void MoveRight()
    {
        // Move the GameObject up by 1 unit instantly
        
        if(!RightBlocked){
            transform.Translate(Vector3.right);
            CurrentTime = 1/AgentSpeed;
        }
    }

    // defines the fitness function
    float FitnessFunction(){
        return CheckPointsPassed * CheckPointValueFloat - SpacesMoved * TimeValueFloat;
    }
}
