using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelChanger : MonoBehaviour {

    public Animator animator;

    private string levelToLoad;


    public void FadeToGameOver()
    {
        FadeToLevel("RamsescapeGameOverScene");
    }

    public void FadeToMenu()
    {
        FadeToLevel("RamsescapeMenuScene");
    }

    public void FadeToVictory()
    {
        FadeToLevel("RamsescapeVictoryScene");
    }

    public void FadeToMainLevel()
    {
        FadeToLevel("Main");
    }

    public void FadeToLevel(string levelName)
    {
        levelToLoad = levelName;
        animator.SetTrigger("FadeOut"); 
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}