using System.Diagnostics;
using UnityEngine;

public class NeroAI : MonoBehaviour
{
    public enum Estado { Perseguir, Saltar, Atacar, Robar }
    private Estado estadoActual = Estado.Perseguir;

    [Header("Referencias")]
    [SerializeField] private Transform jugador;
    private Rigidbody2D rb;
    private Animator anim;
    private bool hacerSalto = false;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float fuerzaSalto = 7f;

    [Header("Detección")]
    [SerializeField] private Transform detectorFrente;
    [SerializeField] private Transform detectorSuelo;
    [SerializeField] private float distanciaRaycast = 1.3f;
    [SerializeField] private LayerMask capaObstaculos;   // plataformas, cajas, etc.
    [SerializeField] private LayerMask capaSuelo;        // piso donde puede pararse

    private bool enSuelo = false;

    [Header("Ataque")]
    [SerializeField] private float distanciaAtaque = 4f;
    [SerializeField] private float distanciaRobar = 1f;
    [SerializeField] private GameObject prefabCuchillo;
    [SerializeField] private float intervaloCuchillo = 0.5f;
    private float timerCuchillo = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        estadoActual = Estado.Perseguir;
    }

    private void Update()
    {
        timerCuchillo += Time.deltaTime;

        DetectarSuelo();
        ActualizarEstados();
        EjecutarEstado();
    }

    private void DetectarSuelo()
    {
        enSuelo = Physics2D.Raycast(detectorSuelo.position, Vector2.down, 0.2f, capaSuelo);


    }

    private bool DetectarObstaculoFrente()
    {
        // derecha o izquierda según el flip
        Vector2 direccion = Vector2.right * Mathf.Sign(transform.localScale.x);

        RaycastHit2D hit = Physics2D.Raycast(
            detectorFrente.position,
            direccion,
            distanciaRaycast,
            capaObstaculos
        );


        return hit.collider != null;
    }

    private void ActualizarEstados()
    {
        float dist = Vector2.Distance(transform.position, jugador.position);

        // 1. Robo (cuando ya lo alcanzó)
        if (dist <= distanciaRobar)
        {
            estadoActual = Estado.Robar;
            return;
        }

        // 2. Ataque a distancia
        if (dist <= distanciaAtaque)
        {
            estadoActual = Estado.Atacar;
            return;
        }

        // 3. Saltar si hay obstáculo y está en el suelo
        if (DetectarObstaculoFrente() && enSuelo)
        {
            estadoActual = Estado.Saltar;
            return;
        }

        // 4. Por defecto: perseguir
        estadoActual = Estado.Perseguir;
    }

    private void EjecutarEstado()
    {
        switch (estadoActual)
        {
            case Estado.Perseguir:
                Perseguir();
                break;

            case Estado.Saltar:
                Saltar();
                break;

            case Estado.Atacar:
                LanzarCuchillo();
                break;

            case Estado.Robar:
                Robar();
                break;
        }
    }

    private void Perseguir()
    {
        anim.Play("correrNero");

        Vector2 dir = (jugador.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * velocidad, rb.linearVelocity.y);

        // Flip hacia el jugador
        if (dir.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (dir.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Saltar()
    {
        anim.Play("saltoNero");

        if (enSuelo)
            hacerSalto = true;  // solo marcamos que debe saltar

        estadoActual = Estado.Perseguir;
    }


    private void LanzarCuchillo()
    {
        anim.Play("ataqueNero");

        if (prefabCuchillo == null)
        {
            return;
        }

        if (timerCuchillo >= intervaloCuchillo)
        {
            timerCuchillo = 0f;

            Vector3 spawn = transform.position + transform.right * 0.5f;

            Instantiate(prefabCuchillo, spawn, transform.rotation);
        }

        estadoActual = Estado.Perseguir;
    }


    private void Robar()
    {
        anim.Play("robarNero");
        rb.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (hacerSalto)
        {
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            hacerSalto = false;
        }
    }
}
