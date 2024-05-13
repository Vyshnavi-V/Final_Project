// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.XR.ARFoundation;


// public class Quicksort : MonoBehaviour
// {
//             public ARPlaneManager arPlaneManager;

//     public GameObject cubePrefab;
//     public TMP_InputField inputField;
//     public GameObject inputCanvas;
//     public Button randomButton;
//     public Camera mainCamera;
//     public float spacing = 0.2f;
//     public Color textColor = Color.black;
//     public Color comparisonColor = Color.yellow;
//     public float sortingDelay = 1f;
//     public float swapSpeed = 1f;

//     public TextMeshProUGUI lowText;
//     public TextMeshProUGUI highText;
//     public TextMeshProUGUI pivotText;
//         public TextMeshProUGUI infotext;

//     public Canvas userCanvas;

//     private List<List<GameObject>> generatedCubes = new List<List<GameObject>>();
//     private bool sortingInProgress = false;
//     private bool paused = false;
//         private ARPlane trackPlane;


//     private void Start()
//     {
//         randomButton.onClick.AddListener(GenerateRandomNumbers);
//     }

//     public void GenerateRandomNumbers()
//     {
//         if (sortingInProgress)
//         {
//             return;
//         }

//         sortingInProgress = true;

//         inputCanvas.SetActive(false);

//         string[] randomNumbers = GenerateRandomArray(5);

//        // GenerateInitialCubes(randomNumbers);
//     }

//     private string[] GenerateRandomArray(int length)
//     {
//         string[] randomArray = new string[length];
//         for (int i = 0; i < length; i++)
//         {
//             randomArray[i] = Random.Range(0, 10).ToString();
//         }
//         return randomArray;
//     }
//     public void OnSubmitButtonClick()
//     {

//         // Start a coroutine to wait for plane detection
//         StartCoroutine(WaitForPlaneDetection());
//     }

//     private IEnumerator WaitForPlaneDetection()
//     {

//         infotext.text = "Don't move the phone.Waiting for plane detection";
//         float elapsedTime = 0f;
//         float maxWaitTime = 60f; // Maximum wait time in seconds (1 minute)

//         while (elapsedTime < maxWaitTime)
//         {
//             // Check if any planes are detected
//             foreach (var trackable in arPlaneManager.trackables)
//             {
//                 if (trackable is ARPlane arPlane)
//                 {
//                     infotext.text = "plane detected";
//                     Debug.LogError("No AR planes detected .");
//                     yield return new WaitForSeconds(2f);
//                     // Plane detected, generate cubes on this plane
//                     trackPlane = arPlane;
//                     GenerateCubes();
//                     yield break; // Exit the coroutine
//                 }
//             }

//             // No planes detected yet, wait for a short duration and check again
//             yield return new WaitForSeconds(0.5f);
//             elapsedTime += 0.5f;
//         }

//         // No plane detected within the time limit, display error message
//         Debug.LogError("No AR planes detected within the time limit.");
//         infotext.text = "No AR plane detected within 1 minute.";
//     }
//     public void GenerateCubes()
//     {
//        foreach (List<GameObject> cubeList in generatedCubes)
//         {
//             foreach (GameObject cube in cubeList)
//             {
//                 // Ensure the GameObject is valid before attempting to destroy it
//                 if (cube != null)
//                 {
//                     Destroy(cube);
//                 }
//             }
//         }
//         if (sortingInProgress)
//         {
//             return;
//         }
//         Vector3 position = new Vector3(0f,0f,0f);
//         moveCanvas(userCanvas,position);
//         sortingInProgress = true;

//         if (generatedCubes.Count > 0)
//         {
//             foreach (List<GameObject> levelCubes in generatedCubes)
//             {
//                 foreach (GameObject cube in levelCubes)
//                 {
//                     Destroy(cube);
//                 }
//             }
//         }

//         inputCanvas.SetActive(false);

//         string Nos = inputField.text;
//         string[] numbers = Nos.Split(',');

//         GenerateInitialCubesOnPlane(numbers);
//     }
//  private void GenerateInitialCubesOnPlane(string[] numbers)
//     {
//         float totalWidth = (numbers.Length - 1) * spacing;
//         float startX = -0.5f;
//         float currentX = startX;

//         List<GameObject> initialCubes = new List<GameObject>();
//         Vector3 planePosition = trackPlane.transform.position;

//         for (int i = 0; i < numbers.Length; i++)
//         {
//             Vector3 cubePosition = new Vector3(planePosition.x + currentX, planePosition.y+0.5f, planePosition.z+1f);

//             GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

//             currentX += spacing*0.05f;

//             initialCubes.Add(cube);
//             SetupCubeAndIndexUI(cube,numbers[i], i);
            
