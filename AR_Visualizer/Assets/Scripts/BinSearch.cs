using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.XR.ARFoundation;


public class BinSearch : MonoBehaviour
{
         public ARPlaneManager arPlaneManager; 

    public GameObject cubePrefab;
    public GameObject indexPrefab;
    public TMP_InputField inputField;
    public TMP_InputField searchInputField;
    public TextMeshProUGUI infoText;
     public TextMeshProUGUI firstText;

    public float spacing = 0.2f;
    public Color textColor = Color.blue;
    public Color indexColor = Color.white;
    public Color searchColor = Color.red;
    public float searchDelay = 1f;
    public float textDelay = 2f;

    private GameObject[] mainCubes;
    private ARPlane trackPlane;
    private GameObject[] indexCubes;
    private bool searchingInProgress = false;
    private bool paused = false;
    public Canvas exitCanvas;
    public Canvas infoCanvas;
    private bool waitForPause = false; // Flag to indicate if coroutine should wait for pause

private void Start()
    {
        //submitButton.onClick.AddListener(OnSubmitButtonClick);
    }
public void OnSubmitButtonClick()
    {

        infoText.text = ""; 
        StartCoroutine(WaitForPlaneDetection());
    }
    private IEnumerator WaitForPlaneDetection()
    {
        infoText.text = "Point the camera to a flat surface";
        float elapsedTime = 0f;
        float maxWaitTime = 60f; // Maximum wait time in seconds (1 minute)

        while (elapsedTime < maxWaitTime)
        {
            // Check if any planes are detected
            foreach (var trackable in arPlaneManager.trackables)
            {
                if (trackable is ARPlane arPlane)
                {
                    infoText.text = "plane detected";
                    yield return new WaitForSeconds(10f);
                    // Plane detected, generate cubes on this plane
                    trackPlane = arPlane;
                    StartSearch();
                    yield break; // Exit the coroutine
                }
            }

            // No planes detected yet, wait for a short duration and check again
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
        }

        // No plane detected within the time limit, display error message
        Debug.LogError("No AR planes detected within the time limit.");
        infoText.text = "No AR plane detected within 1 minute.Exit and scan again";
    }
    public void StartSearch()
    {
        if (searchingInProgress)
            return;

        searchingInProgress = true;

        // Clear previous search results
        ResetSearch();

        // Start generating cubes
        GenerateCubes(trackPlane);

        // Start the search process
        StartCoroutine(BinarySearchCoroutine());
    }

    private void GenerateCubes(ARPlane plane)
    {
        string inputText = inputField.text;
        string[] numbers = inputText.Split(',');

        float currentX = 0f;

        mainCubes = new GameObject[numbers.Length];
        indexCubes = new GameObject[numbers.Length];
        Vector3 planePosition = plane.transform.position;


        for (int i = 0; i < numbers.Length; i++)
        {
            // Generate main value cube
            Vector3 cubePosition = new Vector3(planePosition.x + currentX, planePosition.y + 0.5f, planePosition.z + 1f);

            GameObject mainCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
            mainCubes[i] = mainCube;

            TextMeshProUGUI mainTextMesh = mainCube.GetComponentInChildren<TextMeshProUGUI>();
            mainTextMesh.text = numbers[i];
            mainTextMesh.color = textColor;

            // Generate index cube
            Vector3 indexPosition = new Vector3(planePosition.x + currentX, planePosition.y+0.3f, planePosition.z + 1f);

            GameObject indexCube = Instantiate(indexPrefab, indexPosition, Quaternion.identity);
            indexCubes[i] = indexCube;

            TextMeshProUGUI indexTextMesh = indexCube.GetComponentInChildren<TextMeshProUGUI>();
            indexTextMesh.text = i.ToString();
            indexTextMesh.color = indexColor;

            currentX +=spacing;
        }
        movePPRCanvas(exitCanvas,mainCubes[0].transform.position);
        int n=mainCubes.Length;
        moveBackCanvas(infoCanvas,mainCubes[n/2].transform.position);
        // Start sorting coroutine
    }

