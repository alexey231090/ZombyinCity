using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class TriggerFinish : MonoBehaviour
{
    public static readonly Subject<Unit> OnSaveData = new Subject<Unit>(); // ������� ��� ���������� ������
    

    public string levelName; // ��� ������, �� ������� ����� �������
    
    

    private void OnTriggerEnter(Collider other)
    {
        OnSaveData.OnNext(Unit.Default); // �������� ������� ���������� ������


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
