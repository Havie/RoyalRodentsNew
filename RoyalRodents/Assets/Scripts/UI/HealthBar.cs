using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class HealthBar : MonoBehaviour
{
    public bool _isStaminaBar;
    public Image _BarFill;
    private float _FillAmnt;

    private float _displayTime=5f;
    private float _hitTime;
    private bool _started;
    
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
        if(!_isStaminaBar)
            this.transform.parent.gameObject.SetActive(b);
    }

    public void SetFillAmount(float ratio)
    {
        showBars(true);
        //Debug.Log("Heard to set fill::" + ratio);
        if (_BarFill)
            _BarFill.fillAmount = ratio;
        else
            Debug.LogError("Cant find fill bar");

        //every time were hit, we start a 5 second delay to turn off hp bars
        _hitTime = _displayTime;
        if(!_started)
            StartCoroutine(DisplayDelay());
    }

   
    IEnumerator DisplayDelay()
    {
        _started = true;
        while (_hitTime>0)
        {
            _hitTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(Time.deltaTime);
        showBars(false);
        _started = false;
    }
}
