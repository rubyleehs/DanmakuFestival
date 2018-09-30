using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFamilySpawner : MathfExtras
{
    //==========Start Values==========

    //Bullet Spawning
    public int bulletFamilyBulletCount;
    public float bulletFamilyBulletAngularHalfSpread;
    public float bulletFamilyBulletSpawnRadius;

    public float bulletFamilySpawnInterval;

    //==========Runtime Values==========

    //Bullet Family stats
    private float seperationBetweenBullets;
    public float previousBulletFamilySpawnTime;//accesed my spawnerMaster

    //Individual Bullets stats
    private List<float> bulletSpawnRot;
    private List<Vector3> bulletSpawnPos;
    //========================================
    public void SetupRuntimeValues()
    {
        previousBulletFamilySpawnTime = Time.time;
        if (bulletFamilyBulletCount > 1) seperationBetweenBullets = 2 * bulletFamilyBulletAngularHalfSpread / (bulletFamilyBulletCount - 1);
        else seperationBetweenBullets = bulletFamilyBulletAngularHalfSpread;

        bulletSpawnPos = new List<Vector3>();
        bulletSpawnRot = new List<float>();
        for (int i = 0; i < bulletFamilyBulletCount; i++)
        {
            bulletSpawnRot.Add(i * seperationBetweenBullets - bulletFamilyBulletAngularHalfSpread);//
            bulletSpawnPos.Add(AlterVector(Vector3.up * bulletFamilyBulletSpawnRadius, i * seperationBetweenBullets - bulletFamilyBulletAngularHalfSpread));
        }
    }

    public IEnumerator GetAndHandleBulletFamily()
    {
        BulletFamily _bulletFamily = GameManager.GetBulletFamily();
        _bulletFamily.transform.position = this.transform.position;
        _bulletFamily.transform.rotation = this.transform.rotation;
        for (int i = 0; i < bulletFamilyBulletCount; i++)
        {
            Transform _bullet = GameManager.GetBullet();
            _bullet.parent = _bulletFamily.transform;
            _bullet.localPosition = bulletSpawnPos[i];
            _bullet.localRotation = Quaternion.Euler(Vector3.forward * bulletSpawnRot[i]);
            _bulletFamily.bullets.Add(_bullet);
        }
        yield return null;
    }

}
