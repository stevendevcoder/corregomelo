using UnityEngine;

public class Jugador : MonoBehaviour
{
    [SerializeField] private bool isNero = false; 
    [SerializeField] private float speed = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float cambioCada = 3f; 

    private Animator anim;
    private Rigidbody2D rb;

    private float timer = 0f;
    private int estado = 0; 

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= cambioCada)
        {
            estado = (estado + 1) % 4; // 0→1→2→3→0
            timer = 0f;
        }

        switch (estado)
        {
            case 0: // Reposo
                anim.Play(isNero ? "reposoNero" : "reposo");
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;

            case 1: // Correr izquierda
                anim.Play(isNero ? "correrNero" : "correr");
                transform.localScale = new Vector3(-1, 1, 1);
                rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
                break;

            case 2: // Correr derecha
                anim.Play(isNero ? "correrNero" : "correr");
                transform.localScale = new Vector3(1, 1, 1);
                rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
                break;

            case 3: // Saltar
                anim.Play(isNero ? "saltoNero" : "salto");
                if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                break;
        }
    }
}
