using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NewPoolManager;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform firePose;

    [SerializeField] private float reloadTime = 0.15f;
    [SerializeField] private float fireDelay = 0.2f;
    [SerializeField] private float fireShootPower = 0.0f;

    private void Start()
    {
        firePose = GetComponentInChildren<Transform>();

        reloadTime = 0.15f;
        fireDelay = 0.2f;
        fireShootPower = 10.0f;
    }
    private void Update()
    {
        fireDelay -= Time.deltaTime;
    }

    public void AttackFire()
    {
        if (fireDelay <= 0)
        {
            Fire();
            fireDelay = reloadTime;
        }
    }

    private void Fire()
    {
        //var bullet = PoolManager.GetInstance.MakeBullet("pbullet");
        var bullet = NewPoolManager.GetInstance.GetPrefab(PoolableType.PBullet, "PBullet");
        bullet.transform.position = firePose.position;
        bullet.SetActive(true);
        bullet.GetComponent<Rigidbody2D>().AddForce(Vector2.right * fireShootPower, ForceMode2D.Impulse);
    }
}
