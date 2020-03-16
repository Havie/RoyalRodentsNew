﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSaveableObjects : MonoBehaviour
{
    [SerializeField]
    PlayerStats _playerStats;
    [SerializeField]
    PlayerMovement _playerMovement;
    [SerializeField]
    GameObject[] _buildings;


    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
        else
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }
    public void Save()
    {
        sSaveSystem.SavePlayer(_playerStats, _playerMovement);
    }

    public void Load()
    {
        _playerStats.LoadData();
        _playerMovement.LoadData();
    }
}