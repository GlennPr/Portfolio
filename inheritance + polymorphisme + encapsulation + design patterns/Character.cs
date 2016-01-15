using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// character class, which is used as the base for all characters.
// character acts based on its state and  starting/finishing of events & AI behaviours
public class Character : MonoBehaviour 
{
    private List<Tile> currentPath = new List<Tile>();
    public List<Tile> CurrentPath
    {
        get { return currentPath; }
        set { currentPath = value; }
    }

    private Tile.Side onTileSide = Tile.Side.Center;
    public Tile.Side OnTileSide
    {
        get { return onTileSide; }
        set { onTileSide = value; }
    }

    static int groundState = Animator.StringToHash("Base.Grounded");

    public delegate void taskChanged(System.Type type);
    public event taskChanged taskChangedEvent;

    public delegate void hpChanged();
    public event hpChanged hpChangedEvent;

    public delegate void tileChanged();
    public event tileChanged tileChangedEvent;

    public delegate void hasDied();
    public event hasDied hasDiedEvent;

    public delegate void goingToAttack();
    public event goingToAttack goingToAttackEvent;

    public delegate void gotAttacked();
    public event gotAttacked gotAttackedEvent;

    [SerializeField]
    protected HPBar hpBar = null;

    [SerializeField]
    private Animator animator = null;

    private int maxHP = 0;
    [SerializeField]
    protected int hp = 100;
    public int HP
    {
        get { return hp; }
        set { hp = value; }
    }
 
    [SerializeField]
    protected int attackStrenght = 25;
    public int AttackStrenght
    {
        get { return attackStrenght; }
        set { attackStrenght = value; }
    }

    [SerializeField]
    protected float attackSpeed = 0.8f;
    public float AttackSpeed
    {
        get { return attackSpeed; }
        set { attackSpeed = value; }
    }

    [SerializeField]
    protected float doorOpeningSpeed = 0.7f;
    public float DoorOpeningSpeed
    {
        get { return doorOpeningSpeed; }
        set { doorOpeningSpeed = value; }
    }

    protected Speed normalSpeed = new Speed();
    public Speed NormalSpeed
    {
        get { return normalSpeed; }
        set { normalSpeed = value; }
    }

    protected Tile currentTile = null;
    public Tile CurrentTile
    {
        get { return currentTile; }
        set 
        {
            currentTile = value;
            if(tileChangedEvent != null && tileChangedEvent.Target!=null)
                tileChangedEvent();
        }
    }
  

    public enum DetectionObjects { None = 0, EnemyAgent = 1, PlayerAgent = 2, SecurityCamera = 3, MotionCamera = 4, FloatingCamera = 5, HomingAgent = 6, Turret = 7};

    private List<DetectionObjects> hiddenAgainstList = new List<DetectionObjects>();
    public List<DetectionObjects> HiddenAgainstList
    {
        get { return hiddenAgainstList; }
        set { hiddenAgainstList = value; }
    }

    protected DetectionObjects detectionType = DetectionObjects.None;
    public DetectionObjects DetectionType
    {
        get { return detectionType; }
        set { detectionType = value; }
    }


    public enum CharacterState { Combat = 0, Free = 1, Dead = 2, Spawn = 3, Plan = 4, LeftLevel = 5, GoingTowardsTarget = 6, Launched = 7, Stunned = 8, PushBack = 9};
    
    protected CharacterState currentState = CharacterState.Free;
    public CharacterState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    [SerializeField]
    protected float moveSpeed = 1.6f;

    protected AIBehaviour currentAIBehaviour = null;
    public AIBehaviour CurrentAIBehaviour
    {
        get { return currentAIBehaviour; }
        set { currentAIBehaviour = value; }
    }

    private bool isHidden;
    public bool IsHidden
    {
        get { return isHidden; }
        set { isHidden = value; }
    }

    private bool endOfRouteReached = false;
    public bool EndOfRouteReached
    {
        get { return endOfRouteReached; }
        set { endOfRouteReached = value; }
    }



    protected virtual void Start() 
    {
        normalSpeed.speed = moveSpeed;

        maxHP = hp;
        if (hpBar != null)
            hpBar.MaxHp = maxHP;

        GameFlow.Instance.PhaseChangeEvent += OnPhaseChange;
	}

    // game wide state change, example: an alarm was triggered
    private void OnPhaseChange(GameFlow.GamePhase previousState, GameFlow.GamePhase currentState)
    {
        if (previousState == GameFlow.GamePhase.Start && currentState == GameFlow.GamePhase.Spawn)
        {
            currentTile = WorldGrid.Instance.ObjectInGridTile(transform);
            OnStartAction();
        }

        if (previousState == GameFlow.GamePhase.Plan && currentState == GameFlow.GamePhase.Action)
            OnNormalAction();

        if (currentState == GameFlow.GamePhase.Alarm)
            OnAlarm();

        if (previousState == GameFlow.GamePhase.Alarm && currentState == GameFlow.GamePhase.Action)
            OnAlarmAction();
    }
    protected virtual void OnStartAction()
    { }
    protected virtual void OnNormalAction()
    { }
    protected virtual void OnAlarm()
    { }
    protected virtual void OnAlarmAction()
    { }


