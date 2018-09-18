using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerMasterPattern : VectorStuff {

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
    private List<Vector2> bulletFamilySpawnMovementKeyPointsPos;
    private float bulletFamilySpawnMovementCyclePeriod;
    private float bulletFamilySpawnerSpawnCycleInterval;

    //==========Runtime Values==========

    //Master Stats
    private List<BulletFamilySpawner> bulletFamilySpawner;
    private float curSpawnerMasterPatternRot;
    private float curSpawnerIndividualPatternRot;
    private float seperationBetweenSpawnerPatterns;

    //Individual Spawner stats
    private List<Vector2> spawnerPatternPos;
    private List<float> spawnerPatternRot;

    //==================================


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
        bulletFamilySpawnMovementKeyPointsPos = new List<Vector2>();
        for (int i = 0; i < bulletFamilySpawnMovementKeyPointsCount; i++)
        {
            bulletFamilySpawnMovementKeyPointsPos.Add(AlterVector(Vector2.up * Random.Range(GameManager.generationFields.bulletFamilySpawnMovementKeyPointsReach.x, GameManager.generationFields.bulletFamilySpawnMovementKeyPointsReach.y),Random.Range(0,360),0));
        }
        bulletFamilySpawnMovementCyclePeriod = Random.Range(GameManager.generationFields.bulletFamilySpawnMovementCyclePeriod.x, GameManager.generationFields.bulletFamilySpawnMovementCyclePeriod.y);
        bulletFamilySpawnerSpawnCycleInterval = Random.Range(GameManager.generationFields.bulletFamilySpawnerSpawnCycleInterval.x, GameManager.generationFields.bulletFamilySpawnerSpawnCycleInterval.y);
    }

    public void SetupRuntimeValues()//needs to be called everytime Start Values are changed;
    {
        bulletFamilySpawner = new List<BulletFamilySpawner>();

        curSpawnerMasterPatternRot = Random.Range(0, 360);//so inital Rot is diffrent
        this.transform.rotation = Quaternion.Euler(Vector3.forward * curSpawnerMasterPatternRot);
        curSpawnerIndividualPatternRot = Random.Range(0, 360);

        spawnerPatternPos = new List<Vector2>();
        spawnerPatternRot = new List<float>();
        seperationBetweenSpawnerPatterns = 2 * spawnerMasterAngularHalfSpread / (spawnerPatternCount - 1);
        for (int i = 0; i < spawnerPatternCount; i++)
        {
            spawnerPatternRot.Add(i * seperationBetweenSpawnerPatterns - spawnerMasterAngularHalfSpread);
            spawnerPatternPos.Add(AlterVector(Vector3.up * spawnerMasterPatternRadius, i * seperationBetweenSpawnerPatterns - spawnerMasterAngularHalfSpread, 0));
        }
    }

}
