using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.XR.ARFoundation;

public class SelectionSort : MonoBehaviour
{
     public ARPlaneManager arPlaneManager;
    public GameObject cubePrefab;
    public TMP_InputField userInputField;
    public GameObject inputCanvas;
    public Camera mainCamera;
    public float spacing = 5f;
    public Color textColor = Color.white;
    public Color sortedColor = Color.green; // Color for sorted cubes
    public Color smallestColor = Color.red; // Color for the smallest element
    public float sortingDelay = 1f; // Delay before starting the sorting process
    public float swapSpeed = 12f;
    public GameObject indexPrefab;
    public Color indexColor = Color.black;
    public float scaleFactor;
     public Canvas exitCanvas;

    private GameObject[] cubes;
    private bool sortingInProgress = false;
    private bool paused = false;
    public TextMeshProUGUI iterationText;
     public TextMeshProUGUI actiontext;
    public Canvas infoCanvas;
    private GameObject[] indexes;
    private ARPlane trackPlane;

    private void Start()
    {
        
        iterationText.text="";
    actiontext.text="";
        // You can add a listener to the submit button or call GenerateCubes() from elsewhere in your code.
    }


    public void GenerateRandomCubes()
    {
        int cubeCount = Random.Range(5, 10); // Generates a random number between 5 and 10
        string[] randomNumbers = new string[cubeCount];

        for (int i = 0; i < cubeCount; i++)
        {
            randomNumbers[i] = Random.Range(-50, 100).ToString(); // Generates a random number between -100 and 100
        }

        userInputField.text = string.Join(",", randomNumbers); // Sets the input field text to the generated random numbers
        OnSubmitButtonClick(); // Calls the existing method to generate and sort the cubes
    }

public void OnSubmitButtonClick()
    {

iterationText.text="";
    actiontext.text="";
        // Start a coroutine to wait for plane detection
        StartCoroutine(WaitForPlaneDetection());
    }

    private IEnumerator WaitForPlaneDetection()
    {
        actiontext.text = "Don't move the phone.Waiting for plane detection";
        float elapsedTime = 0f;
        float maxWaitTime = 60f; // Maximum wait time in seconds (1 minute)

        while (elapsedTime < maxWaitTime)
        {
            // Check if any planes are detected
            foreach (var trackable in arPlaneManager.trackables)
            {
                if (trackable is ARPlane arPlane)
                {
                    actiontext.text = "plane detected";
                    yield return new WaitForSeconds(2f);
                    // Plane detected, generate cubes on this plane
                    trackPlane = arPlane;
                    GenerateCubesOnPlane(arPlane);
                    yield break; // Exit the coroutine
                }
            }

            // No planes detected yet, wait for a short duration and check again
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
        }

        // No plane detected within the time limit, display error message
        Debug.LogError("No AR planes detected within the time limit.");
        actiontext.text = "No AR plane detected within 1 minute.";
    }

