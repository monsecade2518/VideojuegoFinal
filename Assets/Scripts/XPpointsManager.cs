using UnityEngine;
using UnityEngine.SceneManagement;

public class XPpointsManager : MonoBehaviour
{
    public static XPpointsManager Instance { get; private set; }

    public int puntosActuales = 0;
    private int puntosInicioRoom = 0;
    private string nombreRoomActual = "";

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name != nombreRoomActual)
        {
            // Entraste a un room nuevo: guardamos el checkpoint de puntos
            nombreRoomActual = escena.name;
            puntosInicioRoom = puntosActuales;
            Debug.Log("Nuevo room: " + nombreRoomActual + ". Puntos guardados: " + puntosInicioRoom);
        }
        else
        {
            // Se recargó el mismo room (el jugador murió): restauramos los puntos
            puntosActuales = puntosInicioRoom;
            Debug.Log("Room reiniciado. Puntos restaurados a: " + puntosActuales);
        }
    }

    public void SumarPuntos(int cantidad)
    {
        puntosActuales += cantidad;
        Debug.Log("Puntos acumulados: " + puntosActuales);
    }

    public bool AlcanzoMeta(int puntosNecesarios)
    {
        //Debug.Log("Puntos actuales: " + puntosActuales + ", Puntos necesarios: " + puntosNecesarios);
        return puntosActuales >= puntosNecesarios;
    }

    // Llama a este método solo al empezar una partida nueva desde el Menú Principal
    public void ReiniciarPartidaCompleta()
    {
        puntosActuales = 0;
        puntosInicioRoom = 0;
        nombreRoomActual = "";
    }

   private void OnGUI()
{
    // Posición y tamaño del HUD
    Rect fondoRect = new Rect(20, 25, 230, 65);

    // Fondo oscuro
    GUI.color = new Color(0.06f, 0.08f, 0.14f, 0.90f);
    GUI.DrawTexture(fondoRect, Texture2D.whiteTexture);

    // Línea decorativa superior
    GUI.color = new Color(0.2f, 0.85f, 1f, 1f);
    GUI.DrawTexture(new Rect(20, 25, 230, 4), Texture2D.whiteTexture);

    // Texto "XP"
    GUIStyle xpStyle = new GUIStyle(GUI.skin.label);
    xpStyle.fontSize = 20;
    xpStyle.fontStyle = FontStyle.Bold;
    xpStyle.normal.textColor = new Color(0.2f, 0.85f, 1f);
    xpStyle.alignment = TextAnchor.MiddleCenter;

    GUI.Label(new Rect(30, 32, 45, 48), "XP", xpStyle);

    // Texto de puntos
    GUIStyle puntosStyle = new GUIStyle(GUI.skin.label);
    puntosStyle.fontSize = 25;
    puntosStyle.fontStyle = FontStyle.Bold;
    puntosStyle.normal.textColor = Color.white;
    puntosStyle.alignment = TextAnchor.MiddleLeft;

    GUI.Label(new Rect(85, 32, 150, 48), puntosActuales.ToString(), puntosStyle);

    // Restaurar color
    GUI.color = Color.white;
}
}