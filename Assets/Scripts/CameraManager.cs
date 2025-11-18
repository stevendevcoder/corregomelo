using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform objetivo;
    [SerializeField] private float suavizado = 0.15f;
    private Vector3 velocidad = Vector3.zero;

    private void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = new Vector3(objetivo.position.x, objetivo.position.y+1f, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidad, suavizado);
    }
}
