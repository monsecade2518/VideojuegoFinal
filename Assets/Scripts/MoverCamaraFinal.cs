using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-10)]
public class MoverCamaraFinal : MonoBehaviour
{
    private const double IntervaloTemblor = 5.0;

    [SerializeField] private Camera camara;
    [SerializeField, Min(0f)] private float distanciaDerecha = 20f;
    [SerializeField, Min(0f)] private float distanciaArriba = 40f;
    [SerializeField, Min(0f)] private float duracionDerecha = 2f;
    [SerializeField, Min(0f)] private float duracionSubida = 2.5f;
    [SerializeField, Min(0f)] private float pausaArriba = 2f;
    [SerializeField, Min(0f)] private float duracionRegreso = 2.5f;
    [SerializeField, Min(0.01f)] private float tamanoOrtograficoRecorrido = 29.53f;
    [SerializeField] private Rect viewportRecorrido = new Rect(0f, 0.1f, 1f, 0.8f);

    [Header("Temblor")]
    [SerializeField, Min(0.1f)] private float duracionTemblor = 2f;
    [SerializeField, Min(0f)] private float intensidadTemblor = 0.15f;

    private bool recorridoUsado;
    private Coroutine recorrido;
    private Vector3 posicionInicial;
    private Rect viewportInicial;
    private float tamanoOrtograficoInicial;
    private CameraFollow2D seguimiento;
    private bool seguimientoActivo;
    private Canvas lienzoFranjas;
    private RectTransform franjaSuperior;
    private RectTransform franjaInferior;
    private RectTransform franjaIzquierda;
    private RectTransform franjaDerecha;
    private double siguienteTemblor = double.PositiveInfinity;
    private double inicioTemblor;
    private bool temblando;

    public bool Temblando => temblando;
    public int NumeroTemblor { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActiveAndEnabled || recorridoUsado || camara == null ||
            InputHelper.GameplayInputBlocked || !other.CompareTag("Player"))
            return;

        recorridoUsado = true;
        seguimiento = camara.GetComponent<CameraFollow2D>();
        LivesManager vidas = LivesManager.Instance;
        if (vidas != null && vidas.CinematicaRoomVista)
        {
            siguienteTemblor = Time.timeAsDouble + IntervaloTemblor;
            return;
        }

        InputHelper.GameplayInputBlocked = true;
        other.GetComponentInParent<PlayerSidescroller>()?.ClearBufferedInput();

        posicionInicial = camara.transform.position;
        viewportInicial = camara.rect;
        tamanoOrtograficoInicial = camara.orthographicSize;
        seguimientoActivo = seguimiento != null && seguimiento.enabled;
        if (seguimientoActivo)
            seguimiento.enabled = false;

