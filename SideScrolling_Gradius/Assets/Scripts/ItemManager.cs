using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private float pItemSpawnTime = 7f;
    [SerializeField] private float lItemSpawnTime = 9f;
    [SerializeField] private float moveSpeed = 0.0f;

    public int yAxis = 0;

    private void Start()
    {
        if (PoolManager.GetInstance != null)
        {
            pItemSpawnTime = Random.Range(10, 20);
            lItemSpawnTime = Random.Range(10, 30);

            yAxis = Random.Range(-6, 6);

            if (SceneController.GetInstance.IsSinglePlayScene())
            {
                StartCoroutine(PowerItem());
                StartCoroutine(ShellItem());
            }
        }
    }

    IEnumerator PowerItem()
    {
        while(true)
        {
            pItemSpawnTime = Random.Range(10, 30);
            yAxis = Random.Range(-6, 6);
            GameObject item = PoolManager.GetInstance.MakeItem("power");
            item.transform.position = new Vector2(transform.position.x, yAxis);
            item.SetActive(true);
            item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
            yield return Cashing.YieldInstruction.WaitForSeconds(pItemSpawnTime);
        }
    }

    IEnumerator ShellItem()
    {
        while (true)
        {
            lItemSpawnTime = Random.Range(20, 80);
            yAxis = Random.Range(-6, 6);
            GameObject item = PoolManager.GetInstance.MakeItem("shell");
            item.transform.position = new Vector2(transform.position.x, yAxis);
            item.SetActive(true);
            item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
            yield return Cashing.YieldInstruction.WaitForSeconds(lItemSpawnTime);
        }
    }
}