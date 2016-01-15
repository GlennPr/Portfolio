using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Guard, an enemy characters that patrols around the world
public class Guard : EnemyCharacter 
{
    [SerializeField]
    private IntVector2 direction = IntVector2.zero;
    [SerializeField]
    private int walkRange = 2;
    [SerializeField]
    private float stopTime = 0.8f;

    private Tile[] guardRoute = new Tile[3];

    private bool goToStart = false;
    public bool GoToStart
    {
        get { return goToStart; }
        set { goToStart = value; }
    }

    private Tile startTile;


    protected override void Start()
    {
        currentType = EnemyType.Guard;
        base.Start();
    }


    protected override void OnStartAction()
    {
        startTile = WorldGrid.Instance.ObjectInGridTile(transform);
        Patrol();
    }
    protected override void OnAlarm()
    {
        StopCurrentBehaviour();
    }
    protected override void OnAlarmAction()
    {
        if (currentState != CharacterState.Dead)
            Patrol();
    }


    public override void BehaviourFinished(AIBehaviour.FinishedBehaviour behaviorType)
    {
        Patrol();
        base.BehaviourFinished(behaviorType);
    }
    public override void BehaviourStopped(AIBehaviour.StopReason reason, GameObject interuptionObject)
    {
        base.BehaviourStopped(reason, interuptionObject);
    }
    public override void BehaviourCancelled(AIBehaviour.CancelReason reason)
    {
        switch (reason)
        {
            case AIBehaviour.CancelReason.CannotMove:
                break;
        }

        base.BehaviourCancelled(reason);
    }

    private void Patrol()
    {
        if (currentState != CharacterState.Dead)
            StartNewBehaviour(new PatrolAIBehaviour(this, startTile, direction, walkRange, stopTime, goToStart));
    }

    public void BeingHacked()
    {
        StopCurrentBehaviour();
    }


    public override void Disabled()
    {
        StopCurrentBehaviour();
        base.Disabled();
    }
}
