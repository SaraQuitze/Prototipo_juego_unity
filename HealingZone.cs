using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingZone : MonoBehaviour
{
    private PlayerController player;
    private Animator anim;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

   void OnTriggerEnter2D(Collider2D zone)
	{
		if (zone.gameObject.name.Equals("Player") && player.vidas <= 2)
			{
                StartCoroutine(Heal());
            }
        else if (zone.gameObject.name.Equals("Player") && player.vidas == player.maxvidas)
            {
                StopCoroutine(Heal());
            }
	}

	public IEnumerator Heal()
	{       
            anim.SetBool("Healing", true);
			player.vidas ++;
            player.healingHp();
			yield return new WaitForSeconds (0.2f);
            anim.SetBool("Healing", false);

	}
}
