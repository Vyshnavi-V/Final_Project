using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.XR.ARFoundation;

public class Insertion : MonoBehaviour
{
        public ARPlaneManager arPlaneManager;

    public GameObject cubePrefab;
    public TMP_InputField inputField;
    public Button randomButton; // Reference to the button
    public float spacing = 2f;
    public Color textColor = Color.white;
    public Color comparisonColor = Color.yellow; // Color for cubes being compared
    public Color sortedColor = Color.green;
    public TextMeshProUGUI actionText;
    public TextMeshProUGUI iterationText;

    public float sortingDelay = 1f; // Delay before starting the sorting process
    public float swapSpeed = 12f;
      public Canvas userCanvas;
    public Canvas exitCanvas;

    private GameObject[] cubes;
    private bool sortingInProgress = false;
    private bool paused = false;
    public TextMeshProUGUI infotext;
    private ARPlane trackPlane;

    private void Start()
    {
        //randomButton.onClick.AddListener(GenerateRandomNumbers); // Add listener to the button
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
        //GenerateCubesOnPlane();
    }

    public void OnSubmitButtonClick()
    {

        // Start a coroutine to wait for plane detection
        StartCoroutine(WaitForPlaneDetection());
    }

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
                    Debug.LogError("No AR planes detected .");
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
        infotext.text = "No AR plane detected within 1 minute.";
    }
        public void GenerateCubesOnPlane(ARPlane plane)
    {
        string Nos = inputField.text;
        string[] numbers = Nos.Split(',');

        float totalWidth = (numbers.Length - 1) * spacing;
        float startX = -0.5f;
        float currentX = startX;
       

        cubes = new GameObject[numbers.Length];
        Vector3[] cubePositions = new Vector3[numbers.Length];
        Vector3 planePosition = plane.transform.position;
          movePPRCanvas(userCanvas,planePosition);
        moveBackCanvas(exitCanvas,planePosition);

        for (int i = 0; i < numbers.Length; i++)
        {
            Vector3 cubePosition = new Vector3(planePosition.x + currentX, planePosition.y+0.5f, planePosition.z+1f);
            cubePositions[i] = cubePosition;
            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

            infotext.text = "plane"+" "+planePosition+" "+"cube"+" "+cubePosition ;

            currentX += spacing *0.05f;

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
       
        StartCoroutine(InsertionSortCoroutine());
    }

    public void ReplaySorting()
    {
        StopAllCoroutines(); // Stop any ongoing sorting coroutine
        sortingInProgress = false;
        paused = false;
        GenerateCubesOnPlane(trackPlane);// Regenerate cubes and start sorting again
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
    int counter =0;
    

    for (int i = 1; i < n; ++i)
    {
        iterationText.text = "Iteration: " + i;
         


        int key = int.Parse(cubes[i].GetComponentInChildren<TextMeshProUGUI>().text);
        int j = i - 1;

        // Display comparison action text
        actionText.text = "Comparing " + key + " with " + int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text);

        yield return new WaitForSeconds(2f); // Delay after comparison
           // Display the result of the comparison
            if (key < int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text))
            {
                actionText.text += " : " + key + " < " + int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text);
            }
            else if (key > int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text))
            {
                actionText.text += " : " + key + " > " + int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text);
            }
            else
            {
                actionText.text += " : " + key + " = " + int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text);
            }

            yield return new WaitForSeconds(2f); // Delay after comparing

        // Change color of the current key cube
        cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;

        while (j >= 0 && int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text) > key)
        {
            // Change color of the cubes being compared
            cubes[j].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;
            cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;

            if(counter==0)
             {  
                actionText.text = "Inserting " + key + " into Sorted Subarray ";
                counter =1;
                yield return new WaitForSeconds(2f);
             }
            // Move cubes up
            float cubeHeight = cubes[j + 1].GetComponent<Renderer>().bounds.size.y;
            while (cubes[j + 1].transform.position.y < cubeHeight || cubes[j].transform.position.y < cubeHeight)
            {
                cubes[j + 1].transform.position = Vector3.MoveTowards(cubes[j + 1].transform.position, new Vector3(cubes[j + 1].transform.position.x, cubeHeight, cubes[j + 1].transform.position.z), Time.deltaTime * swapSpeed);
                cubes[j].transform.position = Vector3.MoveTowards(cubes[j].transform.position, new Vector3(cubes[j].transform.position.x, cubeHeight, cubes[j].transform.position.z), Time.deltaTime * swapSpeed);
                actionText.text = "Inserting " + int.Parse(cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().text) +" and shifting " + int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text) + " to the right";
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
                
                actionText.text = "Inserting " + int.Parse(cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().text) +" and shifting " + int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text) + " to the right";
                yield return null;
            }

            // Move cubes down
            while (cubes[j + 1].transform.position.y > 0f || cubes[j].transform.position.y > 0f)
            {
                cubes[j + 1].transform.position = Vector3.MoveTowards(cubes[j + 1].transform.position, new Vector3(cubes[j + 1].transform.position.x, 0f, cubes[j + 1].transform.position.z), Time.deltaTime * swapSpeed);
                cubes[j].transform.position = Vector3.MoveTowards(cubes[j].transform.position, new Vector3(cubes[j].transform.position.x, 0f, cubes[j].transform.position.z), Time.deltaTime * swapSpeed);
               actionText.text = "Inserting " + int.Parse(cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().text) +" and shifting " + int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text) + " to the right";
                yield return null;
            }

            // Swap cube references
            GameObject tempCube = cubes[j + 1];
            cubes[j + 1] = cubes[j];
            cubes[j] = tempCube;

            // Change color of the cubes back to sorted color after swapping
            cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().color = sortedColor;

            // Display swapping action text
            //actionText.text = "Swapping " + int.Parse(cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().text) + " with " + int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text);

            if (paused)
            {
                yield return new WaitWhile(() => paused == true); // Pause the sorting process
            }

            yield return new WaitForSeconds(2f); // Delay after swapping

            j = j - 1;

            if (j >= 0)
            {
                // Display comparison action text for next comparison
                actionText.text = "Comparing " + key + " with " + int.Parse(cubes[j].GetComponentInChildren<TextMeshProUGUI>().text);

                yield return new WaitForSeconds(2f); // Delay after comparison
            }
        }
        counter =0;
        

        cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().text = key.ToString();
        cubes[j + 1].GetComponentInChildren<TextMeshProUGUI>().color = sortedColor;

        // Change color of the sorted elements to green
        for (int k = 0; k <= i; k++)
        {
            cubes[k].GetComponentInChildren<TextMeshProUGUI>().color = sortedColor;
        }

        if (paused)
        {
            yield return new WaitWhile(() => paused == true); // Pause the sorting process
        }

         
        // Build sorted subarray string
        string sortedSubarray = "Sorted sub array = [";
            for (int k = 0; k <= i; k++)
            {
                if (cubes[k].GetComponentInChildren<TextMeshProUGUI>().color == sortedColor)
                {
                    sortedSubarray += int.Parse(cubes[k].GetComponentInChildren<TextMeshProUGUI>().text);
                    if (k < i)
                    sortedSubarray += ", ";
                }
            }
        sortedSubarray += "]";
        actionText.text = sortedSubarray;
        yield return new WaitForSeconds(2f);
    }

    iterationText.text = "Sorting Completed"; // Update iteration text after sorting completion

    sortingInProgress = false; // Sorting is complete
}    private void moveCanvas(Canvas canvas, Vector3 position)
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
    float offsetX = spacing * 0.5f; 
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
private void moveInfoCanvas(Canvas canvas, Vector3 position)
{
    if (canvas == null)
    {
        Debug.LogError("Canvas parameter is null. Cannot move canvas.");
        return;
    }
    float offsetY = spacing * 0.5f; 
    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
    if (canvasRect != null)
    {
        canvasRect.anchoredPosition3D = position + new Vector3(0f,offsetY , 0f);
    }
    else
    {
        Debug.LogError("RectTransform component not found on the canvas. Cannot move canvas.");
    }
}
}
