using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Image _BarFill;
    private float _FillAmnt;
    
    public void  Awake()
    {
        _BarFill = this.transform.GetComponent<Image>();
        
    }
    public void Start()
    {
        if (!_BarFill)
        {
            _BarFill = this.transform.GetComponent<Image>();
        }
    }

    public void showBars(bool b)
    {
        this.transform.gameObject.SetActive(b);
    }

    public void SetHealth(float ratio)
    {
        Debug.Log("Heard to set fill::" + ratio);
        if (_BarFill)
            _BarFill.fillAmount = ratio;
        else
            Debug.LogError("Cant find health bar");
    }
}


