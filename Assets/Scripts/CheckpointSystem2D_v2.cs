using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CheckpointSystem2D — Sistema de puntos de guardado (manager)
///
/// SETUP:
///   1. Crea un GameObject vacio "CheckpointManager" en la escena, agrega este script
///   2. Para cada checkpoint del nivel: crea un GameObject con BoxCollider2D (Is Trigger = true)
///      y agrega el script Checkpoint2D.cs
///   3. Arrastra cada checkpoint a la lista "checkpoints" de este manager, en orden
/// </summary>
public class CheckpointSystem2D : MonoBehaviour
{
    [Header("Checkpoints del nivel (en orden)")]
    public List<Transform> checkpoints = new List<Transform>();

    private int checkpointActual = 0;

    public void ActivarCheckpoint(int index)
    {
        if (index > checkpointActual)
        {
            checkpointActual = index;
            Debug.Log("[Checkpoint] Activado: " + index);
        }
    }

    public void RespawnEnUltimoCheckpoint(GameObject jugador)
    {
        if (checkpoints.Count == 0) return;

        Transform destino = checkpoints[checkpointActual];
        jugador.transform.position = destino.position;

        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
}