    public void GenerateCubesOnPlane(ARPlane plane)
    {
        //actiontext.text = "This is called";
        
        if (sortingInProgress)
        {
            return;
        }
        
        DestroyPreviousCubesAndIndices();
        sortingInProgress = true;

        // Hide the BubbleInputCanvas

        // Destroy previous cubes and indexes

        string userInput = userInputField.text;
        string[] numbers = userInput.Split(',');

        // Calculate total width
        float totalWidth = (numbers.Length - 1) * spacing;
        float startX = -0.5f;
        Vector3 iterationTextPosition = new Vector3(startX - 0.3f, 0f, 0f);
        Vector3 planePosition = plane.transform.position;
        //MoveCube(infoCanvas, planePosition);
        //actiontext.text = "Move kazhinj";
        float currentX = startX;

        cubes = new GameObject[numbers.Length];
        indexes = new GameObject[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            //actiontext.text = "Foril keri";
            Vector3 cubePosition = new Vector3(planePosition.x + currentX, planePosition.y+0.5f, planePosition.z+1f);

            // Adjust position relative to the plane
            Vector3 indexPosition = new Vector3(planePosition.x + currentX, planePosition.y+0.2f, planePosition.z + 1f); // Adjust position relative to the plane
            //actiontext.text = "plane"+" "+planePosition+" "+"cube"+" "+cubePosition ;
            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
            GameObject index = Instantiate(indexPrefab, indexPosition, Quaternion.identity);

            currentX += spacing*0.05f;

            cubes[i] = cube;
            indexes[i] = index;

            // Set up cube and index UI
            SetupCubeAndIndexUI(cube, index, numbers[i], i);

        }
         movePPRCanvas(exitCanvas,cubes[0].transform.position);
        int n=cubes.Length;
        moveBackCanvas(infoCanvas,cubes[n/2].transform.position);
        // Start sorting coroutine
        StartCoroutine(SelectionSortCoroutine());
    }
 private void SetupCubeAndIndexUI(GameObject cube, GameObject index, string number, int indexNumber)
    {
        Canvas canvas = cube.GetComponentInChildren<Canvas>();
        Canvas indexCanvas = index.GetComponentInChildren<Canvas>();

        if (canvas != null && indexCanvas != null)
        {
            TextMeshProUGUI textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI indexTextMesh = indexCanvas.GetComponentInChildren<TextMeshProUGUI>();

            if (textMesh != null && indexTextMesh != null)
            {
                textMesh.text = number;
                textMesh.color = textColor;
                textMesh.alignment = TextAlignmentOptions.Center;

                indexTextMesh.text = indexNumber.ToString();
                indexTextMesh.color = indexColor;
                indexTextMesh.alignment = TextAlignmentOptions.Center;

                float cubeSize = 24.2f;
                float fontSizeMultiplier = 4f;
                textMesh.fontSize = Mathf.RoundToInt(cubeSize * fontSizeMultiplier);
                indexTextMesh.fontSize = Mathf.RoundToInt(cubeSize * 10f);
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in the canvas of the cube or index prefab.");
            }
        }
        else
        {
            Debug.LogError("Canvas component not found in the children of the cube or index prefab.");
        }
    }

    private void FocusCameraOnCubes()
    {
        if (cubes.Length > 0 && mainCamera != null)
        {
            // Calculate the bounds of all cubes
            Bounds bounds = new Bounds(cubes[0].transform.position, Vector3.zero);
            foreach (GameObject cube in cubes)
            {
                bounds.Encapsulate(cube.GetComponent<Renderer>().bounds);
            }

            // Calculate the camera distance based on the bounds size
            float cameraDistance = bounds.size.magnitude / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            // Set camera position and rotation
            mainCamera.transform.position = bounds.center - mainCamera.transform.forward * cameraDistance;
            mainCamera.transform.LookAt(bounds.center);
        }
    }

    public void PauseSorting()
    {
        paused = true;
    }

    public void ResumeSorting()
    {
        paused = false;
    }

    public void ReplaySorting()
    {
        StopAllCoroutines(); // Stop any ongoing sorting coroutine
        sortingInProgress = false;
        paused = false;
        GenerateCubesOnPlane(trackPlane); // Regenerate cubes and start sorting again
    }

