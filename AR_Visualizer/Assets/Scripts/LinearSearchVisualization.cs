using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LinearSearchVisualization : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject indexPrefab;
    public TMP_InputField inputField;
    public TMP_InputField searchInputField;
    public TextMeshProUGUI infoText;

    public float spacing = 2f;
    public Color textColor = Color.blue;
    public Color indexColor = Color.black;
    public Color searchColor = Color.red;
    public float searchDelay = 1f;
    public float textDelay = 2f;
    public Canvas exitCanvas;
    public Canvas infoCanvas;

    private GameObject[] mainCubes;
    private GameObject[] indexCubes;
    private bool searchingInProgress = false;
    private bool paused = false;

    public void StartSearch()
    {
        if (searchingInProgress)
            return;

        searchingInProgress = true;

        // Clear previous search results
        ResetSearch();

        // Start generating cubes
        GenerateCubes();

        // Start the search process
        StartCoroutine(LinearSearchCoroutine());
    }

    private void GenerateCubes()
    {
        string inputText = inputField.text;
        string[] numbers = inputText.Split(',');

        float currentX = 0f;

        mainCubes = new GameObject[numbers.Length];
        indexCubes = new GameObject[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            Vector3 cubePosition = new Vector3(currentX, 0f, 0f);
            GameObject mainCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
            mainCubes[i] = mainCube;

            TextMeshProUGUI mainTextMesh = mainCube.GetComponentInChildren<TextMeshProUGUI>();
            mainTextMesh.text = numbers[i];
            mainTextMesh.color = textColor;

            Vector3 indexCubePosition = new Vector3(currentX, -0.2f, 0f);
            GameObject indexCube = Instantiate(indexPrefab, indexCubePosition, Quaternion.identity);
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
        StartSearch(); // Start the search process again
    }
    public void DestroyAllObjectsAndResetInfoText()
{
    ResetSearch(); // Destroy all objects created
    infoText.text = ""; // Set infoText to null
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