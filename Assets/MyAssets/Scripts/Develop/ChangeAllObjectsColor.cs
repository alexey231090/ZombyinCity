using UnityEngine;

public class ChangeAllObjectsColor : MonoBehaviour
{
    public Color newColor = Color.white; // ����, � ������� ����� ����������� �������

    [ContextMenu("Change Color of All Objects")]
    public void ChangeColor()
    {
        print(455);
        // �������� ��� ������� � �����
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // �������� �� ���� ��������
        foreach (GameObject obj in allObjects)
        {
            // �������� ��������� Renderer
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // ������ ���� ���� ���������� �� �����
                foreach (Material material in renderer.materials)
                {
                    material.color = newColor;
                }
            }
        }
    }
}