using UnityEngine;
using TMPro;
using System.Collections;

public class ChromEg2 : MonoBehaviour
{
    public GameObject sphere4; // Reference to Sphere 1
    public GameObject sphere5; // Reference to Sphere 2
    public GameObject sphere6; // Reference to Sphere 3
    public GameObject sphere7; // Reference to Sphere 4
    public TextMeshProUGUI textComponent; // Reference to the TMP Text component
    public float delayBetweenChanges = 1.0f; // Delay between color changes

    IEnumerator Start()
    {
        // Change colors sequentially with delay
        yield return StartCoroutine(ChangeToColor(sphere7, Color.red));
        yield return new WaitForSeconds(delayBetweenChanges);

        // Change color of sphere 2 to red and adjust text content
        yield return StartCoroutine(ChangeToColor(sphere6, Color.red));
        UpdateText("No");
        SetTextProperties(sphere6.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        // Change color of sphere 2 to green and adjust text content
        yield return StartCoroutine(ChangeToColor(sphere6, Color.green));
        UpdateText("Yes");
        SetTextProperties(sphere6.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere4, Color.red));
        UpdateText("No");
        SetTextProperties(sphere4.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);
        yield return StartCoroutine(ChangeToColor(sphere4, Color.green));
        UpdateText("Yes");
        SetTextProperties(sphere4.transform.position, textComponent);
        yield return StartCoroutine(ChangeToColor(sphere5, Color.green));
        UpdateText("No");
        SetTextProperties(sphere5.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);
        yield return StartCoroutine(ChangeToColor(sphere5, Color.red));
        UpdateText("Yes");
        SetTextProperties(sphere5.transform.position, textComponent);
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
