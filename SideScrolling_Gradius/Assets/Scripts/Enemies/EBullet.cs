using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //this.gameObject.SetActive(false);
            NewPoolManager.GetInstance.DespawnObject(NewPoolManager.PoolableType.EBullet, this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Border"))
        {
            //this.gameObject.SetActive(false);
            NewPoolManager.GetInstance.DespawnObject(NewPoolManager.PoolableType.EBullet, this.gameObject);
        }
    }
}
