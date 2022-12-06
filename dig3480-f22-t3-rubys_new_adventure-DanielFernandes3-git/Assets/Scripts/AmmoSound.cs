using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSound : MonoBehaviour
{
    public AudioClip ammoClip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RubyController controller = collision.GetComponent<RubyController>();

        controller.PlaySound(ammoClip);
    }
}
