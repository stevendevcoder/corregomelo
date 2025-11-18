using UnityEngine;

public class Canva : MonoBehaviour
{
    [SerializeField] private Camera camara;
    [SerializeField] private Vector3 offset;

    private void LateUpdate()
    {
        if (camara == null) return;

        Vector3 pos = camara.transform.position + offset;
        pos.z = 0; // mantener en UI
        transform.position = pos;
    }
}
