﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The party containing the enemies the player's party is currently fighting
/// </summary>
public class EnemyParty : Party {

    /// <summary>
    /// Behaves very similary to the quivalent method of the PlayerParty class
    /// </summary>
    public override void OrganizeParty(Vector2[] coords, int scaling)
    {
        party = FindObjectOfType<EnemyAvatar>().getParty();

        for (int i = 0; i < party.Count; i ++)
        {
            Instantiate(party[i], new Vector2(coords[i].x * scaling, coords[i].y * scaling), Quaternion.identity, transform);
        }

        Enemy[] members = FindObjectsOfType<Enemy>();

        for (int i = 0; i < party.Count; i++)
        {
            party[i] = members[i];
            party[i].SetParty(this);
        }
    }

    //TODO - HANDLE ENEMIES//
    protected override void Update()
    {
        if (bm == null) return; //Battle Manager not set

        base.Update();

        switch (bm.GetState())
        {
            case "ENEMY_PROJECTION":

                target.ChangeColor("target");
                activeMember.ChangeColor("active");

                break;
        }

        foreach (Enemy e in party)
        {
            if (e.Ready && bm.GetState() == "NORMAL")
            {
                e.Behavior();
                bm.SetState("ENEMY_PROJECTION");
                CalculateAction("ATTACK");

                e.ChangeColor("active");
                target.ChangeColor("target");
            }
        }
    }
}
