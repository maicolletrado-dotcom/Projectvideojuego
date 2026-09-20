using UnityEngine;

/// <summary>
/// Checkpoint2D — Colocar en cada punto de guardado del nivel.
/// Requiere BoxCollider2D con Is Trigger activado.
/// Requiere que Tato tenga el Tag "Player".
/// </summary>
public class Checkpoint2D : MonoBehaviour
{
    public int checkpointIndex;
    private bool activado = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activado) return;
        if (!other.CompareTag("Player")) return;

        CheckpointSystem2D sistema = FindObjectOfType<CheckpointSystem2D>();
        if (sistema != null)
        {
            sistema.ActivarCheckpoint(checkpointIndex);
            activado = true;
        }
    }
}
