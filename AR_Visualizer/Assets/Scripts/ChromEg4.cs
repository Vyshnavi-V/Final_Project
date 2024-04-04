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
    public TextMeshProUGUI textComponent; // Reference to the TMP Text component
    public float delayBetweenChanges = 1.0f; // Delay between color changes

    IEnumerator Start()
    {
        // Change colors sequentially with delay
        yield return StartCoroutine(ChangeToColor(sphere19, Color.red));
        yield return new WaitForSeconds(delayBetweenChanges);

        // Change color of sphere 2 to red and adjust text content
        yield return StartCoroutine(ChangeToColor(sphere23, Color.red));
        UpdateText("No");
        SetTextProperties(sphere23.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        // Change color of sphere 2 to green and adjust text content
        yield return StartCoroutine(ChangeToColor(sphere23, Color.green));
        UpdateText("Yes");
        SetTextProperties(sphere23.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere24, Color.green));
        UpdateText("No");
        SetTextProperties(sphere24.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere24, Color.red));
        UpdateText("Yes");
        SetTextProperties(sphere24.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere20, Color.green));
        UpdateText("No");
        SetTextProperties(sphere20.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere20, Color.red));
        UpdateText("No");
        SetTextProperties(sphere20.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere20, Color.blue));
        UpdateText("Yes");
        SetTextProperties(sphere20.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

         yield return StartCoroutine(ChangeToColor(sphere21, Color.red));
        UpdateText("No");
        SetTextProperties(sphere21.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere21, Color.blue));
        UpdateText("No");
        SetTextProperties(sphere21.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere21, Color.green));
        UpdateText("Yes");
        SetTextProperties(sphere21.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere22, Color.red));
        UpdateText("No");
        SetTextProperties(sphere22.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere22, Color.green));
        UpdateText("No");
        SetTextProperties(sphere22.transform.position, textComponent);
        yield return new WaitForSeconds(delayBetweenChanges);

        yield return StartCoroutine(ChangeToColor(sphere22, Color.blue));
        UpdateText("Yes");
        SetTextProperties(sphere22.transform.position, textComponent);
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
