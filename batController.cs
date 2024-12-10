using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class batController : MonoBehaviour
{
    private CinemachineVirtualCamera cm;
    private SpriteRenderer sp;
    private PlayerController player;
    private Rigidbody2D rb;
    private bool aplicarFuerza;
    private bool agitando;

    public float velocidadDeMovimiento = 3;
    public float radioDeDeteccion = 15;
    public LayerMask layerJugador;

    public Vector2 posicionCabeza;
    //public bool enCabeza;
    public int vidas = 3;
    public string Nombre;


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
       gameObject.name = Nombre;
    }

    //poder ver desde el inspector el rango de colición del murcielago sin tener que crear un collider 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDeDeteccion);
        Gizmos.color = Color.green;
        Gizmos.DrawCube((Vector2)transform.position + posicionCabeza, new Vector2(1, 0.5f) * 0.7f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direccion = player.transform.position - transform.position; 
        float distancia = Vector2.Distance(transform.position, player.transform.position);

        if(distancia <= radioDeDeteccion)
        {
            rb.velocity = direccion.normalized * velocidadDeMovimiento;
            CambiarVista(direccion.normalized.x);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        //se utilizará otro método para asegurarse que la posición funcione correctamente usando la posición en y del jugador Vs la posición en y del murciélago
        //enCabeza = Physics2D.OverlapBox((Vector2)transform.position + posicionCabeza, new Vector2(1, 0.5f) * 0.7f, 0, layerJugador);
    }

    private void CambiarVista(float direccionX)
    {
        if(direccionX < 0 && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            else if(direccionX > 0 && transform.localScale.x < 0)
            {//matf.abs convierte los números en absolutos
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(/*enCabeza*/ transform.position.y + posicionCabeza.y < player.transform.position.y - 0.1f)
            //el 0.1 sale de restar la posición inicial del centro del jugador en Y y la posición en y de los pies.
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * player.fuerzaDeSalto;
                StartCoroutine(AgitarCamara(0.1f));
                Destroy(gameObject, 0.2f);
            }
            else
            {
                player.RecibirDaño((transform.position - player.transform.position).normalized);
            }
        }
    }
    //se usa un fixedUpdate porque se aplicarpan fuerzas sobre el RB 
    private void FixedUpdate()
    {
        if(aplicarFuerza)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 100, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }
//recibir daño del murciélago
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
}
