using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Queue : MonoBehaviour
{
    public GameObject cubePrefab;
    public TMP_InputField inputField;
    public Button insertButton; // Button for enqueue operation
    public Button deleteButton; // Button for dequeue operation
    public TextMeshProUGUI frontText; // Text for front pointer
    public TextMeshProUGUI rearText; // Text for rear pointer
    public TextMeshProUGUI F; // Text for front pointer2
    public TextMeshProUGUI R; // Text for rear pointer2
    public TextMeshProUGUI enqueuedText; // Text for showing enqueued number
    public TextMeshProUGUI dequeuedText; // Text for showing dequeued number
    public TextMeshProUGUI queueStatusText; // Text for showing queue status

    private Queue<string> numberQueue = new Queue<string>(); // Queue to store numbers
    private Dictionary<string, GameObject> cubeDictionary = new Dictionary<string, GameObject>(); // Dictionary to store cube GameObjects
    private float cubeSize; // Size of the cube
    private float gap = 0f; // Gap between cubes
    private float delay = 2f; // Delay between cube generation
    private float currentX = 0f; // Current X position for spawning cubes
    private bool isEnqueuing = false; // Flag to check if enqueuing is in progress

    private void Start()
    {
        cubeSize = cubePrefab.GetComponent<Renderer>().bounds.size.x; // Get the size of the cube
    }

    // Method to enqueue numbers
    public void EnqueueNumbers()
    {
        if (!isEnqueuing)
        {
            StartCoroutine(EnqueueProcess());
        }
    }

    // Coroutine for enqueuing process
    private IEnumerator EnqueueProcess()
    {
        isEnqueuing = true;

        string input = inputField.text;
        string[] numbers = input.Split(',');

        foreach (string number in numbers)
        {
            string trimmedNumber = number.Trim();
            if (!string.IsNullOrEmpty(trimmedNumber))
            {
                numberQueue.Enqueue(trimmedNumber); // Enqueue the number
                GenerateCube(trimmedNumber); // Generate and visualize the cube
                enqueuedText.text = "Enqueued: " + trimmedNumber; // Update enqueued text
                yield return new WaitForSeconds(delay); // Add delay before enqueuing the next number
            }
        }

        UpdateFrontAndRearTextPositions(); // Update positions of front and rear texts
        isEnqueuing = false;
    }

    // Coroutine to move a cube to its final position
    private IEnumerator MoveCubeToPosition(GameObject cube, Vector3 finalPosition, string number)
    {
        // Instantiate the cube at an initial position far to the right
        Vector3 initialPosition = finalPosition + Vector3.right * 5f;
        GameObject newCube = Instantiate(cube, initialPosition, Quaternion.identity);

        // Set the number text of the cube
        newCube.GetComponentInChildren<TextMeshProUGUI>().text = number;

        // Add the cube GameObject to the dictionary
        cubeDictionary[number] = newCube;

        // Move the cube towards its final position gradually
        float duration = 7f; // Duration of the movement
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

    // Method to dequeue a number
    public void DequeueNumber()
    {
        if (numberQueue.Count > 0)
        {
            // Dequeue the number
            string dequeuedNumber = numberQueue.Dequeue();
            dequeuedText.text = "Dequeued: " + dequeuedNumber; // Update dequeued text
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

            UpdateFrontAndRearTextPositions(); // Update positions of front and rear texts

            if (numberQueue.Count == 0)
            {
                queueStatusText.text = "Queue is empty"; // Update queue status text
            }
        }
        else
        {
            queueStatusText.text = "Queue is empty"; // Update queue status text
            Debug.Log("Queue is empty. Cannot dequeue.");
        }
    }

    // Coroutine to move a cube to the left and destroy it gradually
    private IEnumerator MoveAndDestroyCube(GameObject cube)
    {
        Vector3 initialPosition = cube.transform.position;
        Vector3 finalPosition = initialPosition + Vector3.left * 5f;
        float duration = 5f; // Duration of the movement
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

    // Method to update the positions of front and rear texts above the cubes
private void UpdateFrontAndRearTextPositions()
{
    if (numberQueue.Count > 0)
    {
        frontText.gameObject.SetActive(true);
        rearText.gameObject.SetActive(true);

        // Calculate the position for front text
        string frontNumber = numberQueue.Peek();
        Vector3 frontPosition = cubeDictionary[frontNumber].transform.position + new Vector3(80f, 150f, 0f);
        frontText.transform.position = frontPosition;
        F.text = "F: " + frontNumber; // Update F text with front number

        // Calculate the position for rear text
        string rearNumber = numberQueue.ToArray()[numberQueue.Count - 1];
        Vector3 rearPosition = cubeDictionary[rearNumber].transform.position + new Vector3(130f, 150f, 0f);
        rearText.transform.position = rearPosition;
        R.text = "R: " + rearNumber; // Update R text with rear number
    }
    else
    {
        frontText.gameObject.SetActive(false);
        rearText.gameObject.SetActive(false);
    }
}
    // Method to generate a cube with a given number
    private void GenerateCube(string number)
    {
        // Increment currentX for the next cube
        currentX += cubeSize + gap; // Adding a small gap between cubes

        // Calculate the final position of the cube
        Vector3 finalPosition = new Vector3(currentX, 0f, 0f);

        // Start the coroutine to move the cube to its final position
        StartCoroutine(MoveCubeToPosition(cubePrefab, finalPosition, number));
    }
}
