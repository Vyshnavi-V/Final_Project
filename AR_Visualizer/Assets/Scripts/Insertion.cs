using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Insertion : MonoBehaviour
{
    public GameObject cubePrefab;
    public TMP_InputField inputField;
    public Button randomButton; // Reference to the button
    public float spacing = 2f;
    public Color textColor = Color.white;
    public Color comparisonColor = Color.yellow; // Color for cubes being compared
    public float sortingDelay = 1f; // Delay before starting the sorting process
    public float swapSpeed = 12f;

    private GameObject[] cubes;
    private bool sortingInProgress = false;
    private bool paused = false;

    private void Start()
    {
        randomButton.onClick.AddListener(GenerateRandomNumbers); // Add listener to the button
    }

    public void GenerateRandomNumbers()
    {
        string randomNumbers = "";
        for (int i = 0; i < 5; i++)
        {
            randomNumbers += Random.Range(0, 10).ToString();
            if (i < 4) randomNumbers += ",";
        }
        inputField.text = randomNumbers;
        GenerateCubes();
    }

    public void GenerateCubes()
    {
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

        for (int i = 0; i < numbers.Length; i++)
        {
            // Use the current position for each cube
            Vector3 cubePosition = new Vector3(currentX, 0f, 0f);

            Debug.Log("Position of cube " + (i + 1) + ": " + cubePosition); // Debug print

            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

            // Update currentX for the next cube
            currentX += spacing * 2; // Double the spacing to ensure even spacing

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
                    float cubeSize = 24.2f; // Adjust this value based on your cube size
                    float fontSizeMultiplier = 0.05f; // Adjust this multiplier as needed
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

        StartCoroutine(InsertionSortCoroutine());
    }

    public void ReplaySorting()
    {
        StopAllCoroutines(); // Stop any ongoing sorting coroutine
        sortingInProgress = false;
        paused = false;
        GenerateCubes(); // Regenerate cubes and start sorting again
    }

    public void PauseSorting()
    {
        paused = true;
    }

    public void ResumeSorting()
    {
        paused = false;
    }

    private IEnumerator InsertionSortCoroutine()
    {
        yield return new WaitForSeconds(sortingDelay);

        int n = cubes.Length;

        for (int i = 1; i < n; ++i)
        {
            int key = int.Parse(cubes[i].GetComponentInChildren<TextMeshProUGUI>().text);
            int j = i - 1;

            // Change color of the current key cube
            cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;

            while (j >= 0 && int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text) > key)
            {
                // Change color of the cubes being compared
                cubes[j].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;
                cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;

                // Move cubes up
                float cubeHeight = cubes[j + 1].GetComponent<Renderer>().bounds.size.y;
                while (cubes[j + 1].transform.position.y < cubeHeight || cubes[j].transform.position.y < cubeHeight)
                {
                    cubes[j + 1].transform.position = Vector3.MoveTowards(cubes[j + 1].transform.position, new Vector3(cubes[j + 1].transform.position.x, cubeHeight, cubes[j + 1].transform.position.z), Time.deltaTime * swapSpeed);
                    cubes[j].transform.position = Vector3.MoveTowards(cubes[j].transform.position, new Vector3(cubes[j].transform.position.x, cubeHeight, cubes[j].transform.position.z), Time.deltaTime * swapSpeed);
                    yield return null;
                }

                // Move cubes horizontally
                Vector3 tempPosition = cubes[j + 1].transform.position;
                Vector3 newPosition = cubes[j].transform.position;
                newPosition.y = cubes[j + 1].transform.position.y; // Maintain the same y position
                while (cubes[j + 1].transform.position.x != newPosition.x || cubes[j].transform.position.x != tempPosition.x)
                {
                    cubes[j + 1].transform.position = Vector3.MoveTowards(cubes[j + 1].transform.position, new Vector3(newPosition.x, cubes[j + 1].transform.position.y, newPosition.z), Time.deltaTime * swapSpeed);
                    cubes[j].transform.position = Vector3.MoveTowards(cubes[j].transform.position, new Vector3(tempPosition.x, cubes[j].transform.position.y, tempPosition.z), Time.deltaTime * swapSpeed);
                    yield return null;
                }

                // Move cubes down
                while (cubes[j + 1].transform.position.y > 0f || cubes[j].transform.position.y > 0f)
                {
                    cubes[j + 1].transform.position = Vector3.MoveTowards(cubes[j + 1].transform.position, new Vector3(cubes[j + 1].transform.position.x, 0f, cubes[j + 1].transform.position.z), Time.deltaTime * swapSpeed);
                    cubes[j].transform.position = Vector3.MoveTowards(cubes[j].transform.position, new Vector3(cubes[j].transform.position.x, 0f, cubes[j].transform.position.z), Time.deltaTime * swapSpeed);
                    yield return null;
                }

                // Swap cube references
                GameObject tempCube = cubes[j + 1];
                cubes[j + 1] = cubes[j];
                cubes[j] = tempCube;

                // Reset color after comparison
                cubes[j].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
                cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().color = textColor;

                j = j - 1;
            }
            cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().text = key.ToString();

            // Reset color of the current key cube
            cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = textColor;

            if (paused)
            {
                yield return new WaitWhile(() => paused == true); // Pause the sorting process
            }

            yield return new WaitForSeconds(0.5f); // Adjust the delay as needed for visualization
        }

        // Change color of sorted numbers to green
        for (int i = 0; i < n; i++)
        {
            cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        }

        sortingInProgress = false; // Sorting is complete
    }
}
