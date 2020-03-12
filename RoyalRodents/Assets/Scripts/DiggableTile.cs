using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggableTile : MonoBehaviour
{
    [SerializeField] private bool _TopSoil;

    [SerializeField] private Sprite _TunnelSprite;

    private void Awake()
    {
        if(_TopSoil)
            _TunnelSprite = Resources.Load<Sprite>("Environment/GDD_200_Royal_Rodents_Dirt_01_Tile");
        else
            _TunnelSprite = Resources.Load<Sprite>("Environment/GDD_200_Royal_Rodents_Dirt_02_Tile");

    }

    public void DigDown()
    {
       var sr= this.transform.GetComponent<SpriteRenderer>();
        if(sr)
        {
            sr.sprite = _TunnelSprite;
        }
    }
}
