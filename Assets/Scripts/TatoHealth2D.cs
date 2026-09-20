using UnityEngine;

/// <summary>
/// TatoHealth2D — Sistema de vida de Tato
///
/// SETUP:
///   1. Agrega este script al mismo GameObject que TatoController2D
///   2. Ajusta maxHealth en el Inspector si quieres
///   3. Conecta invencibilityTime para dar un respiro tras recibir daño
///
/// Esta mecanica se conecta al TatoHUD.cs para mostrar la vida en pantalla.
/// </summary>
public class TatoHealth2D : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Invencibilidad tras dano")]
    public float invencibilityTime = 1f;
    private float invencibilityTimer = 0f;
    private bool  isInvincible = false;

    [Header("Referencias")]
    public SpriteRenderer spriteParaParpadeo; // opcional, deja vacio si Tato es 3D
    public TatoController2D tatoController;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isInvincible)
        {
            invencibilityTimer -= Time.deltaTime;
            if (invencibilityTimer <= 0f) isInvincible = false;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        isInvincible = true;
        invencibilityTimer = invencibilityTime;

        // Sube la entropia al recibir dano (conecta con el sistema existente)
        if (tatoController != null)
            tatoController.TakeDamage(amount);

        if (currentHealth <= 0f)
            Morir();
    }

    public void Curar(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
    }

    void Morir()
    {
        Debug.Log("[Tato] Ha muerto. Reiniciando en el ultimo checkpoint.");
        // Aqui se conecta con CheckpointSystem2D cuando lo agregues
        CheckpointSystem2D checkpoint = FindObjectOfType<CheckpointSystem2D>();
        if (checkpoint != null)
        {
            checkpoint.RespawnEnUltimoCheckpoint(gameObject);
            currentHealth = maxHealth;
        }
    }
}
