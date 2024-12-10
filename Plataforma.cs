using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plataforma : MonoBehaviour
{
    private bool aplicarFuerza;
    private bool detectarJugador;
    private PlayerController player;
    

    public bool daSalto;
    public int fuerzaSalto = 25;
    public BoxCollider2D plataformaCollider;
    public BoxCollider2D plataformaTrigger;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Start()
    {
        if(!daSalto)
        {
            Physics2D.IgnoreCollision(plataformaCollider, plataformaTrigger, true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(!daSalto)//si la plataforma no es de salto y el jugador viene desde abajo entonces la plataforma collider sin trigger y el jugador no colisionarán sino que el jugador lo va a atravezar
            {
                Physics2D.IgnoreCollision(plataformaCollider, player.GetComponent<CapsuleCollider2D>(), true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(!daSalto)
            {
                Physics2D.IgnoreCollision(plataformaCollider, player.GetComponent<CapsuleCollider2D>(), false);//se le da a la plataforma layer de piso
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            detectarJugador = true;
            if(daSalto)
            {
                aplicarFuerza = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            detectarJugador = true;
        }
    }

    private void Update()
    {
        if(daSalto)
        {
            if(player.transform.position.y - 0.8f > transform.position.y)//si el jugador no está sobre la plataforma
            {
                plataformaCollider.isTrigger = false;
            }
            else
            {
                plataformaCollider.isTrigger = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if(aplicarFuerza)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);//se convirtió la constante de multiplicación en la variable pública fuerzaDaño y la dirección de daño en negativo para que impulse al jugador al lado contrario de la dirección del daño recibido.
            aplicarFuerza = false;
        }
    }

}
