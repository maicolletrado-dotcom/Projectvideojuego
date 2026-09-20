using UnityEngine;

/// <summary>
/// FallDeathZone2D — Colocar debajo del nivel para "matar" a Tato si cae al vacio.
/// Requiere BoxCollider2D con Is Trigger activado, cubriendo todo el ancho del nivel.
/// Requiere que Tato tenga el componente TatoHealth2D y el Tag "Player".
/// </summary>
public class FallDeathZone2D : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        TatoHealth2D health = other.GetComponent<TatoHealth2D>();
        if (health != null)
        {
            health.TakeDamage(health.maxHealth); // dano letal, activa Morir()
        }
    }
}
