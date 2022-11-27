using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            ActiveObj();
            //this.gameObject.SetActive(false);
        }

        if (collision.CompareTag("Border"))
        {
            ActiveObj();
            //this.gameObject.SetActive(false);
        }

        if(collision.CompareTag("Ground"))
        {
            ActiveObj();
            //this.gameObject.SetActive(false);
        }
    }

    private void ActiveObj()
    {
        NewPoolManager.GetInstance.DespawnObject(NewPoolManager.PoolableType.PBullet, this.gameObject);
    }

}
