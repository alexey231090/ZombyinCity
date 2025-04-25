using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonsScripts : MonoBehaviour
{

    public GameObject pausePanel;
    bool isPaused = false;

    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(sceneName);

            }
            else
            {
                SceneManager.LoadScene(sceneName); 
            
            }
            
        }
        else
        {
            Debug.LogError("Error--------------Scene name is null or empty!");
        }
    }
    public void TogglePause()
    {
        if (pausePanel == null)
        {
            Debug.LogError("Error ------------- Pause panel is not assigned!");
            return;
        }

        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }



}
