using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStaminaHitBox : MonoBehaviour
{
    public UIStaminaButton _button;

    public void imClicked()
    {
        if (_button)
            _button.imClicked();
    }
 }
