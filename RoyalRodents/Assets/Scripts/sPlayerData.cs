using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class sPlayerData
{
    //PS
    public float _Health;
    public float _Stamina;


    //PM
    public float[] position;
    //public DiggableTile _CurrentTopTile;
   // public DiggableTile _CurrentTunnelTile;
    public bool _FacingRight;
    public bool _InGround;
    public bool _isAttacking;
    public float _YHeight;

    public sPlayerData(PlayerStats ps, PlayerMovement pm)
    {
        //PS
        _Health = ps.getHealth();
        _Stamina = ps.getStamina();


        //PM
        _YHeight = pm.getYHeight();
        position = new float[3];

        position[0] = pm.getLastAboveGroundLoc().x;
        position[1] = _YHeight; // dont need 
        position[2] = 0; // dont need

        // _CurrentTopTile = pm.getCurrentSoilTile();
        // _CurrentTunnelTile = pm.getCurrentTunnelTile();

        _FacingRight = pm.getIsFacingRight();
        _InGround = pm.getInGround();
        _isAttacking = pm.getIsAttacking();

    }
}
