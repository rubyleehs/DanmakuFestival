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

    //Bullet Family Compound Movement;
    private float bfMoveStartSpeed;
    private float bfAngularMoveStartSpeed;

    private float bfMoveAcceleration;

    private float bfCompondActionDuration;
    private float bfCompondRotSpeed;

    //Bullet Family Bullet Movement
    private float bulletMoveStartSpeed;
    private float bulletAngularMoveStartSpeed;

    private float bulletMoveAcceleration;
    private float bulletMoveAccelerationPingPongInterval;

    private float bulletAngularMoveAcceleration;
    private float bulletAngularMoveAccelerationPingPongInterval;

    //==========Runtime Values==========

    List<BulletFamily> bulletFamilies;

    //Master Stats
    private float curSpawnerMasterPatternRot = 0;
    private float curSpawnerIndividualPatternRot = 0;
    private float seperationBetweenSpawnerPatterns;

    private float timeSincePreviousBFSSpawn = -1;//can use as a startDelay by using time enemy spawn + start Delay

    //Individual Spawner stats
    private List<Vector3> spawnerPatternPos;
    private List<float> spawnerPatternRot;

    //==========Bullet Family Values==========
    //Bullet Family Spawning
    private float bulletFamilySpawnInterval;
    private float bulletFamilyCompoundShapeRotationReadjustment;

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
        spawnerMasterPatternRadius = Random.Range(GameManager.bHGenerationFields.spawnerMasterPatternRadius.x, GameManager.bHGenerationFields.spawnerMasterPatternRadius.y);
        spawnerMasterAngularHalfSpread = Random.Range(GameManager.bHGenerationFields.spawnerMasterAngularHalfSpread.x, GameManager.bHGenerationFields.spawnerMasterAngularHalfSpread.y);
        spawnerPatternCount = Random.Range(GameManager.bHGenerationFields.spawnerPatternCount.x, GameManager.bHGenerationFields.spawnerPatternCount.y + 1);

        //SpawnerPattern movement
        spawnerMasterPatternRotSpeed = Random.Range(GameManager.bHGenerationFields.spawnerMasterPatternRotSpeed.x, GameManager.bHGenerationFields.spawnerMasterPatternRotSpeed.y);
        spawnerPatternRotSpeed = Random.Range(GameManager.bHGenerationFields.spawnerPatternRotSpeed.x, GameManager.bHGenerationFields.spawnerPatternRotSpeed.y);

        //BulletFamily Spawn movement;
        bulletFamilySpawnerMovementKeyPointsCount = Random.Range(GameManager.bHGenerationFields.bulletFamilySpawnerMovementKeyPointsCount.x, GameManager.bHGenerationFields.bulletFamilySpawnerMovementKeyPointsCount.y + 1);
        bulletFamilySpawnerMovementKeyPointsPos = new List<Vector3>();
        for (int i = 0; i < bulletFamilySpawnerMovementKeyPointsCount; i++)
        {
            bulletFamilySpawnerMovementKeyPointsPos.Add(AlterVector(Vector2.up * Random.Range(GameManager.bHGenerationFields.bulletFamilySpawnerMovementKeyPointsReach.x, GameManager.bHGenerationFields.bulletFamilySpawnerMovementKeyPointsReach.y), Random.Range(0, 360)));
        }
        bulletFamilySpawnerMovementCyclePeriod = Random.Range(GameManager.bHGenerationFields.bulletFamilySpawnerMovementCyclePeriod.x, GameManager.bHGenerationFields.bulletFamilySpawnerMovementCyclePeriod.y);
        bulletFamilySpawnerSpawnCycleInterval = Random.Range(GameManager.bHGenerationFields.bulletFamilySpawnerSpawnCycleInterval.x, GameManager.bHGenerationFields.bulletFamilySpawnerSpawnCycleInterval.y);

        //BulletFamily Spawning
        bulletFamilySpawnInterval = Random.Range(GameManager.bHGenerationFields.bulletFamilySpawnInterval.x, GameManager.bHGenerationFields.bulletFamilySpawnInterval.y);

        //BulletFamily Compound Movement
        bfMoveStartSpeed = Random.Range(GameManager.bHGenerationFields.bfMoveStartSpeed.x, GameManager.bHGenerationFields.bfMoveStartSpeed.y);
        bfAngularMoveStartSpeed = Random.Range(GameManager.bHGenerationFields.bfAngularMoveStartSpeed.x, GameManager.bHGenerationFields.bfAngularMoveStartSpeed.y);

        bfMoveAcceleration = Random.Range(GameManager.bHGenerationFields.bfMoveAcceleration.x, GameManager.bHGenerationFields.bfMoveAcceleration.y);

        bfCompondActionDuration = Random.Range(GameManager.bHGenerationFields.bfCompondActionDuration.x, GameManager.bHGenerationFields.bfCompondActionDuration.y);
        bfCompondRotSpeed = Random.Range(GameManager.bHGenerationFields.bfCompondRotSpeed.x, GameManager.bHGenerationFields.bfCompondRotSpeed.y);

        //BulletFamily Bullet Spawning
        bulletFamilySideCount = Random.Range(GameManager.bHGenerationFields.bulletFamilySideCount.x, GameManager.bHGenerationFields.bulletFamilySideCount.y + 1);
        bulletFamilyActiveSideCount = Random.Range(1, bulletFamilySideCount + 1);
        bulletFamilySideSkipInterval = Random.Range(1, bulletFamilySideCount);
        bulletFamilyBulletPerSideCount = Random.Range(GameManager.bHGenerationFields.bulletFamilyBulletPerSideCount.x, GameManager.bHGenerationFields.bulletFamilyBulletPerSideCount.y + 1);
        bulletFamilyBulletSpawnRadius = Random.Range(GameManager.bHGenerationFields.bulletFamilyBulletSpawnRadius.x, GameManager.bHGenerationFields.bulletFamilyBulletSpawnRadius.y);

        //BulletFamily Bullet Movement;
        bulletMoveStartSpeed = Random.Range(GameManager.bHGenerationFields.bulletMoveStartSpeed.x, GameManager.bHGenerationFields.bulletMoveStartSpeed.y);
        bulletAngularMoveStartSpeed = Random.Range(GameManager.bHGenerationFields.bulletMoveStartSpeed.x, GameManager.bHGenerationFields.bulletMoveStartSpeed.y);

        bulletMoveAcceleration = Random.Range(GameManager.bHGenerationFields.bulletMoveAcceleration.x, GameManager.bHGenerationFields.bulletMoveAcceleration.y);
        if (Random.Range(0f, 1f) < 0.6) bulletAngularMoveAcceleration = 0;
        bulletMoveAccelerationPingPongInterval = Random.Range(GameManager.bHGenerationFields.bulletMoveAccelerationPingPongInterval.x, GameManager.bHGenerationFields.bulletMoveAccelerationPingPongInterval.y);

        bulletAngularMoveAcceleration = Random.Range(GameManager.bHGenerationFields.bulletAngularMoveAcceleration.x, GameManager.bHGenerationFields.bulletAngularMoveAcceleration.y);
        bulletAngularMoveAccelerationPingPongInterval = Random.Range(GameManager.bHGenerationFields.bulletAngularMoveAccelerationPingPongInterval.x, GameManager.bHGenerationFields.bulletAngularMoveAccelerationPingPongInterval.y);
        if (Random.Range(0f, 1f) < 0.6) bulletAngularMoveAcceleration = 0;

        //Special Conditions
        if (bulletFamilyBulletPerSideCount <= 2 || Random.Range(0f, 1f) > GameManager.bHGenerationFields.chanceOfCompoundBullets) bfCompondActionDuration = 0;
        if (bulletFamilyActiveSideCount < GameManager.bHGenerationFields.minActiveBFSpawnerSidesForCompoundRot) bfCompondRotSpeed = 0;
    }

    public void SetupRuntimeValues()//needs to be called everytime Start Values are changed;
    {
        //bulletFamilySpawner = new List<BulletFamilySpawner>();
        bulletFamilies = new List<BulletFamily>();
        curSpawnerMasterPatternRot = Random.Range(0, 360);//so inital Rot is diffrent
        this.transform.rotation = Quaternion.Euler(Vector3.forward * curSpawnerMasterPatternRot);
        curSpawnerIndividualPatternRot = Random.Range(0, 360);

        spawnerPatternPos = new List<Vector3>();
        spawnerPatternRot = new List<float>();
        seperationBetweenSpawnerPatterns = 2 * spawnerMasterAngularHalfSpread / (spawnerPatternCount - 1);
        for (int i = 0; i < spawnerPatternCount; i++)
        {
            spawnerPatternRot.Add(i * seperationBetweenSpawnerPatterns - spawnerMasterAngularHalfSpread);
            spawnerPatternPos.Add(AlterVector(Vector3.up * spawnerMasterPatternRadius, i * seperationBetweenSpawnerPatterns - spawnerMasterAngularHalfSpread));
        }
        bulletFamilyCompoundShapeRotationReadjustment = -360f / bulletFamilySideCount * bulletFamilySideSkipInterval * bulletFamilyActiveSideCount * 0.5f;
    }

    private void Update()
    {
        curSpawnerMasterPatternRot += spawnerMasterPatternRotSpeed * Time.deltaTime * GameManager.timeScale;
        curSpawnerIndividualPatternRot += spawnerPatternRotSpeed * Time.deltaTime * GameManager.timeScale;
        timeSincePreviousBFSSpawn += Time.deltaTime * GameManager.timeScale;
        if(timeSincePreviousBFSSpawn > bulletFamilySpawnerSpawnCycleInterval)
        {
            StartCoroutine(GetAndHandleBulletFamilySpawner());
        }
        ManageBFMovement();
    }

    private void ManageBFMovement()
    {
        for (int bfIndex = 0; bfIndex < bulletFamilies.Count; bfIndex++)
        {
            bulletFamilies[bfIndex].index = bfIndex;
            if(bulletFamilies[bfIndex].timeSinceCompoundStart == -Mathf.PI)
            {
                bulletFamilies[bfIndex].timeSinceCompoundStart = 0;
                bulletFamilies[bfIndex].bfMoveSpeed = bfMoveStartSpeed;
                bulletFamilies[bfIndex].bfAngularMoveSpeed = bfAngularMoveStartSpeed;
                bulletFamilies[bfIndex].bulletAngularMoveSpeed = bulletAngularMoveStartSpeed;
                bulletFamilies[bfIndex].bulletMoveSpeed = bulletMoveStartSpeed;
            }
            if (bulletFamilies[bfIndex].timeSinceCompoundStart < bfCompondActionDuration)
            {
                bulletFamilies[bfIndex].timeSinceCompoundStart += Time.deltaTime * GameManager.timeScale;
                bulletFamilies[bfIndex].transform.rotation *= Quaternion.Euler(Vector3.forward * (bfCompondRotSpeed + bfAngularMoveStartSpeed) * Time.deltaTime * GameManager.timeScale);
                bulletFamilies[bfIndex].transform.position += AlterVector(Vector3.up * bulletFamilies[bfIndex].bfMoveSpeed * Time.deltaTime * GameManager.timeScale, bulletFamilyCompoundShapeRotationReadjustment + bulletFamilies[bfIndex].bfAngularMoveSpeed * Time.deltaTime * GameManager.timeScale + bulletFamilies[bfIndex].bulletFamilyCompoundRotationReadjustment);

                //if (this.transform.position.x * this.transform.position.x > GameManager.squaredGameBoundaryDist || (this.transform.position.y * this.transform.position.y > GameManager.squaredGameBoundaryDist)) Desummon();
                bulletFamilies[bfIndex].bfMoveSpeed += bfMoveAcceleration * Time.deltaTime * GameManager.timeScale;
            }
            else
            {
                if (bulletFamilies[bfIndex].timeSincebfBulletMoveIntervalStart == -Mathf.PI)
                {
                    bulletFamilies[bfIndex].timeSincebfBulletMoveIntervalStart = 0.5f * bulletMoveAccelerationPingPongInterval;
                    bulletFamilies[bfIndex].timeSincebfBulletAngularMoveIntervalStart = 0.5f * bulletAngularMoveAccelerationPingPongInterval;
                }
                bulletFamilies[bfIndex].timeSincebfBulletMoveIntervalStart += Time.deltaTime * GameManager.timeScale;
                bulletFamilies[bfIndex].timeSincebfBulletAngularMoveIntervalStart += Time.deltaTime * GameManager.timeScale;

                if (bulletFamilies[bfIndex].bullets.Count > 0)
                {
                    bulletFamilies[bfIndex].bulletMoveSpeed += bulletMoveAcceleration * Time.deltaTime *  GameManager.timeScale;
                    bulletFamilies[bfIndex].bulletAngularMoveSpeed += bulletAngularMoveAcceleration * Time.deltaTime * GameManager.timeScale;

                    for (int i = 0; i < bulletFamilies[bfIndex].bullets.Count; i++)
                    {
                        bulletFamilies[bfIndex].bullets[i].transform.rotation *= Quaternion.Euler(Vector3.forward * bulletFamilies[bfIndex].bulletAngularMoveSpeed * Time.deltaTime * GameManager.timeScale);
                        bulletFamilies[bfIndex].bullets[i].transform.position += bulletFamilies[bfIndex].bullets[i].transform.up * bulletFamilies[bfIndex].bulletMoveSpeed * Time.deltaTime * GameManager.timeScale;

                        if (bulletFamilies[bfIndex].bullets[i].transform.position.x * bulletFamilies[bfIndex].bullets[i].transform.position.x > GameManager.squaredGameBoundaryDist || (bulletFamilies[bfIndex].bullets[i].transform.position.y * bulletFamilies[bfIndex].bullets[i].transform.position.y > GameManager.squaredGameBoundaryDist))
                        {
                            bulletFamilies[bfIndex].bullets[i].gameObject.SetActive(false);
                            GameManager.availableBullets.Add(bulletFamilies[bfIndex].bullets[i]);
                            bulletFamilies[bfIndex].bullets.RemoveAt(i);
                            i--;
                        }
                    }

                    if (bulletFamilies[bfIndex].timeSincebfBulletMoveIntervalStart > bulletMoveAccelerationPingPongInterval)
                    {
                        bulletFamilies[bfIndex].timeSincebfBulletMoveIntervalStart = 0;
                        bulletFamilies[bfIndex].bulletMoveAcceleration = -bulletFamilies[bfIndex].bulletMoveAcceleration;
                    }
                    if (bulletFamilies[bfIndex].timeSincebfBulletAngularMoveIntervalStart > bulletAngularMoveAccelerationPingPongInterval)
                    {
                        bulletFamilies[bfIndex].timeSincebfBulletAngularMoveIntervalStart = 0;
                        bulletFamilies[bfIndex].bulletAngularMoveAcceleration = -bulletFamilies[bfIndex].bulletAngularMoveAcceleration;
                    }
                }
                else
                {
                    bulletFamilies[bfIndex].Desummon();
                    bulletFamilies.RemoveAt(bfIndex);
                    bfIndex--;
                }
            }
        }
    }

    public IEnumerator GetAndHandleBulletFamilySpawner()
    {
        timeSincePreviousBFSSpawn = 0;
        List<BulletFamilySpawner> _bfsList = new List<BulletFamilySpawner>();
        for (int i = 0; i < spawnerPatternCount; i++)
        {
            _bfsList.Add(GameManager.GetBulletFamilySpawner());
            AssignBFSStartValues(_bfsList[i]);
            _bfsList[i].SetupRuntimeValues();
            _bfsList[i].transform.parent = this.transform;
            _bfsList[i].transform.localPosition = spawnerPatternPos[i] + AlterVector(bulletFamilySpawnerMovementKeyPointsPos[0], spawnerPatternRot[i]);
        }
        float _timeSinceHandleStart = 0;
        float _progress = 0;
        Vector3 _bfsBaseLocalPos = bulletFamilySpawnerMovementKeyPointsPos[0];
        Vector3 _previousBFSBaseLocalLot = bulletFamilySpawnerMovementKeyPointsPos[0];
        //float _bfsBaseRot = Vector3.SignedAngle(Vector3.up, AlterVector(bulletFamilySpawnMovementKeyPointsPos[0], spawnerPatternRot[0]),Vector3.forward);
        float _bfsInfluencedRot = 0;//Rot influnced by bezier path
        while (_progress <= 1)
        {
            _timeSinceHandleStart += Time.deltaTime * GameManager.timeScale;
            _progress = _timeSinceHandleStart / bulletFamilySpawnerMovementCyclePeriod;
            _bfsBaseLocalPos = GetBezierCurvePoint(bulletFamilySpawnerMovementKeyPointsPos, _progress);
            _bfsInfluencedRot = Vector3.SignedAngle(/*AlterVector(Vector3.up,_bfsBaseRot)*/ Vector3.up, (_bfsBaseLocalPos - _previousBFSBaseLocalLot), Vector3.forward);
            for (int i = 0; i < spawnerPatternCount; i++)
            {
                _bfsList[i].transform.localPosition = AlterVector(spawnerPatternPos[i], curSpawnerMasterPatternRot) + AlterVector(_bfsBaseLocalPos, spawnerPatternRot[i] + curSpawnerIndividualPatternRot +curSpawnerMasterPatternRot);
                _bfsList[i].transform.rotation = Quaternion.Euler(Vector3.forward * (_bfsInfluencedRot + spawnerPatternRot[i] + curSpawnerMasterPatternRot));
                _bfsList[i].timeSincePreviousBulletFamily += Time.deltaTime * GameManager.timeScale;
                if (_bfsList[i].timeSincePreviousBulletFamily > bulletFamilySpawnInterval)
                {
                    _bfsList[i].timeSincePreviousBulletFamily = 0;//
                    BulletFamily _bf = _bfsList[i].GetAndSetupBulletFamily(); //- bulletFamilyActiveSideCount * bulletFamilySideSkipInterval * bulletFamilySideCount * 0.001388f));
                    _bf.bulletFamilyCompoundRotationReadjustment = _bfsInfluencedRot + spawnerPatternRot[i] + curSpawnerMasterPatternRot;
                    bulletFamilies.Add(_bf);
                }
            }
            _previousBFSBaseLocalLot = _bfsList[0].transform.localPosition;
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < spawnerPatternCount; i++)
        {
            _bfsList[i].gameObject.SetActive(false);
            GameManager.availableBulletFamilySpawners.Add(_bfsList[i]);
        }
    }

    public void AssignBFSStartValues(BulletFamilySpawner _bfs)
    {
        _bfs.spawnerMasterPattern = this;
        //Bullet Spawning
        _bfs.bulletFamilySideCount = bulletFamilySideCount;
        _bfs.bulletFamilyActiveSideCount = bulletFamilyActiveSideCount;
        _bfs.bulletFamilySideSkipInterval = bulletFamilySideSkipInterval;
        _bfs.bulletFamilyBulletPerSideCount = bulletFamilyBulletPerSideCount;
        _bfs.bulletFamilyBulletSpawnRadius = bulletFamilyBulletSpawnRadius;
        _bfs.bulletFamilySpawnInterval = bulletFamilySpawnInterval;
    }

}
