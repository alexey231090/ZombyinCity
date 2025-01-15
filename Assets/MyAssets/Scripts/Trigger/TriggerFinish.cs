using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerFinish : MonoBehaviour
{
    public string levelName; // Имя уровня, на который нужно перейти

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, является ли объект, с которым произошло столкновение, игроком
        if (other.CompareTag("Player"))
        {
            LoadLevel(levelName);
        }
    }

   


    

    public void LoadLevel(string nameLevel)
    {
        StartCoroutine(LoadSceneAsync(nameLevel));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        
    }



}
