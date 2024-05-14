using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.XR.ARFoundation;

public class Insertion : MonoBehaviour
{
        public ARPlaneManager arPlaneManager;

    public GameObject cubePrefab;
    public TMP_InputField InputField;
    public Button randomButton; // Reference to the button
    public float spacing = 2f;
    public Color textColor = Color.white;
    public Color comparisonColor = Color.yellow; // Color for cubes being compared
    public Color sortedColor = Color.green;
    public TextMeshProUGUI actionText;
    public TextMeshProUGUI iterationText;
    public Color indexColor = Color.black;

    public float sortingDelay = 1f; // Delay before starting the sorting process
    public float swapSpeed = 12f;
      public Canvas infoCanvas;
    public Canvas exitCanvas;
    public GameObject indexPrefab;

    private GameObject[] cubes;
    private GameObject[] indexes;
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
        InputField.text = randomNumbers;
        OnSubmitButtonClick();
    }

    public void OnSubmitButtonClick()
    {

actionText.text="";
infotext.text="";
iterationText.text="";
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
        //actionText.text = "This is called";
        
        if (sortingInProgress)
        {
            return;
        }
        
        DestroyPreviousCubesAndIndices();
        sortingInProgress = true;

        // Hide the BubbleInputCanvas

        // Destroy previous cubes and indexes

        string userInput = InputField.text;
        string[] numbers = userInput.Split(',');
        //InputField.text="";

        // Calculate total width
        float totalWidth = (numbers.Length - 1) * spacing;
        float startX = -0.5f;
        Vector3 iterationTextPosition = new Vector3(startX - 0.3f, 0f, 0f);
        Vector3 planePosition = plane.transform.position;
        //MoveCube(infoCanvas, planePosition);
        //actionText.text = "Move kazhinj";
        float currentX = startX;

        cubes = new GameObject[numbers.Length];
        indexes = new GameObject[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            //actionText.text = "Foril keri";
            Vector3 cubePosition = new Vector3(planePosition.x + currentX, planePosition.y+0.5f, planePosition.z+1f);

            // Adjust position relative to the plane
            Vector3 indexPosition = new Vector3(planePosition.x + currentX, planePosition.y+0.2f, planePosition.z + 1f); // Adjust position relative to the plane
            //actionText.text = "plane"+" "+planePosition+" "+"cube"+" "+cubePosition ;
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
        StartCoroutine(InsertionSortCoroutine());
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
    sortingInProgress = false;
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
        float originalY = cubes[0].transform.position.y;
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
            while (cubes[j + 1].transform.position.y < cubeHeight*0.5f || cubes[j].transform.position.y < cubeHeight*0.5f)
            {
                cubes[j + 1].transform.position = Vector3.MoveTowards(cubes[j + 1].transform.position, new Vector3(cubes[j + 1].transform.position.x, cubeHeight*0.5f, cubes[j + 1].transform.position.z), Time.deltaTime * swapSpeed);
                cubes[j].transform.position = Vector3.MoveTowards(cubes[j].transform.position, new Vector3(cubes[j].transform.position.x, cubeHeight*0.5f, cubes[j].transform.position.z), Time.deltaTime * swapSpeed);
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
            while (cubes[j + 1].transform.position.y > originalY || cubes[j].transform.position.y > originalY)
            {
                cubes[j + 1].transform.position = Vector3.MoveTowards(cubes[j + 1].transform.position, new Vector3(cubes[j + 1].transform.position.x, originalY, cubes[j + 1].transform.position.z), Time.deltaTime * swapSpeed);
                cubes[j].transform.position = Vector3.MoveTowards(cubes[j].transform.position, new Vector3(cubes[j].transform.position.x, originalY, cubes[j].transform.position.z), Time.deltaTime * swapSpeed);
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
