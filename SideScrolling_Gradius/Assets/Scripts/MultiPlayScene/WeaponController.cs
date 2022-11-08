using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform firePose;

    [SerializeField] private float reloadTime = 0.15f;
    [SerializeField] private float fireDelay = 0.2f;
    [SerializeField] private float firePower = 0.0f;

    private void Start()
    {
        firePose = GetComponentInChildren<Transform>();

        reloadTime = 0.15f;
        fireDelay = 0.2f;
        firePower = 10.0f;
    }
    private void Update()
    {
        fireDelay -= Time.deltaTime;
        if (fireDelay <= 0)
        {
            AttackFire();
            fireDelay = reloadTime;
        }
    }

    public void AttackFire()
    {
        var bullet = PoolManager.Instance.MakeBullet("pbullet");
        bullet.SetActive(true);
        bullet.GetComponent<Rigidbody2D>().AddForce(Vector2.right * firePower, ForceMode2D.Impulse);
    }
}
