using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class AnimationVine : MonoBehaviour
{
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private string tagInvisibleWall = "InvisibleWall_vine";
    [SerializeField] private Collider2D tope;

    [SerializeField] private SpriteRenderer visualOculto;
    private float alphaOriginal;
    private bool subiendo = false;

    public bool estaLista = false;

     private Rigidbody2D rb;

     private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb = GetComponent<Rigidbody2D>();

    if (visualOculto != null)
    {
        alphaOriginal = visualOculto.color.a;
        CambiarAlpha(0f);
    }
    }

    public void Activar()
    {
        if (subiendo || estaLista) return;
        CambiarAlpha(alphaOriginal);
        subiendo = true;
        Debug.Log("Activando liana");
        
    }
    void FixedUpdate(){
        
        if (subiendo)
        {
            rb.linearVelocityY = velocidad;
            Debug.Log("Subiendo liana");
        }
        else
        {
            rb.linearVelocityY = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!subiendo) return;

        bool llegoAlTope = tope != null
            ? other == tope
            : other.CompareTag(tagInvisibleWall);

        if (!llegoAlTope) return;

        subiendo = false;
        estaLista = true;
    }

    private void CambiarAlpha(float alpha)
    {
        if (visualOculto == null) return;

        Color color = visualOculto.color;
        color.a = alpha;
        visualOculto.color = color;
    }
}
