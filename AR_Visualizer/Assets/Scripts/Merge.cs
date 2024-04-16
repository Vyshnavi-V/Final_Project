using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class Merge : MonoBehaviour
{
    public GameObject cubePrefab;
    public TMP_InputField inputField;
    public float spacing = 2f;
    public Color textColor = Color.white;
    public Color comparisonColor = Color.red; // Color for cubes being compared
    public float sortingDelay = 2f; // Delay before starting the sorting process
    public float swapSpeed = 20f;
    public GameObject linePrefab;
    public float divisionDelay = 2f; // Delay between each division
    public Material lineMaterial; // Material for the line renderer
    public TextMeshProUGUI infoText;

    private GameObject[] cubes;
    List<GameObject> sortedAndGroupedLeft = new List<GameObject>();
    List<GameObject> sortedAndGroupedRight = new List<GameObject>();
    private List<Vector3[]> divisionPositions = new List<Vector3[]>();
    private List<GameObject> divisionLines = new List<GameObject>(); // Store references to division lines
    private List<Material> divisionMaterials = new List<Material>(); // Store references to division materials
    private List<GameObject> finalSorted = new List<GameObject>();
    private bool divisionProcessCompleted = false;


    private void Start()
    {
        // Add a button or event listener here to call GenerateCubes()
    }

    public void GenerateCubes()
    {
        if (cubes != null)
        {
            foreach (var cube in cubes)
            {
                Destroy(cube);
            }
            divisionPositions.Clear();
            ClearDivisionLines();
        }

        string Nos = inputField.text;
        string[] numbers = Nos.Split(',');

        float totalWidth = (numbers.Length - 1) * spacing;
        float startX = -0.5f;
        float currentX = startX;
        float startY = 0f;

        cubes = new GameObject[numbers.Length];
        Vector3[] cubePositions = new Vector3[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            Vector3 cubePosition = new Vector3(currentX, startY, 0f);
            cubePositions[i] = cubePosition;

            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
            currentX += spacing * 0.05f;

            cubes[i] = cube;

            Canvas canvas = cube.GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                TextMeshProUGUI textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();
                if (textMesh != null)
                {
                    textMesh.text = numbers[i];
                    textMesh.color = textColor;
                    textMesh.alignment = TextAlignmentOptions.Center;

                    float cubeSize = 24.2f;
                    float fontSizeMultiplier = 4f;
                    textMesh.fontSize = Mathf.RoundToInt(cubeSize * fontSizeMultiplier);
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
        int startIndex = 0;
        int length = cubes.Length;
        int midpointIndex = startIndex + length / 2;

        // Divide the array into left and right halves
        GameObject[] leftHalf = cubes.Take(midpointIndex).ToArray();
        GameObject[] rightHalf = cubes.Skip(midpointIndex).ToArray();
        divisionPositions.Add(cubePositions);

        // Start recursive division with delay
        StartCoroutine(DivideCubesWithDelay(cubes));
    }
    private IEnumerator DivideCubesWithDelay(GameObject[] cubesToDivide)
    {
        infoText.text = "Merge Sort uses Divide and Conquer Strategy";
        yield return new WaitForSeconds(sortingDelay);

        List<string> numbers = new List<string>();
        for (int i = 0; i < cubesToDivide.Length; i++)
        {
            string num = cubesToDivide[i].GetComponentInChildren<TextMeshProUGUI>().text;
            numbers.Add(num);
        }
        string concatenatedText = string.Join(", ", numbers);
        infoText.text = "Divide(" + concatenatedText + ")";


        yield return StartCoroutine(DelayedDivisionAndMerge(cubesToDivide));
    }


    // Coroutine for recursive division with delay

    private void DivideCubesRecursive(GameObject[] cubesToDivide, int startIndex, int length, float startY)
    {
        if (length <= 1)
        {
            return;
        }
        startY = startY - (spacing * 0.05f);
        // Calculate the midpoint index
        int midpointIndex = startIndex + length / 2;

        // Divide the array into left and right halves
        GameObject[] leftHalf = cubesToDivide.Take(midpointIndex).ToArray();
        GameObject[] rightHalf = cubesToDivide.Skip(midpointIndex).ToArray();

        // Visualize the division
        VisualizeDivision(leftHalf, rightHalf, startY);

        // Change the color of cubes in the left half
        // Recursively divide the left half
        float nextStartY = startY - (spacing); // Increment startY for next division
        StartCoroutine(DelayedDivision(leftHalf, rightHalf, nextStartY));
    }

    private IEnumerator DelayedDivision(GameObject[] leftHalf, GameObject[] rightHalf, float startY)
    {
        yield return new WaitForSeconds(divisionDelay);

        // Divide the left half recursively until it contains single elements
        while (leftHalf.Length > 1)
        {
            // Reset the text color of cubes in the right half from the previous iteration

            // Concatenate the numbers being divided
            string numbersText = string.Join(", ", leftHalf.Select(cube => cube.GetComponentInChildren<TextMeshProUGUI>().text));

            // Update the infoText
            infoText.text = "Divide(" + numbersText + ")";

            // Recursively divide the left half
            DivideCubesRecursive(leftHalf, 0, leftHalf.Length, startY);
            yield return new WaitForSeconds(divisionDelay);

            // Reset the text color of cubes in the left half for the next iteration


            leftHalf = leftHalf.Take(leftHalf.Length / 2).ToArray();
            yield return new WaitForSeconds(divisionDelay);
        }

        // Change the color of cubes in the right half

        // Divide the right half recursively until it contains single elements
        while (rightHalf.Length > 1)
        {
            // Reset the text color of cubes in the right half from the previous iteration

            // Concatenate the numbers being divided
            string numbersText = string.Join(", ", rightHalf.Select(cube => cube.GetComponentInChildren<TextMeshProUGUI>().text));

            // Update the infoText
            infoText.text = "Divide(" + numbersText + ")";

            // Recursively divide the right half
            DivideCubesRecursive(rightHalf, 0, rightHalf.Length, startY);
            yield return new WaitForSeconds(divisionDelay);

            // Reset the text color of cubes in the right half for the next iteration

            rightHalf = rightHalf.Take(rightHalf.Length / 2).ToArray();
            yield return new WaitForSeconds(divisionDelay);
        }

        // Set a flag to indicate that the division process has completed
        divisionProcessCompleted = true;
    }

    private IEnumerator DelayedDivisionAndMerge(GameObject[] cubesToDivide)
    {
        yield return new WaitForSeconds(divisionDelay * 2);

        DivideCubesRecursive(cubesToDivide, 0, cubesToDivide.Length, 0);

        // Wait for the division process to complete
        while (!divisionProcessCompleted)
        {
            yield return null; // Wait until the division process completes
        }

        // After the division process completes, start the merge process
        List<GameObject> sortedCubes = new List<GameObject>(cubesToDivide.OrderBy(cube => cube.transform.position.x));
        yield return new WaitForSeconds(sortingDelay);
        yield return new WaitForSeconds(divisionDelay * 2);
        StartCoroutine(MergeCubesWithDelay(sortedCubes));
    }




    private void VisualizeDivision(GameObject[] leftHalf, GameObject[] rightHalf, float startY)
    {
        float leftMidpoint = leftHalf[leftHalf.Length - 1].transform.position.x;
        float rightMidpoint = rightHalf[0].transform.position.x;
        float midpointPosition = (leftMidpoint + rightMidpoint) / 2f;

        // Instantiate the line prefab at the midpoint position if it doesn't exist
        GameObject line = divisionLines.Count > 0 ? divisionLines[divisionLines.Count - 1] : Instantiate(linePrefab, new Vector3(midpointPosition, startY, 0f), Quaternion.identity);

        // Move the line prefab to the midpoint position
        line.transform.position = new Vector3(midpointPosition, startY, 0f);

        // Create a unique material for the line renderer if it doesn't exist
        Material newLineMaterial = divisionMaterials.Count > 0 ? divisionMaterials[divisionMaterials.Count - 1] : new Material(lineMaterial);

        // Set the material of the line renderer
        Renderer lineRenderer = line.GetComponent<Renderer>();
        if (lineRenderer != null && newLineMaterial != null)
        {
            lineRenderer.material = newLineMaterial;
        }
        else
        {
            Debug.LogError("LineRenderer component or line material not found.");
        }

        // Determine the direction of movement for cubes
        Vector3 lineDirection = Vector3.right; // Assuming line is aligned with x-axis
        float tolerance = 0.001f; // Tolerance to avoid issues with floating point precision
        for (int i = 0; i < cubes.Length; i++)
        {
            Vector3 cubePosition = cubes[i].transform.position;
            Vector3 relativePosition = cubePosition - line.transform.position;
            float dotProduct = Vector3.Dot(relativePosition, lineDirection);

            // Check if the cube is within the tolerance of the line
            if (Mathf.Abs(dotProduct) <= tolerance)
            {
                // Change the color of the text of the cube to highlight it
                continue;
            }

            // Cube is significantly left or right of the line
            if (dotProduct < 0)
            {
                MoveCube(cubes[i], Vector3.left);
            }
            else
            {
                MoveCube(cubes[i], Vector3.right);
            }
        }
    }

    private void ChangeTextColor(GameObject cube, Color color)
    {
        // Assuming cube has a TextMeshProUGUI component, change its color
        TextMeshProUGUI textMesh = cube.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.color = color;
            Debug.Log("Text color changed successfully.");
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in the cube.");
        }
    }




    // Method to move a cube in a given direction
    private void MoveCube(GameObject cube, Vector3 direction)
    {
        cube.transform.Translate(direction * 0.05f, Space.World); // Adjust spacing as needed
    }

    // Clear all division lines
    private void ClearDivisionLines()
    {
        foreach (var line in divisionLines)
        {
            Destroy(line);
        }
        divisionLines.Clear();
        divisionMaterials.Clear();
    }

    private IEnumerator MergeCubesWithDelay(List<GameObject> sortedCubes)
    {
        yield return new WaitForSeconds(divisionDelay); // Add a delay before starting merging

        yield return StartCoroutine(MergeCubesRecursive(sortedCubes, 0, cubes.Take(cubes.Length / 2).ToArray(), cubes.Skip(cubes.Length / 2).ToArray())); // Start merging recursively
        Debug.Log("Merging completed!");
    }

    private IEnumerator MergeCubesRecursive(List<GameObject> sortedCubes, int startIndex, GameObject[] leftHalf, GameObject[] rightHalf)
    {
        yield return new WaitForSeconds(sortingDelay);
        GameObject currentCube;
        GameObject nextCube;
        GameObject firstcube;

        if (startIndex >= sortedCubes.Count - 1)
        {
            // All cubes are merged and sorted
            yield break;
        }

        // Start comparing from the first index and compare each pair of adjacent elements only once
        int lastComparedIndex = startIndex - 1;

        if (leftHalf.Length % 2 == 1)
        {

            for (int i = startIndex; i < sortedCubes.Count - 2; i++)
            {
                yield return new WaitForSeconds(sortingDelay);
                if (i <= lastComparedIndex)
                    continue;
                firstcube = sortedCubes[i];
                currentCube = sortedCubes[i + 1];
                nextCube = sortedCubes[i + 2];
                bool left = Array.IndexOf(leftHalf, currentCube) != -1 && Array.IndexOf(leftHalf, nextCube) != -1;

                // Only perform comparison and move if cubes are from the same halves
                if (left)
                {
                    // Highlight the cubes being compared
                    

                    // Compare the values of cubes
                    int currentValue = int.Parse(currentCube.GetComponentInChildren<TextMeshProUGUI>().text);
                    int nextValue = int.Parse(nextCube.GetComponentInChildren<TextMeshProUGUI>().text);
                    infoText.text = "Merge" + "(" + currentValue + "," + nextValue + ")";

                    // Determine the direction to move the cubes
                    Vector3 currentCubeTargetPosition = currentCube.transform.position;
                    Vector3 nextCubeTargetPosition = nextCube.transform.position;
                    Vector3 firstCubeTargetPosition = firstcube.transform.position;
                    if (currentValue > nextValue)
                    {
                        HighlightCube(firstcube);
                        firstCubeTargetPosition.y -= spacing*0.05f;
                        yield return StartCoroutine(SmoothMoveCube(firstcube, firstCubeTargetPosition));
                        yield return new WaitForSeconds(divisionDelay);
                        HighlightCubes(currentCube, nextCube);
                        nextCubeTargetPosition.x -= spacing*0.05f;
                        nextCubeTargetPosition.y -= spacing*0.05f;
                        yield return StartCoroutine(SmoothMoveCube(nextCube, nextCubeTargetPosition));
                        
                        currentCubeTargetPosition.x += spacing * 0.06f;
                        currentCubeTargetPosition.y -= spacing * 0.05f;
                        yield return StartCoroutine(SmoothMoveCube(currentCube, currentCubeTargetPosition));

                        // Update the position in leftHalf list
                        int currentCubeIndex = Array.IndexOf(leftHalf, currentCube);
                        int nextCubeIndex = Array.IndexOf(leftHalf, nextCube);
                        leftHalf[currentCubeIndex] = nextCube;
                        leftHalf[nextCubeIndex] = currentCube;
                    }
                    else
                    {
                        // If no swapping, just move the cubes down
                        currentCubeTargetPosition.y -= spacing*0.05f;
                        nextCubeTargetPosition.x -= spacing * 0.03f;
                        nextCubeTargetPosition.y -= spacing*0.05f;
                        firstCubeTargetPosition.y -= spacing*0.05f;
                        HighlightCube(firstcube);
                        yield return StartCoroutine(SmoothMoveCube(firstcube,firstCubeTargetPosition));
                        yield return new WaitForSeconds(divisionDelay);
                        HighlightCubes(currentCube, nextCube);

                        yield return StartCoroutine(SmoothMoveCube(currentCube, currentCubeTargetPosition));
                        yield return StartCoroutine(SmoothMoveCube(nextCube, nextCubeTargetPosition));
                    }

                    // Reset the color of the cubes after comparison
                    ResetCubeTextColor(currentCube);
                    ResetCubeTextColor(nextCube);

                    // Update the last compared index
                    lastComparedIndex = i + 1;
                }
            }
        }
        else
        {
            // Compare adjacent pairs of cubes from the same halves and merge if necessary
            for (int i = startIndex; i < sortedCubes.Count - 1; i++)
            {
                yield return new WaitForSeconds(sortingDelay);
                // Skip if the current index was already compared in the previous iteration
                if (i <= lastComparedIndex)
                    continue;

                currentCube = sortedCubes[i];
                nextCube = sortedCubes[i + 1];

                // Determine if the cubes are from the same halves
                bool left = Array.IndexOf(leftHalf, currentCube) != -1 && Array.IndexOf(leftHalf, nextCube) != -1;

                // Only perform comparison and move if cubes are from the same halves
                if (left)
                {
                    // Highlight the cubes being compared
                    HighlightCubes(currentCube, nextCube);

                    // Compare the values of cubes
                    
                    int currentValue = int.Parse(currentCube.GetComponentInChildren<TextMeshProUGUI>().text);
                    Debug.Log("Current" + " " + currentValue + "i" + " " + i);
                    int nextValue = int.Parse(nextCube.GetComponentInChildren<TextMeshProUGUI>().text);
                    infoText.text = "Merge" + "(" + currentValue + "," + nextValue + ")";

                    // Determine the direction to move the cubes
                    Vector3 currentCubeTargetPosition = currentCube.transform.position;
                    Vector3 nextCubeTargetPosition = nextCube.transform.position;

                    if (currentValue > nextValue)
                    {
                        HighlightCubes(currentCube, nextCube);
                        if(i==0)
                        nextCubeTargetPosition.x -= spacing * 0.06f;
                        else
                        nextCubeTargetPosition.x -= spacing * 0.04f;
                        nextCubeTargetPosition.y -= spacing * 0.05f;
                        yield return StartCoroutine(SmoothMoveCube(nextCube, nextCubeTargetPosition));

                        currentCubeTargetPosition.x += spacing * 0.07f;
                        currentCubeTargetPosition.y -= spacing * 0.05f;
                        yield return StartCoroutine(SmoothMoveCube(currentCube, currentCubeTargetPosition));

                        // Update the position in leftHalf list
                        int currentCubeIndex = Array.IndexOf(leftHalf, currentCube);
                        int nextCubeIndex = Array.IndexOf(leftHalf, nextCube);
                        leftHalf[currentCubeIndex] = nextCube;
                        leftHalf[nextCubeIndex] = currentCube;
                    }
                    else
                    {
                        // If no swapping, just move the cubes down
                        currentCubeTargetPosition.y -= spacing*0.05f;
                        if(i==0)
                        nextCubeTargetPosition.x -= spacing * 0.05f;
                        else
                        nextCubeTargetPosition.x -= spacing * 0.03f;
                        nextCubeTargetPosition.y -= spacing * 0.05f;
                        yield return new WaitForSeconds(divisionDelay);
                        HighlightCubes(currentCube, nextCube);

                        yield return StartCoroutine(SmoothMoveCube(currentCube, currentCubeTargetPosition));
                        yield return StartCoroutine(SmoothMoveCube(nextCube, nextCubeTargetPosition));
                    }

                    // Reset the color of the cubes after comparison
                    ResetCubeTextColor(currentCube);
                    ResetCubeTextColor(nextCube);

                    // Update the last compared index
                    lastComparedIndex = i + 1;
                }
            }
        }
        List<GameObject> leftHalfList = leftHalf.ToList();
        for (int i = 0; i < leftHalfList.Count; i++)
        {
            Debug.Log(leftHalfList[i].GetComponentInChildren<TextMeshProUGUI>().text + " ");
        }
        yield return StartCoroutine(GroupPairs(leftHalfList, startIndex, sortedAndGroupedLeft));

        int mid = sortedCubes.Count / 2;
        // Merge the right half after merging the left half
        yield return StartCoroutine(MergeRightHalf(sortedCubes, mid, rightHalf));
        yield return new WaitForSeconds(divisionDelay);
        for (int i = 0; i < sortedAndGroupedLeft.Count; i++)
        {
            Debug.Log("Sorted_Left" + sortedAndGroupedLeft[i].GetComponentInChildren<TextMeshProUGUI>().text);
            Debug.Log("Sorted_Right" + sortedAndGroupedRight[i].GetComponentInChildren<TextMeshProUGUI>().text);
        }

        yield return StartCoroutine(MergeSortedHalves(sortedAndGroupedLeft, sortedAndGroupedRight, finalSorted));
    }


    private IEnumerator MergeRightHalf(List<GameObject> sortedCubes, int startIndex, GameObject[] rightHalf)
    {
        yield return new WaitForSeconds(sortingDelay);
        GameObject currentCube;
        GameObject nextCube;
        GameObject firstCube;

        if (startIndex >= sortedCubes.Count - 1)
        {
            // All cubes are merged and sorted
            yield break;
        }
        bool swapped = false;

        // Start comparing from the first index and compare each pair of adjacent elements only once
        int lastComparedIndex = startIndex - 1;
        if (rightHalf.Length % 2 == 1)
        {
            // Compare adjacent pairs of cubes from the same halves and merge if necessary
            Debug.Log("Length:" + rightHalf.Length);
            for (int i = startIndex; i < sortedCubes.Count - 2; i++)
            {
                yield return new WaitForSeconds(sortingDelay);
                // Skip if the current index was already compared in the previous iteration
                if (i <= lastComparedIndex)
                    continue;

                currentCube = sortedCubes[i + 1];
                nextCube = sortedCubes[i + 2];
                firstCube = sortedCubes[i];

                // Determine if the cubes are from the same halves
                bool right = Array.IndexOf(rightHalf, currentCube) != -1 && Array.IndexOf(rightHalf, nextCube) != -1;

                // Only perform comparison and swap if cubes are from the same halves
                if (right)
                {
                    int currentValue = int.Parse(currentCube.GetComponentInChildren<TextMeshProUGUI>().text);
                    int nextValue = int.Parse(nextCube.GetComponentInChildren<TextMeshProUGUI>().text);
                    infoText.text = "Merge" + "(" + currentValue + "," + nextValue + ")";

                    // Determine the direction to move the cubes
                    Vector3 currentCubeTargetPosition = currentCube.transform.position;
                    Vector3 nextCubeTargetPosition = nextCube.transform.position;
                    Vector3 firstCubeTargetPosition = firstCube.transform.position;

                    // If the current value is greater than the next value, swap their positions
                    if (currentValue > nextValue)
                    {
                        HighlightCube(firstCube);
                        firstCubeTargetPosition.y -= spacing * 0.05f;
                        yield return StartCoroutine(SmoothMoveCube(firstCube, firstCubeTargetPosition));
                        yield return new WaitForSeconds(divisionDelay);
                        HighlightCubes(currentCube, nextCube);
                        nextCubeTargetPosition.x -= spacing * 0.05f;
                        nextCubeTargetPosition.y -= spacing * 0.05f;
                        yield return StartCoroutine(SmoothMoveCube(nextCube, nextCubeTargetPosition));

                        currentCubeTargetPosition.x += spacing * 0.06f;
                        currentCubeTargetPosition.y -= spacing * 0.05f;
                        yield return StartCoroutine(SmoothMoveCube(currentCube, currentCubeTargetPosition));

                        // Update the position in leftHalf list
                        int currentCubeIndex = Array.IndexOf(rightHalf, currentCube);
                        int nextCubeIndex = Array.IndexOf(rightHalf, nextCube);
                        rightHalf[currentCubeIndex] = nextCube;
                        rightHalf[nextCubeIndex] = currentCube;
                    }
                    else
                    {
                        currentCubeTargetPosition.y -= spacing * 0.05f;
                        nextCubeTargetPosition.x -= spacing * 0.02f;
                        nextCubeTargetPosition.y -= spacing * 0.05f;
                        firstCubeTargetPosition.y -= spacing * 0.05f;
                        HighlightCube(firstCube);
                        yield return StartCoroutine(SmoothMoveCube(firstCube, firstCubeTargetPosition));
                        yield return new WaitForSeconds(divisionDelay);
                        HighlightCubes(currentCube, nextCube);

                        yield return StartCoroutine(SmoothMoveCube(currentCube, currentCubeTargetPosition));
                        yield return StartCoroutine(SmoothMoveCube(nextCube, nextCubeTargetPosition));
                    }

                    ResetCubeTextColor(currentCube);
                    ResetCubeTextColor(nextCube);
                    // Reset the color of the cubes after comparison

                    // Update the last compared index
                    lastComparedIndex = i + 1;
                }
            }
        }
        else
        {
            // Compare adjacent pairs of cubes from the same halves and merge if necessary
            for (int i = startIndex; i < sortedCubes.Count - 1; i++)
            {
                yield return new WaitForSeconds(sortingDelay);
                // Skip if the current index was already compared in the previous iteration
                if (i <= lastComparedIndex)
                    continue;

                currentCube = sortedCubes[i];
                nextCube = sortedCubes[i + 1];

                // Determine if the cubes are from the same halves
                bool right = Array.IndexOf(rightHalf, currentCube) != -1 && Array.IndexOf(rightHalf, nextCube) != -1;

                // Only perform comparison and swap if cubes are from the same halves
                if (right)
                {
                    // Highlight the cubes being compared
                    HighlightCubes(currentCube, nextCube);

                    int currentValue = int.Parse(currentCube.GetComponentInChildren<TextMeshProUGUI>().text);
                    Debug.Log("Current" + " " + currentValue + "i" + " " + i);
                    int nextValue = int.Parse(nextCube.GetComponentInChildren<TextMeshProUGUI>().text);
                    infoText.text = "Merge" + "(" + currentValue + "," + nextValue + ")";

                    // Determine the direction to move the cubes
                    Vector3 currentCubeTargetPosition = currentCube.transform.position;
                    Vector3 nextCubeTargetPosition = nextCube.transform.position;

                    // If the current value is greater than the next value, swap their positions
                    if (currentValue > nextValue)
                    {
                        HighlightCubes(currentCube, nextCube);
                        if(i==4)
                        nextCubeTargetPosition.x -= spacing * 0.04f;
                        else
                        nextCubeTargetPosition.x -= spacing * 0.04f;
                        nextCubeTargetPosition.y -= spacing * 0.05f;
                        yield return StartCoroutine(SmoothMoveCube(nextCube, nextCubeTargetPosition));

                        currentCubeTargetPosition.x += spacing * 0.07f;
                        currentCubeTargetPosition.y -= spacing * 0.05f;
                        yield return StartCoroutine(SmoothMoveCube(currentCube, currentCubeTargetPosition));

                        // Update the position in leftHalf list
                        int currentCubeIndex = Array.IndexOf(rightHalf, currentCube);
                        int nextCubeIndex = Array.IndexOf(rightHalf, nextCube);
                        rightHalf[currentCubeIndex] = nextCube;
                        rightHalf[nextCubeIndex] = currentCube;
                    }
                    else
                    {
                        currentCubeTargetPosition.y -= spacing * 0.05f;
                        if (i == 4)
                            nextCubeTargetPosition.x -= spacing * 0.05f;
                        else
                            nextCubeTargetPosition.x -= spacing * 0.03f;
                        nextCubeTargetPosition.y -= spacing * 0.05f;
                        yield return new WaitForSeconds(divisionDelay);
                        HighlightCubes(currentCube, nextCube);

                        yield return StartCoroutine(SmoothMoveCube(currentCube, currentCubeTargetPosition));
                        yield return StartCoroutine(SmoothMoveCube(nextCube, nextCubeTargetPosition));
                    }

                    ResetCubeTextColor(currentCube);
                    ResetCubeTextColor(nextCube);
                    // Reset the color of the cubes after comparison

                    // Update the last compared index
                    lastComparedIndex = i + 1;
                }
            }
        }

        List<GameObject> rightHalfList = rightHalf.ToList();
        yield return StartCoroutine(GroupPairs(rightHalfList, 0, sortedAndGroupedRight));

        // Merge the left half after merging the right half
    }

    private IEnumerator GroupPairs(List<GameObject> sortedCubes, int startIndex, List<GameObject> sortedAndGrouped)
    {
        yield return new WaitForSeconds(sortingDelay);
        float small = 0.05f;
        float large = 0.07f;

        if (startIndex + 3 < sortedCubes.Count)
        {
            GameObject cube1 = sortedCubes[startIndex];
            GameObject cube2 = sortedCubes[startIndex + 1];
            GameObject cube3 = sortedCubes[startIndex + 2];
            GameObject cube4 = sortedCubes[startIndex + 3];

            // Get integer values from the cube's text components
            int value1Cube1 = int.Parse(cube1.GetComponentInChildren<TextMeshProUGUI>().text);
            int value2Cube2 = int.Parse(cube2.GetComponentInChildren<TextMeshProUGUI>().text);
            int value1Cube3 = int.Parse(cube3.GetComponentInChildren<TextMeshProUGUI>().text);
            int value2Cube4 = int.Parse(cube4.GetComponentInChildren<TextMeshProUGUI>().text);
            Debug.Log(value1Cube1 + " " + value2Cube2 + " " + value1Cube3 + " " + value2Cube4);
            Vector3 cube1Position = cube1.transform.position;
            Vector3 cube2Position = cube2.transform.position;
            Vector3 cube3Position = cube2.transform.position + Vector3.right * 4f * small;
            Vector3 cube4Position = cube3Position + Vector3.right * 4f * small;


            yield return new WaitForSeconds(sortingDelay);
            int i = 0, j = 0;
            // Compare elements of pairs and swap if necessary
            if (value1Cube1 > value1Cube3)
            {
                sortedAndGrouped.Add(cube3);
                j++;
                HighlightCubes(cube1, cube3);
                Vector3 newPosition = cube1Position - Vector3.up * large* spacing;
                yield return StartCoroutine(SmoothMoveCubeGroups(cube3, cube3.transform.position, newPosition));
                ResetCubeTextColor(cube1);
                ResetCubeTextColor(cube3);
                // yield return StartCoroutine(SmoothSwapCubePositions(cube1, cube3));
            }
            else
            {
                sortedAndGrouped.Add(cube1);
                i++;
                HighlightCubes(cube1, cube3);
                Vector3 newPosition = cube1Position - Vector3.up *large* spacing;
                yield return StartCoroutine(SmoothMoveCubeGroups(cube1, cube1.transform.position, newPosition));
                ResetCubeTextColor(cube1);
                ResetCubeTextColor(cube3);
            }

            if (i == 1)
            {
                HighlightCubes(cube2, cube3);
                if (value2Cube2 > value1Cube3)
                {
                    sortedAndGrouped.Add(cube3);
                    j++;
                    Vector3 newPosition = cube2Position - Vector3.up * large * spacing;
                    yield return StartCoroutine(SmoothMoveCubeGroups(cube3, cube3.transform.position, newPosition));
                }
                else
                {
                    sortedAndGrouped.Add(cube2);
                    i++;
                    Vector3 newPosition = cube2Position - Vector3.up * large* spacing;
                    yield return StartCoroutine(SmoothMoveCubeGroups(cube2, cube2.transform.position, newPosition));
                }
                ResetCubeTextColor(cube2);
                ResetCubeTextColor(cube3);
            }
            else if (j == 1)
            {
                HighlightCubes(cube1, cube4);
                if (value1Cube1 > value2Cube4)
                {
                    sortedAndGrouped.Add(cube4);
                    j++;
                    Vector3 newPosition = cube2Position - Vector3.up * large* spacing;
                    yield return StartCoroutine(SmoothMoveCubeGroups(cube4, cube4.transform.position, newPosition));
                }
                else
                {
                    sortedAndGrouped.Add(cube1);
                    i++;
                    Vector3 newPosition = cube2Position - Vector3.up * large * spacing;
                    yield return StartCoroutine(SmoothMoveCubeGroups(cube1, cube1.transform.position, newPosition));
                }
                ResetCubeTextColor(cube1);
                ResetCubeTextColor(cube4);
            }
            if (i == 1 && j == 1)
            {
                HighlightCubes(cube2, cube4);
                if (value2Cube2 > value2Cube4)
                {
                    sortedAndGrouped.Add(cube4);
                    j++;
                    Vector3 cube2new = cube4Position - Vector3.up * large * spacing;
                    Vector3 cube4new = cube3Position - Vector3.up * large * spacing;
                    yield return StartCoroutine(SmoothMoveCubeGroups(cube4, cube4.transform.position, cube4new));
                    yield return StartCoroutine(SmoothMoveCubeGroups(cube2, cube2.transform.position, cube2new));

                }
                else
                {
                    i++;
                    sortedAndGrouped.Add(cube2);
                    Vector3 cube2new = cube3Position - Vector3.up * large * spacing;
                    Vector3 cube4new = cube4Position - Vector3.up * large * spacing;
                    yield return StartCoroutine(SmoothMoveCubeGroups(cube2, cube2.transform.position, cube2new));
                    yield return StartCoroutine(SmoothMoveCubeGroups(cube4, cube4.transform.position, cube4new));

                }
                ResetCubeTextColor(cube2);
                ResetCubeTextColor(cube4);
            }

            if (i == 2 && j == 1)
            {
                sortedAndGrouped.Add(cube4);
                Vector3 cube4new = cube4Position - Vector3.up * large * spacing;
                yield return StartCoroutine(SmoothMoveCubeGroups(cube4, cube4.transform.position, cube4new));
            }
            else if (j == 2 && i == 1)
            {
                sortedAndGrouped.Add(cube2);
                Vector3 cube4new = cube4Position - Vector3.up * large* spacing;
                yield return StartCoroutine(SmoothMoveCubeGroups(cube2, cube2.transform.position, cube4new));
            }
            else if (j == 2)
            {
                sortedAndGrouped.Add(cube1);
                sortedAndGrouped.Add(cube2);
                Vector3 cube1new = cube3Position - Vector3.up * large* spacing;
                Vector3 cube2new = cube4Position - Vector3.up * large * spacing;
                yield return StartCoroutine(SmoothMoveCubeGroups(cube1, cube1.transform.position, cube1new));
                yield return StartCoroutine(SmoothMoveCubeGroups(cube2, cube2.transform.position, cube2new));
                ResetCubeTextColor(cube1);
                ResetCubeTextColor(cube2);
            }
            else if (i == 2)
            {
                sortedAndGrouped.Add(cube3);
                sortedAndGrouped.Add(cube4);
                Vector3 cube3new = cube3Position - Vector3.up * large * spacing;
                Vector3 cube4new = cube4Position - Vector3.up * large * spacing;
                yield return StartCoroutine(SmoothMoveCubeGroups(cube3, cube3.transform.position, cube3new));
                yield return StartCoroutine(SmoothMoveCubeGroups(cube4, cube4.transform.position, cube4new));
                ResetCubeTextColor(cube3);
                ResetCubeTextColor(cube4);
            }
            // Reset cube colors

        }
        // Check if there are only three cubes
        // Check if there are only three cubes
        else if (startIndex + 2 < sortedCubes.Count)
        {
            GameObject cube1 = sortedCubes[startIndex];
            GameObject cube2 = sortedCubes[startIndex + 1];
            GameObject cube3 = sortedCubes[startIndex + 2];

            // Get integer values from the cube's text components
            int value1Cube1 = int.Parse(cube1.GetComponentInChildren<TextMeshProUGUI>().text);
            int value2Cube2 = int.Parse(cube2.GetComponentInChildren<TextMeshProUGUI>().text);
            int value1Cube3 = int.Parse(cube3.GetComponentInChildren<TextMeshProUGUI>().text);

            Debug.Log(value1Cube1 + " " + value2Cube2 + " " + value1Cube3);
            Vector3 cube1Position = cube1.transform.position;
            Vector3 cube2Position = cube1.transform.position + Vector3.right * 4f * small;
            Vector3 cube3Position = cube2Position + Vector3.right * 4f * small;


            int j = 0, i = 0;
            if (value1Cube1 > value2Cube2)
            {
                sortedAndGrouped.Add(cube2);
                j++;
                HighlightCubes(cube1, cube2);
                Vector3 cube2new = cube1Position - Vector3.up * spacing * small;
                yield return StartCoroutine(SmoothMoveCubeGroups(cube2, cube2.transform.position, cube2new));
                ResetCubeTextColor(cube1);
                ResetCubeTextColor(cube2);

            }
            else
            {
                sortedAndGrouped.Add(cube1);
                i++;
                HighlightCubes(cube1, cube2);
                Vector3 cube1new = cube1Position - Vector3.up * spacing * small;
                yield return StartCoroutine(SmoothMoveCubeGroups(cube1, cube1.transform.position, cube1new));
                ResetCubeTextColor(cube1);
                ResetCubeTextColor(cube2);
            }
            if (j == 1)
            {
                if (value1Cube1 > value1Cube3)
                {
                    sortedAndGrouped.Add(cube3);
                    j++;
                    HighlightCubes(cube1, cube3);
                    Vector3 cube3new = cube2Position - Vector3.up * spacing * small;
                    yield return StartCoroutine(SmoothMoveCubeGroups(cube3, cube3.transform.position, cube3new));
                    ResetCubeTextColor(cube1);
                    ResetCubeTextColor(cube3);

                }
                else
                {
                    sortedAndGrouped.Add(cube1);
                    i++;
                    HighlightCubes(cube1, cube3);
                    Vector3 cube1new = cube2Position - Vector3.up * spacing * small;
                    yield return StartCoroutine(SmoothMoveCubeGroups(cube1, cube1.transform.position, cube1new));
                    ResetCubeTextColor(cube1);
                    ResetCubeTextColor(cube3);
                }
            }
            else if (i == 1)
            {
                sortedAndGrouped.Add(cube2);
                sortedAndGrouped.Add(cube3);
                HighlightCubes(cube2, cube3);
                Vector3 cube2new = cube2Position - Vector3.up * spacing * small;
                Vector3 cube3new = cube3Position - Vector3.up * spacing * small;
                yield return StartCoroutine(SmoothMoveCubeGroups(cube2, cube2.transform.position, cube2new));
                yield return StartCoroutine(SmoothMoveCubeGroups(cube3, cube3.transform.position, cube3new));
                ResetCubeTextColor(cube2);
                ResetCubeTextColor(cube3);
            }
            if (j == 2)
            {
                sortedAndGrouped.Add(cube1);
                Vector3 cube1new = cube3Position - Vector3.up * small * spacing;
                yield return StartCoroutine(SmoothMoveCubeGroups(cube1, cube1.transform.position, cube1new));
            }
            else if (i == 1 && j == 1)
            {
                sortedAndGrouped.Add(cube3);
                Vector3 cube3new = cube3Position - Vector3.up * small * spacing;
                yield return StartCoroutine(SmoothMoveCubeGroups(cube3, cube3.transform.position, cube3new));
            }
            // Reset cube colors

        }
        else if (startIndex + 1 < sortedCubes.Count)
        {
            GameObject cube1 = sortedCubes[startIndex];
            GameObject cube2 = sortedCubes[startIndex + 1];
            Vector3 cube1Position = cube1.transform.position;
            Vector3 cube2Position = cube1.transform.position + Vector3.right * 4f * small;
            sortedAndGrouped.Add(cube1);
            sortedAndGrouped.Add(cube2);
            Vector3 cube1new = cube1Position - Vector3.up * spacing * small;
            Vector3 cube2new = cube2Position - Vector3.up * spacing * small;
            yield return StartCoroutine(SmoothMoveCubeGroups(cube1, cube1.transform.position, cube1new));
            yield return StartCoroutine(SmoothMoveCubeGroups(cube2, cube2.transform.position, cube2new));
        }
        else
        {
            GameObject cube1 = sortedCubes[startIndex];
            Vector3 cube1Position = cube1.transform.position;
            sortedAndGrouped.Add(cube1);
            Vector3 cube1new = cube1Position - Vector3.up * spacing * small;
            yield return StartCoroutine(SmoothMoveCubeGroups(cube1, cube1.transform.position, cube1new));


        }
        // Check if there are only two cubes


    }



    private IEnumerator MergeSortedHalves(List<GameObject> sortedAndGroupedLeft, List<GameObject> sortedAndGroupedRight, List<GameObject> finalSorted)
    {
        /*
         * 
        for (int i = 0; i < sortedAndGroupedLeft.Count; i++)
        {
            Debug.Log("Sorted_Left" + sortedAndGroupedLeft[i].GetComponentInChildren<TextMeshProUGUI>().text);
            Debug.Log("Sorted_Right" + sortedAndGroupedRight[i].GetComponentInChildren<TextMeshProUGUI>().text);
        }
        */

        int leftIndex = 0;
        int rightIndex = 0;
        float yOffset = -0.07f * spacing; // Adjust this value as needed for the desired spacing between rows

        // Get the position of the first cube and calculate the initial position for arranging cubes in a horizontal line
        Vector3 newPosition = sortedAndGroupedLeft[0].transform.position;

        // Variable to keep track of the horizontal position of cubes
        float horizontalPosition = 0;

        while (leftIndex < sortedAndGroupedLeft.Count && rightIndex < sortedAndGroupedRight.Count)
        {
            GameObject leftCube = sortedAndGroupedLeft[leftIndex];
            GameObject rightCube = sortedAndGroupedRight[rightIndex];

            int leftValue = int.Parse(leftCube.GetComponentInChildren<TextMeshProUGUI>().text);
            int rightValue = int.Parse(rightCube.GetComponentInChildren<TextMeshProUGUI>().text);
            HighlightCubes(leftCube, rightCube);
            if (leftValue <= rightValue)
            {
                // Move only the cube from the left side since it's smaller or equal
                finalSorted.Add(leftCube);
                leftIndex++;

                // Move the left cube to its new position in a horizontal line
                Vector3 leftCubeNewPosition = newPosition + Vector3.right * horizontalPosition;
                leftCubeNewPosition.y += yOffset;
                yield return StartCoroutine(SmoothMoveCubeGroups(leftCube, leftCube.transform.position, leftCubeNewPosition));

            }
            else
            {
                // Move only the cube from the right side since it's smaller
                finalSorted.Add(rightCube);
                rightIndex++;

                // Move the right cube to its new position in a horizontal line
                Vector3 rightCubeNewPosition = newPosition + Vector3.right * horizontalPosition;
                rightCubeNewPosition.y += yOffset;
                yield return StartCoroutine(SmoothMoveCubeGroups(rightCube, rightCube.transform.position, rightCubeNewPosition));

            }
            ResetCubeTextColor(leftCube);
            ResetCubeTextColor(rightCube);
            // Increment the horizontal position for the next cube
            horizontalPosition += 0.05f * 2f;
        }

        // Add remaining cubes from left half, if any
        while (leftIndex < sortedAndGroupedLeft.Count)
        {
            GameObject leftCube = sortedAndGroupedLeft[leftIndex];
            finalSorted.Add(leftCube);
            leftIndex++;

            // Move cube to its new position in a horizontal line
            Vector3 leftCubeNewPosition = newPosition + Vector3.right * horizontalPosition;
            leftCubeNewPosition.y += yOffset;
            yield return StartCoroutine(SmoothMoveCubeGroups(leftCube, leftCube.transform.position, leftCubeNewPosition));

            // Increment the horizontal position for the next cube
            horizontalPosition += 0.1f * spacing;
        }

        // Add remaining cubes from right half, if any
        while (rightIndex < sortedAndGroupedRight.Count)
        {
            GameObject rightCube = sortedAndGroupedRight[rightIndex];
            finalSorted.Add(rightCube);
            rightIndex++;

            // Move cube to its new position in a horizontal line
            Vector3 rightCubeNewPosition = newPosition + Vector3.right * horizontalPosition;
            rightCubeNewPosition.y += yOffset;
            yield return StartCoroutine(SmoothMoveCubeGroups(rightCube, rightCube.transform.position, rightCubeNewPosition));

            // Increment the horizontal position for the next cube
            horizontalPosition += 0.05f * spacing;
        }
        Debug.Log("Right" + rightIndex);
        Debug.Log("Left" + leftIndex);
        for (int i = 0; i < finalSorted.Count(); i++)
        {
            Debug.Log("Final" + i + " " + finalSorted[i].GetComponentInChildren<TextMeshProUGUI>().text);
        }
        yield return null; // Or any appropriate yield statement
    }



    private void HighlightCube(GameObject cube)
    {
        TextMeshProUGUI text = cube.GetComponentInChildren<TextMeshProUGUI>();

        if (text!= null)
            text.color = comparisonColor;
    }

    private IEnumerator SmoothMoveCubeGroups(GameObject cube, Vector3 startPosition, Vector3 targetPosition)
    {
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / (swapSpeed * 0.05f);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            cube.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        cube.transform.position = targetPosition; // Ensure final position is exact
    }

    private IEnumerator SmoothMoveCube(GameObject cube, Vector3 targetPosition)
    {
        Vector3 startPosition = cube.transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        Debug.Log("Distane"+distance);
        float duration = distance / swapSpeed*0.1f; // Adjust duration based on distance and speed

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            cube.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        cube.transform.position = targetPosition; // Ensure final position is exact
    }
    private void HighlightCubes(GameObject cube1, GameObject cube2)
    {
        TextMeshProUGUI text1 = cube1.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI text2 = cube2.GetComponentInChildren<TextMeshProUGUI>();

        if (text1 != null)
            text1.color = comparisonColor;

        if (text2 != null)
            text2.color = comparisonColor;
    }

    // Method to reset the text color of cubes after comparison
    private void ResetCubeTextColor(GameObject cube)
    {
        TextMeshProUGUI textMesh = cube.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.color = textColor;
        }
    }

    private IEnumerator SmoothSwapCubePositions(GameObject cube1, GameObject cube2)
    {
        Vector3 startPositionCube1 = cube1.transform.position;
        Vector3 startPositionCube2 = cube2.transform.position;

        float duration = 1f; // Adjust the duration to control the speed of swapping

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            cube1.transform.position = Vector3.Lerp(startPositionCube1, startPositionCube2, t);
            cube2.transform.position = Vector3.Lerp(startPositionCube2, startPositionCube1, t);

            yield return null;
        }

        // Ensure final positions are exact
        cube1.transform.position = startPositionCube2;
        cube2.transform.position = startPositionCube1;
    }

}
