using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenerationFields {

    [Header("General")]
    public GameObject bulletFamilySpawnerGO;
    public GameObject spawnerMasterPatternGO;
    public GameObject bulletFamilyGO;
    public GameObject bulletGO;

    [Header("EnemyAi")]
    public Vector2Int spawnerMasterPatternCount;

    [Header("Spawner Pattern Spawning")]
    public Vector2 spawnerMasterPatternRadius;
    public Vector2Int spawnerPatternCount;
    public Vector2 spawnerMasterAngularHalfSpread;
    
    [Header("Spawner Pattern Movement")]
    public Vector2 spawnerMasterPatternRotSpeed;
    public Vector2 spawnerPatternRotSpeed;

    [Header("Bullet Family Spawner Movement")]
    public Vector2Int bulletFamilySpawnerMovementKeyPointsCount;
    public Vector2 bulletFamilySpawnerMovementKeyPointsReach; //radius around spawner pattern, not the master spawner pattern
    public Vector2 bulletFamilySpawnerMovementCyclePeriod;
    public Vector2 bulletFamilySpawnerSpawnCycleInterval;

    [Header("Bullet Family Spawning")]
    public Vector2 bulletFamilySpawnInterval;

    [Header("Bullet Family Bullet Spawning")]
    public Vector2Int bulletFamilySideCount;
    public Vector2Int bulletFamilyBulletPerSideCount;
    public Vector2 bulletFamilyBulletSpawnRadius;
}
