using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PShell : MonoBehaviour
{
    public ParticleSystem particle;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            ParticlePlay();
            Invoke("StateSetActive", 1f);
        }

        if (collision.CompareTag("Border"))
        {
            ParticlePlay();
            Invoke("StateSetActive", 1f);
        }

        if (collision.CompareTag("Ground"))
        {
            ParticlePlay();
            Invoke("StateSetActive", 1f);
        }
    }

    private void ParticlePlay()
    {
        particle.Play();
    }

    private void StateSetActive()
    {
        this.gameObject.SetActive(false);
    }
}
