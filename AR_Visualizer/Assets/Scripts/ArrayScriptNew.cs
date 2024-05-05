using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

public class ArrayScriptNew : MonoBehaviour
{
    public ARPlaneManager arPlaneManager;
    public TextMeshProUGUI infotext;
    public GameObject cubePrefab;
    public TMP_InputField sizeInputField;
    public TMP_InputField entriesInputField; 
    public GameObject inputCanvas;

   // public Camera mainCamera;
    public float spacing = 2f;
    //public GameObject insertionCanvas;
    //public GameObject deletionCanvas;
    public TMP_InputField indexInputField;
    public TMP_InputField valueInputField;
    public TMP_InputField deleteIndexInputField;

    private List<GameObject> cubes = new List<GameObject>();

 private void Start()
    {
      
    }
    public void OnSubmitButtonClick()
    {

        // Start a coroutine to wait for plane detection
        StartCoroutine(WaitForPlaneDetection());
    }
    private IEnumerator WaitForPlaneDetection()
    {

        //infotext.text = "Don't move the phone.Waiting for plane detection";
        float elapsedTime = 0f;
        float maxWaitTime = 60f; // Maximum wait time in seconds (1 minute)

        while (elapsedTime < maxWaitTime)
        {
             GenerateCubesOnPlane();
            // Check if any planes are detected
            /*
            foreach (var trackable in arPlaneManager.trackables)
            {
                if (trackable is ARPlane arPlane)
                {
                    infotext.text = "plane detected";
                    Debug.LogError("No AR planes detected .");
                    yield return new WaitForSeconds(2f);
                    // Plane detected, generate cubes on this plane
                    GenerateCubesOnPlane(arPlane);
                    yield break; // Exit the coroutine
                }
            }
            */
            // No planes detected yet, wait for a short duration and check again
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
        }

        // No plane detected within the time limit, display error message
        //Debug.LogError("No AR planes detected within the time limit.");
        //infotext.text = "No AR plane detected within 1 minute.";
    }
        public void GenerateCubesOnPlane()
    {
        string[] entries = entriesInputField.text.Split(','); // Split the input string by commas

        // Check if the number of entries matches the size of the array
        int arraySize = int.Parse(sizeInputField.text);
        if (arraySize <= 0)
        {
            Debug.LogError("Array size must be a positive integer.");
           
        }
        DestroyCubes();
        Vector3 planePosition = new Vector3(-0.2f, -0.5f, 0f);
        float startX = -0.5f;
        float currentX = startX;
        
        for (int i = 0; i < arraySize; i++)
        {
            // Use the current position for each cube
            Vector3 cubePosition = new Vector3(planePosition.x+currentX,planePosition.y+0.5f, planePosition.z+1f);

            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
            cubes.Add(cube);

            // Update currentX for the next cube
            currentX += spacing;

            // Access the TextMeshPro component inside the canvas of the cube prefab and update its text
            if (i < entries.Length)
        {
            // Parse the entry to check if it's a valid number
            int value;
            if (int.TryParse(entries[i].Trim(), out value))
            {
                // Access the TextMeshPro component inside the canvas of the cube prefab and update its text
                TextMeshProUGUI textMesh = cube.GetComponentInChildren<TextMeshProUGUI>();
                if (textMesh != null)
                {
                    textMesh.text = value.ToString(); // Assign the entry value to the cube
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI component not found in the children of the cube prefab.");
                }
            }
            else
            {
                Debug.LogError("Invalid entry at index " + i);
            }
        }
        else
        {
            // If no entry is provided for this cube, display "N"
            TextMeshProUGUI textMesh = cube.GetComponentInChildren<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = "N";
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in the children of the cube prefab.");
            }
        }
        }
       
    }
    // public void CreateArray()
    // {
    //     string[] entries = entriesInputField.text.Split(','); // Split the input string by commas

    //     // Check if the number of entries matches the size of the array
    //     int arraySize = entries.Length;
    //     if (arraySize <= 0)
    //     {
    //         Debug.LogError("Array size must be a positive integer.");
    //         return;
    //     }

    //     // Clean up previously generated cubes
    //     DestroyCubes();

    //     // Calculate total width
    //     float totalWidth = (arraySize - 1) * spacing;

    //     // Calculate starting position
    //     float startX = -totalWidth / 2f;

    //     // Initialize currentX to starting position
    //     float currentX = startX;

    //     for (int i = 0; i < arraySize; i++)
    //     {
    //         // Use the current position for each cube
    //         Vector3 cubePosition = new Vector3(currentX, 0f, 0f);

    //         GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
    //         cubes.Add(cube);

    //         // Update currentX for the next cube
    //         currentX += spacing;

    //         // Access the TextMeshPro component inside the canvas of the cube prefab and update its text
    //         TextMeshProUGUI textMesh = cube.GetComponentInChildren<TextMeshProUGUI>();
    //         if (textMesh != null)
    //         {
    //             textMesh.text = entries[i].Trim(); // Assign the entry value to the cube
    //         }
    //         else
    //         {
    //             Debug.LogError("TextMeshProUGUI component not found in the children of the cube prefab.");
    //         }
    //     }
    //     PositionInsertDeleteCanvas();

    //     // Hide input canvas
    //     //inputCanvas.SetActive(false);

    //     // Hide insertion canvas
    //     //insertionCanvas.SetActive(false);

    //     // Focus camera on generated cubes
    //    // FocusCameraOnCubes();
    // }
    

    public void DestroyCubes()
    {
        foreach (GameObject cube in cubes)
        {
            Destroy(cube);
        }
        cubes.Clear();
    }

   //private void FocusCameraOnCubes()
    //{
      //  if (cubes.Count > 0 && mainCamera != null)
       // {
        //    // Calculate the bounds of all cubes
         //   Bounds bounds = new Bounds(cubes[0].transform.position, Vector3.zero);
          //  foreach (GameObject cube in cubes)
            //{
             //   bounds.Encapsulate(cube.GetComponent<Renderer>().bounds);
            //}

            // Calculate the camera distance based on the bounds size
            //float cameraDistance = bounds.size.magnitude / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            // Set camera position and rotation
            //mainCamera.transform.position = bounds.center - mainCamera.transform.forward * cameraDistance;
            //mainCamera.transform.LookAt(bounds.center);
        //}
    //} 
    

    

   public void InsertValueAtIndex()
{
    StartCoroutine(DelayFn());
}

