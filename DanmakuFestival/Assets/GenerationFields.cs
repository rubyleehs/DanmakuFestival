using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationFields : MonoBehaviour {

    [Header("EnemyAi")]
    public Vector2Int spawnerMasterPatternCount;

    [Header("Spawner Pattern Spawning")]
    public Vector2 spawnerMasterPatternRadius;
    public Vector2Int spawnerPatternCount;
    public Vector2 spawnerMasterAngularHalfSpread;
    
    [Header("Spawner Pattern Movement")]
    public Vector2 spawnerMasterPatternRotSpeed;
    public Vector2 spawnerPatternRotSpeed;

    [Header("Bullet Family Spawn Movement")]
    public Vector2Int bulletFamilySpawnMovementKeyPointsCount;
    public Vector2 bulletFamilySpawnMovementKeyPointsReach; //radius around spawner pattern, not the master spawner pattern
    public Vector2 bulletFamilySpawnMovementCyclePeriod;
    public Vector2 bulletFamilySpawnerSpawnCycleInterval;
}
