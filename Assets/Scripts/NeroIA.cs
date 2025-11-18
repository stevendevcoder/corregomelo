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

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float fuerzaSalto = 7f;

    [Header("Raycasts de Detección")]
    [SerializeField] private Transform detectorFrente;
    [SerializeField] private Transform detectorSuelo;
    [SerializeField] private float distanciaRaycast = 0.5f;
    [SerializeField] private LayerMask capaObstaculos;

    [Header("Ataque")]
    [SerializeField] private float distanciaAtaque = 4f;
    [SerializeField] private float distanciaRobar = 1f;
    [SerializeField] private GameObject prefabCuchillo;
    [SerializeField] private float intervaloCuchillo = 2f;
    private float timerCuchillo = 0f;

    private bool enSuelo = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        timerCuchillo += Time.deltaTime;

        DetectarSuelo();
        DetectarObstaculos();
        ActualizarEstados();
        EjecutarEstado();
    }

    private void DetectarSuelo()
    {
        enSuelo = Physics2D.Raycast(detectorSuelo.position, Vector2.down, 0.2f, capaObstaculos);
    }

    private bool DetectarObstaculos()
    {
        // Detecta frente del ñero
        RaycastHit2D hit = Physics2D.Raycast(detectorFrente.position, transform.right, distanciaRaycast, capaObstaculos);

        return hit.collider != null; // True si hay obstáculo
    }

    private void ActualizarEstados()
    {
        float dist = Vector2.Distance(transform.position, jugador.position);

        // 1. Si está MUY cerca → robar (game over)
        if (dist <= distanciaRobar)
        {
            estadoActual = Estado.Robar;
            return;
        }

        // 2. Si está cerca → lanzar cuchillo
        if (dist <= distanciaAtaque)
        {
            estadoActual = Estado.Atacar;
            return;
        }

        // 3. Si hay obstáculo y está en suelo → saltar
        if (DetectarObstaculos() && enSuelo)
        {
            estadoActual = Estado.Saltar;
            return;
        }

        // 4. Caso base → perseguir
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

        if (dir.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (dir.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Saltar()
    {
        anim.Play("saltoNero");

        rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

        // Volver automáticamente a perseguir
        estadoActual = Estado.Perseguir;
    }

    private void LanzarCuchillo()
    {
        anim.Play("ataqueNero");

        if (timerCuchillo >= intervaloCuchillo)
        {
            timerCuchillo = 0f;

            Vector3 puntoSpawn = transform.position + transform.right * 0.5f;

            GameObject cuchillo = Instantiate(prefabCuchillo, puntoSpawn, transform.rotation);
        }

        // Después de atacar vuelve a perseguir
        estadoActual = Estado.Perseguir;
    }

    private void Robar()
    {
        anim.Play("robarNero");

        rb.linearVelocity = Vector2.zero;


    }

    private void OnDrawGizmosSelected()
    {
        if (detectorFrente != null)
            Gizmos.DrawLine(detectorFrente.position, detectorFrente.position + transform.right * distanciaRaycast);

        if (detectorSuelo != null)
            Gizmos.DrawLine(detectorSuelo.position, detectorSuelo.position + Vector3.down * 0.2f);
    }
}
