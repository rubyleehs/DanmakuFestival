using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour {

    public GameObject spawnerMasterPattern;

    private int spawnerMasterPatternCount;

    public void Start()
    {
        RandomizeStats(); 
    }

    public void RandomizeStats() //Only EnemyAI RandomizeStats() has to be in Start()
    {
        spawnerMasterPatternCount = Random.Range(GameManager.generationFields.spawnerMasterPatternCount.x, GameManager.generationFields.spawnerMasterPatternCount.y);
    }
}
