using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Waypoints : MonoBehaviour
{
    private Vector3 direccion;
    private PlayerController player;
    private CinemachineVirtualCamera cm;
    private Rigidbody2D rb;
    private int indiceActual = 0;
    private SpriteRenderer sp;
    private bool aplicarFuerza;
    private bool agitando;

    public int vidas = 3;
    public Vector2 posicionCabeza;
    public float velocidadDesplazamiento;
    public List<Transform> puntos = new List<Transform>();
    public bool esperando;
    public float tiempoDeEspera;
    public float fuerzaImpacto;

    private void Awake()
    {
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

        // Start is called before the first frame update
    void Start()
    {
        if(gameObject.CompareTag("Enemigo"))//se crea este start porque este mismo código será usado para desplazamiento de objetos que no serán enemigos, entonces únicamente en caso que el tag sea Enemigo, se "activarán" los daños.
        {
            gameObject.name = "Spider";
        }
    }

    private void FixedUpdate()
    {
        MovimientoWaypoints();
        if(gameObject.CompareTag("Enemigo"))
        {
            CambiarEscalaEnemigo();
        }

        if(aplicarFuerza)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * fuerzaImpacto, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemigo"))
        {
            if(player.transform.position.y - 0.7f > transform.position.y + posicionCabeza.y)//si la posición del jugador es mayor a la posición del enemigo, es decir, si el jugador salta sobre el enemigo
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * player.fuerzaDeSalto;
                Destroy(this.gameObject, 0.2f);
            }
            else
            {
                player.RecibirDaño(-(player.transform.position - transform.position).normalized);
            }
        }
        else if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))
        {
            if(player.transform.position.y - 0.7f > transform.position.y)
            {
                player.transform.parent = transform;//esto hará que en caso que el jugador esté sobre la plataforma, esta última se convierta en el objeto padre del jugador para que se desplacen juntos
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))
        {
            if(player.transform.position.y - 0.7f > transform.position.y)
            {
                player.transform.parent = null;
            }
        }
    }
    private void CambiarEscalaEnemigo()
    {
        if(direccion.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if(direccion.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void MovimientoWaypoints()//se organiza que el movimiento sea desde el punto 1 al 2, ese es el indice actual, los puntos serán añadidos en el inspector.
    {
        direccion = (puntos[indiceActual].position - transform.position).normalized;//así se obtiene la dirección en la cual se moverá

        if(!esperando)
        {
            transform.position = (Vector2.MoveTowards(transform.position, puntos[indiceActual].position, velocidadDesplazamiento * Time.deltaTime));//con esto se le dirá al GO que se mueva entre los puntos indicados en el inspector
        }

        if(Vector2.Distance(transform.position, puntos[indiceActual].position) <= 0.7f)//0.7 es el margen de error de posición
        {
            if(!esperando)
            {
                StartCoroutine(Espera());
            }
        }
    }

    private IEnumerator Espera()
    {
        esperando = true;
        yield return new WaitForSeconds(tiempoDeEspera);
        esperando = false;

        indiceActual++;

        if(indiceActual >= puntos.Count)
        {
            indiceActual = 0;
        }
    }

    public void RecibirDaño()
    {
        if(vidas > 0)
        {
            StartCoroutine(EfectoDaño());
            StartCoroutine(AgitarCamara(0.1f));
            aplicarFuerza = true;
            vidas--;
        }
        else
        {
            StartCoroutine(UltimoAgitarCamara(0.4f));
        }
    }

    private void morir()
    {
        if(vidas <= 0)
        {
            Destroy(this.gameObject, 0.2f);
        }
    }


    private IEnumerator AgitarCamara(float tiempo)
    {
        if (!agitando)
        {
            agitando = true;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
            yield return new WaitForSeconds(tiempo);
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            //morir();
            agitando = false;
        }
    }

    private IEnumerator UltimoAgitarCamara(float tiempo)
    {
        if (!agitando)
        {
            transform.localScale = Vector3.zero;//para pulir el efecto del golpe final
            agitando = true;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
            yield return new WaitForSeconds(tiempo);
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            morir();
            agitando = false;
        }
    }

    private IEnumerator EfectoDaño()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }

}
