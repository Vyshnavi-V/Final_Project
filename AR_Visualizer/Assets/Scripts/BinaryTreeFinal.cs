// using System.Collections;
// using UnityEngine;
// using TMPro;
// using System.Collections.Generic;
// using UnityEngine.XR.ARFoundation;


// public class BinaryTreeFinal : MonoBehaviour
// {
//     public ARPlaneManager arPlaneManager;

//     public GameObject nodePrefab;
//     public float verticalSpacing = 2.0f;
//     public float initialHorizontalSpacing = 250.0f; // Initial horizontal spacing
//     public float horizontalSpacingDecrease = 100.0f; // Amount of horizontal spacing decrease per level
//     public TMP_InputField inputField;
//     public TMP_InputField insertionInputField; // Input field for insertion
//     public TMP_InputField deletionInputField; // Input field for deletion
//     public TextMeshProUGUI traversalText; // TextMeshPro element to display traversal order
//     public float delayBetweenNodes = 0.5f; // Delay between visualizing each node
//     private Color initialColor = Color.white; // Initial color of nodes and lines
//     public TextMeshProUGUI infotext;
//     public Canvas traversalCanvas;
//     public Canvas insdelCanvas;

// private float spacing = 5f;
//     private ARPlane trackPlane;
//      private void Start()
//     {
       
//     }
//     private class Node
//     {
//         public int value;
//         public Node left;
//         public Node right;
//         public GameObject visual;
//         public List<GameObject> lines = new List<GameObject>(); // Lines connecting to children
//         public int level; // Level of the node in the binary tree
//         public float position; // Position of the node within its level
//     }

//     private Node root;
//     private List<Node> nodes = new List<Node>();
//     private string traversalOrder = ""; // String to store traversal order
//     public void OnSubmitButtonClick()
//     {

//         // Start a coroutine to wait for plane detection
//         StartCoroutine(WaitForPlaneDetection());
//     }

//     private IEnumerator WaitForPlaneDetection()
//     {

//         infotext.text = "Don't move the phone.Waiting for plane detection";
//         float elapsedTime = 0f;
//         float maxWaitTime = 60f; // Maximum wait time in seconds (1 minute)

//         while (elapsedTime < maxWaitTime)
//         {
//             // Check if any planes are detected
//             foreach (var trackable in arPlaneManager.trackables)
//             {
//                 if (trackable is ARPlane arPlane)
//                 {
//                     infotext.text = "plane detected";
//                     Debug.LogError("No AR planes detected .");
//                     yield return new WaitForSeconds(2f);
//                     trackPlane = arPlane;
//                     AddNodes();
//                     Vector3 planePosition = trackPlane.transform.position;
//                      movePPRCanvas(insdelCanvas,planePosition);
//         moveBackCanvas(traversalCanvas,planePosition);
//                     // Plane detected, generate cubes on this plane
//                     yield break; // Exit the coroutine
//                 }
//             }

//             // No planes detected yet, wait for a short duration and check again
//             yield return new WaitForSeconds(0.5f);
//             elapsedTime += 0.5f;
//         }

//         // No plane detected within the time limit, display error message
//         Debug.LogError("No AR planes detected within the time limit.");
//         infotext.text = "No AR plane detected within 1 minute.";
//     }
//         public void AddNodes()
//     {
//         infotext.text = " Add Nodes";
//         string values = inputField.text;
//         string[] valueArray = values.Split(',');

//         StartCoroutine(AddNodesWithDelay(valueArray));
//     }

//     private IEnumerator AddNodesWithDelay(string[] valueArray)
//     {
//         infotext.text = " Add NodesDelay";
//         foreach (string value in valueArray)
//         {
//             int intValue;
//             if (int.TryParse(value, out intValue))
//             {
//                 AddNode(intValue);
//                 yield return new WaitForSeconds(delayBetweenNodes); // Delay before adding next node
//                 PositionNodes(); // Position nodes after adding each new node
//                 UpdateNodePositions(); // Update node positions after adding each new node
//                 DrawLines(); // Draw lines after adding each new node
//             }
//         }
//     }

