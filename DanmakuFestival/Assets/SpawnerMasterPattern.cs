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
    private int bulletFamilySpawnMovementKeyPointsCount;
    private List<Vector3> bulletFamilySpawnMovementKeyPointsPos;
    private float bulletFamilySpawnMovementCyclePeriod;
    private float bulletFamilySpawnerSpawnCycleInterval;
    //private int bulletFamilySpawnMovementType = 0;

    //==========Runtime Values==========

    //Master Stats
    //private List<IEnumerator> bulletFamilySpawnerMovementHandlers;
    private float curSpawnerMasterPatternRot;
    private float curSpawnerIndividualPatternRot;
    private float seperationBetweenSpawnerPatterns;

    private float previousBFSStartTime = 1;//can use as a startDelay by using time enemy spawn + start Delay

    //Individual Spawner stats
    private List<Vector3> spawnerPatternPos;
    private List<float> spawnerPatternRot;

    //==================================
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
        spawnerPatternCount = Random.Range(GameManager.generationFields.spawnerPatternCount.x, GameManager.generationFields.spawnerPatternCount.y);

        //SpawnerPattern movement
        spawnerMasterPatternRotSpeed = Random.Range(GameManager.generationFields.spawnerMasterPatternRotSpeed.x, GameManager.generationFields.spawnerMasterPatternRotSpeed.y);
        spawnerPatternRotSpeed = Random.Range(GameManager.generationFields.spawnerPatternRotSpeed.x, GameManager.generationFields.spawnerPatternRotSpeed.y);

        //BulletFamily Spawn movement;
        bulletFamilySpawnMovementKeyPointsCount = Random.Range(GameManager.generationFields.bulletFamilySpawnMovementKeyPointsCount.x, GameManager.generationFields.bulletFamilySpawnMovementKeyPointsCount.y);
        bulletFamilySpawnMovementKeyPointsPos = new List<Vector3>();
        for (int i = 0; i < bulletFamilySpawnMovementKeyPointsCount; i++)
        {
            bulletFamilySpawnMovementKeyPointsPos.Add(AlterVector(Vector2.up * Random.Range(GameManager.generationFields.bulletFamilySpawnMovementKeyPointsReach.x, GameManager.generationFields.bulletFamilySpawnMovementKeyPointsReach.y),Random.Range(0,360),0));
        }
        bulletFamilySpawnMovementCyclePeriod = Random.Range(GameManager.generationFields.bulletFamilySpawnMovementCyclePeriod.x, GameManager.generationFields.bulletFamilySpawnMovementCyclePeriod.y);
        bulletFamilySpawnerSpawnCycleInterval = Random.Range(GameManager.generationFields.bulletFamilySpawnerSpawnCycleInterval.x, GameManager.generationFields.bulletFamilySpawnerSpawnCycleInterval.y);
        if(bulletFamilySpawnMovementKeyPointsCount %2 == 0)
        {
            //bulletFamilySpawnMovementType = (int)Random.Range(0, 3);
        }
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
            spawnerPatternRot.Add(i * seperationBetweenSpawnerPatterns - spawnerMasterAngularHalfSpread);
            spawnerPatternPos.Add(AlterVector(Vector3.up * spawnerMasterPatternRadius, i * seperationBetweenSpawnerPatterns - spawnerMasterAngularHalfSpread, 0));
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
            _bfsList[i].transform.localPosition = spawnerPatternPos[i] + AlterVector(bulletFamilySpawnMovementKeyPointsPos[0], spawnerPatternRot[i], 0);
        }
        float _handlingStartTime = Time.time;
        float _progress = 0;
        Vector3 _bfsBaseLocalPos = bulletFamilySpawnMovementKeyPointsPos[0];
        while (_progress <= 1)
        {
            _progress = (Time.time - _handlingStartTime) / bulletFamilySpawnMovementCyclePeriod;
            _bfsBaseLocalPos = GetBezierCurvePoint(bulletFamilySpawnMovementKeyPointsPos, _progress); 
            for (int i = 0; i < spawnerPatternCount; i++)
            {
                _bfsList[i].transform.position = AlterVector(spawnerPatternPos[i], curSpawnerMasterPatternRot,0) + AlterVector(_bfsBaseLocalPos, spawnerPatternRot[i] + curSpawnerIndividualPatternRot +curSpawnerMasterPatternRot, 0);
            }
            yield return null;
        }
        for (int i = 0; i < spawnerPatternCount; i++)
        {
            _bfsList[i].gameObject.SetActive(false);
            GameManager.availableBulletFamilySpawners.Add(_bfsList[i]);//
        }
    }

    /*
    public Vector3 BFSMoveTypeMods(Vector3 _pos, int _index)
    {
        Vector3 _moddedPos = _pos;
        switch (bulletFamilySpawnMovementType)
        {
            case 0:
                break;
            case 1:
                if (_index >= spawnerPatternCount * 0.5f) _moddedPos.x = -_moddedPos.x;
                break;
            case 2:
                if (_index%2 == 0) _moddedPos.x = -_moddedPos.x;
                break;
            default:
                break;
        }
        return _moddedPos;
    }
    */
}
