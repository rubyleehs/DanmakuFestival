using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFamilySpawner : MathfExtras
{
    //==========Start Values==========

    //Bullet Spawning
    public int bulletFamilySideCount;
    public int bulletFamilyActiveSideCount;
    public int bulletFamilySideSkipInterval;//make sure not equal to bulletFamilySideCount;
    public int bulletFamilyBulletPerSideCount;
    public float bulletFamilyBulletSpawnRadius;

    public float bulletFamilySpawnInterval;

    //==========Runtime Values==========

    //Bullet Family stats
    private float bulletFamilyEdgeSeperation;
    private List<Vector3> bulletFamilyEdgePos;
    private List<Vector2Int> bulletFamilySideLinePointsIndex;
    public float previousBulletFamilySpawnTime;//accesed by spawnerMaster

    //Individual Bullets stats
    private List<float> bulletSpawnRot;
    private List<Vector3> bulletSpawnPos;
    //========================================

    public void SetupRuntimeValues()//should be calculated in master then immediately assigned
    {
        previousBulletFamilySpawnTime = Time.time;
        UpdateBulletSpawnPointStats();
    }

    private void UpdateLinePointsIndex()//Is only called by UpdateBulletSpawnPointStats()
    {
        bulletFamilySideLinePointsIndex = new List<Vector2Int>();
        int _startIndex = 0;
        int _endIndex = 0;
        int _indexShiftCount = 0;
        while(bulletFamilySideLinePointsIndex.Count < bulletFamilySideCount)
        {
            _endIndex= (_startIndex + bulletFamilySideSkipInterval) % bulletFamilySideCount;
            if (_endIndex == _indexShiftCount) {
                bulletFamilySideLinePointsIndex.Add(new Vector2Int(_startIndex, _endIndex));
                _indexShiftCount++;
                _startIndex = _indexShiftCount;
            }
            else
            {
                bulletFamilySideLinePointsIndex.Add(new Vector2Int(_startIndex, _endIndex));
                _startIndex = _endIndex;
            }
        }
    }

    private void UpdateBulletSpawnPointStats()
    {
        UpdateLinePointsIndex();

        bulletFamilyEdgePos = new List<Vector3>();
        bulletFamilyEdgeSeperation = -360f / bulletFamilySideCount;
        for (int _edge = 0; _edge < bulletFamilySideCount; _edge++)
        {
            bulletFamilyEdgePos.Add(AlterVector(Vector3.up * bulletFamilyBulletSpawnRadius, _edge * bulletFamilyEdgeSeperation));
        }

        UpdateLinePointsIndex();

        float _bulletSpawnPosRatioSeperation = 0f;
        if (bulletFamilyBulletPerSideCount > 1) _bulletSpawnPosRatioSeperation = 1f / (float)(bulletFamilyBulletPerSideCount - 1);
        Debug.Log(bulletFamilyBulletPerSideCount + " || " + _bulletSpawnPosRatioSeperation);

        bulletSpawnPos = new List<Vector3>();
        bulletSpawnRot = new List<float>();
        float cal_bulletSpawnRot = 0;
        for (int _side = 0; _side < bulletFamilyActiveSideCount; _side++)
        {
            cal_bulletSpawnRot = _side * bulletFamilyEdgeSeperation *bulletFamilySideSkipInterval;
            if (bulletFamilyBulletPerSideCount % 2 == 1) cal_bulletSpawnRot += bulletFamilyEdgeSeperation * bulletFamilySideSkipInterval * 0.5f;
            for (int _sideBullet = 0; _sideBullet < bulletFamilyBulletPerSideCount; _sideBullet++)
            { 
                Vector3 _spawnPos = Vector3.Lerp(bulletFamilyEdgePos[bulletFamilySideLinePointsIndex[_side].x], bulletFamilyEdgePos[bulletFamilySideLinePointsIndex[_side].y], _sideBullet * _bulletSpawnPosRatioSeperation);
                bulletSpawnPos.Add(_spawnPos);
                bulletSpawnRot.Add(cal_bulletSpawnRot);//
            }
        }

        if (bulletFamilyBulletPerSideCount % 2 == 0)
        {
            for (int i = 0; i < bulletSpawnRot.Count; i++)
            {
                if ((i % bulletFamilyBulletPerSideCount) >= bulletFamilyBulletPerSideCount * 0.5f) bulletSpawnRot[i] += bulletFamilyEdgeSeperation * bulletFamilySideSkipInterval;
                //else bulletSpawnRot[i] -= bulletFamilyEdgeSeperation * bulletFamilySideSkipInterval * 0.5f;
            }
        }
    }

    public IEnumerator GetAndHandleBulletFamily()
    {
        BulletFamily _bulletFamily = GameManager.GetBulletFamily();
        _bulletFamily.transform.position = this.transform.position;
        _bulletFamily.transform.rotation = this.transform.rotation;
        for (int i = 0; i < bulletSpawnPos.Count; i++)
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
