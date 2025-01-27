using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento
    public float jumpForce = 7f; // Fuerza de salto ajustada
    public Transform groundCheck; // Punto para verificar si está tocando el suelo
    public float groundCheckRadius = 0.2f; // Radio del círculo de verificación
    public LayerMask groundLayer; // Capa para el suelo

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Verificar si está en el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        anim.SetBool("Grounded", isGrounded); // Actualizar el parámetro Grounded

        // Movimiento lateral
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Actualizar AnimState
        if (isGrounded)
        {
            if (Mathf.Abs(moveInput) > 0)
            {
                anim.SetInteger("AnimState", 2); // Correr
            }
            else
            {
                anim.SetInteger("AnimState", 0); // Idle
            }
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("Jump"); // Activar animación de salto
        }

        // Girar al personaje
        if (moveInput > 0)
            transform.localScale = new Vector3(-1, 1, 1); // Normal
        else if (moveInput < 0)
            transform.localScale = new Vector3(1, 1, 1); // Girar horizontalmente

        // Ataque
        if (Input.GetKeyDown(KeyCode.Z)) // Cambia la tecla si prefieres otra
        {
            anim.SetTrigger("Attack");

            // Verificar si el enemigo está en rango
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Enemy"));
            foreach (Collider2D enemy in hitEnemies)
            {
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(20); // Daño fijo, ajustable
                }
            }
        }
    }
}
