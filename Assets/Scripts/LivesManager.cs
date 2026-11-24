using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance { get; private set; }

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
    }

private void OnGUI()
{
    // Posición y tamaño del HUD
    Rect fondoRect = new Rect(20, 100, 230, 65);

    // Fondo oscuro
    GUI.color = new Color(0.06f, 0.08f, 0.14f, 0.90f);
    GUI.DrawTexture(fondoRect, Texture2D.whiteTexture);

    // Línea decorativa superior
    GUI.color = new Color(0.2f, 0.85f, 1f, 1f);
    GUI.DrawTexture(new Rect(20, 100, 230, 4), Texture2D.whiteTexture);

    // Texto "Lives"
    GUIStyle livesStyle = new GUIStyle(GUI.skin.label);
    livesStyle.fontSize = 20;
    livesStyle.fontStyle = FontStyle.Bold;
    livesStyle.normal.textColor = new Color(0.2f, 0.85f, 1f);
    livesStyle.alignment = TextAnchor.MiddleCenter;

    GUI.Label(new Rect(30, 105, 45, 48), "Live", livesStyle);

    // Texto de vidas
    GUIStyle vidasStyle = new GUIStyle(GUI.skin.label);
    vidasStyle.fontSize = 25;
    vidasStyle.fontStyle = FontStyle.Bold;
    vidasStyle.normal.textColor = Color.white;
    vidasStyle.alignment = TextAnchor.MiddleLeft;

    GUI.Label(new Rect(85, 105, 150, 48), vidasActuales.ToString(), vidasStyle);

    GUI.color = Color.white;
}

}