IEnumerator DelayFn()
{
    int index = int.Parse(indexInputField.text);
    int value = int.Parse(valueInputField.text);

    if (index < 0 || index > cubes.Count)
    {
        Debug.LogError("Invalid index for insertion.");
        //return;
    }

    GameObject cubeAtIndex = cubes[index];
    TextMeshProUGUI textMesh = cubeAtIndex.GetComponentInChildren<TextMeshProUGUI>();
    string currentValue = textMesh.text;

    // If the current value is "N", replace it with the new value
    if (currentValue == "N")
    {
        textMesh.text = value.ToString();
    }

    // Otherwise, insert a new cube with the new value
    else
    {
        // Instantiate a new cube below the cube at the given index
        Vector3 newPosition = cubes[index].transform.position - new Vector3(0f, cubePrefab.transform.localScale.y, 0f);
        GameObject newCube = Instantiate(cubePrefab, newPosition, Quaternion.identity);

        // Set the value of the new cube
        TextMeshProUGUI newTextMesh = newCube.GetComponentInChildren<TextMeshProUGUI>();
        if (newTextMesh != null)
        {
            newTextMesh.text = value.ToString();
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in the children of the cube prefab.");
        }

        yield return new WaitForSeconds(1f);

        // Shift existing cubes to the right starting from the specified index position
        for (int i = index; i < cubes.Count; i++)
        { 
            cubes[i].transform.position += new Vector3(spacing, 0f, 0f);
        }
        yield return new WaitForSeconds(1f);

        // Insert the new cube into the list at the specified index
        cubes.Insert(index, newCube);

        // Move the new cube up by the height of the cube
        newCube.transform.position += new Vector3(0f, cubePrefab.transform.localScale.y, 0f);
        yield return new WaitForSeconds(1f);

        // Destroy the last cube at the rightmost position
        Destroy(cubes[cubes.Count - 1]);
        cubes.RemoveAt(cubes.Count - 1);
    }
}
public void DeleteValueAtIndex()
{
   
    int index = int.Parse(deleteIndexInputField.text);

    if (index < 0 || index >= cubes.Count)
    {
        Debug.LogError("Invalid index for deletion.");
        return;
    }

    // Shift values to the left starting from the index position
    for (int i = index; i < cubes.Count - 1; i++)
    {
        // Update the value of the cube at the current index with the value of the next cube
        GameObject currentCube = cubes[i];
        GameObject nextCube = cubes[i + 1];
        TextMeshProUGUI currentTextMesh = currentCube.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI nextTextMesh = nextCube.GetComponentInChildren<TextMeshProUGUI>();

        if (currentTextMesh != null && nextTextMesh != null)
        {
            currentTextMesh.text = nextTextMesh.text;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in the children of the cube prefab.");
        }
    }

    // Update the value of the last cube to "N"
    GameObject lastCube = cubes[cubes.Count - 1];
    TextMeshProUGUI lastTextMesh = lastCube.GetComponentInChildren<TextMeshProUGUI>();
    if (lastTextMesh != null)
    {
        lastTextMesh.text = "N";
    }
    else
    {
        Debug.LogError("TextMeshProUGUI component not found in the children of the cube prefab.");
    }

}
public void DestroyAllObjects()
{
    // Iterate through the list of cubes and destroy each GameObject
    foreach (GameObject cube in cubes)
    {
        Destroy(cube);
    }

    // Clear the list of cubes
    cubes.Clear();
}






}
