using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScen : MonoBehaviour
{
    private void Awake()
    {
        // Устанавливаем объект как "не уничтожаемый при загрузке новой сцены"
        DontDestroyOnLoad(this.gameObject);

        // Получаем индекс текущей сцены
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Выводим индекс текущей сцены в консоль
        Debug.Log($"-------------------Объект {gameObject.name} создан. Текущая сцена: {sceneIndex}");
    }


    private void OnButton()
    {
        

        if(Input.GetKeyDown(KeyCode.L))
        {

            // Устанавливаем объект как "не уничтожаемый при загрузке новой сцены"
            DontDestroyOnLoad(this.gameObject);

            // Получаем индекс текущей сцены
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Выводим индекс текущей сцены в консоль
            Debug.Log($"-------------------Объект {gameObject.name} создан. Текущая сцена: {sceneIndex}");
        }
    }

    private void Update()
    {
        OnButton();
    }

}
