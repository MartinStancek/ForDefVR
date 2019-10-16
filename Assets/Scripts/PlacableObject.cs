﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlacableObject : MonoBehaviour
{
    public const float SizeOfSquare = 0.5f;

    public GameObject vojakDestroy;

    public bool isScaled = false;
    public bool isGrabbed = false;
    public bool wasChanged = false;

    private bool onCollision = false;
    private Transform lastCollisionObj;
    private Transform snappedOn;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += ObjectGrabbed;
        GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += ObjectUnGrabbed;
        vojakDestroy = GameObject.Find("Kos");
    }

    private void ObjectGrabbed(object sender, InteractableObjectEventArgs e)
    {
        if (isScaled)
        {
            this.transform.localScale /= 5;
            isScaled = false;
            vojakDestroy.GetComponent<VojakDestroy>().destroyed = false;
        }
        isGrabbed = true;
        wasChanged = true;

        transform.localPosition += Vector3.up/100; // [FIX] vojak sa obcas zasekne do podlahy

        snappedOn?.GetComponentInChildren<CubeHighlighter>()?.HighLight();
    }

    private void ObjectUnGrabbed(object sender, InteractableObjectEventArgs e)
    {
        isGrabbed = false;
        if (onCollision)
        {
            SnapToObject();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Policko")
        {
            lastCollisionObj = collision.transform;

            if ( !isGrabbed)
            {
                SnapToObject();
            }
            onCollision = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        onCollision = false;
    }

    private void SnapToObject()
    {
        if (!isScaled)
        {
            this.transform.localScale *= 5;
            this.transform.position = lastCollisionObj.position + new Vector3(0f, lastCollisionObj.localScale.y / 2, 0f)
                + new Vector3(0f, transform.localScale.y / 2, 0f);
            this.transform.rotation = Quaternion.identity;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            lastCollisionObj.GetComponentInChildren<CubeHighlighter>().ResetColor();
            isScaled = true;
            snappedOn = lastCollisionObj;
        }

    }
}
