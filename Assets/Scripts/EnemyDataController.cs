﻿using System.Collections;
using System.IO;
using UnityEngine;

/**
   * classes for managing enemy data especially loading enemy soldiers from xlsx/json files and spread them in scene.
   */

[System.Serializable]
public class Data
{
    public int number_of_levels;
    public Level[] levels;
}

[System.Serializable]
public class Level
{
    public Grid[] grids;
}

[System.Serializable]
public class Grid
{
    public Square[] square;
}

[System.Serializable]
public class Square
{
    public float x;
    public float z;
    public int vojak;
}

[System.Serializable]
public class EnemyDataController : MonoBehaviour
{
    [SerializeField]
    private Transform enemyPlayground;

    public GameObject[] soldierTypes;

    /**
   * load enemy's data, random choose one of the grids and set enemies in scene
   */
    public void LoadData()
    {
        string path = "Database/enemy_data";

        var textAsset = Resources.Load(path) as TextAsset;
        string json = textAsset.text;
        Data data = JsonUtility.FromJson<Data>(json);

        Level level = data.levels[GameManager.currentLevel-1];
        int grid_id = Random.Range(0, level.grids.Length);
        Square[] square = level.grids[grid_id].square;

        int enemySoldierCounter = 0;
        int maxSoldierHP = 0;

        foreach (var obj in square)
        {
            if (obj.vojak != 0)
            {
                var prefab = Resources.Load(soldierTypes[obj.vojak].name, typeof(GameObject)) as GameObject;
                GameObject soldier = GameManager.InstantateScaled(prefab, enemyPlayground);
                soldier.transform.localPosition = new Vector3(obj.x, 0.05f, obj.z);
                soldier.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                soldier.transform.localScale *= 5;
                soldier.tag = "EnemySoldier";
                soldier.GetComponent<HealthControl>().enabled = true;
                soldier.GetComponent<PlacableObject>().setIsScaled(true);

                var weapon = soldier.transform.Find("Weapon");
                if (weapon)
                {
                    enemySoldierCounter++;
                }

                soldier.GetComponent<RandomFancyAnimationSwitch>().soldierAnimator.SetBool("Static", false);

                if (soldier.GetComponent<HealthControl>().GetHealth() > maxSoldierHP)
                {
                    maxSoldierHP = soldier.GetComponent<HealthControl>().GetHealth();
                }
            }
        }
        // global settings
        GameManager.Instance.SetMaxSoldierHP(maxSoldierHP);
        GameManager.Instance.SetStartEnemySoldiersCount(enemySoldierCounter);

        GameManager.Instance.SetEnemySoldiersCount(enemySoldierCounter);
    }
}