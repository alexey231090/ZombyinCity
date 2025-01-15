
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ColliderDelet : MonoBehaviour
{
    public bool showBounds = true; // Булево значение для управления отображением границ объектов
    public bool enableCollidersOnContact = true; // Булево значение для управления состоянием коллайдеров при соприкосновении

    private void OnDrawGizmos()
    {
        // Получаем все объекты в сцене
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // Получаем коллайдер куба
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        // Проходим по всем объектам
        foreach (GameObject obj in allObjects)
        {
            // Проверяем, есть ли у объекта MeshCollider
            MeshCollider meshCollider = obj.GetComponent<MeshCollider>();

            // Если нужно показать границы и у объекта нет MeshCollider или он отключен
            if (showBounds)
            {
                if (meshCollider != null && meshCollider.enabled)
                {
                    // Объект с активным MeshCollider - окрашиваем в зеленый
                    ShowBounds(obj, Color.green);
                }
                else
                {
                    // Объект без MeshCollider - окрашиваем в синий
                    ShowBounds(obj, Color.blue);
                }
            }

            // Проверяем пересечение с коллайдером куба
            if (meshCollider != null && boxCollider.bounds.Intersects(meshCollider.bounds))
            {
                // Включаем или отключаем MeshCollider в зависимости от значения enableCollidersOnContact
                meshCollider.enabled = enableCollidersOnContact;
            }
        }
    }

    private void ShowBounds(GameObject obj, Color color)
    {
        // Получаем компонент BoxCollider
        BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            // Отображаем границы с помощью Gizmos
            Gizmos.color = color; // Устанавливаем цвет границ
            Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
        }
        else
        {
            // Если у объекта нет BoxCollider, можно использовать MeshRenderer для отображения границ
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Gizmos.color = color; // Устанавливаем цвет границ
                Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.size);
            }
        }
    }
}