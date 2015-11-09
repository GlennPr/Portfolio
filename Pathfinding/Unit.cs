using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour 
{
    public GameObject flowVector;
    public GameObject separationVector;
    public GameObject alignmentVector;
    public GameObject cohesionVector;
    public GameObject velocityVector;

    private float pathWeight = 1.3f;
    private float flowWeight = 0.8f;
    private float sepWeight = 0.65f;
    private float alignWeight = 0.6f;
    private float cohWeight = 0f;

    private float collisionRadius = 1.8f;
    private float maxForce = 8; // maximun magnitude of the (combined) force vector that is applied each tick
    private float maxSpeed = 12; // maximun magnitude of the (combined) velocity vector
    private float characterRadius = 2.2f;
    private float neighbourRadius = 7.5f;

    private Vector2 velocity = Vector2.zero;
    private List<Unit> neighbours = new List<Unit>();


    private bool followFlowField = false;
    public GameManager game = null;
    public List<Node> path = new List<Node>();
    private int targetIndex = 0;


    private CharacterController controller;


	void Start()
	{
		controller = GetComponent<CharacterController>();
	}


    // combine forces in order untill max force is reached
    Vector2 CombineForces(float maxForce, List<Vector2> forces)
    {
        Vector2 force = Vector2.zero;

        for(int i = 0; i < forces.Count; i++)
        {
            Vector2 newForce = force + forces[i];

            if (newForce.magnitude > maxForce)
            {
                float amountNeeded = maxForce - force.magnitude;
                float amountAdded = forces[i].magnitude;
                float division = amountNeeded / amountAdded;

                force += division * forces[i];

                return force;
            }
            else
                force = newForce;
        }

        return force;
    }



    // update the movement based on either a Path (list of nodes) or a Flow Field (2D vector field)
    void Update()
    {
        neighbours = GetAllNeighbours();

		if(path.Count > 0 || followFlowField)
		{    
            Vector2 forceAddition = Vector2.zero;
            Vector2 netForce = Vector2.zero;

			Vector2 flow = Vector2.zero;

            // 4 steering Vectors in order: Flow, separation, alignment, cohesion
            // adjusted with weights
            if (followFlowField)
                flow = flowWeight * FlowFieldFollow();
			else if(path.Count > 0)
				flow = pathWeight * PathFollow();
				

            Vector2 sep = sepWeight * Separation();
            Vector2 ali = alignWeight * Alignment();
            Vector2 coh = cohWeight * Cohesion();

            // calculate the combined force, but dont go over the maximum force
            netForce = CombineForces(maxForce, new List<Vector2>(){flow, sep, ali, coh});


            // debug steering forces
            flowVector.transform.LookAt(flowVector.transform.position + new Vector3(flow.x, 0, flow.y));
            separationVector.transform.LookAt(flowVector.transform.position + new Vector3(sep.x, 0, sep.y));

            flowVector.transform.localScale = new Vector3(flowVector.transform.localScale.x, flowVector.transform.localScale.y, flow.magnitude);
            separationVector.transform.localScale = new Vector3(separationVector.transform.localScale.x, separationVector.transform.localScale.y, sep.magnitude);



            // velocity gets adjusted by the calculated force
            velocity += netForce * Time.deltaTime;

            // dont go over the maximum movement speed possible
            if (velocity.magnitude > maxSpeed)
                velocity = (velocity / velocity.magnitude) * maxSpeed;

            // move
            Vector3 movement = new Vector3(velocity.x * Time.deltaTime, 0, velocity.y * Time.deltaTime);
			controller.Move(movement);


            // debug steering forces
            velocityVector.transform.LookAt(velocityVector.transform.position + new Vector3(velocity.x, 0, velocity.y));

            // face forwards
            transform.LookAt(transform.position + movement);  
		}
    }


  

 
    private Vector2 FlowFieldFollow()
    {
        // get current node we are standing on and the flow direction on that location
        Node currentNode = game.nodeGrid.GetNode(transform.position);
        Vector2 desired = game.flowFieldManager.flowFields[game.flowFieldManager.flowFields.Count-1].field[currentNode.sector][currentNode.indexWithinSector];

        // if we have left a flow field sector
        if (desired == Vector2.zero && currentNode != game.intergrationField.prevDestinationNode)
            game.intergrationField.CreateExtraField(currentNode);

        // return the velocity we desire to go to
        desired *= maxSpeed; // we desire this velocity
        desired -= velocity;

        return desired * (maxForce / maxSpeed);
    }

	private Vector2 PathFollow()
	{
        // get previous and next node from the path
        Vector3 prevNodePos = game.nodeGrid.GetNodeWorldPosition(path[targetIndex-1]);
		Vector3 nextNodePos = game.nodeGrid.GetNodeWorldPosition(path[targetIndex]);

        // get direction from our position to the next node's postion
        Vector2 desired = new Vector2(nextNodePos.x - transform.position.x, nextNodePos.z - transform.position.z);

        // get direction between prev and next node
        Vector3 dirBetweenPrevAndNext = (nextNodePos - prevNodePos).normalized;

        //get direction of lines that are on a 90 degree angle (right angle)
        Vector2 rightLine1 = new Vector2(-dirBetweenPrevAndNext.z, dirBetweenPrevAndNext.x);
        Vector2 rightLine2 = new Vector2(dirBetweenPrevAndNext.z, -dirBetweenPrevAndNext.x);
        Vector2 nextPosV2 = new Vector2(nextNodePos.x, nextNodePos.z);


        // if we are within close range of the next node, we can start going to the next node in the path
		if(desired.magnitude < 3.5f)
		{
			if(targetIndex < path.Count - 1)
			targetIndex++;
		}
        else
        {
            // when we cross the right angle lines, we can also start going to the next node in the path
            Vector2 a = nextPosV2 + rightLine1 * 100;
            Vector2 b = nextPosV2 + rightLine2 * 100;


            if (LineCircelIntersect.lineTouchCircle(a, b, new Vector2(transform.position.x, transform.position.z), characterRadius))
            {
                if (targetIndex < path.Count - 1)
                    targetIndex++;
            }
        }

        // return the velocity we desire to go to
		desired = desired.normalized;
        desired *= maxSpeed; // we desire this velocity
		desired -= velocity;
		
		return desired * (maxForce / maxSpeed);
	}


    private Vector2 Separation()
    {
        if (neighbours.Count == 0)
            return Vector2.zero;

        Vector2 totalForce = Vector2.zero;

        // get avarge push force away from neighbours
        foreach(Unit neighbour in neighbours)
        {
            Vector2 pushforce = new Vector2(transform.position.x - neighbour.transform.position.x, transform.position.z - neighbour.transform.position.z);
            totalForce += pushforce.normalized * (neighbourRadius - pushforce.magnitude);
        }

        totalForce /= neighbours.Count;
        totalForce *= maxForce;

        return totalForce;
    }

    private Vector2 Cohesion()
    {
        if (neighbours.Count == 0)
            return Vector2.zero;

        Vector2 pos = new Vector2(transform.position.x, transform.position.z);
        Vector2 centerOfMass = pos;

        foreach (Unit neighbour in neighbours)
            centerOfMass += new Vector2(neighbour.transform.position.x, neighbour.transform.position.z);

        centerOfMass /= neighbours.Count;

        Vector2 desired = centerOfMass - pos;
        desired *= (maxSpeed / desired.magnitude);

        Vector2 force = desired - velocity;
        return force * (maxForce / maxSpeed);
    }


    private Vector2 Alignment()
    {
        if (neighbours.Count == 0)
            return Vector2.zero;

        // get avarge velocity from neighbours
        Vector2 averageHeading = velocity.normalized;
        foreach (Unit neighbour in neighbours)
            averageHeading += neighbour.velocity.normalized;
        averageHeading /= neighbours.Count;

        Vector2 desired = averageHeading * maxSpeed;

        Vector2 force = desired - velocity;
        return force * (maxForce / maxSpeed);
    }




    private List<Unit> GetAllNeighbours()
    {
        List<Unit> neighbours = new List<Unit>();

        foreach (Transform t in transform.parent)
            if ( t != transform && Vector3.Distance(t.position, transform.position) < neighbourRadius)
                neighbours.Add(t.GetComponent<Unit>());

        return neighbours;
    }


    public void SetPath(List<Node> newPath)
    {
        followFlowField = false;

        path = newPath;
        targetIndex = 1;
    }

    public void SetFlowField()
    {
        followFlowField = true;
    }

    

}
