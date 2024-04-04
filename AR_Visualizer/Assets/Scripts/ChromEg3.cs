using UnityEngine;
using TMPro;
using System.Collections;

public class ChromEg3 : MonoBehaviour
{
    public GameObject sphere8; // Reference to Sphere 1
    public GameObject sphere9; // Reference to Sphere 2
    public GameObject sphere10; // Reference to Sphere 3
    public GameObject sphere11; // Reference to Sphere 4
    public GameObject sphere12; // Reference to Sphere 4
    public TextMeshProUGUI textComponent; // Reference to the TMP Text component
    public float delayBetweenChanges = 1.0f; // Delay between color changes

    IEnumerator Start()
    {
        // Change colors sequentially with delay
        yield return StartCoroutine(ChangeToColor(sphere12, Color.red));
        yield return new WaitForSeconds(delayBetweenChanges);

        // Change color of sphere 2 to red and adjust text content
        yield return StartCoroutine(ChangeToColor(sphere9, Color.red));
        UpdateText("No");
        SetTextProperties(sphere9.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        // Change color of sphere 2 to green and adjust text content
        yield return StartCoroutine(ChangeToColor(sphere9, Color.green));
        UpdateText("Yes");
        SetTextProperties(sphere9.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere10, Color.red));
        UpdateText("No");
        SetTextProperties(sphere10.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere10, Color.green));
        UpdateText("Yes");
        SetTextProperties(sphere10.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere8, Color.green));
        UpdateText("No");
        SetTextProperties(sphere8.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere8, Color.red));
        UpdateText("Yes");
        SetTextProperties(sphere8.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

         yield return StartCoroutine(ChangeToColor(sphere11, Color.green));
        UpdateText("No");
        SetTextProperties(sphere11.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere11, Color.red));
        UpdateText("No");
        SetTextProperties(sphere11.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere11, Color.blue));
        UpdateText("Yes");
        SetTextProperties(sphere11.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);
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
