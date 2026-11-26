using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance { get; private set; }
    public bool CinematicaRoomVista { get; set; }

    public int vidasActuales = 3;
    public int vidasMaximas = 3;
    private int vidasInicioRoom = 3;
    private string nombreRoomActual = "";
    private bool reiniciandoEscena = false;

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
        if (reiniciandoEscena) return;

        vidasActuales -= cantidad;
        Debug.Log("Vidas restantes: " + vidasActuales);

        if (vidasActuales <= 0)
        {
            vidasActuales = 0;
            MatarPersonaje();
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

    public void MatarPersonaje()
    {
        if (reiniciandoEscena) return;
        
        reiniciandoEscena = true;
        Debug.Log("El personaje se quedó sin vidas. Reiniciando sala...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    // Dibuja el texto en la esquina superior izquierda (x: 20, y: 20)
    GUI.Label(new Rect(20, 20, 300, 50), "Vidas: " + vidasActuales, estilo);
}

}
