using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Skeleton : MonoBehaviour
{
    private PlayerController player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;
    private CinemachineVirtualCamera cm;
    private bool aplicarFuerza;
    private bool agitando;

    public float distanciaDeteccionJugador = 17;
    public float distanciaDeteccionFlecha = 11;
    public GameObject Flecha;
    public float fuerzaLanzamiento;
    public float velocidadDeMovimiento;
    public int vidas = 3;
    public bool lanzandoFlecha;
    public string nombre;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direccion = (player.transform.position - transform.position).normalized * distanciaDeteccionFlecha;
        Debug.DrawRay(transform.position, direccion, Color.red);

        float distanciaActual = Vector2.Distance(transform.position, player.transform.position);

        if(distanciaActual <= distanciaDeteccionFlecha)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("Caminando", false);

            Vector2 direccionNormailzada = direccion.normalized;
            CambiarVista(direccionNormailzada.x);
            if(!lanzandoFlecha)
            {
                StartCoroutine(lanzarFlecha(direccion, distanciaActual));
            }
        }
        else
        {
            if(distanciaActual <= distanciaDeteccionJugador)
            {
                Vector2 movimiento = new Vector2(direccion.x, 0);
                movimiento = movimiento.normalized;
                //se corrige esta parte para que el esqueleto, cuando caiga (eje y) no se quede flotando, de esta manera, cuando el esqueleto caiga, lo hará a velocidad de rb normal.
                rb.velocity = new Vector2(movimiento.x * velocidadDeMovimiento, rb.velocity.y);
                anim.SetBool("Caminando", true);
                CambiarVista(movimiento.x);
            }
            else
            {
                anim.SetBool("Caminando", false);
            }
        }
    }

    private void CambiarVista(float direccionX)
    {
        if(direccionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if(direccionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionJugador);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionFlecha);
    }

    private IEnumerator lanzarFlecha(Vector2 direccionFlecha, float distancia)
    {
        lanzandoFlecha = true;
        anim.SetBool("Disparando", true);
        yield return new WaitForSeconds(1.42f);
        anim.SetBool("Disparando", false);
        //se repite aquí el cálculo del lanzamiento de la flecha para mejorar la presición. 
        direccionFlecha = (player.transform.position - transform.position).normalized * distanciaDeteccionFlecha;
        direccionFlecha = direccionFlecha.normalized;
        
        GameObject flechaGO = Instantiate(Flecha, transform.position, Quaternion.identity);
        flechaGO.transform.GetComponent<Flecha>().direccionFlecha = direccionFlecha;
        flechaGO.transform.GetComponent<Flecha>().esqueleto = this.gameObject;

        flechaGO.transform.GetComponent<Rigidbody2D>().velocity = direccionFlecha * fuerzaLanzamiento;

        lanzandoFlecha = false;
    }
     /// <summary>
     /// recibir daño, morir, agitar cámara y efecto daño son componentes comunes a todos los enemigos, se puede crear un publico que luego herede a cada uno para evitar repetir código.
     /// </summary>
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
            velocidadDeMovimiento = 0;
            rb.velocity = Vector2.zero;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player.RecibirDaño((transform.position - player.transform.position).normalized);
        }
    }

    private void FixedUpdate()
    {
        if(aplicarFuerza)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 100, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }
}
