using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private int direccionX;
    private Vector2 direccion;
    private Vector2 direccionMovimiento;
    private CapsuleCollider2D colider;

    [Header("Estadísticas del Jugador")]
    public float velocidadDeMovimiento = 10;

    //[Header("Booleanos")]
    //public bool puedeMover;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        colider = GetComponent<CapsuleCollider2D>();
    }

    
    void Update()
    {
        Movimiento();   
    }

    private void Movimiento()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 direccion = new Vector2(x, y);

        Caminar(direccion);
    }

    private void Caminar(Vector2 direccion) {
        //if(puedeMover)
        //{
            rb.velocity = new Vector2(direccion.x * velocidadDeMovimiento, rb.velocity.y);

            /*if(direccion.x < 0 && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            else if(direccion.x > 0 && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }*/
    }
}
