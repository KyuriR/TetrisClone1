using System.Collections;
using TMPro;
using UnityEngine;

public class LineClearPopup : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    public float displayTime = 1f;

    public void Show(int linesCleared)
    {
        switch (linesCleared)
        {
            case 1:
                popupText.text = "LINE CLEAR!";
                break;
            case 2:
                popupText.text = "DOUBLE!";
                break;
            case 3:
                popupText.text = "TRIPLE!";
                break;
            case 4:
                popupText.text = "TETRIS!";
                break;
        }

        StopAllCoroutines();
        StartCoroutine(HidePopup());
    }

    IEnumerator HidePopup()
    {
        popupText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        popupText.gameObject.SetActive(false);
    }
}