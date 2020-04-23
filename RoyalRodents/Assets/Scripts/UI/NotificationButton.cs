using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationButton : MonoBehaviour
{
    float deleteDelayTime = 5f;
    
    //Button Stats
    public float posX;

    //Button Display References
    public TextMeshProUGUI titleDisplay, descriptionDisplay;
    public Image iconDisplay;

    //Camera Reference
    private CameraController cameraReference;

    // Start is called before the first frame update
    void Start()
    {
        cameraReference = Camera.main.GetComponent<CameraController>();
        if (cameraReference == null)
            Debug.LogError("NotificationButton Cant Find Camera Controller");

        StartCoroutine(DeleteDelay());
    }

    private IEnumerator DeleteDelay()
    {
        yield return new WaitForSeconds(deleteDelayTime);

        Destroy(gameObject);
    }

    public void setButton(string title, string des, Sprite icon, float posX)
    {
        this.posX = posX;

        titleDisplay.text = title;
        descriptionDisplay.text = des;
        iconDisplay.sprite = icon;
    }

    public void ButtonClicked()
    {
        if (posX != -1)
        {
            //if assignment menu is closed, open it
            if (!UIAssignmentMenu.Instance.getAssignmentMenuActive())
                UIAssignmentMenu.Instance.ToggleMenu();

            //jump to posX with camera
            cameraReference.SetCameraX(posX);

            //destroy button
            Destroy(gameObject);
        }
    }
}
