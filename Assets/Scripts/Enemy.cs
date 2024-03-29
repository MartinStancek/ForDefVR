﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/**
     * Data structure for storing enemy and player soldier interactions. 
     * Used during determining which enemy should shoot.
     */

class ShootInteraction
{
    public GameObject enemySoldier { get; set; }
    public GameObject playerSoldier { get; set; }
    public double efficiency { get; set; }
}

/**
     * Script which controls enemys' soldiers behaviour.
     */

public class Enemy : MonoBehaviour
{
    /**
     * helper method which calls enemy turn method with 2s delay.
     */

    public void EnemyController()
    {
        Invoke("DoEnemyTurn", 2.0f);
    }

    /**
     * count effectivness for all enemysoldier-playersoldier permutations. 
     * @return shoot interaction object
     */
    private ShootInteraction CountTurnEffectiveness()
    {
        var enemySoldiers = GameObject.FindGameObjectsWithTag("EnemySoldier");
        var playerSoldiers = GameObject.FindGameObjectsWithTag("Vojak");
        ShootInteraction randomInteraction = null;

        List<ShootInteraction> shootInteractions = new List<ShootInteraction>();

        foreach (var enemy in enemySoldiers)
        {
            var weapon = enemy.transform.Find("Weapon");
            if (weapon)                                     //baits and medics dont pass this condition => no chance to choose them.
            {                                                                                          
                foreach (var player in playerSoldiers) {
                    if (randomInteraction == null)
                    {
                        //just in case if noone can shoot player. This enemy soldier will be chosen and hit his own teammate :) What an idiot lol!
                        randomInteraction = new ShootInteraction { enemySoldier = enemy, playerSoldier = player, efficiency = 0 };
                    }
              
                    var weaponRotation = weapon.rotation;   //store old weapon rotation
                    weapon.LookAt(player.transform);
                    var shooter = weapon.GetComponent<Shooter>();
                    
                    if (!shooter.TeamHit())   // enemy soldier didnt hit teammate. Valuable soldier :)      
                    {

                        // Atribute 1: find out possible damage of hit player.
                        // Effectiveness is based on possible player damage multiplied by factor 2.2

                        var playerWeapon = player.transform.Find("Weapon");
                        double efficiency = 0;
                        if (playerWeapon)
                        {
                            efficiency += playerWeapon.GetComponent<Shooter>().GetWeaponDamage() * 2.2;
                        }

                        // Atribute 2: find out current hp of player soldier
                        // Effectiveneess is based on (maxSoldierHP (per game) - current HP of player soldier) multiplied by factor 1.7

                        var HP = player.GetComponent<HealthControl>().GetHealth();
                        efficiency += (GameManager.Instance.GetMaxSoldierHP() - HP) * 1.7;

                        // Atribute 3: find out damage of current enemy soldier
                        // Effectiveness is based on enemy damage multiplied by factor 1.3
                        efficiency += weapon.GetComponent<Shooter>().GetWeaponDamage() * 1.3;

                        // Atribute 4: find out current hp of enemy soldier
                        // Effectiveneess is based on (maxSoldierHP (per game) - current HP of enemy soldier) multiplied by factor 1
                        efficiency += weapon.GetComponent<Shooter>().GetWeaponDamage();

                        // Store <EnemySoldier, PlayerSoldier, efficiency> interaction
                        shootInteractions.Add(new ShootInteraction { enemySoldier = enemy, playerSoldier = player, efficiency = efficiency});
                    }
                    weapon.rotation = weaponRotation;       //set weapon rotation to old state
                }
            }
        }

        if (shootInteractions.Count == 0)   //team hit
        {
            return randomInteraction;
        }
        else
        {
            shootInteractions.Sort((a, b) => a.efficiency.CompareTo(b.efficiency));  // sort interaction ascending based on efficiency 

            // final enemy-player interaction will be chosen from best efficiencies randomly. How many interactions should be considered
            // is based on number of enemy soldiers in current level.

           int randomRange = (int)Math.Round(GameManager.Instance.GetStartEnemySoldiersCount() / 4.5);
           var random_interaction = UnityEngine.Random.Range(0, Math.Min(randomRange, shootInteractions.Count));
           return shootInteractions[random_interaction];
        }
    }

    /**
   * enemy soldier execute its turn.
   */

    private void DoEnemyTurn()
    {
        var interaction = CountTurnEffectiveness();
        var enemy = interaction.enemySoldier;
        var player = interaction.playerSoldier;


        // Find weapon of chosen enemy soldier
        var weapon = enemy.transform.Find("Weapon");
        if (weapon)
        {
            weapon.LookAt(player.transform);
            var shooter = weapon.GetComponent<Shooter>();
            shooter.Shoot();
            shooter.ResetWeapon();
        }
        GameManager.Instance.SetPlayerTurnMode();
    }
}
