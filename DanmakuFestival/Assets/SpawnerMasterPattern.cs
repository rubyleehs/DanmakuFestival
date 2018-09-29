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
    private float curSpawnerMasterPatternRot = 0;
    private float curSpawnerIndividualPatternRot = 0;
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
        spawnerPatternCount = Random.Range(GameManager.generationFields.spawnerPatternCount.x, GameManager.generationFields.spawnerPatternCount.y + 1);

        //SpawnerPattern movement
        spawnerMasterPatternRotSpeed = Random.Range(GameManager.generationFields.spawnerMasterPatternRotSpeed.x, GameManager.generationFields.spawnerMasterPatternRotSpeed.y);
        spawnerPatternRotSpeed = Random.Range(GameManager.generationFields.spawnerPatternRotSpeed.x, GameManager.generationFields.spawnerPatternRotSpeed.y);

        //BulletFamily Spawn movement;
        bulletFamilySpawnMovementKeyPointsCount = Random.Range(GameManager.generationFields.bulletFamilySpawnMovementKeyPointsCount.x, GameManager.generationFields.bulletFamilySpawnMovementKeyPointsCount.y + 1);
        bulletFamilySpawnMovementKeyPointsPos = new List<Vector3>();
        for (int i = 0; i < bulletFamilySpawnMovementKeyPointsCount; i++)
        {
            bulletFamilySpawnMovementKeyPointsPos.Add(AlterVector(Vector2.up * Random.Range(GameManager.generationFields.bulletFamilySpawnMovementKeyPointsReach.x, GameManager.generationFields.bulletFamilySpawnMovementKeyPointsReach.y),Random.Range(0,360)));
        }
        bulletFamilySpawnMovementCyclePeriod = Random.Range(GameManager.generationFields.bulletFamilySpawnMovementCyclePeriod.x, GameManager.generationFields.bulletFamilySpawnMovementCyclePeriod.y);
        bulletFamilySpawnerSpawnCycleInterval = Random.Range(GameManager.generationFields.bulletFamilySpawnerSpawnCycleInterval.x, GameManager.generationFields.bulletFamilySpawnerSpawnCycleInterval.y);
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
            _bfsList[i].transform.localPosition = spawnerPatternPos[i] + AlterVector(bulletFamilySpawnMovementKeyPointsPos[0], spawnerPatternRot[i]);
        }
        float _handlingStartTime = Time.time;
        float _progress = 0;
        Vector3 _bfsBaseLocalPos = bulletFamilySpawnMovementKeyPointsPos[0];
        Vector3 _previousBFSBaseLocalLot = bulletFamilySpawnMovementKeyPointsPos[0];
        //float _bfsBaseRot = Vector3.SignedAngle(Vector3.up, AlterVector(bulletFamilySpawnMovementKeyPointsPos[0], spawnerPatternRot[0]),Vector3.forward);
        float _bfsInfluencedRot = 0;
        while (_progress <= 1)
        {
            _progress = (Time.time - _handlingStartTime) / bulletFamilySpawnMovementCyclePeriod;
            _bfsBaseLocalPos = GetBezierCurvePoint(bulletFamilySpawnMovementKeyPointsPos, _progress);
            _bfsInfluencedRot = Vector3.SignedAngle(/*AlterVector(Vector3.up,_bfsBaseRot)*/ Vector3.up, (_bfsBaseLocalPos - _previousBFSBaseLocalLot), Vector3.forward);
            for (int i = 0; i < spawnerPatternCount; i++)
            {
                _bfsList[i].transform.localPosition = AlterVector(spawnerPatternPos[i], curSpawnerMasterPatternRot) + AlterVector(_bfsBaseLocalPos, spawnerPatternRot[i] + curSpawnerIndividualPatternRot +curSpawnerMasterPatternRot);
                _bfsList[i].transform.rotation = Quaternion.Euler(Vector3.forward * (_bfsInfluencedRot + spawnerPatternRot[i] + curSpawnerMasterPatternRot));
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