    private IEnumerator BinarySearchCoroutine()
    {
      
        yield return new WaitForSeconds(searchDelay);

        int[] numbers = new int[mainCubes.Length];
        for (int i = 0; i < mainCubes.Length; i++)
        {
            TextMeshProUGUI textMesh = mainCubes[i].GetComponentInChildren<TextMeshProUGUI>();
            numbers[i] = int.Parse(textMesh.text);
        }

        int searchNumber = int.Parse(searchInputField.text);
          firstText.text = "Element to be searched: "+ searchNumber;
        int left = 0;
        int right = numbers.Length - 1;
        bool numberFound = false;

        while (left <= right)
        {
            if (paused)
            {
                waitForPause = true; // Set the flag to wait for pause
                yield return new WaitWhile(() => paused == true); // Wait for resume
                waitForPause = false; // Reset the flag
            }

            if (waitForPause)
                yield return null; // Wait for pause command to be processed

            int middle = (left + right) / 2;

            // Change color of the middle cube to red
            TextMeshProUGUI middleTextMesh = mainCubes[middle].GetComponentInChildren<TextMeshProUGUI>();
            middleTextMesh.color = Color.yellow;

            yield return new WaitForSeconds(textDelay);

            int middleNumber = int.Parse(middleTextMesh.text);

            infoText.text = "Current mid index =" + middle +", Mid element a[mid]="+ middleNumber;
            yield return new WaitForSeconds(textDelay);


            infoText.text = "Comparing " + middleNumber + " and " + searchNumber;
            yield return new WaitForSeconds(textDelay);

            middleTextMesh.color = textColor; // Reset color

            if (middleNumber == searchNumber)
            {
                middleTextMesh.color = Color.green;
                infoText.text = middleNumber + " = " + searchNumber + ". Number found at position " + middle;
                numberFound = true;
                break;
            }
            else if (middleNumber < searchNumber)
            {   
                infoText.text = middleNumber + "<" + searchNumber;
                yield return new WaitForSeconds(textDelay);

                // Calculate the horizontal shift
                float shiftAmount = mainCubes[middle].transform.position.x;

                infoText.text ="Move right: Apply binary search to the subarray -->right of the middle element.";
                yield return new WaitForSeconds(textDelay);
                // Destroy cubes to the left of middle including middle
                for (int i = left; i <= middle; i++)
                {
                    Destroy(mainCubes[i]);
                    Destroy(indexCubes[i]);
                }
                yield return new WaitForSeconds(textDelay);
                
                // Update the left value
                left = middle + 1;

                /*
                // Apply horizontal shift to all remaining cubes
                for (int i = left; i <= right; i++)
                {
                    Vector3 newPosition = mainCubes[i].transform.position;
                    newPosition.x -= shiftAmount; // Shift left
                    mainCubes[i].transform.position = newPosition;

                    newPosition = indexCubes[i].transform.position;
                    newPosition.x -= shiftAmount; // Shift left
                    indexCubes[i].transform.position = newPosition;
                }
                */
            }
            else
            {   
                infoText.text = middleNumber + ">" + searchNumber;
                yield return new WaitForSeconds(textDelay);

                infoText.text ="Move left:Apply binary serach to the subarray->left of the middle element.";

                yield return new WaitForSeconds(textDelay);
                // Destroy cubes to the right of middle including middle
                for (int i = middle; i <= right; i++)
                {
                    Destroy(mainCubes[i]);
                    Destroy(indexCubes[i]);
                }
                right = middle - 1;
            }
        }

        if (!numberFound)
        {
            infoText.text = "Number not found";
        }

        searchingInProgress = false;
    }


    private void ResetSearch()
    {
        if (mainCubes != null)
        {
            foreach (GameObject cube in mainCubes)
            {
                Destroy(cube);
            }
        }

        if (indexCubes != null)
        {
            foreach (GameObject cube in indexCubes)
            {
                Destroy(cube);
            }
        }
    }

    // Button functions
    public void PauseSearch()
    {
        paused = true; // Set the paused flag to true
    }

    public void ResumeSearch()
    {
        paused = false; // Set the paused flag to false
    }

    public void ReplaySearch()
    {
        StopAllCoroutines(); // Stop the current search coroutine
        ResetSearch(); // Reset the search
        searchingInProgress = false; // Reset the searching in progress flag
        paused = false; // Reset the paused flag
         infoText.text = ""; 
        StartSearch(); // Start the search process again
    }
     public void DestroyAllObjectsAndResetInfoText()
{
    ResetSearch(); // Destroy all objects created
    infoText.text = ""; // Set infoText to null
    searchingInProgress = false;
}
    private void movePPRCanvas(Canvas canvas, Vector3 position)
{
    if (canvas == null)
    {
        Debug.LogError("Canvas parameter is null. Cannot move canvas.");
        return;
    }
    float offsetX = -0.5f; 
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
    float offsetY = 0.5f; 
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