//         }

//         generatedCubes.Add(initialCubes);

//         StartCoroutine(QuicksortCoroutine(0, generatedCubes[0].Count - 1, 0));
//     }
//     // private void GenerateInitialCubes(string[] numbers)
//     // {
//     //     float totalWidth = (numbers.Length - 1) * spacing;
//     //     float startX = -totalWidth / 2f;
//     //     float currentX = startX;

//     //     List<GameObject> initialCubes = new List<GameObject>();

//     //     for (int i = 0; i < numbers.Length; i++)
//     //     {
//     //         Vector3 cubePosition = new Vector3(currentX, 0f, 0f);

//     //         GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

//     //         currentX += spacing;

//     //         initialCubes.Add(cube);

//     //         Canvas canvas = cube.GetComponentInChildren<Canvas>();
//     //         if (canvas != null)
//     //         {
//     //             TextMeshProUGUI textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();
//     //             if (textMesh != null)
//     //             {
//     //                 textMesh.text = numbers[i];
//     //                 textMesh.color = textColor;
//     //                 textMesh.alignment = TextAlignmentOptions.Center;
//     //             }
//     //             else
//     //             {
//     //                 Debug.LogError("TextMeshProUGUI component not found in the canvas of the cube prefab.");
//     //             }
//     //         }
//     //         else
//     //         {
//     //             Debug.LogError("Canvas component not found in the children of the cube prefab.");
//     //         }
//     //     }

//     //     generatedCubes.Add(initialCubes);

//     //     StartCoroutine(QuicksortCoroutine(0, generatedCubes[0].Count - 1, 0));
//     // }

// private void SetupCubeAndIndexUI(GameObject cube, string number, int indexNumber)
//     {
//         Canvas canvas = cube.GetComponentInChildren<Canvas>();

//         if (canvas != null)
//         {
//             TextMeshProUGUI textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();

//             if (textMesh != null)
//             {
//                 textMesh.text = number;
//                 textMesh.color = textColor;
//                 textMesh.alignment = TextAlignmentOptions.Center;

//                 float cubeSize = 24.2f;
//                 float fontSizeMultiplier = 4f;
//                 textMesh.fontSize = Mathf.RoundToInt(cubeSize * fontSizeMultiplier);
//             }
//             else
//             {
//                 Debug.LogError("TextMeshProUGUI component not found in the canvas of the cube or index prefab.");
//             }
//         }
//         else
//         {
//             Debug.LogError("Canvas component not found in the children of the cube or index prefab.");
//         }
//     }
//     private void FocusCameraOnCubes()
//     {
//         if (generatedCubes.Count > 0 && mainCamera != null)
//         {
//             Bounds bounds = new Bounds(generatedCubes[0][0].transform.position, Vector3.zero);
//             foreach (List<GameObject> levelCubes in generatedCubes)
//             {
//                 foreach (GameObject cube in levelCubes)
//                 {
//                     bounds.Encapsulate(cube.GetComponent<Renderer>().bounds);
//                 }
//             }

//             float cameraDistance = bounds.size.magnitude / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

//             mainCamera.transform.position = bounds.center - mainCamera.transform.forward * cameraDistance;
//             mainCamera.transform.LookAt(bounds.center);
//         }
//     }

//     private IEnumerator QuicksortCoroutine(int low, int high, int level)
//     {
//         yield return new WaitForSeconds(sortingDelay);

//         if (level < generatedCubes.Count && low >= 0 && high < generatedCubes[level].Count && low <= high)
//         {
//             if (low < high)
//             {
//                 int pivotIndex = low;
//                 int pivotValue = int.Parse(generatedCubes[level][pivotIndex].GetComponentInChildren<TextMeshProUGUI>().text);
//                 int left = low + 1;
//                 int right = high;

//                 HighlightCube(generatedCubes[level][pivotIndex], Color.red, true);

//                 while (left <= right)
//                 {
//                     int leftValue = int.Parse(generatedCubes[level][left].GetComponentInChildren<TextMeshProUGUI>().text);
//                     int rightValue = int.Parse(generatedCubes[level][right].GetComponentInChildren<TextMeshProUGUI>().text);

//                     while (left <= right && leftValue <= pivotValue)
//                     {
//                         yield return StartCoroutine(TraverseCube(generatedCubes[level][left], Color.yellow, true));
//                         left++;
//                         if (left <= right)
//                         {
//                             leftValue = int.Parse(generatedCubes[level][left].GetComponentInChildren<TextMeshProUGUI>().text);
//                         }
//                     }

