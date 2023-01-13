using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FoodSpawner : MonoBehaviour
{
    public GameObject food;
    public DiamondSquare terrain;
    int numOfFoodItems = 10;
    public int maxGrass = 30;
    public int counter = 0;
    private int fd = 0;

    public void SpawnFood()
    {
        fd = fd + numOfFoodItems;

        for (int i = 0; i < numOfFoodItems; i++)
        {
            Instantiate(food, new Vector3(Random.Range(0, terrain.size), 200, Random.Range(0, terrain.size)), Quaternion.identity);
        }
        counter += numOfFoodItems;

    }

    private void Start()
    {
        InvokeRepeating("SpawnFood", 1, 5);
    }

    private void Update()
    {
        if (counter == 30)
        {
            CancelInvoke();
        }
    }

}