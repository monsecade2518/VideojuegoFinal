using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class AnimationVine : MonoBehaviour
{
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private string tagInvisibleWall = "InvisibleWall_vine";
    private bool subiendo = false;

    public bool estaLista = false;

     private Rigidbody2D rb;

     private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Activar()
    {
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

    private void OnTriggerEnter2D(Collider2D other){
            if (other.CompareTag(tagInvisibleWall)){
                subiendo = false;
                estaLista = true;
            }
        }
}
