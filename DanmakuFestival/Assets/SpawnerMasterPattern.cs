using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerMasterPattern : MathfExtras {

    //==========Start Values==========
    //SpawnerPattern spawning
    private float spawnerMasterPatternRadius;
    private float spawnerMasterAngularHalfSpread;
    private int spawnerPatternCount;

    //SpawnerPattern movement
    private float spawnerMasterPatternRotSpeed;
    private float spawnerPatternRotSpeed;

    //BulletFamily Spawn movement;
    private int bulletFamilySpawnerMovementKeyPointsCount;
    private List<Vector3> bulletFamilySpawnerMovementKeyPointsPos;
    private float bulletFamilySpawnerMovementCyclePeriod;
    private float bulletFamilySpawnerSpawnCycleInterval;
    //private int bulletFamilySpawnMovementType = 0;

    //==========Runtime Values==========

    //Master Stats
    private float curSpawnerMasterPatternRot = 0;
    private float curSpawnerIndividualPatternRot = 0;
    private float seperationBetweenSpawnerPatterns;

    private float previousBFSStartTime = 1;//can use as a startDelay by using time enemy spawn + start Delay

    //Individual Spawner stats
    private List<Vector3> spawnerPatternPos;
    private List<float> spawnerPatternRot;

    //==========Bullet Family Values==========
    //Bullet Family Spawning
    private float bulletFamilySpawnInterval;

    //Bullet Spawning
    public int bulletFamilySideCount;
    public int bulletFamilyActiveSideCount;
    public int bulletFamilySideSkipInterval;//make sure not equal to bulletFamilySideCount;
    public int bulletFamilyBulletPerSideCount;
    public float bulletFamilyBulletSpawnRadius;

    //========================================

    public void Awake()
    {
        RandomizeStartValues();
        SetupRuntimeValues();
    }

    public void RandomizeStartValues()//values from generation values
    {
        //SpawnerPattern spawning
        spawnerMasterPatternRadius = Random.Range(GameManager.generationFields.spawnerMasterPatternRadius.x, GameManager.generationFields.spawnerMasterPatternRadius.y);
        spawnerMasterAngularHalfSpread = Random.Range(GameManager.generationFields.spawnerMasterAngularHalfSpread.x, GameManager.generationFields.spawnerMasterAngularHalfSpread.y);
        spawnerPatternCount = Random.Range(GameManager.generationFields.spawnerPatternCount.x, GameManager.generationFields.spawnerPatternCount.y + 1);

        //SpawnerPattern movement
        spawnerMasterPatternRotSpeed = Random.Range(GameManager.generationFields.spawnerMasterPatternRotSpeed.x, GameManager.generationFields.spawnerMasterPatternRotSpeed.y);
        spawnerPatternRotSpeed = Random.Range(GameManager.generationFields.spawnerPatternRotSpeed.x, GameManager.generationFields.spawnerPatternRotSpeed.y);

        //BulletFamily Spawn movement;
        bulletFamilySpawnerMovementKeyPointsCount = Random.Range(GameManager.generationFields.bulletFamilySpawnerMovementKeyPointsCount.x, GameManager.generationFields.bulletFamilySpawnerMovementKeyPointsCount.y + 1);
        bulletFamilySpawnerMovementKeyPointsPos = new List<Vector3>();
        for (int i = 0; i < bulletFamilySpawnerMovementKeyPointsCount; i++)
        {
            bulletFamilySpawnerMovementKeyPointsPos.Add(AlterVector(Vector2.up * Random.Range(GameManager.generationFields.bulletFamilySpawnerMovementKeyPointsReach.x, GameManager.generationFields.bulletFamilySpawnerMovementKeyPointsReach.y), Random.Range(0, 360)));
        }
        bulletFamilySpawnerMovementCyclePeriod = Random.Range(GameManager.generationFields.bulletFamilySpawnerMovementCyclePeriod.x, GameManager.generationFields.bulletFamilySpawnerMovementCyclePeriod.y);
        bulletFamilySpawnerSpawnCycleInterval = Random.Range(GameManager.generationFields.bulletFamilySpawnerSpawnCycleInterval.x, GameManager.generationFields.bulletFamilySpawnerSpawnCycleInterval.y);

        //BulletFamilySpawning
        bulletFamilySpawnInterval = Random.Range(GameManager.generationFields.bulletFamilySpawnInterval.x, GameManager.generationFields.bulletFamilySpawnInterval.y);

        //BulletFamily Bullet Spawning
        bulletFamilySideCount = Random.Range(GameManager.generationFields.bulletFamilySideCount.x, GameManager.generationFields.bulletFamilySideCount.y + 1);//
        bulletFamilyActiveSideCount = Random.Range(1 , bulletFamilySideCount + 1);
        bulletFamilySideSkipInterval = Random.Range(1, bulletFamilySideCount);
        bulletFamilyBulletPerSideCount = Random.Range(GameManager.generationFields.bulletFamilyBulletPerSideCount.x, GameManager.generationFields.bulletFamilyBulletPerSideCount.y + 1);
        bulletFamilyBulletSpawnRadius = Random.Range(GameManager.generationFields.bulletFamilyBulletSpawnRadius.x, GameManager.generationFields.bulletFamilyBulletSpawnRadius.y);//
}

    public void SetupRuntimeValues()//needs to be called everytime Start Values are changed;
    {
        //bulletFamilySpawner = new List<BulletFamilySpawner>();

        curSpawnerMasterPatternRot = Random.Range(0, 360);//so inital Rot is diffrent
        this.transform.rotation = Quaternion.Euler(Vector3.forward * curSpawnerMasterPatternRot);
        curSpawnerIndividualPatternRot = Random.Range(0, 360);

        spawnerPatternPos = new List<Vector3>();
        spawnerPatternRot = new List<float>();
        seperationBetweenSpawnerPatterns = 2 * spawnerMasterAngularHalfSpread / (spawnerPatternCount - 1);
        for (int i = 0; i < spawnerPatternCount; i++)
        {
            spawnerPatternRot.Add(i * seperationBetweenSpawnerPatterns - spawnerMasterAngularHalfSpread);//
            spawnerPatternPos.Add(AlterVector(Vector3.up * spawnerMasterPatternRadius, i * seperationBetweenSpawnerPatterns - spawnerMasterAngularHalfSpread));
        }
    }

    public void Update()
    {
        curSpawnerMasterPatternRot += spawnerMasterPatternRotSpeed * Time.deltaTime;
        curSpawnerIndividualPatternRot += spawnerPatternRotSpeed * Time.deltaTime;

        if(Time.time - previousBFSStartTime > bulletFamilySpawnerSpawnCycleInterval)
        {
            StartCoroutine(GetAndHandleBulletFamilySpawner());
        }
        
    }

    public IEnumerator GetAndHandleBulletFamilySpawner()
    {
        previousBFSStartTime = Time.time;
        List<BulletFamilySpawner> _bfsList = new List<BulletFamilySpawner>();
        for (int i = 0; i < spawnerPatternCount; i++)
        {
            _bfsList.Add(GameManager.GetBulletFamilySpawner());
            AssignBFSStartValues(_bfsList[i]);
            _bfsList[i].SetupRuntimeValues();
            _bfsList[i].transform.parent = this.transform;
            _bfsList[i].transform.localPosition = spawnerPatternPos[i] + AlterVector(bulletFamilySpawnerMovementKeyPointsPos[0], spawnerPatternRot[i]);
        }
        float _handlingStartTime = Time.time;
        float _progress = 0;
        Vector3 _bfsBaseLocalPos = bulletFamilySpawnerMovementKeyPointsPos[0];
        Vector3 _previousBFSBaseLocalLot = bulletFamilySpawnerMovementKeyPointsPos[0];
        //float _bfsBaseRot = Vector3.SignedAngle(Vector3.up, AlterVector(bulletFamilySpawnMovementKeyPointsPos[0], spawnerPatternRot[0]),Vector3.forward);
        float _bfsInfluencedRot = 0;//Rot influnced by bezier path
        while (_progress <= 1)
        {
            _progress = (Time.time - _handlingStartTime) / bulletFamilySpawnerMovementCyclePeriod;
            _bfsBaseLocalPos = GetBezierCurvePoint(bulletFamilySpawnerMovementKeyPointsPos, _progress);
            _bfsInfluencedRot = Vector3.SignedAngle(/*AlterVector(Vector3.up,_bfsBaseRot)*/ Vector3.up, (_bfsBaseLocalPos - _previousBFSBaseLocalLot), Vector3.forward);
            for (int i = 0; i < spawnerPatternCount; i++)
            {
                _bfsList[i].transform.localPosition = AlterVector(spawnerPatternPos[i], curSpawnerMasterPatternRot) + AlterVector(_bfsBaseLocalPos, spawnerPatternRot[i] + curSpawnerIndividualPatternRot +curSpawnerMasterPatternRot);
                _bfsList[i].transform.rotation = Quaternion.Euler(Vector3.forward * (_bfsInfluencedRot + spawnerPatternRot[i] + curSpawnerMasterPatternRot));
                if(Time.time - _bfsList[i].previousBulletFamilySpawnTime > bulletFamilySpawnInterval)
                {
                    _bfsList[i].previousBulletFamilySpawnTime = Time.time;//
                    StartCoroutine(_bfsList[i].GetAndHandleBulletFamily());
                }
            }
            _previousBFSBaseLocalLot = _bfsList[0].transform.localPosition;
            yield return null;
        }
        for (int i = 0; i < spawnerPatternCount; i++)
        {
            _bfsList[i].gameObject.SetActive(false);
            GameManager.availableBulletFamilySpawners.Add(_bfsList[i]);
        }
    }

    private void AssignBFSStartValues(BulletFamilySpawner _bfs)
    {
        _bfs.bulletFamilySideCount = bulletFamilySideCount;
        _bfs.bulletFamilyActiveSideCount = bulletFamilyActiveSideCount;
        _bfs.bulletFamilySideSkipInterval = bulletFamilySideSkipInterval;
        _bfs.bulletFamilyBulletPerSideCount = bulletFamilyBulletPerSideCount;
        _bfs.bulletFamilyBulletSpawnRadius = bulletFamilyBulletSpawnRadius;
        _bfs.bulletFamilySpawnInterval = bulletFamilySpawnInterval;
    }
}
