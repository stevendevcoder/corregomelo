using UnityEngine;

public class Jugador : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float fuerzaSalto = 10f;

    private Rigidbody2D rb;
    private Animator anim;

    private float movimientoX = 0f;
    private bool enSuelo = false;
    private bool saltar = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Leer input SOLO aquí
        movimientoX = Input.GetAxisRaw("Horizontal");

        // Registrar salto correctamente (sin perder input)
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && enSuelo)
        {
            saltar = true;
        }

        // Animaciones
        ActualizarAnimaciones();
    }

    private void FixedUpdate()
    {
        // Aplicar movimiento con físicas (evita bugs)
        rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);

        // Saltar si corresponde
        if (saltar)
        {
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            enSuelo = false;
            saltar = false;  // evita doble salto bug
        }
    }

    private void ActualizarAnimaciones()
    {
        if (!enSuelo)
        {
            anim.Play("salto");
            return;
        }

        if (Mathf.Abs(movimientoX) > 0.1f)
        {
            anim.Play("correr");

            // Flip
            if (movimientoX > 0) transform.localScale = new Vector3(1, 1, 1);
            if (movimientoX < 0) transform.localScale = new Vector3(-1, 1, 1);

            return;
        }

        anim.Play("reposo");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Piso") || collision.collider.CompareTag("Plataforma"))
        {
            enSuelo = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Piso") || collision.collider.CompareTag("Plataforma"))
        {
            enSuelo = false;
        }
    }
}
