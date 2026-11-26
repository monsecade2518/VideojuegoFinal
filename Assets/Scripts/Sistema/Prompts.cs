using System;
using System.Collections.Generic;
using UnityEngine;

public class Prompts : MonoBehaviour
{
    private enum Modo
    {
        Panel,
        AlEntrar
    }

    private const float Margen = 20f;
    private const float Separacion = 8f;
    private const float DuracionMinima = 4f;
    private const float PalabrasPorSegundo = 3f;
    private const float AltoBoton = 30f;

    [SerializeField] private Modo modo = Modo.Panel;
    [SerializeField, Tooltip("Asigna el componente Prompts en modo Panel de esta sala.")]
    private Prompts panelDestino = null;
    [SerializeField, TextArea(2, 5)] private string texto = string.Empty;

    private readonly List<Instruccion> instrucciones = new List<Instruccion>();
    private readonly Queue<GUIContent> pendientes = new Queue<GUIContent>();
    private GUIContent avisoActual;
    private float finAviso;
    private GUIStyle estiloHistorial;
    private GUIStyle estiloAviso;
    private GUIStyle estiloSombra;
    private Vector2 desplazamiento;
    private bool bajarAlUltimo;
    private bool historialAbierto;
    private bool mostrado;
    private Func<bool> puedeMostrar;

    private void Start()
    {
        if (modo != Modo.Panel || string.IsNullOrWhiteSpace(texto)) return;

        foreach (string linea in texto.Split('\n'))
            Mostrar(linea);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (modo != Modo.AlEntrar || mostrado || !isActiveAndEnabled || !other.CompareTag("Player")) return;
        if (panelDestino == null || panelDestino.modo != Modo.Panel || string.IsNullOrWhiteSpace(texto)) return;
        if (puedeMostrar != null && !puedeMostrar()) return;

        panelDestino.Mostrar(texto);
        mostrado = true;
    }

    public void ConfigurarAvisoAlEntrar(Prompts panel, string mensaje, Func<bool> condicion)
    {
        modo = Modo.AlEntrar;
        panelDestino = panel;
        texto = mensaje;
        puedeMostrar = condicion;
    }

    public void Mostrar(string mensaje)
    {
        if (modo != Modo.Panel || string.IsNullOrWhiteSpace(mensaje)) return;

        pendientes.Enqueue(new GUIContent(mensaje.Trim()));
    }

    private void Update()
    {
        float ahora = Time.unscaledTime;
        if (avisoActual != null)
        {
            if (ahora < finAviso) return;

            instrucciones.Add(new Instruccion(avisoActual));
            bajarAlUltimo = true;
            avisoActual = null;
        }

        if (pendientes.Count == 0) return;

        avisoActual = pendientes.Dequeue();
        int palabras = avisoActual.text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length;
        finAviso = ahora + Mathf.Max(DuracionMinima, 1f + palabras / PalabrasPorSegundo);
    }

    private void OnGUI()
    {
        if (instrucciones.Count > 0)
            DibujarHistorial();

        if (avisoActual != null && Event.current.type == EventType.Repaint)
            DibujarAvisoCentral();
    }

    private void DibujarHistorial()
    {
        float anchoPanel = Mathf.Min(420f, Screen.width - Margen * 2f);
        Rect boton = new Rect(Margen, Screen.height - Margen - AltoBoton, Mathf.Min(112f, anchoPanel), AltoBoton);
        float altoMaximo = Mathf.Min(Screen.height * 0.45f, boton.y - Margen - Separacion);
        if (anchoPanel <= 60f || altoMaximo <= 0f) return;

        if (GUI.Button(boton, historialAbierto ? "Prompts -" : "Prompts +"))
            historialAbierto = !historialAbierto;

        if (!historialAbierto) return;

        estiloHistorial ??= new GUIStyle(GUI.skin.label)
        {
            fontSize = 17,
            alignment = TextAnchor.MiddleLeft,
            wordWrap = true,
            richText = false,
            padding = new RectOffset(18, 14, 10, 10),
            normal = { textColor = new Color(0.95f, 0.97f, 0.98f, 1f) }
        };

        float anchoTarjeta = anchoPanel;
        float altoContenido = MedirContenido(anchoTarjeta);
        if (altoContenido > altoMaximo)
        {
            anchoTarjeta -= GUI.skin.verticalScrollbar.fixedWidth + 4f;
            altoContenido = MedirContenido(anchoTarjeta);
        }

        float altoVisible = Mathf.Min(altoContenido, altoMaximo);
        if (bajarAlUltimo)
        {
            desplazamiento.y = altoContenido - altoVisible;
            bajarAlUltimo = false;
        }

        Rect panel = new Rect(Margen, boton.y - Separacion - altoVisible, anchoPanel, altoVisible);
        Rect contenido = new Rect(0f, 0f, anchoTarjeta, altoContenido);
        desplazamiento = GUI.BeginScrollView(panel, desplazamiento, contenido, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);

        if (Event.current.type == EventType.Repaint)
        {
            float y = 0f;
            foreach (Instruccion instruccion in instrucciones)
            {
                DibujarEnHistorial(instruccion.Contenido, new Rect(0f, y, anchoTarjeta, instruccion.Alto));
                y += instruccion.Alto + Separacion;
            }
        }

        GUI.EndScrollView();
    }

    private float MedirContenido(float ancho)
    {
        float alto = -Separacion;
        foreach (Instruccion instruccion in instrucciones)
        {
            instruccion.Alto = Mathf.Max(42f, estiloHistorial.CalcHeight(instruccion.Contenido, ancho));
            alto += instruccion.Alto + Separacion;
        }

        return alto;
    }

    private void DibujarEnHistorial(GUIContent contenido, Rect tarjeta)
    {
        Color colorAnterior = GUI.color;
        GUI.color = new Color(0.14f, 0.20f, 0.27f, 0.5f);
        GUI.DrawTexture(tarjeta, Texture2D.whiteTexture);
        GUI.color = new Color(0.40f, 0.76f, 0.88f, 1f);
        GUI.DrawTexture(new Rect(tarjeta.x, tarjeta.y, 4f, tarjeta.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(tarjeta, contenido, estiloHistorial);
        GUI.color = colorAnterior;
    }

    private void DibujarAvisoCentral()
    {
        float ancho = Mathf.Min(520f, Screen.width - Margen * 2f);
        if (ancho <= 60f) return;

        if (estiloAviso == null)
        {
            estiloAviso = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                richText = false,
                padding = new RectOffset(18, 18, 10, 10),
                normal = { textColor = Color.white }
            };
            estiloSombra = new GUIStyle(estiloAviso) { normal = { textColor = Color.black } };
        }

        float alto = Mathf.Max(42f, estiloAviso.CalcHeight(avisoActual, ancho));
        Rect tarjeta = new Rect((Screen.width - ancho) * 0.5f, (Screen.height - alto) * 0.5f, ancho, alto);
        Rect sombra = new Rect(tarjeta.x + 1f, tarjeta.y + 1f, tarjeta.width, tarjeta.height);

        Color colorAnterior = GUI.color;
        GUI.color = new Color(0.14f, 0.20f, 0.27f, 0.25f);
        GUI.DrawTexture(tarjeta, Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(sombra, avisoActual, estiloSombra);
        GUI.Label(tarjeta, avisoActual, estiloAviso);
        GUI.color = colorAnterior;
    }

    private sealed class Instruccion
    {
        public GUIContent Contenido { get; }
        public float Alto { get; set; }

        public Instruccion(GUIContent contenido)
        {
            Contenido = contenido;
        }
    }
}
