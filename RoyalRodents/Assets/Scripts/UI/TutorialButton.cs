using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButton : MonoBehaviour
{
    public TMPro.TextMeshProUGUI descriptionDisplay;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayWilbur();
    }

    public void setButton(string des)
    {
        descriptionDisplay.text = des;
    }

    public void ButtonClicked()
    {
        TutorialController.Instance.IncrementPage();
        Destroy(gameObject);
    }
}
