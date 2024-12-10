using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            AsignarItem();
        }
    }

    private void AsignarItem()
    {
        if(gameObject.CompareTag("Moneda"))
        {
            GameManager.instance.ActualizarContadorMonedas();
        }
        else if (gameObject.CompareTag("PowerUp"))
        {
            GameManager.instance.player.darInmortalidad();
        }
        else if (gameObject.CompareTag("Espada"))
        {
            DATA.espada =  true;
        }

        Destroy(gameObject);
    }
}
