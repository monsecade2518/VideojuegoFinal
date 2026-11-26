using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class AnimationVine : MonoBehaviour
{
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private string tagInvisibleWall = "InvisibleWall_vine";
    [SerializeField] private Collider2D tope;
    private bool subiendo = false;

    public bool estaLista = false;

     private Rigidbody2D rb;

     private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Activar()
    {
        if (subiendo || estaLista) return;
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
}
