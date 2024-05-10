using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;


public class Stack : MonoBehaviour
{
    public ARPlaneManager arPlaneManager;
    public GameObject cubePrefab;
    public GameObject boxPrefab; // Box GameObject
    public TMP_InputField inputField;
    public Button pushButton; // Button for push operation
    public Button popButton; // Button for pop operation

    private Stack<string> numberStack = new Stack<string>(); // Stack to store numbers
    private Dictionary<string, GameObject> cubeDictionary = new Dictionary<string, GameObject>(); // Dictionary to store cube GameObjects
    private float cubeSize; // Size of the cube
    private float gap = 0.005f; // Gap between cubes
    private float delay = 2f; // Delay between cube generation
    private float currentY = -0.5f; // Current Y position for spawning cubes
    private bool isPushing = false; // Flag to check if pushing is in progress
    private ARPlane trackPlane;
    private float spacing = 5f;
    public Canvas opcanvas;
    public Canvas exitCanvas;

    private GameObject box; // Reference to the box GameObject
    private RectTransform boxCanvasRect; // RectTransform of the BoxCanvas
    public TextMeshProUGUI infotext;

    private void Start()
    {
        cubeSize = cubePrefab.GetComponent<Renderer>().bounds.size.y; // Get the size of the cube
        //boxCanvasRect = GameObject.FindGameObjectWithTag("BoxCanvas").GetComponent<RectTransform>(); // Find BoxCanvas by tag

        //GenerateBox(); // Generate and position the box
    }

    // Method to generate and position the box
    private void GenerateBox()
    {
        box = Instantiate(boxPrefab); // Instantiate the box

        // Position the box near the cubes within the BoxCanvas
        Vector3 boxPosition = new Vector3(30f, 0f, 0f); // Example position, you can adjust this as needed
        Vector2 canvasLocalPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(boxCanvasRect, boxPosition, null, out canvasLocalPosition);
        box.GetComponent<RectTransform>().localPosition = canvasLocalPosition;
    }

    // Method to push numbers onto the stack
    public void PushNumbers()
    {
        if (!isPushing)
        {
            StartCoroutine(PushProcess());
        }
    }
/*
    public void OnSubmitButtonClick()
    {

        // Start a coroutine to wait for plane detection
        StartCoroutine(WaitForPlaneDetection());
    }
*/
    /*
    private IEnumerator WaitForPlaneDetection()
    {
        infotext.text = "Don't move the phone.Waiting for plane detection";
        float elapsedTime = 0f;
        float maxWaitTime = 60f; // Maximum wait time in seconds (1 minute)

        while (elapsedTime < maxWaitTime)
        {
            // Check if any planes are detected
            foreach (var trackable in arPlaneManager.trackables)
            {
                if (trackable is ARPlane arPlane)
                {
                    infotext.text = "plane detected";
                    yield return new WaitForSeconds(2f);
                    // Plane detected, generate cubes on this plane
                    trackPlane = arPlane; 
                    Vector3 planePosition = trackPlane.transform.position;
                     movePPRCanvas(opcanvas,planePosition);
                    moveBackCanvas(exitCanvas,planePosition);
                    currentY = arPlane.transform.position.y;               
                    yield break; // Exit the coroutine
                }
            }

            // No planes detected yet, wait for a short duration and check again
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
        }

        // No plane detected within the time limit, display error message
        Debug.LogError("No AR planes detected within the time limit.");
        infotext.text = "No AR plane detected within 1 minute.";
    }
    */
    // Coroutine for pushing process
    private IEnumerator PushProcess()
    {
        isPushing = true;

        string input = inputField.text;
        string[] numbers = input.Split(',');

        foreach (string number in numbers)
        {
            string trimmedNumber = number.Trim();
            if (!string.IsNullOrEmpty(trimmedNumber))
            {
                numberStack.Push(trimmedNumber);
                GenerateCubesOnPlane(trimmedNumber);// Push the number
                yield return new WaitForSeconds(delay); // Add delay before pushing the next number
            }
        }

        isPushing = false;
    }

    // Method to generate a cube with a given number
    private void GenerateCubesOnPlane(string number)
{
    //Vector3 planePosition = trackPlane.transform.position;
    Vector3 planePosition = new Vector3(-0.2f, -0.5f, 0.5f);
    // Set currentY to the plane's Y position

    // Increment currentY for the next cube
    currentY += cubeSize+gap; // Adding a small gap between cubes

    // Calculate the final position of the cube
    Vector3 finalPosition = new Vector3(0f, currentY, 0f);

    // Start the coroutine to move the cube to its final position
    StartCoroutine(MoveCubeToPosition(cubePrefab, finalPosition, number));
}


    // Coroutine to move a cube to its final position
    private IEnumerator MoveCubeToPosition(GameObject cube, Vector3 finalPosition, string number)
    {
        // Instantiate the cube at an initial position far below
        Vector3 initialPosition = finalPosition + Vector3.up * 0.05f;
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

    // Method to pop a number from the stack
    public void PopNumber()
    {
        if (numberStack.Count > 0)
        {
            // Pop the number
            string poppedNumber = numberStack.Pop();
            Debug.Log("Number popped: " + poppedNumber);
            // Check if the popped number exists in the cube dictionary
            if (cubeDictionary.ContainsKey(poppedNumber))
            {
                // Move the cube downwards and destroy it gradually
                GameObject poppedCube = cubeDictionary[poppedNumber];
                StartCoroutine(MoveAndDestroyCube(poppedCube));

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

    // Coroutine to move a cube downwards and destroy it gradually
    private IEnumerator MoveAndDestroyCube(GameObject cube)
    {
        Vector3 initialPosition = cube.transform.position;
        Vector3 finalPosition = initialPosition + Vector3.up * 1f;
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
    private void movePPRCanvas(Canvas canvas, Vector3 position)
{
    if (canvas == null)
    {
        Debug.LogError("Canvas parameter is null. Cannot move canvas.");
        return;
    }
    float offsetX = -spacing * 0.5f; 
    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
    if (canvasRect != null)
    {
        canvasRect.anchoredPosition3D = position + new Vector3(offsetX, 0f, 0f);
    }
    else
    {
        Debug.LogError("RectTransform component not found on the canvas. Cannot move canvas.");
    }
}
private void moveBackCanvas(Canvas canvas, Vector3 position)
{
    if (canvas == null)
    {
        Debug.LogError("Canvas parameter is null. Cannot move canvas.");
        return;
    }
    float offsetX = 5 * 0.5f; 
    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
    if (canvasRect != null)
    {
        canvasRect.anchoredPosition3D = position + new Vector3(offsetX, 0f, 0f);
    }
    else
    {
        Debug.LogError("RectTransform component not found on the canvas. Cannot move canvas.");
    }
}
public void DestroyAllCubes()
{
    foreach (var cubePair in cubeDictionary)
    {
        Destroy(cubePair.Value);
    }
    
    // Clear the cube dictionary
    cubeDictionary.Clear();

    // Clear the number stack
    numberStack.Clear();
}



}