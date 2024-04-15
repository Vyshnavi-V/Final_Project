using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ArrayScriptNew : MonoBehaviour
{
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

    public void CreateArray()
    {
        string[] entries = entriesInputField.text.Split(','); // Split the input string by commas

        // Check if the number of entries matches the size of the array
        int arraySize = entries.Length;
        if (arraySize <= 0)
        {
            Debug.LogError("Array size must be a positive integer.");
            return;
        }

        // Clean up previously generated cubes
        DestroyCubes();

        // Calculate total width
        float totalWidth = (arraySize - 1) * spacing;

        // Calculate starting position
        float startX = -totalWidth / 2f;

        // Initialize currentX to starting position
        float currentX = startX;

        for (int i = 0; i < arraySize; i++)
        {
            // Use the current position for each cube
            Vector3 cubePosition = new Vector3(currentX, 0f, 0f);

            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
            cubes.Add(cube);

            // Update currentX for the next cube
            currentX += spacing;

            // Access the TextMeshPro component inside the canvas of the cube prefab and update its text
            TextMeshProUGUI textMesh = cube.GetComponentInChildren<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = entries[i].Trim(); // Assign the entry value to the cube
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in the children of the cube prefab.");
            }
        }

        // Hide input canvas
        //inputCanvas.SetActive(false);

        // Hide insertion canvas
        //insertionCanvas.SetActive(false);

        // Focus camera on generated cubes
       // FocusCameraOnCubes();
    }


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
// Instantiate a new cube below the cube at the given index
    Vector3 newPosition = cubes[index].transform.position - new Vector3(0f, cubePrefab.transform.localScale.y, 0f);
    GameObject newCube = Instantiate(cubePrefab, newPosition, Quaternion.identity);

    // Set the value of the new cube
    TextMeshProUGUI textMesh = newCube.GetComponentInChildren<TextMeshProUGUI>();
    if (textMesh != null)
    {
        textMesh.text = value.ToString();
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

    // Hide the insertion canvas
    //insertionCanvas.SetActive(false);
}
public void DeleteValueAtIndex()
{
    int index = int.Parse(deleteIndexInputField.text);

    if (index < 0 || index >= cubes.Count)
    {
        Debug.LogError("Invalid index for deletion.");
        return;
    }

    // Destroy the cube at the specified index
    Destroy(cubes[index]);
    
    // Remove the cube reference from the list
    cubes.RemoveAt(index);
    
    // Shift the remaining cubes to the left starting from the deleted index
    for (int i = index; i < cubes.Count; i++)
    {
        cubes[i].transform.position -= new Vector3(spacing, 0f, 0f); // Shift by the width of the cube
    }
    
    // Instantiate a new cube at the rightmost position if there are no more cubes
    if (cubes.Count == 0)
    {
        Vector3 newPosition = new Vector3(0f, 0f, 0f);
        GameObject newCube = Instantiate(cubePrefab, newPosition, Quaternion.identity);
        cubes.Add(newCube);

        // Set the value of the new cube
        TextMeshProUGUI textMesh = newCube.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.text = "0";
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in the children of the cube prefab.");
        }
    }

    // Hide the deletion canvas
    //deletionCanvas.SetActive(false);
}






}
