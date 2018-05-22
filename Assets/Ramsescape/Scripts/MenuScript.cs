using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public Canvas quitMenu;
    public Button startButton;
    public Button quitButton;

    void Start()

    {
        quitMenu = quitMenu.GetComponent<Canvas>();
        startButton = startButton.GetComponent<Button>();
        quitButton = quitButton.GetComponent<Button>();
        quitMenu.enabled = false;
    }

    public void QuitPressed()

    {
        quitMenu.enabled = true;
        startButton.interactable = false;
        quitButton.interactable = false;
    }

    public void NoPressed()

    {
        quitMenu.enabled = false;
        startButton.interactable = true;
        quitButton.interactable = true;
    }

    public void YesPressed()

    {
        Application.Quit();
    }

}
