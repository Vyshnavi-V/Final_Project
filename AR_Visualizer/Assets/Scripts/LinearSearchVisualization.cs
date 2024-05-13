using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.XR.ARFoundation;


public class LinearSearchVisualization : MonoBehaviour
{
     public ARPlaneManager arPlaneManager; 
    public GameObject cubePrefab;
    public GameObject indexPrefab;
    public TMP_InputField inputField;
    public TMP_InputField searchInputField;
    public TextMeshProUGUI infoText;

    public float spacing = 2f;
    public Color textColor = Color.white;
    public Color indexColor = Color.black;
    public Color searchColor = Color.red;
    public float searchDelay = 1f;
    public float textDelay = 2f;
    public Canvas exitCanvas;
    public Canvas infoCanvas;

private ARPlane trackPlane;

    private GameObject[] mainCubes;
    private GameObject[] indexCubes;
    private bool searchingInProgress = false;
    private bool paused = false;

void Start(){
    
}
    public void StartSearch(ARPlane plane)
    {
        infoText.text="";
        if (searchingInProgress)
            return;

        searchingInProgress = true;

        // Clear previous search results
        ResetSearch();

        // Start generating cubes
        GenerateCubes(plane);

        // Start the search process
        StartCoroutine(LinearSearchCoroutine());
    }
    public void OnSubmitButtonClick()
    {

        infoText.text="";
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
                    StartSearch(arPlane);
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
             Vector3 cubePosition = new Vector3(planePosition.x + currentX, planePosition.y + 0.7f, planePosition.z + 1f);

            GameObject mainCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
            mainCubes[i] = mainCube;

            TextMeshProUGUI mainTextMesh = mainCube.GetComponentInChildren<TextMeshProUGUI>();
            mainTextMesh.text = numbers[i];
            mainTextMesh.color = textColor;

            Vector3 indexPosition = new Vector3(planePosition.x + currentX, planePosition.y+0.5f, planePosition.z + 1f);

            GameObject indexCube = Instantiate(indexPrefab, indexPosition, Quaternion.identity);
            indexCubes[i] = indexCube;

            TextMeshProUGUI indexTextMesh = indexCube.GetComponentInChildren<TextMeshProUGUI>();
            indexTextMesh.text = i.ToString();
            indexTextMesh.color = indexColor;

            currentX += 0.2f;
        }
        movePPRCanvas(exitCanvas,mainCubes[0].transform.position);
        int n=mainCubes.Length;
        moveBackCanvas(infoCanvas,mainCubes[n/2].transform.position);
    }

    private IEnumerator LinearSearchCoroutine()
    {
        yield return new WaitForSeconds(searchDelay);

        int searchNumber = int.Parse(searchInputField.text);

        bool numberFound = false;

        for (int i = 0; i < mainCubes.Length; i++)
        {
            if (paused)
            {
                yield return new WaitWhile(() => paused == true); // Pause the search
            }

            TextMeshProUGUI textMesh = mainCubes[i].GetComponentInChildren<TextMeshProUGUI>();
            int cubeNumber = int.Parse(textMesh.text);

            infoText.text = "Comparing " + cubeNumber + " and " + searchNumber;
            yield return new WaitForSeconds(textDelay);

            textMesh.color = searchColor;

            if (cubeNumber != searchNumber)
            {
                infoText.text = cubeNumber + " != " + searchNumber;
                yield return new WaitForSeconds(textDelay);

                if (i != mainCubes.Length - 1)
                {
                    infoText.text = "Checking next number";
                    yield return new WaitForSeconds(textDelay);
                }

                textMesh.color = textColor;
            }
            else
            {
                textMesh.color = Color.green;
                infoText.text = cubeNumber + " = " + searchNumber + ". Number found at position " + i;
                numberFound = true;
                break;
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
        StartSearch(trackPlane); // Start the search process again
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
    float offsetY =  0.5f; 
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