    public virtual void BehaviourFinished(AIBehaviour.FinishedBehaviour behaviorType)
    {
        //Debug.Log("Finished" + behaviorType);
    }
    public virtual void BehaviourStopped(AIBehaviour.StopReason reason, GameObject interuptionObject)
    {
        //Debug.Log("Stopped" + reason);
    }
    public virtual void BehaviourCancelled(AIBehaviour.CancelReason reason)
    {
        //Debug.Log("Cancelled" + reason);
    }

    public void StopCurrentBehaviour()
    {
        if (currentAIBehaviour != null && !currentAIBehaviour.Finished && !currentAIBehaviour.Stopped)
        {
            Debug.Log("currentAIBehaviour.Stop " + currentAIBehaviour.GetType());
            currentAIBehaviour.Stop(AIBehaviour.StopReason.None, null);
        }

        currentAIBehaviour = null;
    }

	public void StopCurrentBehaviour(AIBehaviour.StopReason reason)
	{
		if (currentAIBehaviour != null && !currentAIBehaviour.Finished && !currentAIBehaviour.Stopped)
		{
			Debug.Log("currentAIBehaviour.Stop " + currentAIBehaviour.GetType());
			currentAIBehaviour.Stop(reason, null);
		}
		if(currentAIBehaviour == null)
			Debug.Log("no current behaviour ");
		
		currentAIBehaviour = null;
		
	}
    protected void StartNewBehaviour(AIBehaviour newBehaviour)
    {
        StopCurrentBehaviour();
        currentAIBehaviour = newBehaviour;

        currentAIBehaviour.BehaviourFinishedEvent += BehaviourFinished;
        currentAIBehaviour.BehaviourStoppedEvent += BehaviourStopped;
        currentAIBehaviour.BehaviourCancelledEvent += BehaviourCancelled;

        currentAIBehaviour.Start();
    }

    public void LookAtTarget(Vector3 target)
    {
        transform.LookAt(new Vector3(target.x, target.y, transform.position.z), Vector3.back);
    }

    private void AdjustHPBar()
    {
        if (hpBar != null)
        {
            hpBar.CurrentHp = Mathf.Max(hp, 0);
            hpBar.AdjustBar();
        }

        if (hpChangedEvent != null && hpChangedEvent.Target != null)
            hpChangedEvent();
    }

    public void Heal(int heal)
    {
        if (currentState != CharacterState.Dead)
        {
            hp += heal;

            if (hp > maxHP)
                hp = maxHP;
        }
        AdjustHPBar();
    }

    public virtual void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp < 0)
        {
            Died();
            hp = 0;
        }
        AdjustHPBar();

        if (gotAttackedEvent != null && gotAttackedEvent.Target != null)
            gotAttackedEvent();
    }

    protected virtual void Died()
    {
        currentState = CharacterState.Dead;
        StopCurrentBehaviour();
       
        if (hasDiedEvent != null && hasDiedEvent.Target != null)
            hasDiedEvent();
    }

    public virtual void BeingAttackedBy(Character target)
    {
        if (currentState != CharacterState.Combat)
        {
            currentState = CharacterState.Combat;
            StartNewBehaviour(new CombatAIBehaviour(this, target));
        }
    }

    public void AboutToAttack()
    {
        if (goingToAttackEvent != null && goingToAttackEvent.Target != null)
            goingToAttackEvent();
    }

    public void Animate(string animationName, float value)
    {
        if(animator != null)
            animator.SetFloat(animationName, value);
    }

    public void Animate(string animationName, bool value)
    {
        if (animator != null)
            animator.SetBool(animationName, value);
    }

    public void AnimateTrigger(string animationName)
    {
        if (animator != null)
        {
            if (animationName == "Reset" && animator.GetCurrentAnimatorStateInfo(0).nameHash == groundState)
            { }
            else
                animator.SetTrigger(animationName);
        }
    }

    public void NewTaskStarted(System.Type type)
    {
        if (taskChangedEvent != null && taskChangedEvent.Target != null)
            taskChangedEvent(type);
    }


    void OnDestroy()
    {
        if (GameFlow.Instance != null)
            GameFlow.Instance.PhaseChangeEvent -= OnPhaseChange;
    }

    protected List<PlayerCharacter.PriorityType> HiddenObjectsToPriorities()
    {
        List<PlayerCharacter.PriorityType> list = new List<PlayerCharacter.PriorityType>();

        foreach (DetectionObjects item in hiddenAgainstList)
        {
            switch(item)
            {
                case DetectionObjects.EnemyAgent:
                    list.Add(PlayerCharacter.PriorityType.EnemyAgent);
                    break;
                case DetectionObjects.FloatingCamera:
                    list.Add(PlayerCharacter.PriorityType.FloatingCamera);
                    break;
                case DetectionObjects.HomingAgent:
                    list.Add(PlayerCharacter.PriorityType.HomingGuard);
                    break;
                case DetectionObjects.MotionCamera:
                    list.Add(PlayerCharacter.PriorityType.MotionCamera);
                    break;
                case DetectionObjects.PlayerAgent:
                    list.Add(PlayerCharacter.PriorityType.PlayerAgent);
                    break;
                case DetectionObjects.SecurityCamera:
                    list.Add(PlayerCharacter.PriorityType.SecurityCamera);
                    break;
				case DetectionObjects.Turret:
					list.Add(PlayerCharacter.PriorityType.Turret);
					break;
			}
		}
		
		
		return list;
	}
}
