using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MathfExtras {

    public GenerationFields I_generationFields;
    public static GenerationFields generationFields;

    public static List<BulletFamilySpawner> availableBulletFamilySpawners = new List<BulletFamilySpawner>();
    public static List<BulletFamily> availableBulletFamily = new List<BulletFamily>();
    public static List<Transform> availableBullets = new List<Transform>();

    private void Awake()
    {
        generationFields = I_generationFields;
    }


    public static BulletFamilySpawner GetBulletFamilySpawner()
    {
        BulletFamilySpawner _chosenBFS;
        if (availableBulletFamilySpawners.Count == 0)
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

    public static BulletFamily GetBulletFamily()
    {
        BulletFamily _chosenBF;
        if (availableBulletFamilySpawners.Count == 0 || true)
        {
            _chosenBF = Instantiate(generationFields.bulletFamilyGO).GetComponent<BulletFamily>();
        }
        else
        {
            _chosenBF = availableBulletFamily[0];
            _chosenBF.bullets = new List<Transform>();
            _chosenBF.gameObject.SetActive(true);
            availableBulletFamilySpawners.RemoveAt(0);
        }
        return _chosenBF;
    }

    public static Transform GetBullet()
    {
        Transform _chosenBullet;
        if (availableBullets.Count == 0 || true)
        {
            _chosenBullet = Instantiate(generationFields.bulletGO).transform;
        }
        else
        {
            _chosenBullet = availableBullets[0];
            _chosenBullet.gameObject.SetActive(true);
            availableBulletFamilySpawners.RemoveAt(0);
        }
        return _chosenBullet;
    }
}


