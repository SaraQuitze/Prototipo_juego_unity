using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTransicion : MonoBehaviour
{
        public void aparecerJuego()
    {
        gameObject.GetComponent<Animator>().SetTrigger("aparecer");
    }

    public void activarDefault()
    {
        gameObject.GetComponent<Animator>().SetTrigger("default");
    }
}
