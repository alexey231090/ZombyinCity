using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScen : MonoBehaviour
{
    private void Awake()
    {
        // ������������� ������ ��� "�� ������������ ��� �������� ����� �����"
        DontDestroyOnLoad(this.gameObject);

        // �������� ������ ������� �����
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ������� ������ ������� ����� � �������
        Debug.Log($"-------------------������ {gameObject.name} ������. ������� �����: {sceneIndex}");
    }


    private void OnButton()
    {
        

        if(Input.GetKeyDown(KeyCode.L))
        {

            // ������������� ������ ��� "�� ������������ ��� �������� ����� �����"
            DontDestroyOnLoad(this.gameObject);

            // �������� ������ ������� �����
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            // ������� ������ ������� ����� � �������
            Debug.Log($"-------------------������ {gameObject.name} ������. ������� �����: {sceneIndex}");
        }
    }

    private void Update()
    {
        OnButton();
    }

}
