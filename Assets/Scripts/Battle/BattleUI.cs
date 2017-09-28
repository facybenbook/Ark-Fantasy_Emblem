﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

    public static bool moving = false;

    /// <summary>The Battle Manager, mostly just for its GetState() method</summary>
    public BattleManager bm;

    /// <summary>The canvas containing all the buttons for issuing player Orders</summary>
    public GameObject commandsList;

    /// <summary>
    /// The text associated with the Tech button on the command list. 
    /// If a Tech timer is above 0, that number is displayed in red on the button.
    /// </summary>
    public Text techButtonText;

    /// <summary>
    /// The window containing the active party member's special abilties for selection
    /// </summary>
    public Canvas specialSelection;

    ///<summary>An overlaying, half-transparent image that visually declares to the player when the game is paused or not </summary>
    public Canvas pauseScreen;

    /// <summary>
    /// The button which manually pauses the action. Unavailabe in Battle Prep state
    /// </summary>
    public Button pauseButton;

    /// <summary>Text which appears when the player is choosing a target</summary>
    public Text targetingText;

    /// <summary>On-screen button to cancel targetting if the player does not prefer using hotkeys</summary>
    public Button targetCancelButton;

    /// <summary>
    /// Window which pops up when a character gains enough experience to level up after a move
    /// </summary>
    public Image levelUpWindow;

    /// <summary>
    /// Text to display in the level up window when a character is leveled up
    /// </summary>
    public Text levelUpText;

    /***BATTLE PROJECTION***/

    /// <summary>
    /// A box which is used to display the battle projection stats
    /// </summary>
    public Image battleProjection;

    /// <summary>
    /// Text within the Battle Projection canvas regarding the initiator
    /// </summary>
    public Text projectionInfo;

    /// <summary>
    /// A box which displays battle projection stats for enemy moves
    /// </summary>
    public Image enemyProjection;

    /// <summary>
    /// Text within the enemy Battle Projection canvas regarding the action
    /// </summary>
    public Text eProjectionInfo;

    /***BUTTONS***/

    /// <summary> The cancel button in the BP window; unavailable if it is an Enemy Projection</summary>
    public Button cancelButton;

    /***STATES***/

    /// <summary>
    /// Determines which UI elements to turn on or off depending on the game state
    /// </summary>
    public void ChangingState()
    {
        switch (bm.GetState())
        {
            case "NORMAL":
                pauseScreen.gameObject.SetActive(false);
                commandsList.SetActive(false);
                SetProjection(false, "");
                SetEnemyProjection(false, "");
                cancelButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(true);
                levelUpWindow.gameObject.SetActive(false);
                break;

            case "ANIMATING":
                
                break;

            case "COMMANDING":
                SetTargetting(false);

                commandsList.SetActive(true);
                specialSelection.enabled = false;
                pauseButton.gameObject.SetActive(false);

                int timer = bm.pParty.GetActiveMember().TechTimer;

                if (timer == 0)
                {
                    techButtonText.text = "5) TECH";
                    techButtonText.color = Color.black;
                }
                else if (timer == 1)
                {
                    techButtonText.text = "5) TECH recharging: " + timer + " turn";
                    techButtonText.color = Color.gray;
                }
                else
                {
                    techButtonText.text = "5) TECH recharging: " + timer + " turns";
                    techButtonText.color = Color.gray;
                }

                break;

            case "SPECIAL_SELECTION":
                commandsList.SetActive(false);
                SetTargetting(false);
                SetProjection(false, "");
                specialSelection.enabled = true;
                break;

            case "TILE_SELECTION":
                commandsList.SetActive(false);
                SetProjection(false, "");
                break;

            case "SELECTION":
                commandsList.SetActive(false);
                SetProjection(false, "");

                SetTargetting(true);
                specialSelection.enabled = false;
                break;

            case "LEVEL_UP":
                levelUpWindow.gameObject.SetActive(true);
                break;

            case "PLAYER_PROJECTION":
                SetTargetting(false);
                SetTargetting(false);
                specialSelection.enabled = false;
                break;

            case "ENEMY_PROJECTION":
                cancelButton.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(false);
                break;

            case "PAUSED":
                pauseScreen.gameObject.SetActive(true);
                break;

            default: break;
        }
    }

    /// <summary>
    /// Physical attack projection. Pauses game and display BP window.
    /// </summary>
    /// <param name="atk">Amount of damage that might be done</param>
    /// <param name="hit">% chance to hit target</param>
    /// <param name="crit">% chance to land a critical on target</param>
    public void SetProjectionInfo(bool isPlayer, int atk, int hit, int crit, string description)
    {
        string projectionText =
            "ATK: " + atk + "\n" +
            "HIT: " + hit + "%\n" +
            "CRIT: " + crit + "%" + "\n\n" +
            description;

        if (isPlayer) SetProjection(true, projectionText);
        else SetEnemyProjection(true, projectionText);
            
    }

    /// <summary>
    /// Enable battle projection according to move type
    /// </summary>
    /// <param name="isPlayer">If the entity making a move is the player or not</param>
    /// <param name="special">The active special, if one is being used (can be null)</param>
    /// <param name="user">The entity making the move</param>
    public void SetProjectionInfo(bool isPlayer, Special special, Entity user)
    {

    }

    /// <summary>
    /// Projection method for healing or repair spells/techs
    /// </summary>
    /// <param name="effect">Amount that the target will be healed for</param>
    public void SetProjectionInfo(bool isPlayer, int effect, int crit, string description)
    {
        string projectionText =
            "HP Up: " + effect +
            "\nBonus: " + crit + "%" + "\n\n" +
            description;

        if (isPlayer) SetProjection(true, projectionText);
        else SetEnemyProjection(true, projectionText);
    }

    /// <summary>
    /// Projection method for status effect actions
    /// </summary>
    /// <param name="effect">The status effect in question</param>
    /// <param name="hit">The chance the effect will actually be instilled upon the target</param>
    public void SetProjectionInfo(bool isPlayer, string effect, int hit, string description)
    {
        string projectionText =
            effect + "\n" +
            "Success: " + hit + "%" + "\n\n" +
            description;

        if (isPlayer) SetProjection(true, projectionText);
        else SetEnemyProjection(true, projectionText);
    }

    /// <summary>
    /// Projection method for moving to a new tile
    /// </summary>
    /// <param name="isPlayer">True - the player is making this move; False - it's the enemy's move</param>
    /// <param name="newTile">The new tile to be moving to</param>
    public void SetProjectionInfo(bool isPlayer, Tile newTile)
    {
        string projectionText =
            "Move To " + newTile.tileName + "\n\nTerrain Effects:";

        if (newTile.effect1 != Tile.EFFECT.NONE) projectionText += "\n" + newTile.effect1;
        if (newTile.effect2 != Tile.EFFECT.NONE) projectionText += "\n" + newTile.effect2;

        if (isPlayer) SetProjection(true, projectionText);
        else SetEnemyProjection(true, projectionText);

        moving = true;
    }

    /// <summary>
    /// Projection method for multi-target attacks
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="atk"></param>
    /// <param name="description"></param>
    public void SetProjectionInfo(bool isPlayer, int atk, string description)
    {
        string projectionText =
            "Base ATK: " + atk + "\n" +
            description + "\n" +
            "Hit chance and damage vary";

        if (isPlayer) SetProjection(true, projectionText);
        else SetEnemyProjection(true, projectionText);
    }

    /// <summary>
    /// Enable Level Up window and change text approriately
    /// </summary>
    /// <param name="text">The stats of the entity leveling up</param>
    public void SetLevelUpText(string text)
    {
        bm.SetState("LEVEL_UP");
        levelUpText.text = text;
    }

    //Shorthand to enabling/disabling the Battle Projection game object
    private void SetProjection(bool b, string text)
    {
        battleProjection.gameObject.SetActive(b);
        projectionInfo.text = text;
    }

    //Shorthand to enabling.disabling the Enemy's BP game object
    private void SetEnemyProjection(bool b, string text)
    {
        enemyProjection.gameObject.SetActive(b);
        eProjectionInfo.text = text;
    }

    //Shorthand to enable/disable targetting UI elements
    private void SetTargetting(bool b)
    {
        targetingText.enabled = b;
        targetCancelButton.gameObject.SetActive(b);
    }
}
