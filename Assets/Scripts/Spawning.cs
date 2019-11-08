﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spawning : MonoBehaviour
{
    public GameObject vojak;
    public int vojakSpawnPocetnost;

    public TextMesh pocetnostCounter;
        
    private GameObject vojakSpawned;
    private Vector3 vojakPosition;
    private Quaternion vojakRotation;
        
    private bool spawned = false;

    private String vojakName;

    public GameObject vojakPreFab;


    const int expectedHashSetSize = 3;
   
    // Start is called before the first frame update
    void Start()
    {
        vojakPosition = vojak.transform.position;
        vojakRotation = vojak.transform.rotation;
        vojakName = vojak.name;

        pocetnostCounter.text = "" + vojakSpawnPocetnost;
    }

    private void OnTriggerExit(Collider other)
    {
        if (vojakSpawnPocetnost > 0 && ((vojakSpawned != null && vojakSpawned.GetComponent<PlacableObject>().isGrabbed) || !spawned) && other.gameObject.transform.parent.name == vojakName)
        {
            if (vojakSpawnPocetnost > 1)
            {
                spawnVojak();
            }
            other.gameObject.transform.parent.transform.parent = this.transform.parent.transform.parent;
            decreaseCounter();
        }
    }

    public void onWrongPlacement()
    {
        if (vojakSpawnPocetnost == 0)
        {
            spawnVojak();
        }
        increaseCounter();
        spawned = false;

    }

    private void increaseCounter()
    {
        vojakSpawnPocetnost += 1;
        pocetnostCounter.text = "" + vojakSpawnPocetnost;
    }

    private void decreaseCounter()
    {
        vojakSpawnPocetnost -= 1;
        pocetnostCounter.text = "" + vojakSpawnPocetnost;
    }

    private void spawnVojak()
    {
        vojakSpawned = Instantiate(vojakPreFab, vojakPosition, vojakRotation) as GameObject;
        vojakSpawned.name = vojakName;
        vojakSpawned.transform.parent = this.transform.parent;
        vojakSpawned.GetComponent<PlacableObject>().spawner = this.gameObject;
        spawned = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
