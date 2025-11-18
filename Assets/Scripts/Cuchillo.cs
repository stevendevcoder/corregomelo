using System.Diagnostics;
using UnityEngine;

public class Cuchillo : MonoBehaviour
{
    [SerializeField] private float velocidad = 8f;

    private void Update()
    {
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            
        }

        Destroy(gameObject);
    }
}
