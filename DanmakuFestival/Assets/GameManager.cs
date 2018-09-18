using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GenerationFields I_generationFields;
    public static GenerationFields generationFields;

    private void Awake()
    {
        generationFields = I_generationFields;
    }

}
