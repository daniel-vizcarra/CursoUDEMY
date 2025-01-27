using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;
    public float attackRange = 1.5f;
    public float detectionRange = 5f;
    public float attackCooldown = 2f; // Tiempo entre ataques
    public Transform patrolPoint1;
    public Transform patrolPoint2;
    public Transform player;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform currentPatrolPoint;
    private bool isChasing = false;
    private float lastAttackTime; // Marca de tiempo del último ataque

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentPatrolPoint = patrolPoint1;
    }

    void Update()
    {
        // Verificar si el jugador sigue existiendo
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero; // Detener al enemigo
            anim.SetFloat("Speed", 0); // Animación de idle
            return;
        }

        // Calcular distancia al jugador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Cambiar entre patrullaje y persecución
        isChasing = distanceToPlayer <= detectionRange;

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        // Movimiento de patrullaje
        Vector2 direction = (currentPatrolPoint.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Cambiar de dirección al llegar al punto
        if (Vector2.Distance(transform.position, currentPatrolPoint.position) < 0.2f)
        {
            currentPatrolPoint = currentPatrolPoint == patrolPoint1 ? patrolPoint2 : patrolPoint1;
        }

        // Animación y dirección
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        Flip(direction.x);
    }

    void ChasePlayer()
    {
        // Perseguir al jugador
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        Flip(direction.x);

        // Atacar al jugador si está en rango
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            // Verificar si el cooldown ha pasado
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time; // Registrar el tiempo del ataque
            }
        }
    }

    void Attack()
    {
        rb.linearVelocity = Vector2.zero; // Detener al enemigo durante el ataque
        anim.SetTrigger("Attack");

        // Infligir daño al jugador
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10); // Inflige daño al jugador
        }
    }

    void Flip(float directionX)
    {
        if (directionX > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (directionX < 0)
            transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
