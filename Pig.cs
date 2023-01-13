using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Pig : MonoBehaviour
{
  
    public enum StateType { state_wander, state_chase};
    public StateType currState = StateType.state_wander;

    private Vector3 wanderDest;
    public int stopRange = 3;
    public float searchRange = 200;
    private Rigidbody rb;
    private NavMeshAgent nav;

    void Start()
    {
        //currentTime = startTime;
    }

    public void Update()
    {
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        //UpdateState(currState);
        Wander();
    }

    public void ChangeState(StateType newState)
    {
        currState = newState;
    }

    public void UpdateState(StateType CurrentState)
    {
        switch (CurrentState)
        {
             case StateType.state_chase:

                break;

            case StateType.state_wander:

                break;

        }//end switch

    }

    private void Wander()
    {
        if (Vector3.Distance(transform.position, wanderDest) < stopRange || wanderDest == Vector3.zero) //and are not going to 0,0,0
        {
            //get a nearby random point on the navmesh - this is done by casting a sphere from our current location
            wanderDest = RandomNavSphere(transform.position, 100, LayerMask.GetMask("Default"));
        }
        else//we have not yet reached the target wander destination
            nav.SetDestination(wanderDest);//move to current wander destination
    }

    // This method returns a random point within a sphere of specified size (dist) that is on the terrain - the terrain must be the on the layermask specified
    //https://forum.unity.com/threads/solved-random-wander-ai-using-navmesh.327950/

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        //this is what gets a random point inside a sphere of Dist sized radius
        Vector3 randDirection = Random.insideUnitSphere * dist;

        //add the random point vector to our current positional vector
        randDirection += origin;

        //will store information about the point on the navmesh
        NavMeshHit navHit;

        //we sample the point on the navmesh
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        //we return the position of our random point inside the sphere
        return navHit.position;
    }


    /*private void Chase()
    {
        nav.SetDestination(cow.gameObject.transform.position);
    }*/
}