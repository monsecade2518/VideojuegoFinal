using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance { get; private set; }
    public bool CinematicaRoomVista { get; set; }
    public bool ReiniciandoEscena => reiniciandoEscena;

    public int vidasActuales = 3;
    public int vidasMaximas = 3;
    private int vidasInicioRoom = 3;
    private string nombreRoomActual = "";
    private bool reiniciandoEscena = false;

    [SerializeField] private Texture2D iconoXP;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += AlCargarEscena;
    private void OnDisable() => SceneManager.sceneLoaded -= AlCargarEscena;

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (!reiniciandoEscena || escena.name != nombreRoomActual)
            CinematicaRoomVista = false;

        reiniciandoEscena = false;

        if (escena.name != nombreRoomActual)
        {
            nombreRoomActual = escena.name;
            vidasInicioRoom = vidasActuales;
        }
        else
        {
            vidasActuales = vidasInicioRoom;
        }
    }

    public void RestarVidas(int cantidad)
    {
        if (reiniciandoEscena || cantidad <= 0) return;

        vidasActuales = Mathf.Max(0, vidasActuales - cantidad);
        Debug.Log("Vidas restantes: " + vidasActuales);

        if (vidasActuales == 0)
        {
            MatarPersonaje();
            return;
        }

        if (cantidad == 1 || cantidad == 2)
        {
            PlayerSidescroller jugador = FindFirstObjectByType<PlayerSidescroller>();
            jugador?.GetComponentInChildren<AnimationController>()?.ReproducirDano();
        }
    }

    public void SumarVidas(int cantidad)
    {
        if (reiniciandoEscena) return;

        vidasActuales += cantidad;
        Debug.Log("Vidas restantes: " + vidasActuales);

        if (vidasActuales > vidasMaximas)
        {
            vidasActuales = vidasMaximas;
        }
    }

    public void ReiniciarPorCaida()
    {
        if (reiniciandoEscena) return;

        vidasActuales = 0;
        reiniciandoEscena = true;
        ReiniciarEscena(SceneManager.GetActiveScene());
    }

    public void MatarPersonaje()
    {
        if (reiniciandoEscena) return;

        vidasActuales = 0;
        reiniciandoEscena = true;
        Scene escena = SceneManager.GetActiveScene();
        PlayerSidescroller jugador = FindFirstObjectByType<PlayerSidescroller>();
        if (jugador != null)
            jugador.DetenerPorMuerte();

        AnimationController animacion = jugador != null ? jugador.GetComponentInChildren<AnimationController>() : null;

        if (animacion != null && animacion.ReproducirMuerte())
        {
            StartCoroutine(ReiniciarTrasMuerte(escena, animacion));
            return;
        }

        ReiniciarEscena(escena);
    }

    private IEnumerator ReiniciarTrasMuerte(Scene escena, AnimationController animacion)
    {
        while (reiniciandoEscena && animacion != null && animacion.MuerteEnCurso)
            yield return null;

        if (reiniciandoEscena && SceneManager.GetActiveScene() == escena)
            ReiniciarEscena(escena);
    }

    private void ReiniciarEscena(Scene escena)
    {
        Debug.Log("El personaje se quedó sin vidas. Reiniciando sala...");
        SceneManager.LoadScene(escena.buildIndex);
    }

    public void ReiniciarPartidaCompleta()
    {
        vidasActuales = 3;
        vidasInicioRoom = 3;
        nombreRoomActual = "";
        reiniciandoEscena = false;
        CinematicaRoomVista = false;
    }

    private void OnGUI()
{
    GUIStyle estilo = new GUIStyle(GUI.skin.label);
    estilo.fontSize = 28;
    estilo.normal.textColor = Color.white;

    if (iconoXP != null)
        GUI.DrawTexture(new Rect(20, 20, 32, 32), iconoXP, ScaleMode.ScaleToFit);

    GUI.Label(new Rect(60, 20, 300, 50), vidasActuales.ToString(), estilo);
}

}