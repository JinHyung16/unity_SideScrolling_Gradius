using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            this.gameObject.SetActive(false);
        }

        if (collision.CompareTag("Border"))
        {
            this.gameObject.SetActive(false);
        }

        if(collision.CompareTag("Ground"))
        {
            this.gameObject.SetActive(false);
        }
    }

}
