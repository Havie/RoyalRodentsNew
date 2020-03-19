using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiggableTile : MonoBehaviour
{
    [SerializeField] private bool _NotDiggable;
    [SerializeField] private bool _TopSoil;
    [SerializeField] private bool _isOpen;
    private Sprite _TunnelSprite;

    public bool _Debugg=true;

    private void Awake()
    {
        if(_TopSoil)
            _TunnelSprite = Resources.Load<Sprite>("Environment/Dirt_03_topSoil");
        else
            _TunnelSprite = Resources.Load<Sprite>("Environment/GDD_200_Royal_Rodents_Dirt_02_Tile");

        if(_Debugg)
        {
            if (_NotDiggable)
                _TunnelSprite = Resources.Load<Sprite>("Environment/Impassible");
            else
                _TunnelSprite = Resources.Load<Sprite>("Environment/Passible");

            this.GetComponent<SpriteRenderer>().sprite = _TunnelSprite;

        }

    }

    public void DigTile()
    {
        if (!_NotDiggable)
        {
            var sr = this.transform.GetComponent<SpriteRenderer>();
            if (sr)
            {
                sr.sprite = _TunnelSprite;
            }
            _isOpen = true;
        }
        else
            Debug.LogWarning("Called Dig on a non diggable tile");
    }
    public bool isTopSoil()
    {
        return _TopSoil;
    }
    public bool isOpen()
    {
        return _isOpen;
    }
    public bool isDiggable()
    {
        return !_NotDiggable;
    }
}
