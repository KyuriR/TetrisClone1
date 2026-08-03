using UnityEngine;
using System.Collections;

public class LineFlashEffect : MonoBehaviour
{
    public Board board;
    public Sprite squareSprite;
    public Color flashColor = Color.white;
    public float flashDuration = 0.25f;

    private void OnEnable()
    {
        if (board != null)
        {
            board.OnRowCleared += HandleRowCleared;
        }
    }

    private void OnDisable()
    {
        if (board != null)
        {
            board.OnRowCleared -= HandleRowCleared;
        }
    }

    private void HandleRowCleared(int row)
    {
        RectInt bounds = board.Bounds;

        GameObject flash = new GameObject("LineFlash");
        flash.transform.position = new Vector3(bounds.xMin + bounds.width / 2f, row + 0.5f, 0f);
        flash.transform.localScale = new Vector3(bounds.width, 1f, 1f);

        SpriteRenderer sr = flash.AddComponent<SpriteRenderer>();
        sr.sprite = squareSprite;
        sr.color = flashColor;
        sr.sortingOrder = 5;

        StartCoroutine(FadeAndDestroy(sr, flash));
    }

    private IEnumerator FadeAndDestroy(SpriteRenderer sr, GameObject go)
    {
        float elapsed = 0f;
        Color start = sr.color;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashDuration;
            Color c = start;
            c.a = Mathf.Lerp(start.a, 0f, t);
            sr.color = c;
            yield return null;
        }

        Destroy(go);
    }
}
