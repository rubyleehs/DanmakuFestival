using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour {

    private int spawnerMasterPatternCount;

    public void Start()
    {
        RandomizeStats();
        for (int i = 0; i < spawnerMasterPatternCount; i++)
        {
            Instantiate(GameManager.generationFields.spawnerMasterPatternGO,this.transform.position,Quaternion.identity,this.transform);
        }
    }

    public void RandomizeStats() //Only EnemyAI RandomizeStats() has to be in Start()
    {
        spawnerMasterPatternCount = Random.Range(GameManager.generationFields.spawnerMasterPatternCount.x, GameManager.generationFields.spawnerMasterPatternCount.y);
    }
}
