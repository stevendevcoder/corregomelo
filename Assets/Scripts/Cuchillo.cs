using UnityEngine;

public class Cuchillo : MonoBehaviour
{
    [SerializeField] private float velocidad = 8f;
    [SerializeField] private float velocidadRotacion = 720f;

    private Vector2 direccionInicial;

    private void Start()
    {
        // Guardar la dirección inicial hacia donde mira el cuchillo
        direccionInicial = transform.right;
    }

    private void Update()
    {
        // Movimiento independiente de la rotación
        transform.position += (Vector3)direccionInicial * velocidad * Time.deltaTime;

        // Rotación libre
        transform.Rotate(0, 0, velocidadRotacion * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // Aquí: GameOver
        }

        Destroy(gameObject);
    }
}
