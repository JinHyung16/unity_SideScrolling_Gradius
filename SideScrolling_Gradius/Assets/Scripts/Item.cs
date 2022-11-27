using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Border"))
        {
            if(collision.gameObject.name == "Left")
            {
                //this.gameObject.SetActive(false);
                NewPoolManager.GetInstance.DespawnObject(NewPoolManager.PoolableType.Item, this.gameObject);
            }
        }
        if(collision.CompareTag("Player"))
        {
            //this.gameObject.SetActive(false);
            NewPoolManager.GetInstance.DespawnObject(NewPoolManager.PoolableType.Item, this.gameObject);
        }
    }
}