//     public void InsertNode()
//     {
//         int valueToInsert;
//         if (int.TryParse(insertionInputField.text, out valueToInsert))
//         {
//             AddNode(valueToInsert);
//             PositionNodes();
//             UpdateNodePositions();
//             DrawLines();
//         }
//     }

//     public void DeleteNode()
//     {
//         int valueToDelete;
//         if (int.TryParse(deletionInputField.text, out valueToDelete))
//         {
//             Debug.Log("Deleting node with value: " + valueToDelete);
//             DeleteNodeWithValue(valueToDelete);
//             PositionNodes();
//             UpdateNodePositions();
//             DrawLines();
//         }
//     }

//     public void InorderTraversal()
//     {
//         traversalOrder = "";
//         StartCoroutine(DoInorderTraversal(root));
//     }

//     private IEnumerator DoInorderTraversal(Node node)
//     {
//         if (node != null)
//         {
//             yield return StartCoroutine(DoInorderTraversal(node.left));
//             yield return new WaitForSeconds(delayBetweenNodes);
//             VisitNode(node);
//             traversalOrder += (node.value + ", ");
//             yield return StartCoroutine(DoInorderTraversal(node.right));
//         }

//         // Update the traversal text after traversal is complete
//         UpdateTraversalText();
//     }

//     public void PreorderTraversal()
//     {
//         traversalOrder = "";
//         StartCoroutine(DoPreorderTraversal(root));
//     }

//     private IEnumerator DoPreorderTraversal(Node node)
//     {
//         if (node != null)
//         {
//             VisitNode(node);
//             traversalOrder += (node.value + ", ");
//             UpdateTraversalText();
//             yield return new WaitForSeconds(delayBetweenNodes);
//             yield return StartCoroutine(DoPreorderTraversal(node.left));
//             yield return StartCoroutine(DoPreorderTraversal(node.right));
//         }
//     }

//     public void PostorderTraversal()
//     {
//         traversalOrder = "";
//         StartCoroutine(DoPostorderTraversal(root));
//     }

//     private IEnumerator DoPostorderTraversal(Node node)
//     {
//         if (node != null)
//         {
//             yield return StartCoroutine(DoPostorderTraversal(node.left));
//             yield return StartCoroutine(DoPostorderTraversal(node.right));
//             yield return new WaitForSeconds(delayBetweenNodes);
//             VisitNode(node);
//             traversalOrder += (node.value + ", ");
//         }

//         // Update the traversal text after traversal is complete
//         UpdateTraversalText();
//     }

//     private void VisitNode(Node node)
//     {
//         // Change the color of spheres and lines to green
//         node.visual.GetComponent<Renderer>().material.color = Color.green;
//         foreach (GameObject line in node.lines)
//         {
//             line.GetComponent<LineRenderer>().material.color = Color.green;
//         }
//     }

//     private void UpdateTraversalText()
//     {
//         // Update the traversal text with the current traversal order
//         traversalText.text = "Traversal order: " + traversalOrder;
//     }

//     private void ClearTraversalText()
//     {
//         // Clear the traversal text
//         traversalText.text = "";
//     }

//     private void DeleteNodeWithValue(int value)
//     {
//         root = DeleteNodeFromTree(root, value);
//     }


//     private Node DeleteNodeFromTree(Node root, int value)
//     {
//         if (root == null)
//         {
//             Debug.Log("Node with value " + value + " not found.");
//             return root;
//         }

