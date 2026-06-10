using UnityEngine;

/// <summary>
/// Tato — Controlador 2D (Rigidbody2D)
/// Para proyectos Unity 2D o 2.5D con física 2D.
///
/// DIFERENCIAS vs TatoController.cs (3D):
///   - Usa Rigidbody2D en lugar de Rigidbody
///   - Usa Physics2D.OverlapCircle para detectar suelo
///   - Usa Physics2D.OverlapCircle para detectar enemigos
///   - Compatible con CapsuleCollider2D y BoxCollider2D
///
/// SETUP:
///   1. Agrega este script al GameObject raíz de Tato
///   2. Agrega Rigidbody2D  → Gravity Scale: 3 | Collision: Continuous
///                          → Freeze Rotation Z: ✓
///   3. Agrega CapsuleCollider2D → Size (0.5, 1.8) | Direction: Vertical
///   4. Crea hijo vacío "GroundCheck" en los pies → Position (0, -0.9)
///   5. Crea hijo vacío "AttackPoint"             → Position (0.6, 0)
///   6. Crea layer "Ground" y asígnalo al suelo
///   7. Asigna groundCheck, groundLayer, attackPoint en el Inspector
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class TatoController2D : MonoBehaviour
{
    // ─── Movimiento ───────────────────────────────────────────────────────────
    [Header("Movimiento")]
    public float moveSpeed    = 6f;
    public float jumpForce    = 14f;
    public float fallMultiplier    = 2.8f;
    public float lowJumpMultiplier = 2.2f;

    // ─── Suelo ────────────────────────────────────────────────────────────────
    [Header("Suelo")]
    public Transform  groundCheck;
    public float      groundCheckRadius = 0.22f;
    public LayerMask  groundLayer;

    // ─── Ataque ───────────────────────────────────────────────────────────────
    [Header("Ataque")]
    public Transform  attackPoint;
    public float      attackRange   = 1.2f;
    public float      attackDamage  = 10f;
    public LayerMask  enemyLayer;
    public float      attackCooldown = 0.5f;

    // ─── Entropía ─────────────────────────────────────────────────────────────
    [Header("Entropía")]
    [Range(0f, 1f)]
    public float nivelEntropiaActual  = 0f;
    public float velocidadEntropía    = 0.008f;

    // ─── Privados ─────────────────────────────────────────────────────────────
    private Rigidbody2D rb2d;
    private Animator    anim;
    private bool        isGrounded;
    private float       moveInput;
    private bool        facingRight  = true;
    private bool        canAttack    = true;
    private float       attackTimer  = 0f;
    private bool        isAttacking  = false;
    private bool        jumpPressed  = false;

    // Hashes Animator
    private static readonly int HashSpeed      = Animator.StringToHash("Speed");
    private static readonly int HashIsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int HashVelocityY  = Animator.StringToHash("VelocityY");
    private static readonly int HashJump       = Animator.StringToHash("Jump");
    private static readonly int HashAttack     = Animator.StringToHash("Attack");
    private static readonly int HashEntropia   = Animator.StringToHash("Entropia");

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Congelar rotación Z para que Tato no ruede
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // ─── Update ───────────────────────────────────────────────────────────────
    void Update()
    {
        // 1. Input
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. Detección de suelo con Physics2D
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 3. Salto — guardamos la petición para aplicarla en FixedUpdate
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpPressed = true;
            anim.SetTrigger(HashJump);
        }

        // 4. Ataque
        if (Input.GetButtonDown("Fire1") && canAttack && !isAttacking)
            StartCoroutine(DoAttack());

        // 5. Cooldown
        if (!canAttack)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f) canAttack = true;
        }

        // 6. Flip
        if      (moveInput > 0f && !facingRight) Flip();
        else if (moveInput < 0f &&  facingRight) Flip();

        // 7. Animator
        anim.SetFloat(HashSpeed,      Mathf.Abs(moveInput));
        anim.SetBool (HashIsGrounded, isGrounded);
        anim.SetFloat(HashVelocityY,  rb2d.linearVelocity.y);
        anim.SetFloat(HashEntropia,   nivelEntropiaActual);

        // 8. Entropía crece con el tiempo
        nivelEntropiaActual = Mathf.Clamp01(nivelEntropiaActual + velocidadEntropía * Time.deltaTime);
    }

    // ─── FixedUpdate ──────────────────────────────────────────────────────────
    void FixedUpdate()
    {
        // Movimiento horizontal
        rb2d.linearVelocity = new Vector2(moveInput * moveSpeed, rb2d.linearVelocity.y);

        // Salto
        if (jumpPressed)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
            jumpPressed = false;
        }

        // Caída pesada (Hollow Knight feel)
        if (rb2d.linearVelocity.y < 0f)
        {
            rb2d.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb2d.linearVelocity.y > 0f && !Input.GetButton("Jump"))
        {
            rb2d.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    // ─── Ataque ───────────────────────────────────────────────────────────────
    private System.Collections.IEnumerator DoAttack()
    {
        isAttacking = true;
        canAttack   = false;
        attackTimer = attackCooldown;
        anim.SetTrigger(HashAttack);

        yield return new WaitForSeconds(0.15f);

        if (attackPoint != null)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
            foreach (var e in enemies)
            {
                EnemyHealth2D eh = e.GetComponent<EnemyHealth2D>();
                if (eh != null) eh.TakeDamage(attackDamage);
            }
        }

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    // ─── Flip ─────────────────────────────────────────────────────────────────
    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // ─── API pública ──────────────────────────────────────────────────────────
    public void TakeDamage(float amount)
    {
        nivelEntropiaActual = Mathf.Clamp01(nivelEntropiaActual + amount * 0.05f);
    }

    public void RescatarFamiliar(string nombre)
    {
        nivelEntropiaActual = Mathf.Clamp01(nivelEntropiaActual - 0.15f);
        Debug.Log($"[Tato] {nombre} rescatado. Entropía: {nivelEntropiaActual:F2}");
    }

    // ─── Gizmos ───────────────────────────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (attackPoint != null)
        {
            Gizmos.color = new Color(1f, 0.4f, 0f, 0.5f);
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}

// ─── Stub EnemyHealth2D ───────────────────────────────────────────────────────
public class EnemyHealth2D : MonoBehaviour
{
    public float maxHealth = 50f;
    private float hp;
    void Start() => hp = maxHealth;
    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0f) Destroy(gameObject);
    }
}
