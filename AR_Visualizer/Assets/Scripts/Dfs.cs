using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Dfs : MonoBehaviour
{
    public GameObject lineParent;
    public GameObject sphereParent;
    public GameObject orderCanvas;
    public GameObject cubePrefab; 
    public float cubeDelay = 0.5f; 
    public float squareDelayBetweenChanges = 1f; 
    public float sphereDelayBetweenChanges = 1.5f; 
    public float popDelay = 2.5f; 

    private List<Transform> squares = new List<Transform>();
    private List<Transform> spheres = new List<Transform>();
    private TextMeshProUGUI orderText;
    private Stack<string> numberStack = new Stack<string>(); 
    private Dictionary<string, GameObject> cubeDictionary = new Dictionary<string, GameObject>(); 
    private float cubeSize; 
    private float gap = 10f; 
    private float currentY = 0f;
 

    void Start()
    {
        foreach (Transform child in lineParent.transform)
        {
            squares.Add(child);
        }

        foreach (Transform child in sphereParent.transform)
        {
            spheres.Add(child);
        }

        orderText = orderCanvas.GetComponentInChildren<TextMeshProUGUI>();

        StartCoroutine(ChangeSquareColors());
        StartCoroutine(ChangeSphereColors());

        cubeSize = cubePrefab.GetComponent<Renderer>().bounds.size.x;
    }

    IEnumerator ChangeSquareColors()
    {
        foreach (Transform square in squares)
        {
            string squareName = square.name;
            if (squareName != "Line 11" && squareName != "Line 12")
            {
                square.GetComponent<Renderer>().material.color = Color.green; 
            }
            yield return new WaitForSeconds(squareDelayBetweenChanges);
        }

        StartCoroutine(PerformQueueOperations()); // Start queue operations after square color change
    }

    IEnumerator ChangeSphereColors()
    {
        foreach (Transform sphere in spheres)
        {
            sphere.GetComponent<Renderer>().material.color = Color.yellow; 
            TextMeshProUGUI sphereText = sphere.GetComponentInChildren<TextMeshProUGUI>();
            orderText.text += sphereText.text + " ";

            yield return new WaitForSeconds(sphereDelayBetweenChanges);
        }
    }

    IEnumerator PerformQueueOperations()
    {
        yield return new WaitForSeconds(cubeDelay * spheres.Count);

        StartCoroutine(PushPopOperations()); // Start pushing and popping numbers
    }

    IEnumerator PushPopOperations()
    {
        PushNumber("0");
        yield return new WaitForSeconds(cubeDelay);

        

        PushNumber("1");
        yield return new WaitForSeconds(cubeDelay);

        PushNumber("2");
        yield return new WaitForSeconds(cubeDelay);

        

        PushNumber("3");
        yield return new WaitForSeconds(cubeDelay);

        PushNumber("5");
        yield return new WaitForSeconds(cubeDelay);

        PushNumber("6");
        yield return new WaitForSeconds(cubeDelay);

        

        PushNumber("7");
        yield return new WaitForSeconds(cubeDelay);

        PushNumber("8");
        yield return new WaitForSeconds(cubeDelay);

        PushNumber("9");
        yield return new WaitForSeconds(cubeDelay);

        PushNumber("4");
        yield return new WaitForSeconds(cubeDelay);

        PopNumber();
        yield return new WaitForSeconds(popDelay);

        PopNumber();
        yield return new WaitForSeconds(popDelay);

        PopNumber();
        yield return new WaitForSeconds(popDelay);

        PopNumber();
        yield return new WaitForSeconds(popDelay);

        PopNumber();
        yield return new WaitForSeconds(popDelay);

        PopNumber();
        yield return new WaitForSeconds(popDelay);

        PopNumber();
        yield return new WaitForSeconds(popDelay);

        PopNumber();
        yield return new WaitForSeconds(popDelay);

        PopNumber();
        yield return new WaitForSeconds(popDelay);

        PopNumber();
        yield return new WaitForSeconds(popDelay);


    }

    void PushNumber(string number)
    {
        numberStack.Push(number); 
        GenerateCube(number); 
    }

    void PopNumber()
    {
        if (numberStack.Count > 0)
        {
            string poppedNumber = numberStack.Pop();
            Debug.Log("Number popped: " + poppedNumber);

            if (cubeDictionary.ContainsKey(poppedNumber))
            {
                GameObject poppedCube = cubeDictionary[poppedNumber];
                StartCoroutine(MoveAndDestroyCubeCoroutine(poppedCube)); // Use the correct coroutine name
                cubeDictionary.Remove(poppedNumber);
            }
            else
            {
                Debug.LogError("Cube GameObject not found for popped number: " + poppedNumber);
            }
        }
        else
        {
            Debug.Log("Stack is empty. Cannot pop.");
        }
    }

    private void GenerateCube(string number)
    {
        // Increment currentY for the next cube
        currentY += cubeSize + gap; // Adding a small gap between cubes

        // Calculate the final position of the cube
        Vector3 finalPosition = new Vector3(0f, currentY, 0f);

        // Start the coroutine to move the cube to its final position
        StartCoroutine(MoveCubeToPosition(cubePrefab, finalPosition, number));
    }

    private IEnumerator MoveCubeToPosition(GameObject cube, Vector3 finalPosition, string number)
    {
        // Instantiate the cube at an initial position far below
        Vector3 initialPosition = finalPosition + Vector3.up * 20f;
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

    private IEnumerator MoveAndDestroyCubeCoroutine(GameObject cube)
    {
        Vector3 initialPosition = cube.transform.position;
        Vector3 finalPosition = initialPosition + Vector3.down * 20f;
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
}