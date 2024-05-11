
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Dfs : MonoBehaviour
{
    public GameObject lineParent;
    public GameObject sphereParent;
    public GameObject orderCanvas;
    public GameObject cubePrefab; 
    public float cubeDelay = 0.5f; 
    public float squareDelayBetweenChanges = 1f; 
    public float sphereDelayBetweenChanges = 1.5f; 
    public float popDelay = 2.5f; 
    public TextMeshProUGUI actionText;
    private List<Transform> squares = new List<Transform>();
    private List<Transform> spheres = new List<Transform>();
    public TextMeshProUGUI orderText;
    private Stack<string> numberStack = new Stack<string>(); 
    private Dictionary<string, GameObject> cubeDictionary = new Dictionary<string, GameObject>(); 
    private float cubeSize; 
    private float gap = 0.01f; 
    private float currentY = -0.52f;
 

    void Start()
    {
        actionText.text ="";
        orderText.text ="Node Order:";
        foreach (Transform child in lineParent.transform)
        {
            squares.Add(child);
        }

        foreach (Transform child in sphereParent.transform)
        {
            spheres.Add(child);
        }

        orderText = orderCanvas.GetComponentInChildren<TextMeshProUGUI>();
        

        StartCoroutine(ChangeSquareColors());
        StartCoroutine(ChangeSphereColors());
        StartCoroutine(PushPopOperations());

        cubeSize = cubePrefab.GetComponent<Renderer>().bounds.size.x;
    }

    IEnumerator ChangeSquareColors()
    {
        foreach (Transform square in squares)
        {
            string squareName = square.name;
            if (squareName != "Line 10" && squareName != "Line 11" && squareName!="Line 9")
            {
                square.GetComponent<Renderer>().material.color = Color.blue; 
            }
            yield return new WaitForSeconds(squareDelayBetweenChanges);
        }

        StartCoroutine(PerformQueueOperations()); // Start queue operations after square color change
    }

    IEnumerator ChangeSphereColors()
    {
        foreach (Transform sphere in spheres)
        {
            string sphereName = sphere.name;
             if(sphereName != "Sphere 9"){
            sphere.GetComponent<Renderer>().material.color = Color.green; 
            
           
            TextMeshProUGUI sphereText = sphere.GetComponentInChildren<TextMeshProUGUI>();
            orderText.text += sphereText.text + " ";

            yield return new WaitForSeconds(sphereDelayBetweenChanges);
             }
        }
    }

    IEnumerator PerformQueueOperations()
    {
        yield return new WaitForSeconds(cubeDelay * spheres.Count);

        StartCoroutine(PushPopOperations()); // Start pushing and popping numbers
    }

    IEnumerator PushPopOperations()
    {
        PushNumber("0");
        actionText.text = "We start with node 0. Push it into the stack";
        yield return new WaitForSeconds(4f);

        
        actionText.text = "Push child of node 0--> node 1 into the stack";
        PushNumber("1");
        yield return new WaitForSeconds(4f);
        actionText.text = "Push child of node 1--> node 2 into the stack";
        PushNumber("2");
        yield return new WaitForSeconds(2f);

        foreach(Transform sphere in spheres){
             string sphereName = sphere.name;
             if(sphereName=="Sphere 2"){
                sphere.GetComponent<Renderer>().material.color = Color.black;
             }
        }
        PopNumber();
        actionText.text = "No child for node 2.Pop it.Backtrack to node 1";

        yield return new WaitForSeconds(2f);
        
        PushNumber("3");
        actionText.text = "Push child of node 1--> node 3 into stack";
        yield return new WaitForSeconds(4f);
        


        PushNumber("5");
        actionText.text = "Push child of node 3--> node 5 into the stack";

        yield return new WaitForSeconds(4f);


        PushNumber("6");
        actionText.text = "Push child of node 5--> node 6 into the stack";

        yield return new WaitForSeconds(2f);


        foreach(Transform sphere in spheres){
             string sphereName = sphere.name;
             if(sphereName=="Sphere 5"){
                sphere.GetComponent<Renderer>().material.color = Color.black;
             }
        }
        PopNumber();
        actionText.text = "No child for node 6.Pop it.Backtrack to node 5";

        yield return new WaitForSeconds(2f);
        
        PushNumber("7");
        actionText.text = "Push child of node 5--> node 7 into the stack";

        yield return new WaitForSeconds(4f);


        PushNumber("8");
                actionText.text = "Push child of node 7--> node 8 into the stack";

        yield return new WaitForSeconds(4f);

        PushNumber("9");
                actionText.text = "Push child of node 8--> node 9 into the stack";

        yield return new WaitForSeconds(2f);

        foreach(Transform sphere in spheres){
             string sphereName = sphere.name;
             if(sphereName=="Sphere 8"){
                sphere.GetComponent<Renderer>().material.color = Color.black;
             }
        }
                actionText.text = "No child for node 9.Pop it.Backtrack to node 8";

        PopNumber();
        yield return new WaitForSeconds(2f);

        foreach(Transform sphere in spheres){
             string sphereName = sphere.name;
             if(sphereName=="Sphere 7"){
                sphere.GetComponent<Renderer>().material.color = Color.black;
             }
        }
        PopNumber();
                actionText.text = "No child for node 8.Pop it.Backtrack to node 7";

        yield return new WaitForSeconds(2f);

        foreach(Transform sphere in spheres){
             string sphereName = sphere.name;
             if(sphereName=="Sphere 6"){
                sphere.GetComponent<Renderer>().material.color = Color.black;
             }
        }
        PopNumber();
                actionText.text = "No child for node 7.Pop it.Backtrack to node 5";

        yield return new WaitForSeconds(2f);

        foreach(Transform sphere in spheres){
             string sphereName = sphere.name;
             if(sphereName=="Sphere 4"){
                sphere.GetComponent<Renderer>().material.color = Color.black;
             }
        }
        PopNumber();
        actionText.text = "No child for node 5.Pop it.Backtrack to node 3";

        yield return new WaitForSeconds(2f);

        foreach(Transform sphere in spheres){
             string sphereName = sphere.name;
             if(sphereName=="Sphere 3"){
                sphere.GetComponent<Renderer>().material.color = Color.black;
             }
        }
        PopNumber();
                actionText.text = "No child for node 3.Pop it.Backtrack to node 1";

        yield return new WaitForSeconds(2f);

        foreach(Transform sphere in spheres){
            string sphereName = sphere.name;
             if(sphereName=="Sphere 9"){
                sphere.GetComponent<Renderer>().material.color = Color.green;
                TextMeshProUGUI sphereText = sphere.GetComponentInChildren<TextMeshProUGUI>();

                orderText.text += sphereText.text + " ";

             }
        }
         foreach(Transform line in squares){
            string sphereName = line.name;
             if(sphereName=="Line 9"){
                line.GetComponent<Renderer>().material.color = Color.blue;
             }
        }

        PushNumber("4");
                        actionText.text = "Push child of node 1--> node 4 into the stack";

        yield return new WaitForSeconds(2f);

    foreach(Transform sphere in spheres){
            string sphereName = sphere.name;
             if(sphereName=="Sphere 9"){
                sphere.GetComponent<Renderer>().material.color = Color.black;
             }
        }
        PopNumber();
         actionText.text = "No child for node 4.Pop it.Backtrack to node 1";

        yield return new WaitForSeconds(2f);
        foreach(Transform sphere in spheres){
            string sphereName = sphere.name;
             if(sphereName=="Sphere 1"){
                sphere.GetComponent<Renderer>().material.color = Color.black;
             }
        }
        PopNumber();
                        actionText.text = "No child for node 1.Pop it.Backtrack to node 0.";

        yield return new WaitForSeconds(2f);
        foreach(Transform sphere in spheres){
            string sphereName = sphere.name;
             if(sphereName=="Sphere 0"){
                sphere.GetComponent<Renderer>().material.color = Color.black;
             }
        }
        PopNumber();
            actionText.text = "No child for node 1.Pop it.No nodes to traverse.";

        yield return new WaitForSeconds(4f);
        actionText.text = "DFS order is given above";




        
        


    }

    void PushNumber(string number)
    {
        numberStack.Push(number); 
        GenerateCube(number); 
    }

    void PopNumber()
    {
        if (numberStack.Count > 0)
        {
            string poppedNumber = numberStack.Pop();
            Debug.Log("Number popped: " + poppedNumber);

            if (cubeDictionary.ContainsKey(poppedNumber))
            {
                GameObject poppedCube = cubeDictionary[poppedNumber];
                StartCoroutine(MoveAndDestroyCubeCoroutine(poppedCube)); // Use the correct coroutine name
                cubeDictionary.Remove(poppedNumber);
            }
            else
            {
                Debug.LogError("Cube GameObject not found for popped number: " + poppedNumber);
            }
        }
        else
        {
            Debug.Log("Stack is empty. Cannot pop.");
        }
    }

    private void GenerateCube(string number)
    {
        // Increment currentY for the next cube
        currentY += cubeSize + gap; // Adding a small gap between cubes

        // Calculate the final position of the cube
        Vector3 finalPosition = new Vector3(2.2f, currentY, 0.5f);

        // Start the coroutine to move the cube to its final position
        StartCoroutine(MoveCubeToPosition(cubePrefab, finalPosition, number));
    }

    private IEnumerator MoveCubeToPosition(GameObject cube, Vector3 finalPosition, string number)
    {
        // Instantiate the cube at an initial position far below
        Vector3 initialPosition = finalPosition + Vector3.up * 20f;
        GameObject newCube = Instantiate(cube, initialPosition, Quaternion.identity);

        // Set the number text of the cube
        newCube.GetComponentInChildren<TextMeshProUGUI>().text = number;

        // Add the cube GameObject to the dictionary
        cubeDictionary[number] = newCube;

        // Move the cube towards its final position gradually
        float duration = 1.2f; // Duration of the movement
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

    private IEnumerator MoveAndDestroyCubeCoroutine(GameObject cube)
    {
        Vector3 initialPosition = cube.transform.position;
        Vector3 finalPosition = initialPosition + Vector3.up * 20f;
        float duration = 1.2f; // Duration of the movement
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
        currentY -= cubeSize;
        currentY -= gap;
        // Destroy the cube
        Destroy(cube);
    }
}