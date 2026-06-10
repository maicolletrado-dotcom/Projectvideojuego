using UnityEngine;

/// <summary>
/// CameraFollow — Cámara que sigue a Tato suavemente
/// 
/// SETUP:
///   1. Selecciona la Main Camera en la Hierarchy
///   2. Arrastra este script a la cámara (Add Component)
///   3. Arrastra Tato_Unity al campo "Target"
///   4. Ajusta Offset y SmoothTime al gusto
///
/// Funciona tanto para proyecto 2D como 3D.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [Tooltip("El transform de Tato")]
    public Transform target;

    [Header("Posición")]
    [Tooltip("Desplazamiento de la cámara respecto a Tato")]
    public Vector3 offset = new Vector3(0f, 2f, -10f);

    [Tooltip("Suavidad del seguimiento (menor = más pegado, mayor = más suave)")]
    [Range(0.05f, 1f)]
    public float smoothTime = 0.25f;

    [Header("Límites opcionales")]
    [Tooltip("Activar límites de cámara (útil para niveles con bordes)")]
    public bool useLimits = false;
    public float minX = -50f;
    public float maxX =  50f;
    public float minY = -10f;
    public float maxY =  20f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Posición objetivo
        Vector3 targetPos = target.position + offset;

        // Aplicar límites si están activos
        if (useLimits)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        }

        // Mantener Z fijo (importante para proyectos 2D)
        targetPos.z = transform.position.z;

        // Mover suavemente
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}
