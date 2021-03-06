﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcDamageHandler : MonoBehaviour
{
    public float health = 5;
    public int collectibleSpawnCountMin = 2;
    public int collectibleSpawnCountMax = 5;
    public GameObject damageParticle;
    public GameObject collectibleBubble;
    public GameObject specialCollectibleBubble;
    public string dropsUpgrade = "none";
    
    private GameEventHandler _eventHandler;
    private Architect _architect;

    
    private void Awake()
    {
        _eventHandler = FindObjectOfType<GameEventHandler>();
        _architect = FindObjectOfType<Architect>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        Vector3 pointOfCollision = other.gameObject.transform.position;
        // Only take damage from player weapons
        if (other.CompareTag("PlayerProjectile"))
        {
            health -= other.GetComponent<ProjectileBehavior>().damage;
            Destroy(other.gameObject);
            
            _eventHandler.npcGetHit.Play();
            SpawnDamageParticles(pointOfCollision);
        }
        
        if (other.CompareTag("PlayerMissile"))
        {
            health -= other.GetComponent<MissileBehavior>().damage;
            Destroy(other.gameObject);
            
            _eventHandler.npcGetHit.Play();
            SpawnDamageParticles(pointOfCollision);
        }
        
        if (health <= 0)
        {
            Destroy(gameObject);
            
            _eventHandler.explosion.Play();
            
            SpawnDamageParticles(pointOfCollision);
            SpawnCollectibleBubbles();

            // When child count reaches 1, the parent bubble is in fact empty
            // No, I don't think it makes sense either
            if (gameObject.transform.parent.childCount == 1)
            {
                _architect.ConquerBubble(transform.parent.position);
            }
        }
    }

    void SpawnDamageParticles(Vector3 position)
    {
        int numberOfParticles = UnityEngine.Random.Range(5, 7);
        
        // Spawn collectible bubbles
        for (int i = 0; i < numberOfParticles; i++) 
        {
            Instantiate(damageParticle, position, transform.rotation);
        }
    }

    void SpawnCollectibleBubbles()
    {
        int numberOfBubbles = UnityEngine.Random.Range(collectibleSpawnCountMin, collectibleSpawnCountMax);
        
        // Spawn collectible bubbles
        for (int i = 0; i < numberOfBubbles; i++) 
        {
            Instantiate(collectibleBubble, transform.position, transform.rotation);
        }

        // Spawn special bubble, if this enemy has not been destroyed before
        if (!_eventHandler.unlockedUpgrades.Contains(dropsUpgrade) && dropsUpgrade != "none")
        {
            Instantiate(specialCollectibleBubble, transform.position, transform.rotation);
            
            // Unlock the upgrade and make it available for purchase
            _eventHandler.unlockedUpgrades.Add(dropsUpgrade);
            _eventHandler.SetAvailableUpgradeData();
        }
    }
}