    private IEnumerator SelectionSortCoroutine()
{
    yield return new WaitForSeconds(sortingDelay);

    int n = cubes.Length;

    for (int i = 0; i < n - 1; i++)
    {
        iterationText.text = "Iteration: " + (i + 1);
        actiontext.text = "Finding smallest element for iteration " + (i + 1);

        yield return new WaitForSeconds(2f); // Delay before finding smallest element

        int minIndex = i;
        int currentMinValue = int.Parse(cubes[minIndex].GetComponentInChildren<TextMeshProUGUI>().text);
        actiontext.text = "Current minimum = " + currentMinValue;

        yield return new WaitForSeconds(2f); // Delay after finding current minimum

        for (int j = i + 1; j < n; j++)
        {
            int currentValue = int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text);

            actiontext.text = "Comparing " + currentMinValue + " with " + currentValue;

            yield return new WaitForSeconds(2f); // Delay after comparing

            if (currentValue < currentMinValue)
            {
                // Change color of the previously found smallest cube back to the original color
                if (minIndex != i)
                {
                    cubes[minIndex].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
                }

                minIndex = j;
                currentMinValue = int.Parse(cubes[minIndex].GetComponentInChildren<TextMeshProUGUI>().text);
                actiontext.text = "Current minimum = " + currentMinValue;
            }

            if (paused)
            {
                yield return new WaitWhile(() => paused == true); // Pause the sorting process
            }
        }

        // Change color of the current smallest cube to red just before swapping
        cubes[minIndex].GetComponentInChildren<TextMeshProUGUI>().color = smallestColor;

        yield return new WaitForSeconds(2f); // Delay before swapping

        int smallestElement = int.Parse(cubes[minIndex].GetComponentInChildren<TextMeshProUGUI>().text);

        actiontext.text = "Smallest number = " + smallestElement;

        yield return new WaitForSeconds(2f); // Delay after displaying smallest number

        actiontext.text = "Swapping " + smallestElement + " to position array(" + i + ")";

        // Swap the found minimum element with the first element
        Vector3 tempPosition = cubes[i].transform.position;
        Vector3 newPosition = cubes[minIndex].transform.position;
        float originalY = tempPosition.y;
    

// Move cubes up
float cubeHeight = cubes[i].GetComponent<Renderer>().bounds.size.y;
//actiontext.text = "CubeHeight "+cubeHeight;
// Define a new variable for half the cube height
float halfCubeHeight = cubeHeight * 0.9f;

// Move cubes up to half the distance
while (cubes[i].transform.position.y < halfCubeHeight || cubes[minIndex].transform.position.y < halfCubeHeight)
{
    cubes[i].transform.position = Vector3.MoveTowards(cubes[i].transform.position, new Vector3(cubes[i].transform.position.x, halfCubeHeight, cubes[i].transform.position.z), Time.deltaTime * swapSpeed);
    cubes[minIndex].transform.position = Vector3.MoveTowards(cubes[minIndex].transform.position, new Vector3(cubes[minIndex].transform.position.x, halfCubeHeight, cubes[minIndex].transform.position.z), Time.deltaTime * swapSpeed);
    yield return null;
}

// Move cubes horizontally
while (cubes[i].transform.position.x != newPosition.x || cubes[minIndex].transform.position.x != tempPosition.x)
{
    cubes[i].transform.position = Vector3.MoveTowards(cubes[i].transform.position, new Vector3(newPosition.x, halfCubeHeight , newPosition.z), Time.deltaTime * swapSpeed);
    cubes[minIndex].transform.position = Vector3.MoveTowards(cubes[minIndex].transform.position, new Vector3(tempPosition.x, halfCubeHeight , tempPosition.z), Time.deltaTime * swapSpeed);
    yield return null;
}

// Move cubes down to original Y position
while (cubes[i].transform.position.y > originalY || cubes[minIndex].transform.position.y > originalY)
{
    cubes[i].transform.position = Vector3.MoveTowards(cubes[i].transform.position, new Vector3(cubes[i].transform.position.x, originalY, cubes[i].transform.position.z), Time.deltaTime * swapSpeed);
    cubes[minIndex].transform.position = Vector3.MoveTowards(cubes[minIndex].transform.position, new Vector3(cubes[minIndex].transform.position.x, originalY, cubes[minIndex].transform.position.z), Time.deltaTime * swapSpeed);
    yield return null;
}


        GameObject tempCube = cubes[i];
        cubes[i] = cubes[minIndex];
        cubes[minIndex] = tempCube;

        // Change color of the sorted cube
        cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = sortedColor;
        // Change color of the smallest cube back to the original color
        cubes[minIndex].GetComponentInChildren<TextMeshProUGUI>().color = textColor;

        // Check if the cube is in its correct sorted position
        if (i == minIndex)
        {
            // If it is, change its color to green
            cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = sortedColor;
        }

        actiontext.text = "Element sorted";
    }

    cubes[n - 1].GetComponentInChildren<TextMeshProUGUI>().color = sortedColor;

    actiontext.text = "Sorting Completed";

    sortingInProgress = false; // Sorting is complete
}
    public void DestroyPreviousCubesAndIndices()
{
    if (cubes != null)
    {
        foreach (GameObject cube in cubes)
        {
            Destroy(cube);
        }
    }

    if (indexes != null)
    {
        foreach (GameObject index in indexes)
        {
            Destroy(index);
        }
    }

    // Reset cube and index arrays
    cubes = null;
    indexes = null;
    //userInputField.text="";
    iterationText.text="";
    actiontext.text="";
    sortingInProgress = false;
     
}
private void movePPRCanvas(Canvas canvas, Vector3 position)
{
    if (canvas == null)
    {
        Debug.LogError("Canvas parameter is null. Cannot move canvas.");
        return;
    }
    float offsetX = -spacing * 0.07f; 
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
    float offsetY = spacing * 0.07f; 
    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
    if (canvasRect != null)
    {
        canvasRect.anchoredPosition3D = position + new Vector3(0f, offsetY, 0f);
    }
    else
    {
        Debug.LogError("RectTransform component not found on the canvas. Cannot move canvas.");
    }
}


}