using UnityEngine;

public class ConeControll : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);

    }
}
