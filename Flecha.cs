using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flecha : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    public LayerMask layerPiso;
    public GameObject esqueleto;
    public Vector2 direccionFlecha;
    public float radioDeColision = 0.25f;
    public bool tocaSuelo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().RecibirDaño(-(collision.transform.position - esqueleto.transform.position).normalized);
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        tocaSuelo = Physics2D.OverlapCircle((Vector2)transform.position, radioDeColision, layerPiso);
        if(tocaSuelo)
        {
            rb.bodyType = RigidbodyType2D.Static;
            bc.enabled = false;
            this.enabled = false;
        }
        //Atan2 calcula la tangente de 2 vectores en radianes y luego se multiplica por Mathf.Rad2Deg en donde se convierte de radianes a grados
        float angulo = Mathf.Atan2(direccionFlecha.y, direccionFlecha.x) * Mathf.Rad2Deg;
        //esto le permitirá a la flecha rotar en la dirección del jugador incluso estando en el aire, la dinámica de movimiento será más intuitiva
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.y, transform.localEulerAngles.x, angulo);
    }
}
 