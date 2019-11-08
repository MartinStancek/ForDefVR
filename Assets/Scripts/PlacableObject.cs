﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlacableObject : MonoBehaviour
{
    public const float SizeOfSquare = 0.5f;

    public GameObject spawner;
    public float yCoordinate;

    public bool isScaled = false;
    public bool isGrabbed = false;
    private bool wasDestroyed = false;
    private bool enteredSpawningArea = false;

    private bool onCollision = false;
    private Transform lastCollisionObj;

    private GameObject targetPolicko;
    private Transform snappedOn;
 

    public Spawning spawning;
    public Rigidbody rigidBody;

    //public List<BoxCollider> boxColliders;
    public GameObject cube;

    

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += ObjectGrabbed;
        GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += ObjectUnGrabbed;
        spawning = spawner.gameObject.GetComponent<Spawning>();

    }

    
    private void ObjectGrabbed(object sender, InteractableObjectEventArgs e)
    {
        if (isScaled)
        {
            this.transform.localScale /= 5;
            isScaled = false;
        }
        isGrabbed = true;
        snappedOn?.GetComponentInChildren<CubeHighlighter>()?.HighLight();
    }

    private void ObjectUnGrabbed(object sender, InteractableObjectEventArgs e)
    {
        isGrabbed = false;
        rigidBody.isKinematic = false;
        if (enteredSpawningArea)
        {
            Destroy(this.gameObject);
            spawning.onWrongPlacement();
        }
        if (onCollision)
        {
            SnapToObject();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Policko")
        {
            CubeHighlighter cubeHighlighter = collision.gameObject.transform.GetChild(0).GetComponent<CubeHighlighter>();
            if ( !isGrabbed)
            {
                if (cubeHighlighter.occupyingObject == null || cubeHighlighter.occupyingObject == this.gameObject)
                {
                    if (targetPolicko == null)
                    {
                        targetPolicko = collision.gameObject;
                        lastCollisionObj = targetPolicko.transform;
                    }
                    if (targetPolicko == collision.gameObject)
                    {
                        cubeHighlighter.occupyingObject = this.gameObject;
                        cubeHighlighter.placableObject = this;
                        SnapToObject();
                    }
                }
                else if (!wasDestroyed && targetPolicko == collision.gameObject)
                {
                    Destroy(this.gameObject);
                    spawning.onWrongPlacement();
                    wasDestroyed = true;
                }
                    
            }
            onCollision = true;
        }
        else if (collision.gameObject.tag == "Ground" && !wasDestroyed)
        {
            Destroy(this.gameObject);
            spawning.onWrongPlacement();
            wasDestroyed = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        onCollision = false;
        
        if (collision.gameObject.tag == "Policko")
        {
            CubeHighlighter cubeHighlighter = collision.gameObject.transform.GetChild(0).GetComponent<CubeHighlighter>();
            if (cubeHighlighter.occupyingObject == this.gameObject && isGrabbed)
            {
                targetPolicko = null;
                cubeHighlighter.occupyingObject = null;
                cubeHighlighter.placableObject = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Spawning")
        {
            enteredSpawningArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == spawner)
        {
            enteredSpawningArea = false;
        }
    }

    private void SnapToObject()
    {
        if (!isScaled)
        {
            rigidBody.isKinematic = true;           
            this.transform.localScale *= 5;
            this.transform.position = lastCollisionObj.position + new Vector3(0f, lastCollisionObj.localScale.y*3, 0f);                
            this.transform.rotation = Quaternion.identity;
            lastCollisionObj.GetComponentInChildren<CubeHighlighter>().ResetColor();
            isScaled = true;
            snappedOn = lastCollisionObj;
        }
    }
}
