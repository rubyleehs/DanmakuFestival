using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MathfExtras {
    private static GameManager gameManager;
    public float I_dijkstraUpdateInterval;
    public static float dijkstraUpdateInterval;


    [HideInInspector]
    public static float timeScale = 1;

    public float I_squaredGameBoundaryDist;
    public static float squaredGameBoundaryDist;

    public BHGenerationFields I_bHGenerationFields;
    public static BHGenerationFields bHGenerationFields;

    public RoomGenerationFields I_roomGenerationFields;
    public static RoomGenerationFields roomGenerationFields;

    public static List<BulletFamilySpawner> availableBulletFamilySpawners = new List<BulletFamilySpawner>();
    public static List<BulletFamily> availableBulletFamily = new List<BulletFamily>();
    public static List<Transform> availableBullets = new List<Transform>();


    private void Awake()
    {
        if (gameManager != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            gameManager = this;
            dijkstraUpdateInterval = I_dijkstraUpdateInterval;
            bHGenerationFields = I_bHGenerationFields;
            roomGenerationFields = I_roomGenerationFields;//
            squaredGameBoundaryDist = I_squaredGameBoundaryDist;
        }
    }


    public static BulletFamilySpawner GetBulletFamilySpawner()
    {
        BulletFamilySpawner _chosenBFS;
        if (availableBulletFamilySpawners.Count == 0)
        {
            _chosenBFS = Instantiate(bHGenerationFields.bulletFamilySpawnerGO).GetComponent<BulletFamilySpawner>();
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
        if (availableBulletFamily.Count == 0 )
        {
            _chosenBF = Instantiate(bHGenerationFields.bulletFamilyGO).GetComponent<BulletFamily>();
        }
        else
        {
            _chosenBF = availableBulletFamily[0];
            _chosenBF.bullets = new List<Transform>();
            _chosenBF.gameObject.SetActive(true);
            availableBulletFamily.RemoveAt(0);
        }
        return _chosenBF;
    }

    public static Transform GetBullet()
    {
        Transform _chosenBullet;
        if (availableBullets.Count == 0 || true)
        {
            _chosenBullet = Instantiate(bHGenerationFields.bulletGO).transform;
        }
        else
        {
            _chosenBullet = availableBullets[0];
            _chosenBullet.gameObject.SetActive(true);
            availableBullets.RemoveAt(0);
        }
        return _chosenBullet;
    }
}