//         // Search for the node to be deleted
//         if (value < root.value)
//         {
//             root.left = DeleteNodeFromTree(root.left, value);
//         }
//         else if (value > root.value)
//         {
//             root.right = DeleteNodeFromTree(root.right, value);
//         }
//         else
//         {
//             // Node found, perform deletion
//             if (root.left == null)
//             {
//                 // Node has no left child or only right child
//                 DestroyNode(root);
//                 return root.right;
//             }
//             else if (root.right == null)
//             {
//                 // Node has no right child or only left child
//                 DestroyNode(root);
//                 return root.left;
//             }
//             else
//             {
//                 // Node has two children
//                 // Find in-order successor (smallest node in right subtree)
//                 Node successorParent = root;
//                 Node successor = root.right;
//                 while (successor.left != null)
//                 {
//                     successorParent = successor;
//                     successor = successor.left;
//                 }
//                 // Replace node value with successor value
//                 root.value = successor.value;
//                 // Update the text displayed on the visual object with the new value
//                 root.visual.GetComponentInChildren<TextMeshProUGUI>().text = successor.value.ToString();
//                 // Delete the successor node
//                 if (successorParent == root)
//                 {
//                     // If successor is immediate right child of root
//                     root.right = successor.right;
//                 }
//                 else
//                 {
//                     // If successor is not immediate right child of root
//                     successorParent.left = successor.right;
//                 }
//                 // Destroy the successor node
//                 DestroyNode(successor);
//             }
//         }
//         return root;
//     }

//     private void DestroyNode(Node node)
//     {
//         Destroy(node.visual);
//         nodes.Remove(node);
//         foreach (GameObject line in node.lines)
//         {
//             Destroy(line);
//         }
//     }

//     private Node FindMin(Node node)
//     {
//         while (node.left != null)
//         {
//             node = node.left;
//         }
//         return node;
//     }

//     public void AddNode(int value)
//     {
//         infotext.text = " Add Node";
//         Node newNode = new Node { value = value };
//         if (root == null)
//         {
//             root = newNode;
//         }
//         else
//         {
//             AddNodeToTree(root, newNode);
//         }
//         nodes.Add(newNode);
//     }

//     private void AddNodeToTree(Node current, Node newNode)
//     {
//         infotext.text = " Add NodestoTree";
//         if (newNode.value < current.value)
//         {
//             if (current.left == null)
//             {
//                 current.left = newNode;
//             }
//             else
//             {
//                 AddNodeToTree(current.left, newNode);
//             }
//         }
//         else
//         {
//             if (current.right == null)
//             {
//                 current.right = newNode;
//             }
//             else
//             {
//                 AddNodeToTree(current.right, newNode);
//             }
//         }
//     }

//     private void PositionNodes()
//     {
//         if (root != null)
//         {
//             AssignLevels(root, 0);
//             AssignPositions(root, 0, 0);
//         }
//     }

//     private void AssignLevels(Node node, int level)
//     {
//         if (node != null)
//         {
//             node.level = level;
//             AssignLevels(node.left, level + 1);
//             AssignLevels(node.right, level + 1);
//         }
//     }

// //     private void AssignPositions(Node node, int level, float position)
// // {
// //     infotext.text = "node created";
// //     if (node != null)
// //     {
// //         if (node == root) // If the node is the root
// //         {
// //             // Set the root node position above the detected plane
// //             node.position = trackPlane.transform.position.y + verticalSpacing;
// //         }
// //         else
// //         {
// //             node.position = position;

// //             // Calculate new horizontal spacing based on the level
// //             float horizontalSpacing = initialHorizontalSpacing - horizontalSpacingDecrease * level;

// //             AssignPositions(node.left, level + 1, position - horizontalSpacing); // Left child
// //             AssignPositions(node.right, level + 1, position + horizontalSpacing); // Right child
// //         }
// //     }
// // }
// private void AssignPositions(Node node, int level, float position)
// {
//     if (node != null)
//     {
//         // Check if it's the root node
//         if (level == 0)
//         {
//             // Position the root node at the plane's position
//             node.position = 0f; // Or set it to the desired x-position of the plane
//         }
//         else
//         {
//             // Calculate new horizontal spacing based on the level
//             float horizontalSpacing = initialHorizontalSpacing - horizontalSpacingDecrease * level;

//             // Assign positions for left and right children based on root's position
//             if (node.left != null)
//             {
//                 AssignPositions(node.left, level + 1, position - horizontalSpacing);
//             }
//             if (node.right != null)
//             {
//                 AssignPositions(node.right, level + 1, position + horizontalSpacing);
//             }
//         }
//     }
// }


