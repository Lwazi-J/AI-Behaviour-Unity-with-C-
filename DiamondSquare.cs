using UnityEngine; // MonoBehaviour, TerrainCollider, TerrainData

/// <summary>
/// When attached to a Terrain GameObject, 
/// it can be used to randomized the heights
/// using the Diamond-Square algorithm
/// </summary>
[RequireComponent(typeof(TerrainCollider))]
public class DiamondSquare : MonoBehaviour
{
    // Data container for heights of a terrain
    public TerrainData data;
    // Size of the sides of a terrain
    public int size;
    // 2D array of heights
    private float[,] heights;
    // Control variable to determine smoothness of heights
    public float rangeReductionValue = 0.55f;

    /// <summary>
    /// Used for initialization
    /// </summary>
    private void Awake()
    {
        data = transform.GetComponent<TerrainCollider>().terrainData;
        size = data.heightmapResolution;

        //ExecuteDiamondSquare();
        //Reset();//this resets the terrain

        return;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteDiamondSquare();
        }

    }

    /// <summary>
    /// Resets the values of the terrain. If randomizeCornerValues is true then the
    /// corner heights will be randomized, else it will be flat.
    /// </summary>
    public void Reset()
    {
        heights = new float[size, size];

        //first step of DS algorithm
        heights[0, 0] = Random.value;
        heights[size - 1, 0] = Random.value;
        heights[0, size - 1] = Random.value;
        heights[size - 1, size - 1] = Random.value;

        // Update the terrain data
        data.SetHeights(0, 0, heights);

        return;
    }

    /// <summary>
    /// Executes the DiamondSquare algorithm on the terrain.
    /// </summary>
    public void ExecuteDiamondSquare()
    {
        heights = new float[size, size];
        float average, range = 0.5f;
        int sideLength, halfSide, x, y;

        // While the side length is greater than 1
        //and reduce the side length by half each iteration
        for (sideLength = size - 1; sideLength > 1; sideLength /= 2)
        {
            halfSide = sideLength / 2;

            // Run Diamond Step
            for (x = 0; x < size - 1; x += sideLength)//adding sideLength takes you to the opposite corner of current square on x axis
            {
                for (y = 0; y < size - 1; y += sideLength)//adding sideLength takes you to the opposite corner of current square on Y axis
                {
                    // Get the average of the corners
                    average = heights[x, y];//top left
                    average += heights[x + sideLength, y];//top right
                    average += heights[x, y + sideLength];//bottom left
                    average += heights[x + sideLength, y + sideLength];//bottom right
                    average /= 4.0f;//4 corners so divide by 4 to get average

                    // add a random value
                    average += (Random.value * (range * 2.0f)) - range;
                    //put this average in the middle
                    heights[x + halfSide, y + halfSide] = average;
                }
            }//diamond step done

            // Run Square Step
            for (x = 0; x < size - 1; x += halfSide)//add half the length of a side to get the middle
            {
                for (y = (x + halfSide) % sideLength; y < size - 1; y += sideLength)
                {
                    // Get the average of the corners
                    average = heights[(x - halfSide + size - 1) % (size - 1), y];//top point of diamond
                    average += heights[(x + halfSide) % (size - 1), y];
                    average += heights[x, (y + halfSide) % (size - 1)];
                    average += heights[x, (y - halfSide + size - 1) % (size - 1)];
                    average /= 4.0f;

                    // Offset by a random value
                    average += (Random.value * (range * 2.0f)) - range;

                    // Set the height value to be the calculated average
                    heights[x, y] = average;

                    // Set the height on the opposite edge if this is
                    // an edge piece
                    if (x == 0)
                    {
                        heights[size - 1, y] = average;
                    }

                    if (y == 0)
                    {
                        heights[x, size - 1] = average;
                    }
                }
            }

            // Lower the random value range
            range -= range * rangeReductionValue;
        }

        // Update the terrain heights
        data.SetHeights(0, 0, heights);
        return;
    }
}
