﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK; 

[RequireComponent(typeof(TriggerGrabAndUse))]
public class Shooter : MonoBehaviour
{
    [SerializeField]
    private GameObject shootPoint;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private float bulletSpeed;

    [SerializeField]
    private int bulletCount = 1;

    [SerializeField]
    private PlayableObject playableObject;

    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private Material disabledColor;

    [SerializeField]
    private ParticleSystem shotParticles;

    [SerializeField]
    private Animator shotTrajectileAnimator;

    [SerializeField]
    private int damage;

    private bool shootingEnabled = true;
    private Material[] materialsOriginal;
    private Material[] materialsDisabled;

    private int originalBulletCount;

    private int layerMask = 1 << 8;

    private TriggerGrabAndUse grabMechansm;

    // Start is called before the first frame update
    void Start()
    {
        shotParticles.Stop();
        GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += (object sender, InteractableObjectEventArgs e) => { Shoot(); };

        GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += ObjectUngrabbed;
        originalBulletCount = bulletCount;

        materialsOriginal = new Material[meshRenderer.materials.Length];
        materialsDisabled = new Material[meshRenderer.materials.Length];
        
        for (var i = 0; i < meshRenderer.materials.Length; i++)
        {
            materialsDisabled[i] = disabledColor;
            materialsOriginal[i] = meshRenderer.materials[i];
        }

        grabMechansm = GetComponent<TriggerGrabAndUse>();
    }

    public void Shoot()
    {
        if (bulletCount > 0 && shootingEnabled)
        {
            shotParticles.Play();

            shotTrajectileAnimator.Play("Trajectile");

            RaycastHit hit;

            if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out hit, 100, layerMask))
            {
                HealthControl healthControl = hit.transform.GetComponent<HealthControl>();
                if (healthControl != null)
                {
                    healthControl.TakeDamage(damage);
                }
            }
            bulletCount--;
        }
        else
        {
            //TODO: upozornenie ze uz nema naboje a mal by pustit zbran
        }

        if(bulletCount == 0)
        {
            grabMechansm.ActionFinished();
        }
    }
  
    void Update()
    {
        Debug.DrawRay(shootPoint.transform.position, shootPoint.transform.forward, Color.green);
    }

    private void ObjectUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        if (bulletCount == 0 || GameManager.Instance.gamemode == GameManager.GameMode.MENU)
        {
            playableObject.AfterFinishedAction();
            ControlShooting(true);
        }


    }

    public void ResetWeapon() {
        bulletCount = originalBulletCount;
    }

    public void ControlShooting(bool value)
    {
        // To avoid a sequence of coincident changes
        if (value != shootingEnabled)
        {
            shootingEnabled = value;
            meshRenderer.materials = value ? materialsOriginal : materialsDisabled;
        }
    }
}
