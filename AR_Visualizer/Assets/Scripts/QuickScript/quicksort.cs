using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Quicksort : MonoBehaviour
{
    public GameObject cubePrefab;
    public TMP_InputField inputField;
    public GameObject inputCanvas;
    public Button randomButton;
    public Camera mainCamera;
    public float spacing = 2f;
    public Color textColor = Color.black;
    public Color comparisonColor = Color.yellow;
    public float sortingDelay = 1f;
    public float swapSpeed = 3f;

    public TextMeshProUGUI lowText;
    public TextMeshProUGUI highText;
    public TextMeshProUGUI pivotText;

    private List<List<GameObject>> generatedCubes = new List<List<GameObject>>();
    private bool sortingInProgress = false;
    private bool paused = false;

    private void Start()
    {
        randomButton.onClick.AddListener(GenerateRandomNumbers);
    }

    public void GenerateRandomNumbers()
    {
        if (sortingInProgress)
        {
            return;
        }

        sortingInProgress = true;

        inputCanvas.SetActive(false);

        string[] randomNumbers = GenerateRandomArray(5);

        GenerateInitialCubes(randomNumbers);
    }

    private string[] GenerateRandomArray(int length)
    {
        string[] randomArray = new string[length];
        for (int i = 0; i < length; i++)
        {
            randomArray[i] = Random.Range(0, 10).ToString();
        }
        return randomArray;
    }

    public void GenerateCubes()
    {
        if (sortingInProgress)
        {
            return;
        }

        sortingInProgress = true;

        if (generatedCubes.Count > 0)
        {
            foreach (List<GameObject> levelCubes in generatedCubes)
            {
                foreach (GameObject cube in levelCubes)
                {
                    Destroy(cube);
                }
            }
        }

        inputCanvas.SetActive(false);

        string Nos = inputField.text;
        string[] numbers = Nos.Split(',');

        GenerateInitialCubes(numbers);
    }

    private void GenerateInitialCubes(string[] numbers)
    {
        float totalWidth = (numbers.Length - 1) * spacing;
        float startX = -totalWidth / 2f;
        float currentX = startX;

        List<GameObject> initialCubes = new List<GameObject>();

        for (int i = 0; i < numbers.Length; i++)
        {
            Vector3 cubePosition = new Vector3(currentX, 0f, 0f);

            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

            currentX += spacing;

            initialCubes.Add(cube);

            Canvas canvas = cube.GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                TextMeshProUGUI textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();
                if (textMesh != null)
                {
                    textMesh.text = numbers[i];
                    textMesh.color = textColor;
                    textMesh.alignment = TextAlignmentOptions.Center;
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

        generatedCubes.Add(initialCubes);

        StartCoroutine(QuicksortCoroutine(0, generatedCubes[0].Count - 1, 0));
    }

    private void FocusCameraOnCubes()
    {
        if (generatedCubes.Count > 0 && mainCamera != null)
        {
            Bounds bounds = new Bounds(generatedCubes[0][0].transform.position, Vector3.zero);
            foreach (List<GameObject> levelCubes in generatedCubes)
            {
                foreach (GameObject cube in levelCubes)
                {
                    bounds.Encapsulate(cube.GetComponent<Renderer>().bounds);
                }
            }

            float cameraDistance = bounds.size.magnitude / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            mainCamera.transform.position = bounds.center - mainCamera.transform.forward * cameraDistance;
            mainCamera.transform.LookAt(bounds.center);
        }
    }

    private IEnumerator QuicksortCoroutine(int low, int high, int level)
    {
        yield return new WaitForSeconds(sortingDelay);

        if (level < generatedCubes.Count && low >= 0 && high < generatedCubes[level].Count && low <= high)
        {
            if (low < high)
            {
                int pivotIndex = low;
                int pivotValue = int.Parse(generatedCubes[level][pivotIndex].GetComponentInChildren<TextMeshProUGUI>().text);
                int left = low + 1;
                int right = high;

                HighlightCube(generatedCubes[level][pivotIndex], Color.red, true);

                while (left <= right)
                {
                    int leftValue = int.Parse(generatedCubes[level][left].GetComponentInChildren<TextMeshProUGUI>().text);
                    int rightValue = int.Parse(generatedCubes[level][right].GetComponentInChildren<TextMeshProUGUI>().text);

                    while (left <= right && leftValue <= pivotValue)
                    {
                        yield return StartCoroutine(TraverseCube(generatedCubes[level][left], Color.yellow, true));
                        left++;
                        if (left <= right)
                        {
                            leftValue = int.Parse(generatedCubes[level][left].GetComponentInChildren<TextMeshProUGUI>().text);
                        }
                    }

                    while (left <= right && rightValue > pivotValue)
                    {
                        yield return StartCoroutine(TraverseCube(generatedCubes[level][right], Color.green, false, true));
                        right--;
                        if (left <= right)
                        {
                            rightValue = int.Parse(generatedCubes[level][right].GetComponentInChildren<TextMeshProUGUI>().text);
                        }
                    }

                    if (left < right)
                    {
                        yield return StartCoroutine(SwapCubes(generatedCubes[level], left, right, pivotIndex));
                        yield return new WaitForSeconds(0.5f);
                    }
                }

                yield return StartCoroutine(SwapCubes(generatedCubes[level], pivotIndex, right, pivotIndex));

                VisualizePartition(low, right, level);

                yield return StartCoroutine(QuicksortCoroutine(low, right - 1, level + 1));

                if (right + 1 < high)
                {
                    int secondLastLevel = generatedCubes.Count - 1;
                    yield return StartCoroutine(QuicksortCoroutine(right + 1, high, secondLastLevel));
                }
            }
            else
            {
                Debug.Log("Partition at level " + level + " contains only one element.");
                yield return null;
            }
        }
        else
        {
            Debug.LogError("Invalid indices. Check the values of low, high, or the generatedCubes list.");
        }
    }

    private IEnumerator SwapCubes(List<GameObject> cubes, int index1, int index2, int pivotIndex)
    {
        GameObject cube1 = cubes[index1];
        GameObject cube2 = cubes[index2];

        Vector3 initialPos1 = cube1.transform.position;
        Vector3 initialPos2 = cube2.transform.position;
        float swapDuration = 1f;

        HighlightCube(cubes[pivotIndex], Color.red, true);
        HighlightCube(cubes[index1], Color.yellow, false, true);
        HighlightCube(cubes[index2], Color.green, false, true);

        yield return new WaitForSeconds(0.5f);

        float cubeHeight = cube1.GetComponent<Renderer>().bounds.size.y;
        float startTime = Time.time;
        while (Time.time - startTime < swapDuration)
        {
            float fracJourney = (Time.time - startTime) / swapDuration;
            cube1.transform.position = Vector3.Lerp(initialPos1, initialPos1 + Vector3.up * cubeHeight, fracJourney);
            cube2.transform.position = Vector3.Lerp(initialPos2, initialPos2 + Vector3.up * cubeHeight, fracJourney);
            yield return null;
        }

        GameObject temp = cubes[index1];
        cubes[index1] = cubes[index2];
        cubes[index2] = temp;

        SwapTextPositions(cubes[index1], cubes[index2]);

        startTime = Time.time;
        while (Time.time - startTime < swapDuration)
        {
            float fracJourney = (Time.time - startTime) / swapDuration;
            cube1.transform.position = Vector3.Lerp(initialPos1 + Vector3.up * cubeHeight, initialPos2, fracJourney);
            cube2.transform.position = Vector3.Lerp(initialPos2 + Vector3.up * cubeHeight, initialPos1, fracJourney);
            yield return null;
        }

        cube1.transform.position = initialPos2;
        cube2.transform.position = initialPos1;

        if (index1 != pivotIndex && index2 != pivotIndex)
        {
            HighlightCube(cubes[index1], textColor);
            HighlightCube(cubes[index2], textColor);
        }
    }

    private void SwapTextPositions(GameObject cube1, GameObject cube2)
    {
        TextMeshProUGUI textMesh1 = cube1.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI textMesh2 = cube2.GetComponentInChildren<TextMeshProUGUI>();

        Vector3 tempPos = textMesh1.transform.localPosition;
        textMesh1.transform.localPosition = textMesh2.transform.localPosition;
        textMesh2.transform.localPosition = tempPos;
    }

    private IEnumerator TraverseCube(GameObject cube, Color color, bool isLow = false, bool isHigh = false)
    {
        HighlightCube(cube, color, false, isLow, isHigh);
        yield return new WaitForSeconds(2f);
        HighlightCube(cube, textColor);
    }
private void HighlightCube(GameObject cube, Color color, bool isPivot = false, bool isLow = false, bool isHigh = false)
{
    TextMeshProUGUI textMesh = cube.GetComponentInChildren<TextMeshProUGUI>();
    if (textMesh != null)
    {
        // Store the original text and position
        string originalText = textMesh.text;
        Vector3 originalPosition = textMesh.transform.position;

        // Set the text color
        textMesh.color = color;

        // Set text position above the cube
        Vector3 textPosition = cube.transform.position;
        float yOffset = 0f;

        if (isPivot)
        {
            textMesh.color = Color.red;
            EnableText(pivotText);
            pivotText.transform.position = textPosition + Vector3.up * 130f; // Adjust pivot text position
            yOffset = 1.5f;
        }
        else if (isHigh && !isLow)
        {
            EnableText(highText);
            highText.transform.position = textPosition + Vector3.up * 130f; // Adjust high text position
            yOffset = 0.5f;
            // If the cube is both high and low, hide the "low" text
            if (lowText.gameObject.activeSelf)
            {
                DisableText(lowText);
            }
        }
        else if (isLow && !isHigh)
        {
            EnableText(lowText);
            lowText.transform.position = textPosition + Vector3.up * 130f; // Adjust low text position
            yOffset = 1.0f;
            // If the cube is both low and high, hide the "high" text
            if (highText.gameObject.activeSelf)
            {
                DisableText(highText);
            }
        }
        else
        {
            textMesh.color = textColor;
            DisableText(lowText);
            DisableText(highText);
        }

        // Set the original text and position back
        textMesh.text = originalText;
        textMesh.transform.position = originalPosition + Vector3.up * yOffset; // Adjust the original position
    }
}




private void EnableText(TextMeshProUGUI textObject)
{
    textObject.gameObject.SetActive(true);
}

private void DisableText(TextMeshProUGUI textObject)
{
    textObject.gameObject.SetActive(false);
}


    private void VisualizePartition(int low, int high, int level)
    {
        List<GameObject> clonedCubes = new List<GameObject>();
        float yOffset = -700f * (generatedCubes.Count - level);

        for (int i = 0; i < generatedCubes[level].Count; i++)
        {
            Vector3 newPosition = generatedCubes[level][i].transform.position;
            newPosition.y += yOffset;
            clonedCubes.Add(Instantiate(generatedCubes[level][i], newPosition, Quaternion.identity));
            clonedCubes[i].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
        }

        generatedCubes.Add(clonedCubes);
    }

    public void PauseSorting()
    {
        if (sortingInProgress && !paused)
        {
            paused = true;
            Time.timeScale = 0f;
        }
    }

    public void PlaySorting()
    {
        if (sortingInProgress && paused)
        {
            paused = false;
            Time.timeScale = 1f;
            StartCoroutine(QuicksortCoroutine(0, generatedCubes[0].Count - 1, 0));
        }
    }
}