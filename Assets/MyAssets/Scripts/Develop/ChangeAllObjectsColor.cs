using UnityEngine;

public class ChangeAllObjectsColor : MonoBehaviour
{
    public Color newColor = Color.white; // Цвет, в который будут перекрашены объекты

    [ContextMenu("Change Color of All Objects")]
    public void ChangeColor()
    {
        print(455);
        // Получаем все объекты в сцене
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // Проходим по всем объектам
        foreach (GameObject obj in allObjects)
        {
            // Получаем компонент Renderer
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Меняем цвет всех материалов на новый
                foreach (Material material in renderer.materials)
                {
                    material.color = newColor;
                }
            }
        }
    }
}