//     private void UpdateNodePositions()
//     {
//         foreach (Node node in nodes)
//         {
//             if (node.visual == null)
//             {
//                 node.visual = Instantiate(nodePrefab, Vector3.zero, Quaternion.identity);
//                 node.visual.GetComponentInChildren<TextMeshProUGUI>().text = node.value.ToString();
//             }

//             // Calculate horizontal position based on position within the level
//             float posX = node.position;

//             // Calculate vertical position based on level
//             float posY = -node.level * verticalSpacing;

//             // Position the visual representation of the node
//             node.visual.transform.position = new Vector3(posX, posY, 0);
//         }
//     }
// private void DrawLines()
// {
//     // Destroy existing lines
//     foreach (Node node in nodes)
//     {
//         foreach (GameObject line in node.lines)
//         {
//             Destroy(line);
//         }
//         node.lines.Clear();
//     }

//     // Draw new lines
//     foreach (Node node in nodes)
//     {
//         if (node == root) // If the current node is the root
//         {
//             if (node.left != null)
//             {
//                 DrawLine(node.visual.transform.position, node.left.visual.transform.position, node.lines, Color.red); // Draw line to left child in red
//             }
//             if (node.right != null)
//             {
//                 DrawLine(node.visual.transform.position, node.right.visual.transform.position, node.lines, Color.blue); // Draw line to right child in blue
//             }
//         }
//         else
//         {
//             if (node.left != null)
//             {
//                 Color lineColor = (node.value < root.value) ? Color.red : Color.blue; // Determine line color based on node value compared to root value
//                 DrawLine(node.visual.transform.position, node.left.visual.transform.position, node.lines, lineColor);
//             }
//             if (node.right != null)
//             {
//                 Color lineColor = (node.value < root.value) ? Color.red : Color.blue; // Determine line color based on node value compared to root value
//                 DrawLine(node.visual.transform.position, node.right.visual.transform.position, node.lines, lineColor);
//             }
//         }
//     }
// }



//     private void DrawLine(Vector3 start, Vector3 end, List<GameObject> lines, Color color)
// {
//     GameObject lineObject = new GameObject("Line");
//     LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
//     lineRenderer.startWidth = 0.02f;
//     lineRenderer.endWidth = 0.02f;
//     lineRenderer.positionCount = 2;
//     lineRenderer.SetPosition(0, start);
//     lineRenderer.SetPosition(1, end);
//     lineRenderer.material.color = color; // Set line color
//     lines.Add(lineObject);
// }

//     public void ResetColorsAndText()
//     {
//         // Reset node colors
//         foreach (Node node in nodes)
//         {
//             node.visual.GetComponent<Renderer>().material.color = initialColor;
//             foreach (GameObject line in node.lines)
//             {
//                 line.GetComponent<LineRenderer>().material.color = initialColor;
//             }
//         }

