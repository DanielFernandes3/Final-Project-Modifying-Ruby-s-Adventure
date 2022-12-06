using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerupEffect PowerupEffect;
    public AudioClip powerupClip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RubyController controller = collision.GetComponent<RubyController>();

        Destroy(gameObject);
        PowerupEffect.Apply(collision.gameObject);

        controller.PlaySound(powerupClip);
    }
}
