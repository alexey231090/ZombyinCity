using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerFinish : MonoBehaviour
{
    public string levelName; // ��� ������, �� ������� ����� �������

    private void OnTriggerEnter(Collider other)
    {
        // ���������, �������� �� ������, � ������� ��������� ������������, �������
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
