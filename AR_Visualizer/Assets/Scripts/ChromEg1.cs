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

    IEnumerator Start()
    {
        // Change colors sequentially with delay
        yield return StartCoroutine(ChangeToColor(sphere1, Color.red));
        yield return new WaitForSeconds(delayBetweenChanges);

        // Change color of sphere 2 to red and adjust text content
        yield return StartCoroutine(ChangeToColor(sphere2, Color.red));
        SetTextProperties(sphere2.transform.position, textComponent);
        UpdateText("X");
        yield return new WaitForSeconds(delayBetweenChanges);

        // Change color of sphere 2 to green and adjust text content
        yield return StartCoroutine(ChangeToColor(sphere2, Color.green));
        SetTextProperties(sphere2.transform.position, textComponent);
        UpdateText("√");
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere3, Color.red));
        UpdateText("X");
        SetTextProperties(sphere3.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);
        yield return StartCoroutine(ChangeToColor(sphere3, Color.blue));
        UpdateText("√");
        SetTextProperties(sphere3.transform.position, textComponent);
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

}
