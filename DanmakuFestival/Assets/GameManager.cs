using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MathfExtras {

    public GenerationFields I_generationFields;
    public static GenerationFields generationFields;

    public static List<BulletFamilySpawner> availableBulletFamilySpawners = new List<BulletFamilySpawner>();

    private void Awake()
    {
        generationFields = I_generationFields;
    }


    public static BulletFamilySpawner GetBulletFamilySpawner()
    {
        BulletFamilySpawner _chosenBFS;
        if (availableBulletFamilySpawners.Count == 0 || true)
        {
            _chosenBFS = Instantiate(generationFields.bulletFamilySpawnerGO).GetComponent<BulletFamilySpawner>();
        }
        else
        {
            _chosenBFS = availableBulletFamilySpawners[0];
            _chosenBFS.gameObject.SetActive(true);
            availableBulletFamilySpawners.RemoveAt(0);
        }
        return _chosenBFS;
    }
}


