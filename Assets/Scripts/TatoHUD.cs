using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TatoHUD — Interfaz en pantalla: vida y nivel de entropia
///
/// SETUP EN UNITY:
///   1. GameObject > UI > Canvas (si no existe uno ya)
///   2. Dentro del Canvas, crear:
///      - UI > Slider  -> nombrarlo "SliderVida"
///      - UI > Slider  -> nombrarlo "SliderEntropia"
///      - UI > Text (TMP) -> nombrarlo "TextoVida" (opcional, "100/100")
///   3. Arrastra este script a un GameObject vacio llamado "HUDManager"
///   4. Asigna los 2 sliders y el TatoController2D en el Inspector
///   5. En cada Slider: Min Value 0, Max Value 1, Whole Numbers desmarcado
///
/// El SliderVida se llena de verde a rojo segun el HP actual.
/// El SliderEntropia se llena y cambia de color segun el nivel de caos.
/// </summary>
public class TatoHUD : MonoBehaviour
{
    [Header("Referencias")]
    public TatoController2D tato;
    public TatoHealth2D tatoHealth;

    [Header("UI - Vida")]
    public Slider sliderVida;
    public Image  fillVida;          // arrastra el objeto "Fill" del slider de vida
    public Text   textoVida;         // opcional

    [Header("UI - Entropia")]
    public Slider sliderEntropia;
    public Image  fillEntropia;      // arrastra el objeto "Fill" del slider de entropia

    [Header("Colores")]
    public Color colorVidaAlta   = new Color(0.29f, 0.68f, 0.31f); // verde
    public Color colorVidaMedia  = new Color(0.90f, 0.63f, 0.06f); // ambar
    public Color colorVidaBaja   = new Color(0.80f, 0.23f, 0.13f); // rojo Tato scarf

    public Color colorEntropiaBaja  = new Color(0.29f, 0.68f, 0.31f); // orden
    public Color colorEntropiaMedia = new Color(0.90f, 0.63f, 0.06f); // caos creciente
    public Color colorEntropiaAlta  = new Color(0.80f, 0.23f, 0.13f); // entropia maxima

    void Update()
    {
        ActualizarVida();
        ActualizarEntropia();
    }

    void ActualizarVida()
    {
        if (tatoHealth == null || sliderVida == null) return;

        float ratio = tatoHealth.currentHealth / tatoHealth.maxHealth;
        sliderVida.value = ratio;

        if (fillVida != null)
        {
            if (ratio > 0.6f)      fillVida.color = colorVidaAlta;
            else if (ratio > 0.3f) fillVida.color = colorVidaMedia;
            else                   fillVida.color = colorVidaBaja;
        }

        if (textoVida != null)
            textoVida.text = Mathf.CeilToInt(tatoHealth.currentHealth) + " / " + Mathf.CeilToInt(tatoHealth.maxHealth);
    }

    void ActualizarEntropia()
    {
        if (tato == null || sliderEntropia == null) return;

        float e = tato.nivelEntropiaActual;
        sliderEntropia.value = e;

        if (fillEntropia != null)
        {
            if (e < 0.3f)      fillEntropia.color = colorEntropiaBaja;
            else if (e < 0.7f) fillEntropia.color = colorEntropiaMedia;
            else                fillEntropia.color = colorEntropiaAlta;
        }
    }
}
