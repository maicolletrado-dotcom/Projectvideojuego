using UnityEngine;

public class CamaraDolly : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadX = 1.5f;
    
    [Header("Límites de la Escena")]
    public float limiteDerechoX = 25.0f; // El punto exacto donde la cámara debe parar

    void Update()
    {
        // Solo se desplaza si la posición actual es menor al límite
        if (transform.position.x < limiteDerechoX)
        {
            transform.Translate(Vector3.right * velocidadX * Time.deltaTime);
        }
    }
}