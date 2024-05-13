using UnityEngine;
using TMPro;
using System.Collections;

public class ChromEg5 : MonoBehaviour
{
    public GameObject sphere19; // Reference to Sphere 1
    public GameObject sphere20; // Reference to Sphere 2
    public GameObject sphere21; // Reference to Sphere 3
    public GameObject sphere22; // Reference to Sphere 4
    public GameObject sphere23; // Reference to Sphere 4
    public GameObject sphere24; // Reference to Sphere 4
    public TextMeshProUGUI textComponent; // Reference to the TMP Text
    public TextMeshProUGUI infoText; // Reference to the TMP Text component
    public float delayBetweenChanges = 1.0f; // Delay between color changes

void Start(){
infoText.text="";
}
    public void ChromaticMethod(){
        StartChromatic();
    }
    IEnumerator StartChromatic()
    {

        
        infoText.text = "We first select the node with highest degree--> node 5(3) or node 6(3)";
        yield return new WaitForSeconds(delayBetweenChanges);


        infoText.text = "Select node 5";
        yield return new WaitForSeconds(delayBetweenChanges);



       yield return StartCoroutine(ChangeToColor(sphere23, Color.red));
        infoText.text = "Color node 5 with red";
        UpdateText("Red -->Yes");
        SetTextProperties(sphere23.transform.position, textComponent);

        yield return new WaitForSeconds(delayBetweenChanges);
         infoText.text = "Color all the neigbors of node 5";
          yield return new WaitForSeconds(delayBetweenChanges);

        // Change color of sphere 2 to red and adjust text content
        yield return StartCoroutine(ChangeToColor(sphere19, Color.red));
        UpdateText("Red-->No");
                infoText.text = "Red is not possible for node 2 as neigboring node-->node 5 is red in color";

        SetTextProperties(sphere19.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

         yield return StartCoroutine(ChangeToColor(sphere19, Color.green));
        UpdateText("Green-->Yes");
        infoText.text = "We color node 2 with green-->No neighboring nodes with green color.";

        SetTextProperties(sphere19.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere22, Color.red));
        UpdateText("Red-->No");
                infoText.text = "Red is not possible for node 1 as neigboring node-->node 5 is red in color";

        SetTextProperties(sphere22.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere22, Color.green));
        UpdateText("Green-->No");
                infoText.text = "Green is not possible for node 1 as neigboring node-->node 2 is green in color";

        SetTextProperties(sphere22.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

         yield return StartCoroutine(ChangeToColor(sphere22, Color.yellow));
        UpdateText("Yellow-->Yes");
        infoText.text = "We color node 1 with yellow-->No neighboring nodes with yellow color.";

        SetTextProperties(sphere24.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere24, Color.red));
        UpdateText("Red-->No");
                infoText.text = "Red is not possible for node 6 as neigboring node-->node 5 is red in color";

        SetTextProperties(sphere24.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        
         yield return StartCoroutine(ChangeToColor(sphere24, Color.green));
        UpdateText("Green-->Yes");
        infoText.text = "We color node 6 with green-->No neighboring nodes with green color.";

        SetTextProperties(sphere24.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        

         yield return StartCoroutine(ChangeToColor(sphere20,Color.red));
        UpdateText("Red-->No");
                infoText.text = "Red is not possible for node 3 as neigboring node-->node 5 is red in color";
SetTextProperties(sphere20.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);
         yield return StartCoroutine(ChangeToColor(sphere20, Color.green));
        UpdateText("Green-->No");
                infoText.text = "Green is not possible for node 3 as neigboring node-->node 2 is green in color";
        SetTextProperties(sphere20.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere20, Color.yellow));
        UpdateText("Yellow-->Yes");
        infoText.text = "We color node3  with yellow-->No neighboring nodes with yellow color.";

        SetTextProperties(sphere24.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);
yield return StartCoroutine(ChangeToColor(sphere21, Color.red));
        UpdateText("Red->Yes");
        infoText.text = "We color node 4 with red-->No neighboring nodes with red color.";

        SetTextProperties(sphere21.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);
                infoText.text = "Coloring finished";


        
    }

    IEnumerator ChangeToColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
        yield return null;
    }

    void UpdateText(string content)
    {
        if (textComponent != null)
        {
            textComponent.text = content;
        }
    }

    void SetTextProperties(Vector3 position, TextMeshProUGUI text)
{
    if (text != null)
    {
        Vector3 offset = new Vector3(0f, -5f, 0f); // Adjust the downward offset
        text.rectTransform.position = position + offset; // Adjust the position of the text
    }
}
public void exitMethod(){
    infoText.text="";
    StopAllCoroutines();
}

}
