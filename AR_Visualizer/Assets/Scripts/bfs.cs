using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Bfs : MonoBehaviour
{
    public GameObject lineParent;
    public GameObject sphereParent;
    //public GameObject orderCanvas;
    public GameObject cubePrefab; // Cube prefab for queue visualization
    public float cubeDelay = 0.5f; // Delay between cube generation
    public float squareDelayBetweenChanges = 1f; // Delay between square color changes
    public float sphereDelayBetweenChanges = 1.5f; // Delay between sphere color changes
    public float dequeueDelay = 2.5f; // Delay before dequeuing and destroying cubes

    private List<Transform> squares = new List<Transform>();
    private List<Transform> spheres = new List<Transform>();
    public TextMeshProUGUI orderText;
    public TextMeshProUGUI actionText;
    private Queue<string> numberQueue = new Queue<string>(); // Queue to store numbers
    private Dictionary<string, GameObject> cubeDictionary = new Dictionary<string, GameObject>(); // Dictionary to store cube GameObjects
    private float cubeSize; // Size of the cube
    private float gap = 0.01f; // Gap between cubes
    private float currentX = 0f; // Current X position for spawning cubes
    private bool isDequeueing = false; // Flag to check if dequeueing is in progress
    public Material sphereMaterial;
    void Start()
{
    actionText.text ="";
    orderText.text="";
    orderText.text ="Node Order:";
}

    // Get all squares within the lineParent
    public void BFSMethod(){
        if(squares.Count>0){
            squares.Clear();
        }
        if(spheres.Count>0){
            spheres.Clear();
        }
        if (cubeDictionary.Count > 0)
        {
            foreach (var cube in cubeDictionary.Values)
            {
                Destroy(cube);
            }

            // Clear the dictionary after destroying all objects
            cubeDictionary.Clear();
        }
        while (numberQueue.Count > 0)
{
    numberQueue.Dequeue();
}
    foreach (Transform child in lineParent.transform)
    {
        squares.Add(child);
    }

    // Get all spheres within the sphereParent
    foreach (Transform child in sphereParent.transform)
    {
        spheres.Add(child);
    }
    foreach (Transform square in squares)
        {
            
                square.GetComponent<Renderer>().material.color = Color.white; // Change square color
            
        }

        foreach (Transform sphere in spheres)
        {
            sphere.GetComponent<Renderer>().material = sphereMaterial; // Change sphere color

        
            
        }
    // Get the TextMeshProUGUI component from the orderCanvas
    //orderText = orderCanvas.GetComponentInChildren<TextMeshProUGUI>();

    // Start the color change coroutine for squares
    StartCoroutine(ChangeSquareColors());

    // Start the color change coroutine for spheres
    StartCoroutine(ChangeSphereColors());

    // Start queue operations immediately
    StartCoroutine(EnqueueDequeueOperations());

    // Initialize cubeSize
    cubeSize = cubePrefab.GetComponent<Renderer>().bounds.size.x;
}

    IEnumerator ChangeSquareColors()
    {
        // Iterate through squares and change color with delay
        foreach (Transform square in squares)
        {
            string squareName = square.name;
            if (squareName != "Line 10" && squareName != "Line 11")
            {
                square.GetComponent<Renderer>().material.color = Color.blue; // Change square color
            }
            yield return new WaitForSeconds(squareDelayBetweenChanges);
        }

        // Perform queue operations after color change completes
        //StartCoroutine(PerformQueueOperations());
    }

    IEnumerator ChangeSphereColors()
    {
        // Iterate through spheres and change color with delay
        foreach (Transform sphere in spheres)
        {
            sphere.GetComponent<Renderer>().material.color = Color.green; // Change sphere color

            // Get the Text component from the sphere's canvas
            TextMeshProUGUI sphereText = sphere.GetComponentInChildren<TextMeshProUGUI>();
            // Update the text in the orderCanvas with the text from the sphere
            orderText.text += sphereText.text + " ";

            yield return new WaitForSeconds(sphereDelayBetweenChanges);
            
        }
    }

    IEnumerator PerformQueueOperations()
    {
        // Delay for both ChangeSquareColors and ChangeSphereColors completion
        yield return new WaitForSeconds(cubeDelay * spheres.Count);

        // Perform queue operations
        StartCoroutine(EnqueueDequeueOperations());
    }

    IEnumerator EnqueueDequeueOperations()
    {
        // Enqueue 0
        EnqueueNumber("0");
        actionText.text = "We enqueue the node 0 into the queue";
        //yield return new WaitForSeconds(cubeDelay);

        // Dequeue 0
        yield return new WaitForSeconds(3.5f);
        actionText.text = "Dequeue node 0 and enqueue the children of node 0 --> node 1 and node 2";
        DequeueNumber();
        yield return new WaitForSeconds(0.5f);

        // Enqueue 1
        EnqueueNumber("1");
        yield return new WaitForSeconds(4f);
        // Enqueue 2
        EnqueueNumber("2");
        //yield return new WaitForSeconds(cubeDelay);

        // Dequeue 1
        yield return new WaitForSeconds(3.5f);
        actionText.text = "Dequeue node 1 and enqueue the children of node 1 --> node 3 and node 4";
        DequeueNumber();
        yield return new WaitForSeconds(0.5f);

        // Enqueue 3
        EnqueueNumber("3");
        yield return new WaitForSeconds(4f);

        // Enqueue 4
        EnqueueNumber("4");
        //yield return new WaitForSeconds(cubeDelay);

        // Dequeue 2
        yield return new WaitForSeconds(3.5f);
        actionText.text = "Dequeue node 2 and enqueue the children of node 2 --> no children";

        DequeueNumber();
        yield return new WaitForSeconds(1.5f);

        // Dequeue 3
        //yield return new WaitForSeconds(dequeueDelay);
        actionText.text = "Dequeue node 3 and enqueue the children of node 3 --> node 5";

        DequeueNumber();
        yield return new WaitForSeconds(0.5f);

        // Enqueue 5

        EnqueueNumber("5");
        //yield return new WaitForSeconds(cubeDelay);

        // Dequeue 4
        yield return new WaitForSeconds(3.5f);
        actionText.text = "Dequeue node 4 and enqueue the children of node 4 --> no children";

        DequeueNumber();
        yield return new WaitForSeconds(1.5f);

        // Dequeue 5
        //yield return new WaitForSeconds(dequeueDelay);
        actionText.text = "Dequeue node 5 and enqueue the children of node 5 --> node 6,node 7 and node 8";

        DequeueNumber();
        yield return new WaitForSeconds(0.5f);

        // Enqueue 6

        EnqueueNumber("6");
        yield return new WaitForSeconds(4f);

        // Enqueue 7
        EnqueueNumber("7");
        yield return new WaitForSeconds(4f);

        // Enqueue 8
        EnqueueNumber("8");
        //yield return new WaitForSeconds(cubeDelay);

        // Dequeue 6
        yield return new WaitForSeconds(3.5f);
        actionText.text = "Dequeue node 6 and enqueue the children of node 6 --> no children";

        DequeueNumber();
        yield return new WaitForSeconds(1.5f);

        // Dequeue 7
                actionText.text = "Dequeue node 7 and enqueue the children of node 7 --> no children";

        //yield return new WaitForSeconds(dequeueDelay);
        DequeueNumber();
        //yield return new WaitForSeconds(cubeDelay);

        // Dequeue 8
        yield return new WaitForSeconds(1.5f);
                actionText.text = "Dequeue node 8 and enqueue the children of node 8 --> node 9";

        DequeueNumber();
        yield return new WaitForSeconds(0.2f);

        // Enqueue 9
        EnqueueNumber("9");
        //yield return new WaitForSeconds(cubeDelay);

        // Dequeue 9
        yield return new WaitForSeconds(3.5f);
                        actionText.text = "Dequeue node 9 and add the children of node 9 --> no children";

        DequeueNumber();
        yield return new WaitForSeconds(3f);
        actionText.text = "BFS completed and the order is given above";


        
    }

    void EnqueueNumber(string number)
    {
        numberQueue.Enqueue(number); // Enqueue the number
        GenerateCube(number); // Generate and visualize the cube
    }

    void DequeueNumber()
    {
        if (numberQueue.Count > 0)
        {
            // Dequeue the number
            string dequeuedNumber = numberQueue.Dequeue();
            Debug.Log("Number dequeued: " + dequeuedNumber);

            // Check if the dequeued number exists in the cube dictionary
            if (cubeDictionary.ContainsKey(dequeuedNumber))
            {
                // Move the cube to the left and destroy it gradually
                GameObject dequeuedCube = cubeDictionary[dequeuedNumber];
                StartCoroutine(MoveAndDestroyCube(dequeuedCube));

                cubeDictionary.Remove(dequeuedNumber);
            }
            else
            {
                Debug.LogError("Cube GameObject not found for dequeued number: " + dequeuedNumber);
            }
        }
        else
        {
            Debug.Log("Queue is empty. Cannot dequeue.");
        }
    }

 void GenerateCube(string number)
{
    // Increment currentX for the next cube
    currentX += cubeSize + gap; // Adding a small gap between cubes

    // Calculate the final position of the cube, slightly to the left
    float xOffset = -0.2f; // Adjust as needed
    Vector3 finalPosition = new Vector3(currentX + xOffset, 0.192f, 0.5f);

    // Start the coroutine to move the cube to its final position
    StartCoroutine(MoveCubeToPosition(cubePrefab, finalPosition, number));
}


    IEnumerator MoveCubeToPosition(GameObject cube, Vector3 finalPosition, string number)
    {
        // Instantiate the cube at an initial position far to the right
        Vector3 initialPosition = finalPosition + Vector3.right * 0.5f;
        GameObject newCube = Instantiate(cube, initialPosition, Quaternion.identity);

        // Set the number text of the cube
        newCube.GetComponentInChildren<TextMeshProUGUI>().text = number;

        // Add the cube GameObject to the dictionary
        cubeDictionary[number] = newCube;

        // Move the cube towards its final position gradually
        float duration = 1.2f; // Duration of the movement
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the interpolation factor
            float t = elapsedTime / duration;

            // Move the cube towards its final position using Lerp
            newCube.transform.position = Vector3.Lerp(initialPosition, finalPosition, t);

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the cube is at its final position
        newCube.transform.position = finalPosition;
    }

    IEnumerator MoveAndDestroyCube(GameObject cube)
    {
        // Wait for the delay before dequeuing and destroying
        yield return new WaitForSeconds(dequeueDelay);

        Vector3 initialPosition = cube.transform.position;
        Vector3 finalPosition = initialPosition + Vector3.left * 0.5f;
        float duration = 1.2f; // Duration of the movement
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the interpolation factor
            float t = elapsedTime / duration;

            // Move the cube towards its final position using Lerp
            cube.transform.position = Vector3.Lerp(initialPosition, finalPosition, t);

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the cube is at its final position
        cube.transform.position = finalPosition;

        // Destroy the cube
        Destroy(cube);
    }
    public void exitMethod(){
    orderText.text="";
    actionText.text="";
    currentX-=cubeSize;
    currentX-=gap;
    if (cubeDictionary.Count > 0)
        {
            foreach (var cube in cubeDictionary.Values)
            {
                Destroy(cube);
            }

            // Clear the dictionary after destroying all objects
            cubeDictionary.Clear();
        }

    StopAllCoroutines();
}
}