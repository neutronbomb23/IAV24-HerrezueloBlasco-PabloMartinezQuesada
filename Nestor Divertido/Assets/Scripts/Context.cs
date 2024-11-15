﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context
{
    public Context(Player entity)
    {
        this.player = entity;
        this.enemies = new List<Enemy>();
        this.sampledPositions = new List<Vector3>();
        this.pickups = new List<Pickup>();
    }

    public void SetPlayer (Player player)
    {
        this.player = player;
    }

    public Player player { get; private set; }

    public List<Enemy> enemies { get; private set; }

    public List<Pickup> pickups { get; private set; }

    public List<Vector3> sampledPositions { get; private set; }

    public Enemy nearestEnemy { get; private set; }
    public Pickup nearestPickup { get; private set; }

    public void SetNearestPickup()
    {
        Pickup nearest = null;
        float minorDistSqr = Mathf.Infinity;

        foreach (var pickup in this.pickups)
        {
            float sqrDist = (player.transform.position - pickup.transform.position).sqrMagnitude;
            if (sqrDist < minorDistSqr)
            {
                minorDistSqr = sqrDist;
                nearest = pickup;
            }
        }
        this.nearestPickup = nearest;
    }

    public void SetNearestEnemy()
    {
        Enemy nearest = null;
        float minorDistSqr = Mathf.Infinity;

        foreach (var enemy in this.enemies)
        {
            float sqrDist = (player.transform.position - enemy.transform.position).sqrMagnitude;
            if (sqrDist < minorDistSqr)
            {
                minorDistSqr = sqrDist;
                nearest = enemy;
            }
        }
        this.nearestEnemy = nearest;
    }
}
