using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

   // public GameObject MenuCanvas;
  //  public GameObject InstructionText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {

        Application.Quit();
    }

    public void HowToPlay()
    {
        //MenuCanvas.SetActive(false);
       // InstructionText.SetActive(true);
    }

    public void Back()
    {
        //MenuCanvas.SetActive(true);
        //InstructionText.SetActive(false);
    }
}