//         // Clear traversal text
//        traversalText.text = "";
//     }
//     private void movePPRCanvas(Canvas canvas, Vector3 position)
// {
//     if (canvas == null)
//     {
//         Debug.LogError("Canvas parameter is null. Cannot move canvas.");
//         return;
//     }
//     float offsetX = -spacing * 0.5f; 
//     RectTransform canvasRect = canvas.GetComponent<RectTransform>();
//     if (canvasRect != null)
//     {
//         canvasRect.anchoredPosition3D = position + new Vector3(offsetX, 0f, 0f);
//     }
//     else
//     {
//         Debug.LogError("RectTransform component not found on the canvas. Cannot move canvas.");
//     }
// }
// private void moveBackCanvas(Canvas canvas, Vector3 position)
// {
//     if (canvas == null)
//     {
//         Debug.LogError("Canvas parameter is null. Cannot move canvas.");
//         return;
//     }
//     float offsetX = spacing * 0.5f; 
//     RectTransform canvasRect = canvas.GetComponent<RectTransform>();
//     if (canvasRect != null)
//     {
//         canvasRect.anchoredPosition3D = position + new Vector3(offsetX, 0f, 0f);
//     }
//     else
//     {
//         Debug.LogError("RectTransform component not found on the canvas. Cannot move canvas.");
//     }
// }


  
// }
using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BinaryTree : MonoBehaviour
{
    public GameObject nodePrefab;
    public float verticalSpacing = 2.0f;
    public float initialHorizontalSpacing = 250.0f; // Initial horizontal spacing
    public float horizontalSpacingDecrease = 100.0f; // Amount of horizontal spacing decrease per level
    public TMP_InputField inputField;
    public TMP_InputField insertionInputField; // Input field for insertion
    public TMP_InputField deletionInputField; // Input field for deletion
    public TextMeshProUGUI traversalText; // TextMeshPro element to display traversal order
    public float delayBetweenNodes = 0.5f; // Delay between visualizing each node
    private Color initialColor = Color.white; 
    public TextMeshProUGUI actionText;// Initial color of nodes and lines

    private class Node
    {
        public int value;
        public Node left;
        public Node right;
        public GameObject visual;
        public List<GameObject> lines = new List<GameObject>(); // Lines connecting to children
        public int level; // Level of the node in the binary tree
        public float position; // Position of the node within its level
    }

    private Node root;
    private List<Node> nodes = new List<Node>();
    private string traversalOrder = ""; // String to store traversal order

    public void AddNodes()
    {
        
        traversalText.text="";
        string values = inputField.text;
        string[] valueArray = values.Split(',');
        actionText.text="Value Greater than a node-->Right";
        traversalText.text="Value Lesser than a node-->Left";
        StartCoroutine(AddNodesWithDelay(valueArray));
    }
    private void Start()
    {
       
    }
    private IEnumerator AddNodesWithDelay(string[] valueArray)
    {
        yield return new WaitForSeconds(delayBetweenNodes*3);
        foreach (string value in valueArray)
        {
            int intValue;
            if (int.TryParse(value, out intValue))
            {
                AddNode(intValue);
                yield return new WaitForSeconds(delayBetweenNodes); // Delay before adding next node
                PositionNodes(); // Position nodes after adding each new node
                UpdateNodePositions(); // Update node positions after adding each new node
                DrawLines(); // Draw lines after adding each new node
            }
        }
    }

    // public void InsertNode()
    // {
    //     int valueToInsert;
    //     if (int.TryParse(insertionInputField.text, out valueToInsert))
    //     {
    //         AddNode(valueToInsert);
    //         PositionNodes();
    //         UpdateNodePositions();
    //         DrawLines();
    //     }
    // }

    // public void DeleteNode()
    // {
    //     int valueToDelete;
    //     if (int.TryParse(deletionInputField.text, out valueToDelete))
    //     {
    //         Debug.Log("Deleting node with value: " + valueToDelete);
    //         DeleteNodeWithValue(valueToDelete);
    //         PositionNodes();
    //         UpdateNodePositions();
    //         DrawLines();
    //     }
    // }

    public void InsertNode()
    {
        actionText.text="";
        traversalText.text="";
        StartCoroutine(InsertNodeWithDelay());
    }

    public void DeleteNode()
    {
        StartCoroutine(DeleteNodeWithDelay());
    }

    IEnumerator InsertNodeWithDelay()
    {
        yield return new WaitForSeconds(1f); // Adjust the delay time as needed (1 second delay in this example)

        int valueToInsert;
        if (int.TryParse(insertionInputField.text, out valueToInsert))
        {
            AddNode(valueToInsert);
            PositionNodes();
            UpdateNodePositions();
            DrawLines();
        }
    }

    IEnumerator DeleteNodeWithDelay()
    {
        yield return new WaitForSeconds(1f); // Adjust the delay time as needed (1 second delay in this example)

        int valueToDelete;
        if (int.TryParse(deletionInputField.text, out valueToDelete))
        {
            Debug.Log("Deleting node with value: " + valueToDelete);
            actionText.text="";
        traversalText.text="";
        actionText.text ="Deleted node is replaced with it's inorder succesor";
            DeleteNodeWithValue(valueToDelete);
            PositionNodes();
            UpdateNodePositions();
            DrawLines();
        }
    }

    public void InorderTraversal()
    {
        traversalOrder = "";
        StartCoroutine(DoInorderTraversal(root));
    }

    private IEnumerator DoInorderTraversal(Node node)
    {
        actionText.text ="Left --> Root--> Right ";
         yield return new WaitForSeconds(delayBetweenNodes*3);
        if (node != null)
        {
            yield return StartCoroutine(DoInorderTraversal(node.left));
            yield return new WaitForSeconds(delayBetweenNodes);
            VisitNode(node);
            traversalOrder += (node.value + ", ");
            yield return StartCoroutine(DoInorderTraversal(node.right));
        }

        // Update the traversal text after traversal is complete
        UpdateTraversalText();
    }

    public void PreorderTraversal()
    {
        traversalOrder = "";
        StartCoroutine(DoPreorderTraversal(root));
    }

    private IEnumerator DoPreorderTraversal(Node node)
    {
        actionText.text ="Root-->Left --> Right ";
         yield return new WaitForSeconds(delayBetweenNodes*3);
        if (node != null)
        {
            VisitNode(node);
            traversalOrder += (node.value + ", ");
            UpdateTraversalText();
            yield return new WaitForSeconds(delayBetweenNodes);
            yield return StartCoroutine(DoPreorderTraversal(node.left));
            yield return StartCoroutine(DoPreorderTraversal(node.right));
        }
    }

    public void PostorderTraversal()
    {
        traversalOrder = "";
        StartCoroutine(DoPostorderTraversal(root));
    }

    private IEnumerator DoPostorderTraversal(Node node)
    {
        actionText.text ="Left --> Right --> Root";
        yield return new WaitForSeconds(delayBetweenNodes*3);
        if (node != null)
        {
            yield return StartCoroutine(DoPostorderTraversal(node.left));
            yield return StartCoroutine(DoPostorderTraversal(node.right));
            yield return new WaitForSeconds(delayBetweenNodes);
            VisitNode(node);
            traversalOrder += (node.value + ", ");
        }

        // Update the traversal text after traversal is complete
        UpdateTraversalText();
    }

    private void VisitNode(Node node)
    {
        // Change the color of spheres and lines to green
        node.visual.GetComponent<Renderer>().material.color = Color.green;
        /*
        foreach (GameObject line in node.lines)
        {
            line.GetComponent<LineRenderer>().material.color = Color.green;
        }
        */
    }

    private void UpdateTraversalText()
    {
        // Update the traversal text with the current traversal order
        traversalText.text = "Traversal order: " + traversalOrder;
    }

    private void ClearTraversalText()
    {
        // Clear the traversal text
        traversalText.text = "";
    }

    private void DeleteNodeWithValue(int value)
    {
        root = DeleteNodeFromTree(root, value);
    }


    private Node DeleteNodeFromTree(Node root, int value)
    {
        if (root == null)
        {
            Debug.Log("Node with value " + value + " not found.");
            return root;
        }

        // Search for the node to be deleted
        if (value < root.value)
        {
            root.left = DeleteNodeFromTree(root.left, value);
        }
        else if (value > root.value)
        {
            root.right = DeleteNodeFromTree(root.right, value);
        }
        else
        {
            // Node found, perform deletion
            if (root.left == null)
            {
                // Node has no left child or only right child
                DestroyNode(root);
                return root.right;
            }
            else if (root.right == null)
            {
                // Node has no right child or only left child
                DestroyNode(root);
                return root.left;
            }
            else
            {
                // Node has two children
                // Find in-order successor (smallest node in right subtree)
                Node successorParent = root;
                Node successor = root.right;
                while (successor.left != null)
                {
                    successorParent = successor;
                    successor = successor.left;
                }
                // Replace node value with successor value
                root.value = successor.value;
                // Update the text displayed on the visual object with the new value
                root.visual.GetComponentInChildren<TextMeshProUGUI>().text = successor.value.ToString();
                // Delete the successor node
                if (successorParent == root)
                {
                    // If successor is immediate right child of root
                    root.right = successor.right;
                }
                else
                {
                    // If successor is not immediate right child of root
                    successorParent.left = successor.right;
                }
                // Destroy the successor node
                DestroyNode(successor);
            }
        }
        return root;
    }

    private void DestroyNode(Node node)
    {
        Destroy(node.visual);
        nodes.Remove(node);
        foreach (GameObject line in node.lines)
        {
            Destroy(line);
        }
    }

    private Node FindMin(Node node)
    {
        while (node.left != null)
        {
            node = node.left;
        }
        return node;
    }

    public void AddNode(int value)
    {
        Node newNode = new Node { value = value };
        if (root == null)
        {
            root = newNode;
        }
        else
        {
            AddNodeToTree(root, newNode);
        }
        nodes.Add(newNode);
    }

    private void AddNodeToTree(Node current, Node newNode)
    {
        if (newNode.value < current.value)
        {
            if (current.left == null)
            {
                current.left = newNode;
            }
            else
            {
                AddNodeToTree(current.left, newNode);
            }
        }
        else
        {
            if (current.right == null)
            {
                current.right = newNode;
            }
            else
            {
                AddNodeToTree(current.right, newNode);
            }
        }
    }

    private void PositionNodes()
    {
        if (root != null)
        {
            AssignLevels(root, 0);
            AssignPositions(root, 0, 0);
        }
    }

    private void AssignLevels(Node node, int level)
    {
        if (node != null)
        {
            node.level = level;
            AssignLevels(node.left, level + 1);
            AssignLevels(node.right, level + 1);
        }
    }

    private void AssignPositions(Node node, int level, float position)
    {
        if (node != null)
        {
            node.position = position;

            // Calculate new horizontal spacing based on the level
            float horizontalSpacing = initialHorizontalSpacing - horizontalSpacingDecrease * level;

            AssignPositions(node.left, level + 1, position - horizontalSpacing); // Left child
            AssignPositions(node.right, level + 1, position + horizontalSpacing); // Right child
        }
    }

    private void UpdateNodePositions()
    {
        foreach (Node node in nodes)
        {
            if (node.visual == null)
            {
                
                node.visual = Instantiate(nodePrefab, Vector3.zero, Quaternion.identity);
                node.visual.GetComponentInChildren<TextMeshProUGUI>().text = node.value.ToString();
            }

            // Calculate horizontal position based on position within the level
            float posX = node.position;

            // Calculate vertical position based on level
            float posY = -node.level * verticalSpacing;

            // Position the visual representation of the node
            node.visual.transform.position = new Vector3(posX, posY, 0);
        }
    }

    private void DrawLines()
    {
        // Destroy existing lines
        foreach (Node node in nodes)
        {
            foreach (GameObject line in node.lines)
            {
                Destroy(line);
            }
            node.lines.Clear();
        }

        // Draw new lines
        foreach (Node node in nodes)
        {
            if (node.left != null)
            {
                DrawLine(node.visual.transform.position, node.left.visual.transform.position, node.lines);
            }
            if (node.right != null)
            {
                DrawLine(node.visual.transform.position, node.right.visual.transform.position, node.lines);
            }
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, List<GameObject> lines)
    {
        GameObject lineObject = new GameObject("Line");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lines.Add(lineObject);
    }

    public void ResetColorsAndText()
    {
        // Reset node colors
        foreach (Node node in nodes)
        {
            node.visual.GetComponent<Renderer>().material.color = initialColor;
            /*
            foreach (GameObject line in node.lines)
            {
                line.GetComponent<LineRenderer>().material.color = initialColor;
            }
            */
        }

        // Clear traversal text
       traversalText.text = "";
    }
    public void DestroyObjects()
    {
        // Destroy node visuals and lines
    foreach (Node node in nodes)
    {
        if (node.visual != null)
        {
            Destroy(node.visual);
        }
        foreach (GameObject line in node.lines)
        {
            Destroy(line);
        }
    }
    actionText.text="";
    traversalText.text="";

    // Clear the list of nodes
    nodes.Clear();

    // Reset root node
    root = null;
    }
   


  
}