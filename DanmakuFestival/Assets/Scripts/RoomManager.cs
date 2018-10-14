using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MathfExtras {

    //========Start Values========
    private int numberOfLayers;
    private int[] numberOfRooms;
    private float[] layerRotSpeed;

    //========Runtime Values========
    private List<List<RoomGen>> rooms;
    private List<Transform> layers;

    public void Start()
    {
        RandomizeStartValues();
        CreateWorldMap();
    }
    private void RandomizeStartValues()
    {
        numberOfLayers = RandomValue(GameManager.roomGenerationFields.numberOfLayers);
        numberOfRooms = new int[numberOfLayers];
        layerRotSpeed = new float[numberOfLayers];
        for (int i = 0; i < numberOfRooms.Length; i++)
        {
            numberOfRooms[i] = RandomValue(GameManager.roomGenerationFields.numberOfRoomsAtLayer[i]);
            layerRotSpeed[i] = RandomValue(GameManager.roomGenerationFields.layerRotSpeed);
            if (i % 2 == 0) layerRotSpeed[i] *= -1;
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime * GameManager.timeScale;
        for (int i = 0; i < numberOfLayers; i++)
        {
            layers[i].rotation *= Quaternion.Euler(Vector3.forward * layerRotSpeed[i] * dt);
            foreach (Transform child in layers[i])
            {
                child.rotation *= Quaternion.Euler(Vector3.forward * -layerRotSpeed[i] * dt);
                if ((PlayerManager.player.position - child.position).sqrMagnitude <= Mathf.Pow(GameManager.roomGenerationFields.revealRoomRadius *GameManager.roomGenerationFields.roomScale, 2))
                {
                    child.gameObject.SetActive(true);
                }
                else child.gameObject.SetActive(false);//
            }
        }
    }

    private void CreateWorldMap()
    {
        rooms = new List<List<RoomGen>>();
        layers = new List<Transform>();
        for (int _l = 0; _l < numberOfLayers; _l++)
        {
           CreateLayer(_l);
        }
    }

    private void CreateLayer(int _layerIndex)
    {
        Transform _layer = Instantiate(GameManager.roomGenerationFields.layerGO, Vector3.zero,Quaternion.identity,this.transform).transform;
        layers.Add(_layer);
        List<RoomGen> _layerRooms = new List<RoomGen>();
        Vector2[] roomPoints = GetPointsAroundOrigin(Vector2.zero, numberOfRooms[_layerIndex], (_layerIndex + 0.5f) * GameManager.roomGenerationFields.distanceBetweenLayers * GameManager.roomGenerationFields.roomScale, 180, Random.Range(0f,360f));
        for (int i = 0; i < numberOfRooms[_layerIndex]; i++)
        {
            RoomGen _room = Instantiate(GameManager.roomGenerationFields.roomGO, roomPoints[i], Quaternion.identity, _layer).GetComponent<RoomGen>();
            //_room.transform.position = roomPoints[i];
            _layerRooms.Add(_room);
        }
        rooms.Add(_layerRooms);
    }
}
