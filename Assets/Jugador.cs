using System.Diagnostics;
using UnityEngine;

public class Jugador : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float fuerzaSalto = 10f;

    private Rigidbody2D rb;
    private Animator anim;

    private float movimientoX;
    private bool saltar = false;

    [Header("Detección de suelo")]
    [SerializeField] private Transform sensorSuelo;
    [SerializeField] private float distanciaSuelo = 0.25f;

    // Aquí debes asignar varias layers: Piso + Obstaculos
    [SerializeField] private LayerMask capasQueCuentanComoSuelo;

    private bool enSuelo;
    private float tiempoCoyote = 0f;
    private float coyoteMax = 0.15f; // tiempo de perdón

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        movimientoX = Input.GetAxisRaw("Horizontal");

        DetectarSuelo();

        // Coyote time
        if (enSuelo)
            tiempoCoyote = coyoteMax;
        else
            tiempoCoyote -= Time.deltaTime;

        // Input de salto
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && tiempoCoyote > 0)
        {
            saltar = true;
        }

        ActualizarAnimaciones();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);

        if (saltar)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

            saltar = false;
            tiempoCoyote = 0;
        }
    }

    private void DetectarSuelo()
    {
        // Detecta suelo, plataformas y obstáculos según el LayerMask
        enSuelo = Physics2D.Raycast(sensorSuelo.position, Vector2.down, distanciaSuelo, capasQueCuentanComoSuelo);

    }

    private void ActualizarAnimaciones()
    {
        if (!enSuelo && tiempoCoyote <= 0)
        {
            anim.Play("salto");
            return;
        }

        if (Mathf.Abs(movimientoX) > 0.1f)
        {
            anim.Play("correr");

            if (movimientoX > 0) transform.localScale = new Vector3(1, 1, 1);
            if (movimientoX < 0) transform.localScale = new Vector3(-1, 1, 1);

            return;
        }

        anim.Play("reposo");
    }
}
