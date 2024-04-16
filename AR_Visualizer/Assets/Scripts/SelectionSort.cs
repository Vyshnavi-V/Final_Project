using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SelectionSort : MonoBehaviour
{
    public GameObject cubePrefab;
    public TMP_InputField inputField;
    public GameObject inputCanvas;
    public Camera mainCamera;
    public float spacing = 5f;
    public Color textColor = Color.white;
    public Color sortedColor = Color.green; // Color for sorted cubes
    public Color smallestColor = Color.red; // Color for the smallest element
    public float sortingDelay = 1f; // Delay before starting the sorting process
    public float swapSpeed = 12f;

    private GameObject[] cubes;
    private bool sortingInProgress = false;
    private bool paused = false;
    public TextMeshProUGUI iterationText;

    private void Start()
    {
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

        inputField.text = string.Join(",", randomNumbers); // Sets the input field text to the generated random numbers
        GenerateCubes(); // Calls the existing method to generate and sort the cubes
    }

    public void GenerateCubes()
    {
        inputCanvas.SetActive(false);
        if (sortingInProgress)
        {
            // If sorting is already in progress, ignore the button click
            return;
        }

        sortingInProgress = true; // Set flag to indicate sorting is in progress

        if (cubes != null)
        {
            // Clean up previously generated cubes
            foreach (GameObject cube in cubes)
            {
                Destroy(cube);
            }
        }

        string Nos = inputField.text;
        string[] numbers = Nos.Split(',');

        Debug.Log("Number of elements in numbers array: " + numbers.Length); // Debug print

        // Calculate total width
        float totalWidth = (numbers.Length - 1) * spacing;

        // Calculate starting position
        float startX = -totalWidth / 2f;

        // Initialize currentX to starting position
        float currentX = startX;

        cubes = new GameObject[numbers.Length]; // Initialize the array to store cube references

        // Create a parent GameObject for the cubes
        GameObject proAnchor = new GameObject("proAnchor");
        Vector3 cubePosition = new Vector3(currentX, 0f, 0f);
        GameObject cube1 = Instantiate(cubePrefab,cubePosition,Quaternion.identity);
        /*
        for (int i = 0; i < numbers.Length; i++)
        {
            // Use the current position for each cube
            Vector3 cubePosition = new Vector3(currentX, 0f, 0f);

            Debug.Log("Position of cube " + (i + 1) + ": " + cubePosition); // Debug print

            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

            // Set the cube as a child of the proAnchor GameObject
            cube.transform.SetParent(proAnchor.transform);

            // Update currentX for the next cube
            currentX += spacing * 0.1f; // Double the spacing to ensure even spacing

            cubes[i] = cube; // Store reference to the cube in the array

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

                    // Set font size based on cube size
                    //float cubeSize = 24.2f; // Adjust this value based on your cube size
                    //float fontSizeMultiplier = 0.05f; // Adjust this multiplier as needed
                    //textMesh.fontSize = Mathf.RoundToInt(cubeSize * fontSizeMultiplier);
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI component not found in the canvas of the cube prefab.");
                }
            }
            else
            {
                Debug.LogError("Canvas component not found in the children of the cube prefab.");
            }*/
        //}

        //StartCoroutine(SelectionSortCoroutine());

        // Focus camera on generated cubes
        //FocusCameraOnCubes();

        // Hide input canvas
        
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
        GenerateCubes(); // Regenerate cubes and start sorting again
    }

    private IEnumerator SelectionSortCoroutine()
    {
        yield return new WaitForSeconds(sortingDelay);

        int n = cubes.Length;

        for (int i = 0; i < n - 1; i++)
        {
            iterationText.text = "Iteration: " + (i + 1);
            int minIndex = i;
            for (int j = i + 1; j < n; j++)
            {
                // Compare current cube with the minimum
                int currentValue = int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text);
                int minValue = int.Parse(cubes[minIndex].GetComponentInChildren<TextMeshProUGUI>().text);

                if (currentValue < minValue)
                {
                    // Change color of the previous smallest cube back to the original color
                    if (minIndex != i)
                    {
                        cubes[minIndex].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
                    }

                    minIndex = j;
                    // Change color of the new smallest cube to red
                    cubes[minIndex].GetComponentInChildren<TextMeshProUGUI>().color = smallestColor;
                }

                if (paused)
                {
                    yield return new WaitWhile(() => paused == true); // Pause the sorting process
                }

                yield return new WaitForSeconds(0.5f); // Adjust the delay as needed for visualization
            }

            // Swap the found minimum element with the first element
            Vector3 tempPosition = cubes[i].transform.position;
            Vector3 newPosition = cubes[minIndex].transform.position;
            // Move cubes up
            float cubeHeight = cubes[i].GetComponent<Renderer>().bounds.size.y;
            while (cubes[i].transform.position.y < cubeHeight || cubes[minIndex].transform.position.y < cubeHeight)
            {
                cubes[i].transform.position = Vector3.MoveTowards(cubes[i].transform.position, new Vector3(cubes[i].transform.position.x, cubeHeight, cubes[i].transform.position.z), Time.deltaTime * swapSpeed);
                cubes[minIndex].transform.position = Vector3.MoveTowards(cubes[minIndex].transform.position, new Vector3(cubes[minIndex].transform.position.x, cubeHeight, cubes[minIndex].transform.position.z), Time.deltaTime * swapSpeed);
                yield return null;
            }

            // Move cubes horizontally
            while (cubes[i].transform.position.x != newPosition.x || cubes[minIndex].transform.position.x != tempPosition.x)
            {
                cubes[i].transform.position = Vector3.MoveTowards(cubes[i].transform.position, new Vector3(newPosition.x, cubes[i].transform.position.y, newPosition.z), Time.deltaTime * swapSpeed);
                cubes[minIndex].transform.position = Vector3.MoveTowards(cubes[minIndex].transform.position, new Vector3(tempPosition.x, cubes[minIndex].transform.position.y, tempPosition.z), Time.deltaTime * swapSpeed);
                yield return null;
            }

            // Move cubes down
            while (cubes[i].transform.position.y > 0f || cubes[minIndex].transform.position.y > 0f)
            {
                cubes[i].transform.position = Vector3.MoveTowards(cubes[i].transform.position, new Vector3(cubes[i].transform.position.x, 0f, cubes[i].transform.position.z), Time.deltaTime * swapSpeed);
                cubes[minIndex].transform.position = Vector3.MoveTowards(cubes[minIndex].transform.position, new Vector3(cubes[minIndex].transform.position.x, 0f, cubes[minIndex].transform.position.z), Time.deltaTime * swapSpeed);
                yield return null;
            }
            GameObject tempCube = cubes[i];
            cubes[i] = cubes[minIndex];
            cubes[minIndex] = tempCube;

            // Change color of the sorted cube
            cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = sortedColor;
            // Change color of the smallest cube back to the original color
            if (i + 1 < n)
            {
                cubes[i + 1].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
            }
        }

        cubes[n - 1].GetComponentInChildren<TextMeshProUGUI>().color = sortedColor;

        sortingInProgress = false; // Sorting is complete
    }
}