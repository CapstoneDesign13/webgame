using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitHandler : MonoBehaviour
{
    public ResourceLoader resourceLoader;
    public MapManager map;
    public TurnManager turn; 

    void Start()
    {
        // Create shared database
        ModDatabase sharedDatabase = ScriptableObject.CreateInstance<ModDatabase>();
        sharedDatabase.Initialize(); // set the singleton instance

        resourceLoader.database = sharedDatabase;

        resourceLoader.LoadMods();

        map.SpawnInitialUnits();
        turn.StartPlayerTurn();
    }
}