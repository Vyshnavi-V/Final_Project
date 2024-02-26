using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class Merge : MonoBehaviour
{
    public GameObject cubePrefab;
    public TMP_InputField inputField;
    public float spacing = 2f;
    public Color textColor = Color.white;
    public Color comparisonColor = Color.yellow; // Color for cubes being compared
    public float sortingDelay = 2f; // Delay before starting the sorting process
    public float swapSpeed = 12f;
    public GameObject linePrefab;
    public float divisionDelay = 2f; // Delay between each division
    public Material lineMaterial; // Material for the line renderer

    private GameObject[] cubes;
    private List<Vector3[]> divisionPositions = new List<Vector3[]>();
    private List<GameObject> divisionLines = new List<GameObject>(); // Store references to division lines
    private List<Material> divisionMaterials = new List<Material>(); // Store references to division materials

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
        float startX = -totalWidth / 2f;
        float currentX = startX;
        float startY = 0f;

        cubes = new GameObject[numbers.Length];
        Vector3[] cubePositions = new Vector3[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            Vector3 cubePosition = new Vector3(currentX, startY, 0f);
            cubePositions[i] = cubePosition;

            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
            currentX += spacing * 2;

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
                    float fontSizeMultiplier = 0.05f;
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

        divisionPositions.Add(cubePositions);

        // Start recursive division with delay
        StartCoroutine(DivideCubesWithDelay(cubes));
    }

    // Coroutine for recursive division with delay
    private IEnumerator DivideCubesWithDelay(GameObject[] cubesToDivide)
    {
        yield return new WaitForSeconds(sortingDelay);
        DivideCubesRecursive(cubesToDivide, 0, cubesToDivide.Length, 0);
        yield return new WaitForSeconds(sortingDelay);
        // After the last recursion, record positions and start merging
        List<GameObject> sortedCubes = new List<GameObject>(cubesToDivide.OrderBy(cube => cube.transform.position.x));
        yield return new WaitForSeconds(sortingDelay);
        StartCoroutine(MergeCubesWithDelay(sortedCubes));
    }


    // Recursive method to divide the cubes into halves
    private void DivideCubesRecursive(GameObject[] cubesToDivide, int startIndex, int length, float startY)
    {
        if (length <= 1)
        {
            return;
        }
        startY = startY - (spacing * 2);
        // Calculate the midpoint index
        int midpointIndex = startIndex + length / 2;

        // Divide the array into left and right halves
        GameObject[] leftHalf = cubesToDivide.Skip(startIndex).Take(midpointIndex - startIndex).ToArray();
        GameObject[] rightHalf = cubesToDivide.Skip(midpointIndex).Take(length - (midpointIndex - startIndex)).ToArray();

        // Visualize the division
        VisualizeDivision(leftHalf, rightHalf, startY);

        // Recursively divide the left and right halves
        float nextStartY = startY - (spacing); // Increment startY for next division
        StartCoroutine(DelayedDivision(leftHalf, rightHalf, nextStartY));
    }

    // Coroutine for delaying division
    private IEnumerator DelayedDivision(GameObject[] leftHalf, GameObject[] rightHalf, float startY)
    {
        yield return new WaitForSeconds(divisionDelay);
        DivideCubesRecursive(leftHalf, 0, leftHalf.Length, startY);
        DivideCubesRecursive(rightHalf, 0, rightHalf.Length, startY);
    }

    // Method to visualize the division of cubes

    // Method to visualize the division of cubes
    // Method to visualize the division of cubes
    // Method to visualize the division of cubes
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
        for (int i = 0; i < cubes.Length; i++)
        {
            Vector3 cubePosition = cubes[i].transform.position;
            Vector3 relativePosition = cubePosition - line.transform.position;
            float dotProduct = Vector3.Dot(relativePosition, lineDirection);
            if (dotProduct < 0) // Cube is left of the line
            {
                MoveCube(cubes[i], Vector3.left);
            }
            else // Cube is right of the line
            {
                MoveCube(cubes[i], Vector3.right);
            }
        }
    }

    // Method to move a cube in a given direction
    private void MoveCube(GameObject cube, Vector3 direction)
    {
        cube.transform.Translate(direction * spacing, Space.World); // Adjust spacing as needed
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
    private IEnumerator GroupIntoSetsOfFour(List<GameObject> sortedCubes)
    {
        int startIndex = 0;
        while (startIndex < sortedCubes.Count - 3)
        {
            yield return StartCoroutine(GroupPairs(sortedCubes, startIndex));
            startIndex += 4;
        }
    }

    private IEnumerator GroupPairs(List<GameObject> sortedCubes, int startIndex)
    {
        GameObject cube1 = sortedCubes[startIndex];
        GameObject cube2 = sortedCubes[startIndex + 1];
        GameObject cube3 = sortedCubes[startIndex + 2];
        GameObject cube4 = sortedCubes[startIndex + 3];


        yield return new WaitForSeconds(divisionDelay);

        // Get integer values from the cube's text components
        int value1Cube1 = int.Parse(cube1.GetComponentInChildren<TextMeshProUGUI>().text);
        int value2Cube2 = int.Parse(cube2.GetComponentInChildren<TextMeshProUGUI>().text);
        int value1Cube3 = int.Parse(cube3.GetComponentInChildren<TextMeshProUGUI>().text);
        int value2Cube4 = int.Parse(cube4.GetComponentInChildren<TextMeshProUGUI>().text);
        Debug.Log(value1Cube1 + " " + value2Cube2 + " " + value1Cube3 + " " + value2Cube4);

        Vector3 targetPosition1 = cube2.transform.position + Vector3.right * spacing * 2;
        cube3.transform.position = targetPosition1;
        Vector3 targetPosition2 = cube3.transform.position + Vector3.right * spacing * 2;
        cube4.transform.position = targetPosition2;
        yield return StartCoroutine(SmoothMoveCubeGroups(cube3, cube3.transform.position, targetPosition1));
        yield return StartCoroutine(SmoothMoveCubeGroups(cube4, cube4.transform.position, targetPosition2));
        yield return new WaitForSeconds(sortingDelay);
        // Compare element 1 of both pairs

        if (value1Cube1 > value1Cube3)
        {
            //SwapCubePositions(cube1, cube3);
            HighlightCubes(cube1, cube3);

            yield return StartCoroutine(SmoothSwapCubePositions(cube1, cube3));
        }

        else
        {
            if (value2Cube2 > value1Cube3)
            {
                //SwapCubePositions(cube2, cube3);
                HighlightCubes(cube2, cube3);
                yield return StartCoroutine(SmoothSwapCubePositions(cube2, cube3));
            }

        }

        // Compare element 2 of the first pair with element 1 of the second pair
        if (value2Cube2 > value2Cube4)
        {
            HighlightCubes(cube2, cube4);
            yield return StartCoroutine(SmoothSwapCubePositions(cube2, cube4));

        }

        if (value2Cube2 > value1Cube1 && cube2.transform.position.x < cube1.transform.position.x)
        {
            HighlightCubes(cube2, cube1);
            yield return StartCoroutine(SmoothSwapCubePositions(cube2, cube1));
        }


        /*
        else if (value1Cube3 < value2Cube2 && value1Cube3 < value2Cube4)
        {
            // Case: cube3 < cube1 < cube4 < cube2
            SwapCubePositions(cube1, cube3);
            SwapCubePositions(cube2, cube4);
            yield return StartCoroutine(SmoothSwapCubePositions(cube1, cube3));
            yield return StartCoroutine(SmoothSwapCubePositions(cube2, cube4));
        }
        else if (value1Cube3 < value2Cube2 && value1Cube3 > value2Cube4)
        {
            // Case: cube3 < cube1 < cube2 < cube4
            SwapCubePositions(cube1, cube3);
            SwapCubePositions(cube2, cube4);
            SwapCubePositions(cube3, cube4);
            yield return StartCoroutine(SmoothSwapCubePositions(cube1, cube3));
            yield return StartCoroutine(SmoothSwapCubePositions(cube2, cube4));
            yield return StartCoroutine(SmoothSwapCubePositions(cube3, cube4));
        }

        // Move cube3 and cube4 just after cube2

        */



        // Reset cube colors
        ResetCubeTextColor(cube1);
        ResetCubeTextColor(cube2);
        ResetCubeTextColor(cube3);
        ResetCubeTextColor(cube4);
    }




    private IEnumerator SmoothMoveCubeGroups(GameObject cube, Vector3 startPosition, Vector3 targetPosition)
    {
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / swapSpeed;

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
    private IEnumerator MergeCubesWithDelay(List<GameObject> sortedCubes)
    {
        yield return new WaitForSeconds(divisionDelay); // Add a delay before starting merging

        yield return StartCoroutine(MergeCubesRecursive(sortedCubes, 0)); // Start merging recursively
        Debug.Log("Merging completed!");

        yield return new WaitForSeconds(divisionDelay);
        yield return StartCoroutine(GroupIntoSetsOfFour(sortedCubes));
    }

    private IEnumerator MergeCubesRecursive(List<GameObject> sortedCubes, int startIndex)
    {
        if (startIndex >= sortedCubes.Count - 1)
        {
            // All cubes are merged and sorted
            yield break;
        }

        bool swapped = false; // Flag to indicate if any swap occurred in this iteration

        // Compare adjacent pairs of cubes and merge if necessary
        for (int i = startIndex; i < sortedCubes.Count - 1; i += 2)
        {
            GameObject currentCube = sortedCubes[i];
            GameObject nextCube = sortedCubes[i + 1];

            // Highlight the cubes being compared
            HighlightCubes(currentCube, nextCube);

            // Compare the values of cubes
            int currentValue = int.Parse(currentCube.GetComponentInChildren<TextMeshProUGUI>().text);
            int nextValue = int.Parse(nextCube.GetComponentInChildren<TextMeshProUGUI>().text);

            // If the current value is greater than the next value, swap their positions
            if (currentValue > nextValue)
            {
                // Move the next cube next to the current cube
                Vector3 newPosition = currentCube.transform.position;
                newPosition.x += spacing * 2;
                nextCube.transform.position = newPosition;

                // Swap cubes in the sorted list
                sortedCubes[i] = nextCube;
                sortedCubes[i + 1] = currentCube;

                // Visualize the swapping operation
                yield return StartCoroutine(SmoothSwapCubePositions(currentCube, nextCube));
                yield return new WaitForSeconds(divisionDelay);

                swapped = true; // Set flag indicating a swap occurred
            }
            else
            {
                // If no swap occurs, move the right cube towards the left cube by spacing distance
                Vector3 newPosition = currentCube.transform.position;
                newPosition.x += spacing * 2;
                nextCube.transform.position = newPosition;

                // Visualize the movement
                yield return StartCoroutine(SmoothMoveCube(nextCube, newPosition));
                yield return new WaitForSeconds(divisionDelay);
            }

            // Reset the color of the cubes after comparison
            ResetCubeTextColor(currentCube);
            ResetCubeTextColor(nextCube);
        }

        // If no swap occurred in this iteration, move the comparison window
        if (!swapped)
        {
            yield return null; // This effectively moves the window along the list without performing any swap
        }

        // Recur for the next set of adjacent pairs
        yield return StartCoroutine(MergeCubesRecursive(sortedCubes, startIndex + 2));
    }


    private IEnumerator SmoothMoveCube(GameObject cube, Vector3 targetPosition)
    {
        Vector3 startPosition = cube.transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / swapSpeed; // Adjust duration based on distance and speed

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


    private void SwapCubePositions(GameObject cube1, GameObject cube2)
    {
        Vector3 tempPosition = cube1.transform.position;
        cube1.transform.position = cube2.transform.position;
        cube2.transform.position = tempPosition;
    }

}


