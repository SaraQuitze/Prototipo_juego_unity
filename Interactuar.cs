using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactuar : MonoBehaviour
{
    private bool puedeIneractuar;
    private BoxCollider2D bc;
    private SpriteRenderer sp;
    private GameObject indicadorInteractuar;
    private Animator anim;

    public bool esCofre;
    public bool esPalanca;
    public bool palancaAccionar;
    public bool esCheck;
    public bool esSelectorLvl;
    public GameObject[] objetos;
    public UnityEvent evento;//activará el evento que realizará la palanca 

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if(transform.GetChild(0) != null)
        {
            indicadorInteractuar = transform.GetChild(0).gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            puedeIneractuar = true;
            indicadorInteractuar.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            puedeIneractuar = false;
            indicadorInteractuar.SetActive(false);
        }
    }

    private void palanca()
    {
        if(esPalanca && !palancaAccionar)
        {
            anim.SetBool("activar", true);
            palancaAccionar = true;
            evento.Invoke();
            indicadorInteractuar.SetActive(false);
            bc.enabled = false;
            this.enabled = false;
        }
        
    }

    private void Checkpoint()
    {
        if(esCheck)
        {
            evento.Invoke();
        }
    }

    private void SeleccionarNivel()
    {
        if(esSelectorLvl)
        {
            evento.Invoke();
        }
    }
    private void cofre()
    {
        if(esCofre)
        {
            Instantiate(objetos[Random.Range(0, objetos.Length)], transform.position, Quaternion.identity);
            anim.SetBool("openUp", true);
            bc.enabled = false;
        }
    }

    private void Update()
    {
        if(puedeIneractuar && Input.GetKeyDown(KeyCode.C))
        {
            cofre();
            palanca();
            Checkpoint();
            SeleccionarNivel();
        }
    }
}
