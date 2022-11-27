using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBossShell : MonoBehaviour
{
    public GameObject effect;
    public Transform playerTrans;

    [SerializeField] private float moveSpeed;

    private void Update()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        Movement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject e = Instantiate(effect, transform.position, transform.rotation);
            Destroy(e, 0.3f);

            //this.gameObject.SetActive(false);
            NewPoolManager.GetInstance.DespawnObject(NewPoolManager.PoolableType.EBullet, this.gameObject);
        }

        if (collision.CompareTag("PBullet"))
        {
            GameObject e = Instantiate(effect, transform.position, transform.rotation);
            Destroy(e, 0.3f);

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

    private void Movement()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTrans.position, moveSpeed * Time.deltaTime);
    }
}
