﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject soldierModel;

    [SerializeField]
    private GameObject weapon;

    private Transform sceneObjects;


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<VRTK_InteractableObject>().InteractableObjectUnused += ObjectChoosenToPlay;

        sceneObjects = GameObject.FindGameObjectWithTag("SceneObjects").transform;
    }

    private void ObjectChoosenToPlay(object sender, InteractableObjectEventArgs e)
    {
        sceneObjects.localScale *= 3;
        VRTK_DeviceFinder.PlayAreaTransform().position = transform.position; //teleport to soldier place 
        soldierModel.SetActive(false);
        weapon.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
