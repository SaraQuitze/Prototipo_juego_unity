using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreadeAtaque : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemigo"))
        {
            if(collision.name == "Bat")
            {
               collision.GetComponent<batController>().RecibirDaño();
            }
            else if (collision.name == "Skeleton")
            {
                collision.GetComponent<Skeleton>().RecibirDaño();
            }
            else if (collision.name == "Skeleton(Clone)")
            {
                collision.GetComponent<Skeleton>().RecibirDaño();
            }
            else if (collision.name == "Spider")
            {
                collision.GetComponent<Waypoints>().RecibirDaño();
            }
            else if (collision.name == "Boss")
            {
                collision.GetComponent<Boss>().RecibirDaño();
            }
        }
        else if (collision.CompareTag("Destruible"))
        {/*cuando se le da Is Triggered en las propiedades del box collider del objeto destruible, el jugador y demás animaciones podrán pasar frente
        o tras de él (dependiendo de la capa en que se encuentre)*/
            collision.GetComponent<Animator>().SetBool("destruir", true);
        }
    }
}
