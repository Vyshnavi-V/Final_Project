using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class quicksort : MonoBehaviour
{
    public GameObject cubePrefab;
    public TMP_InputField inputField;
    public GameObject inputCanvas;
    public Button randomButton; // Corrected reference to the Button component
    public Camera mainCamera;
    public float spacing = 2f;
    public Color textColor = Color.black;
    public Color comparisonColor = Color.yellow; // Color for cubes being compared
    public float sortingDelay = 1f; // Delay before starting the sorting process
    public float swapSpeed = 3f;

    private List<List<GameObject>> generatedCubes = new List<List<GameObject>>(); // Stores generated cubes for each partition level
    private bool sortingInProgress = false;
    private bool paused = false;

    private void Start()
    {
        // Add a listener to the RandomButton
        randomButton.onClick.AddListener(GenerateRandomNumbers);
    }

    public void GenerateRandomNumbers()
    {
        if (sortingInProgress)
        {
            // If sorting is already in progress, ignore the button click
            return;
        }

        sortingInProgress = true; // Set flag to indicate sorting is in progress

        // Disable input canvas
        inputCanvas.SetActive(false);

        // Generate random numbers
        string[] randomNumbers = GenerateRandomArray(5); // Generating numbers of length 5

        // Generate initial set of cubes
        GenerateInitialCubes(randomNumbers);
    }

    private string[] GenerateRandomArray(int length)
    {
        string[] randomArray = new string[length];
        for (int i = 0; i < length; i++)
        {
            randomArray[i] = Random.Range(0, 10).ToString();
        }
        return randomArray;
    }



    public void GenerateCubes()
    {
        if (sortingInProgress)
        {
            // If sorting is already in progress, ignore the button click
            return;
        }

        sortingInProgress = true; // Set flag to indicate sorting is in progress

        if (generatedCubes.Count > 0)
        {
            // Clean up previously generated cubes for all levels
            foreach (List<GameObject> levelCubes in generatedCubes)
            {
                foreach (GameObject cube in levelCubes)
                {
                    Destroy(cube);
                }
            }
        }

        inputCanvas.SetActive(false); // Disable input canvas when submit button is clicked

        string Nos = inputField.text;
        string[] numbers = Nos.Split(',');

        Debug.Log("Number of elements in numbers array: " + numbers.Length); // Debug print

        // Generate initial set of cubes
        GenerateInitialCubes(numbers);
    }

    private void GenerateInitialCubes(string[] numbers)
    {
        // Calculate total width
        float totalWidth = (numbers.Length - 1) * spacing;

        // Calculate starting position
        float startX = -totalWidth / 2f;

        // Initialize currentX to starting position
        float currentX = startX;

        List<GameObject> initialCubes = new List<GameObject>();

        for (int i = 0; i < numbers.Length; i++)
        {
            // Use the current position for each cube
            Vector3 cubePosition = new Vector3(currentX, 0f, 0f);

            Debug.Log("Position of cube " + (i + 1) + ": " + cubePosition); // Debug print

            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

            // Update currentX for the next cube
            currentX += spacing;

            initialCubes.Add(cube);

            // Access the TextMeshPro component inside the canvas of the cube prefab and update its text
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

        generatedCubes.Add(initialCubes);

        StartCoroutine(QuicksortCoroutine(0, generatedCubes[0].Count - 1, 0));
    }

    private void FocusCameraOnCubes()
    {
        if (generatedCubes.Count > 0 && mainCamera != null)
        {
            // Calculate the bounds of all cubes
            Bounds bounds = new Bounds(generatedCubes[0][0].transform.position, Vector3.zero);
            foreach (List<GameObject> levelCubes in generatedCubes)
            {
                foreach (GameObject cube in levelCubes)
                {
                    bounds.Encapsulate(cube.GetComponent<Renderer>().bounds);
                }
            }

            // Calculate the camera distance based on the bounds size
            float cameraDistance = bounds.size.magnitude / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            // Set camera position and rotation
            mainCamera.transform.position = bounds.center - mainCamera.transform.forward * cameraDistance;
            mainCamera.transform.LookAt(bounds.center);
        }
    }

    private IEnumerator QuicksortCoroutine(int low, int high, int level)
    {
        yield return new WaitForSeconds(sortingDelay);

        if (level < generatedCubes.Count && low >= 0 && high < generatedCubes[level].Count && low <= high)
        {
            if (low < high)
            {
                int pivotIndex = low;
                int pivotValue = int.Parse(generatedCubes[level][pivotIndex].GetComponentInChildren<TextMeshProUGUI>().text);
                int left = low + 1;
                int right = high;

                HighlightCube(generatedCubes[level][pivotIndex], Color.red, true); // Highlight pivot cube with red color

                while (left <= right)
                {
                    int leftValue = int.Parse(generatedCubes[level][left].GetComponentInChildren<TextMeshProUGUI>().text);
                    int rightValue = int.Parse(generatedCubes[level][right].GetComponentInChildren<TextMeshProUGUI>().text);

                    while (left <= right && leftValue <= pivotValue)
                    {
                        yield return StartCoroutine(TraverseCube(generatedCubes[level][left], Color.yellow, true)); // Highlight i in yellow
                        left++;
                        if (left <= right)
                        {
                            leftValue = int.Parse(generatedCubes[level][left].GetComponentInChildren<TextMeshProUGUI>().text);
                        }
                    }

                    while (left <= right && rightValue > pivotValue)
                    {
                        yield return StartCoroutine(TraverseCube(generatedCubes[level][right], Color.green, false, true)); // Highlight j in green
                        right--;
                        if (left <= right)
                        {
                            rightValue = int.Parse(generatedCubes[level][right].GetComponentInChildren<TextMeshProUGUI>().text);
                        }
                    }

                    if (left < right)
                    {
                        yield return StartCoroutine(SwapCubes(generatedCubes[level], left, right, pivotIndex)); // Pass pivot index to SwapCubes
                        yield return new WaitForSeconds(0.5f); // Delay for visualization
                    }
                }

                yield return StartCoroutine(SwapCubes(generatedCubes[level], pivotIndex, right, pivotIndex)); // Pass pivot index to SwapCubes

                VisualizePartition(low, right, level);

                yield return StartCoroutine(QuicksortCoroutine(low, right - 1, level + 1));

                if (right + 1 < high)
                {
                    int secondLastLevel = generatedCubes.Count - 1;
                    yield return StartCoroutine(QuicksortCoroutine(right + 1, high, secondLastLevel));
                }
            }
            else
            {
                // Handle the case when there's only one element left in the partition
                Debug.Log("Partition at level " + level + " contains only one element.");
                yield return null; // No need to further partition, just return
            }
        }
        else
        {
            Debug.LogError("Invalid indices. Check the values of low, high, or the generatedCubes list.");
        }
    }

    private IEnumerator SwapCubes(List<GameObject> cubes, int index1, int index2, int pivotIndex)
    {
        GameObject cube1 = cubes[index1];
        GameObject cube2 = cubes[index2];

        Vector3 initialPos1 = cube1.transform.position;
        Vector3 initialPos2 = cube2.transform.position;
        float swapDuration = 1f;

        // Highlight pivot cube in red
        HighlightCube(cubes[pivotIndex], Color.red, true);

        // Highlight low cube in yellow and high cube in green
        HighlightCube(cubes[index1], Color.yellow, false, true);
        HighlightCube(cubes[index2], Color.green, false, true);

        // Delay for visualization
        yield return new WaitForSeconds(0.5f);

        // Move cubes up
        float cubeHeight = cube1.GetComponent<Renderer>().bounds.size.y;
        float startTime = Time.time;
        while (Time.time - startTime < swapDuration)
        {
            float fracJourney = (Time.time - startTime) / swapDuration;
            cube1.transform.position = Vector3.Lerp(initialPos1, initialPos1 + Vector3.up * cubeHeight, fracJourney);
            cube2.transform.position = Vector3.Lerp(initialPos2, initialPos2 + Vector3.up * cubeHeight, fracJourney);
            yield return null;
        }

        // Swap cubes in the list
        GameObject temp = cubes[index1];
        cubes[index1] = cubes[index2];
        cubes[index2] = temp;

        // Swap text positions
        SwapTextPositions(cubes[index1], cubes[index2]);

        // Move cubes down to their new positions
        startTime = Time.time;
        while (Time.time - startTime < swapDuration)
        {
            float fracJourney = (Time.time - startTime) / swapDuration;
            cube1.transform.position = Vector3.Lerp(initialPos1 + Vector3.up * cubeHeight, initialPos2, fracJourney);
            cube2.transform.position = Vector3.Lerp(initialPos2 + Vector3.up * cubeHeight, initialPos1, fracJourney);
            yield return null;
        }

        // Ensure exact final positions (optional)
        cube1.transform.position = initialPos2;
        cube2.transform.position = initialPos1;

        // Reset cube colors after swapping, except for cubes just before the pivot
        if (index1 != pivotIndex && index2 != pivotIndex)
        {
            HighlightCube(cubes[index1], textColor);
            HighlightCube(cubes[index2], textColor);
        }
    }


    private void SwapTextPositions(GameObject cube1, GameObject cube2)
    {
        // Swap positions of 'i', 'j', and 'p' texts
        TextMeshProUGUI textMesh1 = cube1.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI textMesh2 = cube2.GetComponentInChildren<TextMeshProUGUI>();

        Vector3 tempPos = textMesh1.transform.localPosition;
        textMesh1.transform.localPosition = textMesh2.transform.localPosition;
        textMesh2.transform.localPosition = tempPos;
    }

    private IEnumerator TraverseCube(GameObject cube, Color color, bool isLow = false, bool isHigh = false)
    {
        HighlightCube(cube, color, false, isLow, isHigh);
        yield return new WaitForSeconds(2f); // Delay for visualization
        HighlightCube(cube, textColor); // Reset cube color back to white
    }

    private void HighlightCube(GameObject cube, Color color, bool isPivot = false, bool isLow = false, bool isHigh = false)
    {
        TextMeshProUGUI textMesh = cube.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.color = color;
            if (isPivot)
            {
                // Ensure pivot is always highlighted in red
                textMesh.color = Color.red;
                EnableText(cube, "pText");
            }
            else if (isLow)
            {
                EnableText(cube, "iText");
            }
            else if (isHigh)
            {
                EnableText(cube, "jText");
            }
            else
            {
                // Reset other texts
                textMesh.color = textColor;
                DisableText(cube, "iText");
                DisableText(cube, "jText");
                DisableText(cube, "pText");
            }
        }
    }

    private void EnableText(GameObject cube, string textObjectName)
    {
        Transform pointerCanvas = cube.transform.Find("PointerCanvas");
        if (pointerCanvas != null)
        {
            Transform textObject = pointerCanvas.Find(textObjectName);
            if (textObject != null)
            {
                textObject.gameObject.SetActive(true);
            }
        }
    }

    private void DisableText(GameObject cube, string textObjectName)
    {
        Transform pointerCanvas = cube.transform.Find("PointerCanvas");
        if (pointerCanvas != null)
        {
            Transform textObject = pointerCanvas.Find(textObjectName);
            if (textObject != null)
            {
                textObject.gameObject.SetActive(false);
            }
        }
    }

    private void VisualizePartition(int low, int high, int level)
    {
        List<GameObject> clonedCubes = new List<GameObject>();
        float yOffset = -700f * (generatedCubes.Count - level); // Adjust the offset based on the level

        for (int i = 0; i < generatedCubes[level].Count; i++)
        {
            Vector3 newPosition = generatedCubes[level][i].transform.position;
            newPosition.y += yOffset;
            clonedCubes.Add(Instantiate(generatedCubes[level][i], newPosition, Quaternion.identity));
            clonedCubes[i].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
        }

        generatedCubes.Add(clonedCubes);
    }
    // Method to pause the sorting process
    public void PauseSorting()
    {
        if (sortingInProgress && !paused)
        {
            paused = true;
            Time.timeScale = 0f; // Pause the time
        }
    }

    // Method to resume the sorting process
    public void PlaySorting()
    {
        if (sortingInProgress && paused)
        {
            paused = false;
            Time.timeScale = 1f; // Resume the time
            StartCoroutine(QuicksortCoroutine(0, generatedCubes[0].Count - 1, 0)); // Resume the coroutine
        }
    }

}
