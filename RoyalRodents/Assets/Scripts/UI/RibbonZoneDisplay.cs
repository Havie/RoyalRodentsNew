using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RibbonZoneDisplay : MonoBehaviour
{
    private Image _sr;

    public Color EnemyRed, PlayerBlue, NeutralWhite;
    
    // Start is called before the first frame update
    void Start()
    {
        _sr = GetComponent<Image>();
    }

    public void SetZoneRibbonDisplay(int zone)
    {
        Debug.Log("Attempting to set ribbon color");
        
        if (_sr)
        {
            if (zone == 0)
                _sr.color = NeutralWhite;
            else if (zone == 1)
                _sr.color = PlayerBlue;
            else if (zone == 2)
                _sr.color = EnemyRed;
        }
    }
}
