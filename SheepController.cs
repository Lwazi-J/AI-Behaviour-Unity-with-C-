using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SheepController : MonoBehaviour
{

    private NavMeshAgent nav;
    public GameObject foodTarget;//the food item the sheep wants to eat
    private GameObject[] foodItems;//used to store array of all food items that exist
    List<GameObject> foodList = new List<GameObject>();//a list of all valid food items

    public enum States { Searching, Eating };//stores all potential states
    public States currState;//the current state of our sheep

    float distance;
    
    public float searchRange = 30;//how far into the unity world the sheep can detect food
    public int eatRange = 4;//how far away from the food the sheep can be to eat the food
    public int stopRange = 4;//how close we get to a target before considering the target to be reached, meaning we can stop moving towards teh target
    private NavMeshPath path;//used by the calculatePath method to determine if we can find a path to a target - in this case the food

    //[SerializeField]//just so we can see this in the inspector
    private Vector3 wanderDest;//stores the destination to wander to

    // Use this for initialization
    void Start()
    {
        currState = States.Searching;//begin in searching state
        foodTarget = null;//make sure we have no initial food target
        nav = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    // Update is called once per frame
    // This only calls the FSM - you can add other things in here if you really want to but I like to keep it clean
    void Update()
    {
        FSM(currState);
    }

    // This method should return true if food is found and false if not
    private bool FindFood()
    {
        foodList.Clear();//clear the list to make sure we re-evaluate all food items

        /* The line below is very slow (computationally expensive) so you might want to not call this every frame but rather only once every X number of seconds
           You should do that if you suffer from performance issues */

        foodItems = GameObject.FindGameObjectsWithTag("Grass");

        foreach (GameObject food in foodItems)
        {
            //if food is in range and a path to the food is possible
            if (Vector3.Distance(transform.position, food.transform.position) < searchRange && nav.CalculatePath(food.transform.position, path))
            {
                foodList.Add(food);//add food to list because it is in range and pathable
            }
        }
        //loop through all discovered food items


        if (foodList.Count > 0)//if we have foudn some valid food
        {
            float dist = Mathf.Infinity;//set it to the biggest possible number
            GameObject closest = new GameObject();//for storing the nearest food item
            closest = null;//making sure it starts ot as null

            // loop through all valid food items and replace the closest variable with the nearest food item
            foreach (GameObject food in foodList)
            {
                if (Vector3.Distance(transform.position, food.transform.position) < dist)//food is new closest food item
                {
                    dist = Vector3.Distance(transform.position, food.transform.position);//update closest distance
                    closest = food;//update closest food item
                }
            }

            foodTarget = closest;//set our food target to be the closest food item

            if (foodTarget != null)//then a food target has been found that is within range, as an available path and is the closest
            {
                return true;
            }

            else//nothing found so return false
            {
                return false;
            }
        }
        else//nothing found so return false
        {
            return false;
        }
     }
    // The FSM
    private void FSM(States currentState)
    {
        switch (currentState)
        {
            case States.Searching:
                if (foodTarget == null)//if we have no food target
                {
                    if(FindFood()) //if we can see food
                    {
                        ChangeState(States.Eating);
                        Eat();
                    }
                }
                Wander();
                break;

            case States.Eating:
                if (foodTarget != null)
                {
                    if (nav.CalculatePath(foodTarget.transform.position, path))
                    {                  
                        nav.SetPath(path);
                        Eat();
                    }
                    else
                    {
                        ChangeState(States.Searching);
                        //returnn to searching state                        
                        //foodTarget = null;//target is no longer valid so we can null it
                    }
                }
                else//no path available (perhaps it moved off the navmesh)
                    ChangeState(States.Searching);//returnn to searching state                        
                break;
        }
    }

    // Check distance to food, if smaller than specified range - destroy food item
    private void Eat()
    {
        if (foodTarget != null)//if we have a food target
        {
            if (Vector3.Distance(transform.position, foodTarget.transform.position) < eatRange)
            {
                Destroy(foodTarget);
            }
        }
    }

    // Changes the state of our sheep
    public void ChangeState(States newState)
    {
        currState = newState;//replaces current with new
    }

    // If new wander destination is required, search for one and set it as new destination, else continue to current destination
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

}
