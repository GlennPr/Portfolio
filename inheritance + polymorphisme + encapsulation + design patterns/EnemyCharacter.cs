using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// defines variables and function that are only applied to enemy characters
public class EnemyCharacter : Character 
{
	protected Tile rootTile = null;
	public Tile RootTile
	{
		get { return rootTile; }
		set { rootTile = value;}
	}
	private List<Tile> surroundingTiles = new List<Tile>();

    public delegate void Disable();
    public event Disable DisableEvent;

    public enum EnemyType {Empty = 0, None = 1, SecurityCamera = 2, Guard = 3, Agent = 4, Homing = 5, FloatingCamera = 6, Turret = 7}
    protected EnemyType currentType = EnemyType.Empty;

    public EnemyType CurrentType
    {
        get { return currentType; }
    }

    protected Connections connections;

	protected float alertCooldown = 5;
    protected bool coolDown;

    [SerializeField]
    private float detectRange = 4.7f;

    [SerializeField]
    protected GameObject viewCone = null;

    [SerializeField]
    protected float aggroGenerationPerSecond = 1.0f;

    private List<Tile> visionTiles = new List<Tile>();
    public List<Tile> VisionTiles
    {
        get { return visionTiles; }
    }

    [SerializeField]
    protected float hackDuration = 2.0f;
	protected float hackedAmount = 0;




    protected override void Start()
    {
        connections = GameObject.Find("TileConnections").GetComponent<Connections>();

        if (viewCone != null)
        {
            if (gameObject.name != "SecurityCam Standstill" && gameObject.name != "Guard")
                viewCone.transform.localScale = new Vector3 (detectRange, viewCone.transform.localScale.y, detectRange);
            else
                viewCone.transform.localScale = new Vector3(viewCone.transform.localScale.x, detectRange, viewCone.transform.localScale.z);
        }
        base.Start();
    }

    public void Setup(Tile root)
    {
        rootTile = root;
        rootTile.IsLegit = false;

        surroundingTiles.AddRange(rootTile.GetAllClearConnectedTiles());
    }


    public void ViewDisabled(float duration)
    {
        currentState = CharacterState.Dead;
        if (gameObject.layer == Layers.SoundCamera || gameObject.layer == Layers.MotionCamera || gameObject.layer == Layers.Camera)
            gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        else
            gameObject.transform.GetChild(0).gameObject.SetActive(false);

        Routine.WaitForSeconds(this, duration, Enable);
    }

    public virtual void Disabled()
    {
        if (currentState != CharacterState.Dead && DisableEvent != null && DisableEvent.Target != null)
            DisableEvent();

        currentState = CharacterState.Dead;
		Debug.Log (gameObject.name);
        gameObject.SetActive(false);
    }

    private void Enable()
    {
        gameObject.SetActive(true);
        currentState = CharacterState.Free;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layers.Tile)
        {
            Tile tile = other.GetComponent<Tile>();

            if (!visionTiles.Contains(tile))
                visionTiles.Add(tile);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Layers.Tile)
        {
            Tile tile = other.GetComponent<Tile>();

            if (visionTiles.Contains(tile))
                visionTiles.Remove(tile);
        }
    }


    public Routine IsBeingHacked(PlayerCharacter owner)
    {
        return Routine.Start(HackRoutine(owner), owner);
    }

    private IEnumerator HackRoutine(PlayerCharacter owner)
    {
        while (hackedAmount < hackDuration)
        {
            hackedAmount += (owner.CurrentItemCaptureMultiplyer + owner.ExtraCaptureMultiplyer)* Time.deltaTime;
			UpdateHPBar();
            yield return null;
        }

        Disabled();
    }
	  
    protected override void Died()
    {
        base.Died();
    }



        
}
