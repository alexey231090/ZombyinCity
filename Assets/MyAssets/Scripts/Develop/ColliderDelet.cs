
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ColliderDelet : MonoBehaviour
{
    public bool showBounds = true; // ������ �������� ��� ���������� ������������ ������ ��������
    public bool enableCollidersOnContact = true; // ������ �������� ��� ���������� ���������� ����������� ��� ���������������

    private void OnDrawGizmos()
    {
        // �������� ��� ������� � �����
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // �������� ��������� ����
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        // �������� �� ���� ��������
        foreach (GameObject obj in allObjects)
        {
            // ���������, ���� �� � ������� MeshCollider
            MeshCollider meshCollider = obj.GetComponent<MeshCollider>();

            // ���� ����� �������� ������� � � ������� ��� MeshCollider ��� �� ��������
            if (showBounds)
            {
                if (meshCollider != null && meshCollider.enabled)
                {
                    // ������ � �������� MeshCollider - ���������� � �������
                    ShowBounds(obj, Color.green);
                }
                else
                {
                    // ������ ��� MeshCollider - ���������� � �����
                    ShowBounds(obj, Color.blue);
                }
            }

            // ��������� ����������� � ����������� ����
            if (meshCollider != null && boxCollider.bounds.Intersects(meshCollider.bounds))
            {
                // �������� ��� ��������� MeshCollider � ����������� �� �������� enableCollidersOnContact
                meshCollider.enabled = enableCollidersOnContact;
            }
        }
    }

    private void ShowBounds(GameObject obj, Color color)
    {
        // �������� ��������� BoxCollider
        BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            // ���������� ������� � ������� Gizmos
            Gizmos.color = color; // ������������� ���� ������
            Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
        }
        else
        {
            // ���� � ������� ��� BoxCollider, ����� ������������ MeshRenderer ��� ����������� ������
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Gizmos.color = color; // ������������� ���� ������
                Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.size);
            }
        }
    }
}