//                     while (left <= right && rightValue > pivotValue)
//                     {
//                         yield return StartCoroutine(TraverseCube(generatedCubes[level][right], Color.green, false, true));
//                         right--;
//                         if (left <= right)
//                         {
//                             rightValue = int.Parse(generatedCubes[level][right].GetComponentInChildren<TextMeshProUGUI>().text);
//                         }
//                     }

//                     if (left < right)
//                     {
//                         yield return StartCoroutine(SwapCubes(generatedCubes[level], left, right, pivotIndex));
//                         yield return new WaitForSeconds(0.5f);
//                     }
//                 }

//                 yield return StartCoroutine(SwapCubes(generatedCubes[level], pivotIndex, right, pivotIndex));

//                 VisualizePartition(low, right, level);

//                 yield return StartCoroutine(QuicksortCoroutine(low, right - 1, level + 1));

//                 if (right + 1 < high)
//                 {
//                     int secondLastLevel = generatedCubes.Count - 1;
//                     yield return StartCoroutine(QuicksortCoroutine(right + 1, high, secondLastLevel));
//                 }
//             }
//             else
//             {
//                 Debug.Log("Partition at level " + level + " contains only one element.");
//                 yield return null;
//             }
//         }
//         else
//         {
//             Debug.LogError("Invalid indices. Check the values of low, high, or the generatedCubes list.");
//         }
//     }

//     private IEnumerator SwapCubes(List<GameObject> cubes, int index1, int index2, int pivotIndex)
//     {
//         GameObject cube1 = cubes[index1];
//         GameObject cube2 = cubes[index2];

//         Vector3 initialPos1 = cube1.transform.position;
//         Vector3 initialPos2 = cube2.transform.position;
//         float swapDuration = 1f;

//         HighlightCube(cubes[pivotIndex], Color.red, true);
//         HighlightCube(cubes[index1], Color.yellow, false, true);
//         HighlightCube(cubes[index2], Color.green, false, true);

//         yield return new WaitForSeconds(0.5f);

//         float cubeHeight = cube1.GetComponent<Renderer>().bounds.size.y;
//         cubeHeight = cubeHeight*0.8f;
//         float startTime = Time.time;
//         while (Time.time - startTime < swapDuration)
//         {
//             float fracJourney = (Time.time - startTime) / swapDuration;
//             cube1.transform.position = Vector3.Lerp(initialPos1, initialPos1 + Vector3.up * cubeHeight, fracJourney);
//             cube2.transform.position = Vector3.Lerp(initialPos2, initialPos2 + Vector3.up * cubeHeight, fracJourney);
//             yield return null;
//         }

//         GameObject temp = cubes[index1];
//         cubes[index1] = cubes[index2];
//         cubes[index2] = temp;

//         SwapTextPositions(cubes[index1], cubes[index2]);

//         startTime = Time.time;
//         while (Time.time - startTime < swapDuration)
//         {
//             float fracJourney = (Time.time - startTime) / swapDuration;
//             cube1.transform.position = Vector3.Lerp(initialPos1 + Vector3.up * cubeHeight, initialPos2, fracJourney);
//             cube2.transform.position = Vector3.Lerp(initialPos2 + Vector3.up * cubeHeight, initialPos1, fracJourney);
//             yield return null;
//         }

//         cube1.transform.position = initialPos2;
//         cube2.transform.position = initialPos1;

//         if (index1 != pivotIndex && index2 != pivotIndex)
//         {
//             HighlightCube(cubes[index1], textColor);
//             HighlightCube(cubes[index2], textColor);
//         }
//     }

//     private void SwapTextPositions(GameObject cube1, GameObject cube2)
//     {
//         TextMeshProUGUI textMesh1 = cube1.GetComponentInChildren<TextMeshProUGUI>();
//         TextMeshProUGUI textMesh2 = cube2.GetComponentInChildren<TextMeshProUGUI>();

//         Vector3 tempPos = textMesh1.transform.localPosition;
//         textMesh1.transform.localPosition = textMesh2.transform.localPosition;
//         textMesh2.transform.localPosition = tempPos;
//     }
// private IEnumerator TraverseCube(GameObject cube, Color color, bool isLow = false, bool isHigh = false)
// {
//     if (isLow && isHigh)
//     {
//         // Add code to handle the case when the cube is both "low" and "high"
//     }
//     else if (isLow)
//     {
//         HighlightCube(cube, color, false, true, false);
//     }
//     else if (isHigh)
//     {
//         HighlightCube(cube, color, false, false, true);
//     }
//     else
//     {
//         HighlightCube(cube, color);
//     }
//     yield return new WaitForSeconds(2f);
//     HighlightCube(cube, textColor);
// }

