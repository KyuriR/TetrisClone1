using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    public TMP_Text text;

    public float floatSpeed = 1.5f;
    public float lifetime = 1f;

    private float elapsed;
    private Color startColor;

    private void Awake()
    {
        if (text == null)
        {
            Debug.LogError("ScorePopup: 'Text' field is not assigned in the Inspector. Drag the TextMeshPro component from this prefab into this field.");
            return;
        }

        startColor = text.color;
    }

    public void Initialize(int points)
    {
        if (text == null)
        {
            return;
        }

        text.text = "+" + points + "!";
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (text != null)
        {
            float t = elapsed / lifetime;
            Color c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0f, t);
            text.color = c;
        }

        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}