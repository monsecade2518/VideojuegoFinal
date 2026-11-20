using UnityEngine;

public class CambiarFondoZona : MonoBehaviour
{
    [Header("Arrastra aquí los objetos padres de tus fondos")]
    public GameObject padreExterior; // Arrastra Ext_Bg2
    public GameObject padreCueva;    // Arrastra Int_Bg

    [Header("Configuración de Zona")]
    [Tooltip("Márcalo si el jugador inicia la escena DENTRO de la cueva (ej. Room3).")]
    public bool empezarEnCueva = false;

    [Tooltip("Déjalo desmarcado para la ENTRADA a la cueva. Márcalo para la SALIDA de la cueva.")]
    public bool esSalida = false;

    [Header("Ajustes")]
    public float velocidadTransicion = 2f;
    public string tagJugador = "Player";

    private SpriteRenderer[] spritesExterior;
    private SpriteRenderer[] spritesCueva;
    private float[] alphasMaximosExterior;
    private float[] alphasMaximosCueva;
    private ParallaxFondo[] parallaxExterior;
    private ParallaxFondo[] parallaxCueva;

    private static bool enCueva = false;
    private static float alphaCuevaActual = 0f;
    private static bool inicializado = false;

    void Start()
    {
        if (padreExterior != null)
        {
            padreExterior.SetActive(true);
            spritesExterior = padreExterior.GetComponentsInChildren<SpriteRenderer>(true);
            parallaxExterior = padreExterior.GetComponentsInChildren<ParallaxFondo>(true);
            alphasMaximosExterior = GuardarAlphasOriginales(spritesExterior);
        }

        if (padreCueva != null)
        {
            padreCueva.SetActive(true);
            spritesCueva = padreCueva.GetComponentsInChildren<SpriteRenderer>(true);
            parallaxCueva = padreCueva.GetComponentsInChildren<ParallaxFondo>(true);
            alphasMaximosCueva = GuardarAlphasOriginales(spritesCueva);
        }

        if (!inicializado)
        {
            enCueva = empezarEnCueva;
            alphaCuevaActual = empezarEnCueva ? 1f : 0f;

            SetAlphaGrupo(spritesExterior, alphasMaximosExterior, empezarEnCueva ? 0f : 1f);
            SetAlphaGrupo(spritesCueva, alphasMaximosCueva, empezarEnCueva ? 1f : 0f);

            // El exterior siempre acompaña la cámara para estar en su sitio al salir
            SetParallaxActivo(parallaxExterior, true);
            SetParallaxActivo(parallaxCueva, empezarEnCueva);

            inicializado = true;
        }
    }

    void OnDestroy()
    {
        inicializado = false;
    }

    void Update()
    {
        float alphaObjetivo = enCueva ? 1f : 0f;

        if (!Mathf.Approximately(alphaCuevaActual, alphaObjetivo))
        {
            alphaCuevaActual = Mathf.MoveTowards(alphaCuevaActual, alphaObjetivo, velocidadTransicion * Time.deltaTime);

            SetAlphaGrupo(spritesCueva, alphasMaximosCueva, alphaCuevaActual);
            SetAlphaGrupo(spritesExterior, alphasMaximosExterior, 1f - alphaCuevaActual);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagJugador))
        {
            CambiarEstado(!esSalida);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tagJugador))
        {
            bool salioPorLaDerecha = other.transform.position.x > transform.position.x;

            if (!esSalida)
                CambiarEstado(salioPorLaDerecha);
            else
                CambiarEstado(!salioPorLaDerecha);
        }
    }

    private void CambiarEstado(bool entrandoACueva)
    {
        if (enCueva == entrandoACueva) return;
        enCueva = entrandoACueva;

        SetParallaxActivo(parallaxCueva, enCueva);
    }

    private void SetParallaxActivo(ParallaxFondo[] lista, bool activo)
    {
        if (lista == null) return;

        foreach (ParallaxFondo p in lista)
        {
            if (p != null)
            {
                if (activo) p.ReiniciarPosicionCamara();
                p.enabled = activo;
            }
        }
    }

    private float[] GuardarAlphasOriginales(SpriteRenderer[] sprites)
    {
        if (sprites == null) return null;
        float[] alphas = new float[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            alphas[i] = sprites[i] != null ? sprites[i].color.a : 1f;
        }
        return alphas;
    }

    private void SetAlphaGrupo(SpriteRenderer[] sprites, float[] alphasMaximos, float multiplicador)
    {
        if (sprites == null || alphasMaximos == null) return;

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
            {
                Color c = sprites[i].color;
                c.a = alphasMaximos[i] * multiplicador;
                sprites[i].color = c;
            }
        }
    }
}