// private void HighlightCube(GameObject cube, Color color, bool isPivot = false, bool isLow = false, bool isHigh = false)
// {
//     TextMeshProUGUI textMesh = cube.GetComponentInChildren<TextMeshProUGUI>();
//     if (textMesh != null)
//     {
//         // Store the original text and position
//         string originalText = textMesh.text;
//         Vector3 originalPosition = textMesh.transform.position;

//         // Set the text color
//         textMesh.color = color;

//         // Set text position above the cube
//         Vector3 textPosition = cube.transform.position;
//         float yOffset = 0f;

//         if (isPivot)
//         {
//             textMesh.color = Color.red;
//             EnableText(pivotText);
//             pivotText.transform.position = textPosition + Vector3.up * 30f; // Adjust pivot text position
//             yOffset = 1.5f;
//         }
//         else if (isHigh && !isLow)
//         {
//             EnableText(highText);
//             highText.transform.position = textPosition + Vector3.up * 130f; // Adjust high text position
//             yOffset = 0.5f;
//             // If the cube is both high and low, hide the "low" text
//             if (lowText.gameObject.activeSelf)
//             {
//                 DisableText(lowText);
//             }
//         }
//         else if (isLow && !isHigh)
//         {
//             EnableText(lowText);
//             lowText.transform.position = textPosition + Vector3.up * 130f; // Adjust low text position
//             yOffset = 1.0f;
//             // If the cube is both low and high, hide the "high" text
//             if (highText.gameObject.activeSelf)
//             {
//                 DisableText(highText);
//             }
//         }
//         else
//         {
//             textMesh.color = textColor;
//             DisableText(lowText);
//             DisableText(highText);
//         }

//         // Set the original text and position back
//         textMesh.text = originalText;
//         textMesh.transform.position = originalPosition + Vector3.up * yOffset; // Adjust the original position
//     }
// }




// private void EnableText(TextMeshProUGUI textObject)
// {
//     textObject.gameObject.SetActive(true);
// }

// private void DisableText(TextMeshProUGUI textObject)
// {
//     textObject.gameObject.SetActive(false);
// }


//     private void VisualizePartition(int low, int high, int level)
//     {
//         List<GameObject> clonedCubes = new List<GameObject>();
//         float yOffset = -0.5f * (generatedCubes.Count - level);

//         for (int i = 0; i < generatedCubes[level].Count; i++)
//         {
//             Vector3 newPosition = generatedCubes[level][i].transform.position;
//             newPosition.y += yOffset;
//             clonedCubes.Add(Instantiate(generatedCubes[level][i], newPosition, Quaternion.identity));
//             clonedCubes[i].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
//         }

//         generatedCubes.Add(clonedCubes);
//     }

//     public void PauseSorting()
//     {
//         if (sortingInProgress && !paused)
//         {
//             paused = true;
//             Time.timeScale = 0f;
//         }
//     }

//     public void PlaySorting()
//     {
//         if (sortingInProgress && paused)
//         {
//             paused = false;
//             Time.timeScale = 1f;
//             StartCoroutine(QuicksortCoroutine(0, generatedCubes[0].Count - 1, 0));
//         }
//     }
//     private void moveCanvas(Canvas canvas, Vector3 position)
// {
//     if (canvas == null)
//     {
//         Debug.LogError("Canvas parameter is null. Cannot move canvas.");
//         return;
//     }
//     float offsetX = -spacing * 0.5f; 
//     RectTransform canvasRect = canvas.GetComponent<RectTransform>();
//     if (canvasRect != null)
//     {
//         canvasRect.anchoredPosition3D = position + new Vector3(offsetX, 0f, 0f);
//     }
//     else
//     {
//         Debug.LogError("RectTransform component not found on the canvas. Cannot move canvas.");
//     }
// }



