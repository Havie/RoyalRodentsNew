using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIESCMenu : MonoBehaviour
{
    public void imClicked()
    {
        GameManager.Instance.ShowPauseMenu();
    }
}
