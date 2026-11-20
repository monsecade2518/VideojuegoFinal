using UnityEngine;

public class CambiarFondoZona : MonoBehaviour
{
    [Header("Arrastra aquí los objetos padres de tus fondos")]
    public GameObject padreExterior; // Arrastra Ext_Bg2
    public GameObject padreCueva;    // Arrastra Int_Bg

    [Header("Configuración")]
    public float velocidadTransicion = 2f;
    public string tagJugador = "Player";

    private SpriteRenderer[] spritesExterior;
    private SpriteRenderer[] spritesCueva;
    private ParallaxFondo[] parallaxExterior;
    private ParallaxFondo[] parallaxCueva;

    private bool enCueva = false;
    private float alphaCuevaActual = 0f;

    void Start()
    {
        if (padreExterior != null)
        {
            padreExterior.SetActive(true);
            spritesExterior = padreExterior.GetComponentsInChildren<SpriteRenderer>(true);
            parallaxExterior = padreExterior.GetComponentsInChildren<ParallaxFondo>(true);
        }

        if (padreCueva != null)
        {
            padreCueva.SetActive(true);
            spritesCueva = padreCueva.GetComponentsInChildren<SpriteRenderer>(true);
            parallaxCueva = padreCueva.GetComponentsInChildren<ParallaxFondo>(true);
        }

        // Empezamos afuera: exterior visible y moviéndose; cueva invisible y QUIETA
        SetAlphaGrupo(spritesExterior, 1f);
        SetAlphaGrupo(spritesCueva, 0f);
        SetParallaxActivo(parallaxExterior, true);
        SetParallaxActivo(parallaxCueva, false);
        alphaCuevaActual = 0f;
    }

    void Update()
    {
        float alphaObjetivo = enCueva ? 1f : 0f;

        if (!Mathf.Approximately(alphaCuevaActual, alphaObjetivo))
        {
            alphaCuevaActual = Mathf.MoveTowards(alphaCuevaActual, alphaObjetivo, velocidadTransicion * Time.deltaTime);

            SetAlphaGrupo(spritesCueva, alphaCuevaActual);
            SetAlphaGrupo(spritesExterior, 1f - alphaCuevaActual);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagJugador))
        {
            CambiarEstado(other.transform.position.x < transform.position.x);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tagJugador))
        {
            CambiarEstado(other.transform.position.x > transform.position.x);
        }
    }

    private void CambiarEstado(bool entrandoACueva)
    {
        if (enCueva == entrandoACueva) return;
        enCueva = entrandoACueva;

        // Encendemos el parallax de la zona a la que entramos (reiniciando su referencia de cámara)
        SetParallaxActivo(parallaxCueva, enCueva);
        SetParallaxActivo(parallaxExterior, !enCueva);
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

    private void SetAlphaGrupo(SpriteRenderer[] sprites, float alpha)
    {
        if (sprites == null) return;

        foreach (SpriteRenderer sr in sprites)
        {
            if (sr != null)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
        }
    }
}