        CrearFranjas();
        AplicarViewport(viewportInicial);
        recorrido = StartCoroutine(ReproducirRecorrido());
    }

    private IEnumerator ReproducirRecorrido()
    {
        Vector3 puntoDerecha = posicionInicial + Vector3.right * distanciaDerecha;
        Vector3 puntoArriba = puntoDerecha + Vector3.up * distanciaArriba;

        yield return AnimarTramo(puntoDerecha, viewportRecorrido, tamanoOrtograficoRecorrido, duracionDerecha);
        yield return AnimarTramo(puntoArriba, viewportRecorrido, tamanoOrtograficoRecorrido, duracionSubida);

        if (pausaArriba > 0f)
            yield return new WaitForSeconds(pausaArriba);

        float duracionIda = duracionDerecha + duracionSubida;
        float proporcionBajada = duracionIda > 0f ? duracionSubida / duracionIda : 0.5f;
        float duracionBajada = duracionRegreso * proporcionBajada;
        float duracionIzquierda = duracionRegreso - duracionBajada;

        yield return AnimarTramo(puntoDerecha, viewportRecorrido, tamanoOrtograficoRecorrido, duracionBajada);
        yield return AnimarTramo(posicionInicial, viewportInicial, tamanoOrtograficoInicial, duracionIzquierda);

        // Mostrar el encuadre restaurado un fotograma antes de reanudar LateUpdate.
        yield return null;
        recorrido = null;
        RestaurarCamara();
        LivesManager vidas = LivesManager.Instance;
        if (vidas != null)
            vidas.CinematicaRoomVista = true;

        siguienteTemblor = Time.timeAsDouble + IntervaloTemblor;
    }

    private void Update()
    {
        double ahora = Time.timeAsDouble;
        if (temblando)
        {
            float transcurrido = (float)(ahora - inicioTemblor);
            if (transcurrido >= duracionTemblor || seguimiento == null || !seguimiento.isActiveAndEnabled)
            {
                DetenerTemblor();
            }
            else
            {
                float fase = transcurrido * 16f;
                float amplitud = intensidadTemblor * Mathf.Sin(Mathf.PI * transcurrido / duracionTemblor);
                float horizontal = Mathf.Clamp01(Mathf.PerlinNoise(fase, 2f)) * 2f - 1f;
                float vertical = Mathf.Clamp01(Mathf.PerlinNoise(fase, 13f)) * 2f - 1f;
                seguimiento.SetShakeOffset(new Vector2(horizontal, vertical) * amplitud);
            }
        }

        if (ahora < siguienteTemblor)
            return;

        siguienteTemblor += IntervaloTemblor;
        if (siguienteTemblor <= ahora)
            siguienteTemblor = ahora + IntervaloTemblor;

        if (temblando || seguimiento == null || !seguimiento.isActiveAndEnabled)
            return;

        inicioTemblor = ahora;
        NumeroTemblor++;
        temblando = true;
    }

    private void DetenerTemblor()
    {
        if (!temblando)
            return;

        temblando = false;
        if (seguimiento != null)
            seguimiento.SetShakeOffset(Vector2.zero);
    }

    private IEnumerator AnimarTramo(Vector3 destino, Rect viewportDestino, float tamanoDestino, float duracion)
    {
        Vector3 origen = camara.transform.position;
        Rect viewportOrigen = camara.rect;
        float tamanoOrigen = camara.orthographicSize;

        for (float transcurrido = 0f; transcurrido < duracion; transcurrido += Time.deltaTime)
        {
            float progreso = Mathf.SmoothStep(0f, 1f, transcurrido / duracion);
            camara.transform.position = Vector3.Lerp(origen, destino, progreso);
            camara.orthographicSize = Mathf.Lerp(tamanoOrigen, tamanoDestino, progreso);
            AplicarViewport(new Rect(
                Mathf.Lerp(viewportOrigen.x, viewportDestino.x, progreso),
                Mathf.Lerp(viewportOrigen.y, viewportDestino.y, progreso),
                Mathf.Lerp(viewportOrigen.width, viewportDestino.width, progreso),
                Mathf.Lerp(viewportOrigen.height, viewportDestino.height, progreso)));
            yield return null;
        }

        camara.transform.position = destino;
        AplicarViewport(viewportDestino);
        camara.orthographicSize = tamanoDestino;
    }

    private void CrearFranjas()
    {
        var objeto = new GameObject("Franjas de la cinemática", typeof(RectTransform), typeof(Canvas));
        SceneManager.MoveGameObjectToScene(objeto, gameObject.scene);
        lienzoFranjas = objeto.GetComponent<Canvas>();
        lienzoFranjas.renderMode = RenderMode.ScreenSpaceOverlay;
        lienzoFranjas.targetDisplay = camara.targetDisplay;

        franjaSuperior = CrearFranja("Superior");
        franjaInferior = CrearFranja("Inferior");
        franjaIzquierda = CrearFranja("Izquierda");
        franjaDerecha = CrearFranja("Derecha");
    }

    private RectTransform CrearFranja(string nombre)
    {
        var objeto = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        var imagen = objeto.GetComponent<Image>();
        imagen.color = Color.black;
        imagen.raycastTarget = false;
        RectTransform rect = imagen.rectTransform;
        rect.SetParent(lienzoFranjas.transform, false);
        return rect;
    }

    private void AplicarViewport(Rect viewport)
    {
        camara.rect = viewport;
        float izquierda = Mathf.Clamp01(viewport.xMin);
        float derecha = Mathf.Clamp01(viewport.xMax);
        float abajo = Mathf.Clamp01(viewport.yMin);
        float arriba = Mathf.Clamp01(viewport.yMax);

        lienzoFranjas.enabled = izquierda > 0f || derecha < 1f || abajo > 0f || arriba < 1f;
        AjustarFranja(franjaSuperior, new Vector2(0f, arriba), Vector2.one);
        AjustarFranja(franjaInferior, Vector2.zero, new Vector2(1f, abajo));
        AjustarFranja(franjaIzquierda, new Vector2(0f, abajo), new Vector2(izquierda, arriba));
        AjustarFranja(franjaDerecha, new Vector2(derecha, abajo), new Vector2(1f, arriba));
    }

    private static void AjustarFranja(RectTransform franja, Vector2 minimo, Vector2 maximo)
    {
        franja.anchorMin = minimo;
        franja.anchorMax = maximo;
        franja.offsetMin = Vector2.zero;
        franja.offsetMax = Vector2.zero;
    }

    private void OnDisable()
    {
        siguienteTemblor = double.PositiveInfinity;
        DetenerTemblor();

        if (recorrido == null)
            return;

        StopCoroutine(recorrido);
        recorrido = null;
        RestaurarCamara();
    }

    private void RestaurarCamara()
    {
        if (camara != null)
        {
            camara.transform.position = posicionInicial;
            camara.rect = viewportInicial;
            camara.orthographicSize = tamanoOrtograficoInicial;
        }

        if (seguimiento != null)
            seguimiento.enabled = seguimientoActivo;

        if (lienzoFranjas != null)
        {
            lienzoFranjas.enabled = false;
            Destroy(lienzoFranjas.gameObject);
            lienzoFranjas = null;
        }

        InputHelper.GameplayInputBlocked = false;
    }
}
