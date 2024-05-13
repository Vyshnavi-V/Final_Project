using UnityEngine;
using TMPro;
using System.Collections;

public class Chromatic : MonoBehaviour
{
    public GameObject sphere1; // Reference to Sphere 1
    public GameObject sphere2; // Reference to Sphere 2
    public GameObject sphere3; // Reference to Sphere 3
    public TextMeshProUGUI textComponent; // Reference to the TMP Text component
    public float delayBetweenChanges = 1.0f; // Delay between color changes
    public TextMeshProUGUI infoText;

void Start(){
    infoText.text="";
}
public void ChromaticMethod(){
        StartChromatic();
    }
    private IEnumerator StartChromatic()
    {
        // Change colors sequentially with delay
        infoText.text="";
        yield return StartCoroutine(ChangeToColor(sphere1, Color.red));
        infoText.text = "We color node 1 with color red";
        UpdateText("Red -->Yes");
        SetTextProperties(sphere1.transform.position, textComponent);

        yield return new WaitForSeconds(delayBetweenChanges);

        // Change color of sphere 2 to red and adjust text content
        yield return StartCoroutine(ChangeToColor(sphere2, Color.red));
        SetTextProperties(sphere2.transform.position, textComponent);
        UpdateText("Red --> No");
        infoText.text = "Red is not possible for node 2 as neigboring node-->node 1 is red in color";


        yield return new WaitForSeconds(delayBetweenChanges);

        // Change color of sphere 2 to green and adjust text content
        yield return StartCoroutine(ChangeToColor(sphere2, Color.green));
        UpdateText("Green --> Yes");

        infoText.text = "We color node 2 with green-->No neighboring nodes with green color.";

        SetTextProperties(sphere2.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere3, Color.red));
        UpdateText("Red --> No");
        infoText.text = "Red is not possible for node 3 as neigboring node-->node 1 is red in color";
        SetTextProperties(sphere3.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere3, Color.green));
        UpdateText("Green --> No");
        infoText.text = "Green is not possible for node 3 as neigboring node-->node 2 is green in color";
        SetTextProperties(sphere3.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere3, Color.yellow));
        UpdateText("Yellow --> Yes");
        infoText.text = "We color node 3 with yellow-->No neighboring nodes with yellow color.";

        SetTextProperties(sphere3.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);


        infoText.text = "Coloring Finished";

    }

    IEnumerator ChangeToColor(GameObject obj, Color color)
    {
        yield return new WaitForSeconds(delayBetweenChanges);

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
        Vector3 offset = new Vector3(0f, -0.05f, 0f); // Adjust the downward offset
        text.rectTransform.position = position + offset; // Adjust the position of the text
    }
}
public void exitMethod(){
    infoText.text="";
    StopAllCoroutines();
}

}
