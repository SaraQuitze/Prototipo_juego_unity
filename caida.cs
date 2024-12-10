using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class caida : MonoBehaviour
{
    public GameObject Checkpoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.transform.position = Checkpoint.transform.position;
            collision.GetComponent<PlayerController>().RecibirDaño();
        }
    }
}
