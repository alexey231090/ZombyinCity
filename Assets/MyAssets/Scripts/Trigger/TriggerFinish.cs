using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class TriggerFinish : MonoBehaviour
{
    public static readonly Subject<Unit> OnSaveData = new Subject<Unit>(); // Событие для сохранения данных
    

    public string levelName; // Имя уровня, на который нужно перейти
    
    

    private void OnTriggerEnter(Collider other)
    {
        OnSaveData.OnNext(Unit.Default); // Вызываем событие сохранения данных


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