// }
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Quicksort : MonoBehaviour
{
    // Public variables accessible in Unity Editor
    public GameObject cubePrefab; // Prefab for the cube object
    public TMP_InputField inputField; // Input field for entering numbers
    public GameObject inputCanvas; // Canvas for input UI
    public Button randomButton; // Button to generate random numbers
    public Camera mainCamera; // Main camera
    public float spacing = -0.2f; // Spacing between cubes
    public Color textColor = Color.black; // Text color for cube numbers
    public Color comparisonColor = Color.yellow; // Color for comparison text
    public float sortingDelay = 1f; // Delay between sorting steps
    public float swapSpeed = 1f; // Speed of cube swapping animation

    // UI Text elements
    public TextMeshProUGUI lowText; // Text for displaying low value
    public TextMeshProUGUI highText; // Text for displaying high value
    public TextMeshProUGUI pivotText; // Text for displaying pivot value
    public TextMeshProUGUI L; // Text for displaying current low value during sorting
    public TextMeshProUGUI H; // Text for displaying current high value during sorting
    public TextMeshProUGUI P; // Text for displaying current pivot value during sorting
    public TextMeshProUGUI comparisonText; // Text for displaying comparison during sorting
    public TextMeshProUGUI StatusText; // Text for displaying sorting status
    public Canvas exitCanvas;
    public Canvas infoCanvas;

    // Private variables
    private List<List<GameObject>> generatedCubes = new List<List<GameObject>>(); // List to store generated cubes
    private bool sortingInProgress = false; // Flag to track if sorting is in progress
    private bool paused = false; // Flag to track if sorting is paused

    private void Start()
    {
        comparisonText.text="";
        StatusText.text="";
        highText.text="";
        lowText.text="";
        pivotText.text="";
        // Add listener to random button click event
        randomButton.onClick.AddListener(GenerateRandomNumbers);
    }

    // Method to generate random numbers and start sorting
    public void GenerateRandomNumbers()
    {
        // Check if sorting is already in progress
        if (sortingInProgress)
        {
            return;
        }

        sortingInProgress = true; // Set sorting in progress flag

        inputCanvas.SetActive(false); // Hide input canvas

        string[] randomNumbers = GenerateRandomArray(5); // Generate random array of numbers

        GenerateInitialCubes(randomNumbers); // Generate initial cubes and start sorting
    }

    // Method to generate random array of numbers
    private string[] GenerateRandomArray(int length)
    {
        string[] randomArray = new string[length];
        for (int i = 0; i < length; i++)
        {
            randomArray[i] = Random.Range(0, 10).ToString();
        }
        return randomArray;
    }
    
    // Method to generate cubes based on input numbers
    public void GenerateCubes()
    {
        // Check if sorting is already in progress
        if (sortingInProgress)
        {
            return;
        }

        sortingInProgress = true; // Set sorting in progress flag

        // Destroy existing cubes
        if (generatedCubes.Count > 0)
        {
            foreach (List<GameObject> levelCubes in generatedCubes)
            {
                foreach (GameObject cube in levelCubes)
                {
                    Destroy(cube);
                }
            }
        }

        inputCanvas.SetActive(false); // Hide input canvas

        string Nos = inputField.text; // Get input numbers as string
        string[] numbers = Nos.Split(','); // Split input string into array of numbers
        //inputField.text="";

        GenerateInitialCubes(numbers); // Generate initial cubes and start sorting
    }

    // Method to generate initial cubes based on input numbers
    private void GenerateInitialCubes(string[] numbers)
    {
        float totalWidth = (numbers.Length - 1) * spacing; // Calculate total width of cubes
        float startX = -0.2f; // Calculate starting X position
        float currentX = startX; // Initialize current X position

        List<GameObject> initialCubes = new List<GameObject>(); // List to store initial cubes

        // Loop through input numbers to create cubes
        for (int i = 0; i < numbers.Length; i++)
        {
            // Calculate cube position
            Vector3 cubePosition = new Vector3(currentX, 0f, 0f);

            // Instantiate cube prefab at calculated position
            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

            currentX += spacing*0.05f; // Update current X position

            initialCubes.Add(cube); // Add cube to initial cubes list

            // Set number text on cube
            Canvas canvas = cube.GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                TextMeshProUGUI textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();
                if (textMesh != null)
                {
                    textMesh.text = numbers[i];
                    textMesh.color = textColor;
                    textMesh.alignment = TextAlignmentOptions.Center;
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI component not found in the canvas of the cube prefab.");
                }
            }
            else
            {
                Debug.LogError("Canvas component not found in the children of the cube prefab.");
            }
        }

        generatedCubes.Add(initialCubes); // Add initial cubes list to generated cubes list
        movePPRCanvas(exitCanvas,initialCubes[0].transform.position);
        int n=initialCubes.Count;
        moveBackCanvas(infoCanvas,initialCubes[n/2].transform.position);
        StartCoroutine(QuicksortCoroutine(0, generatedCubes[0].Count - 1, 0)); // Start sorting coroutine
    }

    // Method to focus camera on cubes
    private void FocusCameraOnCubes()
    {
        if (generatedCubes.Count > 0 && mainCamera != null)
        {
            Bounds bounds = new Bounds(generatedCubes[0][0].transform.position, Vector3.zero);
            foreach (List<GameObject> levelCubes in generatedCubes)
            {
                foreach (GameObject cube in levelCubes)
                {
                    bounds.Encapsulate(cube.GetComponent<Renderer>().bounds);
                }
            }

            float cameraDistance = bounds.size.magnitude / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            mainCamera.transform.position = bounds.center - mainCamera.transform.forward * cameraDistance;
            mainCamera.transform.LookAt(bounds.center);
        }
    }

    // Coroutine for Quicksort algorithm
    private IEnumerator QuicksortCoroutine(int low, int high, int level)
    {
        yield return new WaitForSeconds(sortingDelay); // Wait for sorting delay

        // Check if indices are valid and sorting can continue
        if (level < generatedCubes.Count && low >= 0 && high < generatedCubes[level].Count && low <= high)
        {
            // Check if partition contains more than one element
            if (low < high)
            {
                // Initialize pivot index and value
                int pivotIndex = low;
                HighlightCube(generatedCubes[level][pivotIndex], Color.red, true);
                L.text = "Low: " + generatedCubes[level][pivotIndex].GetComponentInChildren<TextMeshProUGUI>().text;
                int pivotValue = int.Parse(generatedCubes[level][pivotIndex].GetComponentInChildren<TextMeshProUGUI>().text);

                // Initialize left and right pointers
                int left = low + 1;
                if (left < generatedCubes[level].Count)
                {
                    HighlightCube(generatedCubes[level][left], Color.yellow, false, true);
                    L.text = "Low: " + generatedCubes[level][left].GetComponentInChildren<TextMeshProUGUI>().text;
                }
                int right = high;
                if (right >= 0 && right < generatedCubes[level].Count)
                {
                    HighlightCube(generatedCubes[level][right], Color.green, false, true);
                    H.text = "High: " + generatedCubes[level][right].GetComponentInChildren<TextMeshProUGUI>().text;
                }

                // Partitioning step
                while (left <= right)
                {
                    // Get values of left and right pointers
                    int leftValue = int.Parse(generatedCubes[level][left].GetComponentInChildren<TextMeshProUGUI>().text);
                    int rightValue = int.Parse(generatedCubes[level][right].GetComponentInChildren<TextMeshProUGUI>().text);

                    // Update comparison text for left pointer
                    UpdateComparisonText("Comparing: " + leftValue + "(L)" + (leftValue < pivotValue ? "<" : ">=") + " " + pivotValue + "(P)");
                    L.text = "Low: " + leftValue.ToString();

                    // Traverse the array from left to right until a suitable element is found
                    while (left <= right && leftValue <= pivotValue)
                    {
                        yield return StartCoroutine(TraverseCube(generatedCubes[level][left], Color.yellow, true));
                        left++;
                        if (left <= right)
                        {
                            leftValue = int.Parse(generatedCubes[level][left].GetComponentInChildren<TextMeshProUGUI>().text);
                        }
                        UpdateComparisonText("Comparing: " + leftValue + "(L) " + (leftValue < pivotValue ? "<" : ">=") + " " + pivotValue + "(P)");
                        if (left <= right)
                            L.text = "Low: " + leftValue.ToString();
                    }

                    // Check if left pointer exceeds right pointer
                    if (left > right)
                        break;

                    // If leftValue is greater than pivotValue, update Low and add a delay
                    if (leftValue > pivotValue)
                    {
                        L.text = "Low: " + leftValue.ToString();
                        HighlightCube(generatedCubes[level][left], Color.yellow, false, true);
                        yield return new WaitForSeconds(sortingDelay);
                    }

                    // Update comparison text for right pointer
                    UpdateComparisonText("Comparing: " + pivotValue + "(P)" + (pivotValue < rightValue ? "<" : ">=") + " " + rightValue + "(R)");
                    H.text = "High: " + rightValue.ToString();

                    // Traverse the array from right to left until a suitable element is found
                    while (left <= right && rightValue > pivotValue)
                    {
                        yield return StartCoroutine(TraverseCube(generatedCubes[level][right], Color.green, false, true));
                        right--;
                        if (left <= right)
                        {
                            rightValue = int.Parse(generatedCubes[level][right].GetComponentInChildren<TextMeshProUGUI>().text);
                        }
                        UpdateComparisonText("Comparing: " + pivotValue + "(P) " + (pivotValue < rightValue ? "<" : ">=") + " " + rightValue + "(R)");
                        H.text = "High: " + rightValue.ToString();
                    }

                    // Check if left pointer exceeds right pointer after right pointer traversal
                    if (left < right)
                    {
                        yield return StartCoroutine(SwapCubes(generatedCubes[level], left, right, pivotIndex));
                        yield return new WaitForSeconds(0.5f);
                    }
                }

                UpdateComparisonText("Comparing:"); // Clear comparison text after partitioning

                // Swap pivot with right pointer
                yield return StartCoroutine(SwapCubes(generatedCubes[level], pivotIndex, right, pivotIndex));

                // Visualize partition
                VisualizePartition(low, right, level);

                // Recursive calls for left and right partitions
                yield return StartCoroutine(QuicksortCoroutine(low, right - 1, level + 1));
                if (right + 1 < high)
                {
                    int secondLastLevel = generatedCubes.Count - 1;
                    yield return StartCoroutine(QuicksortCoroutine(right + 1, high, secondLastLevel));
                }
            }
            else
            {
                // If partition contains only one element
                Debug.Log("Partition at level " + level + " contains only one element.");
                yield return null;
            }
        }
        else
        {
            // Invalid indices error
            Debug.LogError("Invalid indices. Check the values of low, high, or the generatedCubes list.");
        }

        // At the end of sorting process
        if (low == 0 && high == generatedCubes[0].Count - 1)
        {
            // Sorting completed
            StatusText.text = "Sorted!";
        }
    }

    // Coroutine to swap cubes
    private IEnumerator SwapCubes(List<GameObject> cubes, int index1, int index2, int pivotIndex)
    {
        // Get cube objects to swap
        GameObject cube1 = cubes[index1];
        GameObject cube2 = cubes[index2];

        // Get initial positions of cubes
        Vector3 initialPos1 = cube1.transform.position;
        Vector3 initialPos2 = cube2.transform.position;
        float swapDuration = 1f;

        // Highlight cubes during swap
        HighlightCube(cubes[pivotIndex], Color.red, true);
        HighlightCube(cubes[index1], Color.yellow, false, true);
        HighlightCube(cubes[index2], Color.green, false, true);
        L.text = "Low:";

        yield return new WaitForSeconds(0.5f);

        float cubeHeight = cube1.GetComponent<Renderer>().bounds.size.y;
        L.text = "Low:";
        H.text = "High:";

        float startTime = Time.time;
        // Perform swap animation
        while (Time.time - startTime < swapDuration)
        {
            float fracJourney = (Time.time - startTime) / swapDuration;
            cube1.transform.position = Vector3.Lerp(initialPos1, initialPos1 + Vector3.up * cubeHeight, fracJourney);
            cube2.transform.position = Vector3.Lerp(initialPos2, initialPos2 + Vector3.up * cubeHeight, fracJourney);
            yield return null;
        }

        // Swap cube objects in the list
        GameObject temp = cubes[index1];
        cubes[index1] = cubes[index2];
        cubes[index2] = temp;

        // Swap text positions
        SwapTextPositions(cubes[index1], cubes[index2]);

        // Update low and high text after swapping
        L.text = "Low: " + cubes[index1].GetComponentInChildren<TextMeshProUGUI>().text;
        H.text = "High: " + cubes[index2].GetComponentInChildren<TextMeshProUGUI>().text;

        startTime = Time.time;
        // Reverse swap animation
        while (Time.time - startTime < swapDuration)
        {
            float fracJourney = (Time.time - startTime) / swapDuration;
            cube1.transform.position = Vector3.Lerp(initialPos1 + Vector3.up * cubeHeight, initialPos2, fracJourney);
            cube2.transform.position = Vector3.Lerp(initialPos2 + Vector3.up * cubeHeight, initialPos1, fracJourney);
            yield return null;
        }

        // Reset cube positions
        cube1.transform.position = initialPos2;
        cube2.transform.position = initialPos1;

        // Highlight cubes if they are not the pivot
        if (index1 != pivotIndex && index2 != pivotIndex)
        {
            HighlightCube(cubes[index1], textColor);
            HighlightCube(cubes[index2], textColor);
        }

        // Update status text with swapped values
        StatusText.text = "Swapped: " + cube1.GetComponentInChildren<TextMeshProUGUI>().text + "(P) and " + cube2.GetComponentInChildren<TextMeshProUGUI>().text + "(H)";
    }

    // Method to swap text positions on cubes
    private void SwapTextPositions(GameObject cube1, GameObject cube2)
    {
        TextMeshProUGUI textMesh1 = cube1.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI textMesh2 = cube2.GetComponentInChildren<TextMeshProUGUI>();

        Vector3 tempPos = textMesh1.transform.localPosition;
        textMesh1.transform.localPosition = textMesh2.transform.localPosition;
        textMesh2.transform.localPosition = tempPos;
    }

    // Coroutine to traverse cube with highlighting
    private IEnumerator TraverseCube(GameObject cube, Color color, bool isLow = false, bool isHigh = false)
    {
        HighlightCube(cube, color, false, isLow, isHigh);
        yield return new WaitForSeconds(2f);
        HighlightCube(cube, textColor);
    }

    // Method to highlight cube and update text
    private void HighlightCube(GameObject cube, Color color, bool isPivot = false, bool isLow = false, bool isHigh = false)
    {
        TextMeshProUGUI textMesh = cube.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null)
        {
            // Store the original text and position
            string originalText = textMesh.text;
            Vector3 originalPosition = textMesh.transform.position;

            // Set the text color
            textMesh.color = color;

            // Set text position above the cube
            Vector3 textPosition = cube.transform.position;
            float yOffset = 0f;

            /*// Adjust text position based on flags
            if (isPivot)
            {
                textMesh.color = Color.red;
                EnableText(pivotText);
                pivotText.transform.position = textPosition + Vector3.up * 30f;
                yOffset = 1.5f;
                P.text = "Pivot: " + originalText;
            }
            else if (isHigh && !isLow)
            {
                EnableText(highText);
                highText.transform.position = textPosition + Vector3.up * 130f;
                yOffset = 0.5f;
                H.text = "High: " + originalText;
                if (lowText.gameObject.activeSelf)
                {
                    DisableText(lowText);
                }
            }
            else if (isLow && !isHigh)
            {
                EnableText(lowText);
                lowText.transform.position = textPosition + Vector3.up * 130f;
                yOffset = 1.0f;
                L.text = "Low: " + originalText;
                if (highText.gameObject.activeSelf)
                {
                    DisableText(highText);
                }
            }
            else
            {
                textMesh.color = textColor;
                DisableText(lowText);
                DisableText(highText);
            }
*/
            // Set the original text and position back
            textMesh.text = originalText;
            textMesh.transform.position = originalPosition + Vector3.up * yOffset;
        }
    }

    // Method to enable text object
    private void EnableText(TextMeshProUGUI textObject)
    {
        textObject.gameObject.SetActive(true);
    }

    // Method to disable text object
    private void DisableText(TextMeshProUGUI textObject)
    {
        textObject.gameObject.SetActive(false);
    }

    // Method to update comparison text
    private void UpdateComparisonText(string text)
    {
        comparisonText.text = text;
    }

    // Method to visualize partition
    private void VisualizePartition(int low, int high, int level)
    {
        List<GameObject> clonedCubes = new List<GameObject>();
        float yOffset = -0.4f * (generatedCubes.Count - level);

        // Clone cubes for visualization
        for (int i = 0; i < generatedCubes[level].Count; i++)
        {
            Vector3 newPosition = generatedCubes[level][i].transform.position;
            newPosition.y += yOffset;
            clonedCubes.Add(Instantiate(generatedCubes[level][i], newPosition, Quaternion.identity));
            clonedCubes[i].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
        }

        generatedCubes.Add(clonedCubes); // Add cloned cubes to generated cubes list
    }

    // Method to restart sorting
    public void RestartSorting()
    {
        // Stop all running coroutines
        StopAllCoroutines();

        // Reset sorting flags
        sortingInProgress = false;
        paused = false;

        // Reset UI elements
        lowText.text = "";
        highText.text = "";
        pivotText.text = "";
        L.text = "Low:" + "";
        H.text = "High:" + "";
        P.text = "Pivot:" + "";
        comparisonText.text = "Comparing:" + "";
        StatusText.text = "Swapped:" + "";

        // Destroy generated cubes
        foreach (List<GameObject> levelCubes in generatedCubes)
        {
            foreach (GameObject cube in levelCubes)
            {
                Destroy(cube);
            }
        }

        // Clear generated cubes list
        generatedCubes.Clear();

        // Show input canvas
        inputCanvas.SetActive(true);
    }

    // Method to pause or resume sorting
    public void PauseResumeSorting()
    {
        if (sortingInProgress)
        {
            if (!paused)
            {
                paused = true;
                StatusText.text = "Paused";
                StopAllCoroutines();
            }
            else
            {
                paused = false;
                StatusText.text = "Sorting...";
                StartCoroutine(QuicksortCoroutine(0, generatedCubes[0].Count - 1, 0));
            }
        }
    }
    public void DestroyCubes(){
        if (generatedCubes.Count > 0)
        {
            foreach (List<GameObject> levelCubes in generatedCubes)
        {
            foreach (GameObject cube in levelCubes)
            {
                Destroy(cube);
            }
        }
            generatedCubes.Clear();
        }
        sortingInProgress = false;
        comparisonText.text="";
        StatusText.text="";
        highText.text="";
        lowText.text="";
        pivotText.text="";
    }
    private void movePPRCanvas(Canvas canvas, Vector3 position)
{
    if (canvas == null)
    {
        Debug.LogError("Canvas parameter is null. Cannot move canvas.");
        return;
    }
    float offsetX = -spacing * 0.1f; 
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
    float offsetY = spacing * 0.1f; 
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