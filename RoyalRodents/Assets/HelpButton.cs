using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButton : MonoBehaviour
{
    public void imClicked()
    {
        GameManager.Instance.ShowHelpMenu();
        SoundManager.Instance.PlayClick();
    }
}
