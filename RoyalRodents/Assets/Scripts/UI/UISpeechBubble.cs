using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpeechBubble : MonoBehaviour
{
    private static UISpeechBubble _instance;

    public GameObject R_Face;
    public GameObject R_Bubble;
    public GameObject R_Speech;

    public GameObject L_Face;
    public GameObject L_Bubble;
    public GameObject L_Speech;


    public static UISpeechBubble Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<UISpeechBubble>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            //if not, set instance to this
            _instance = this;
        }
        //If instance already exists and it's not this:
        else if (_instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
            return;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        ShowRightSide(false);
        ShowLeftSide(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ShowRightSide(true);
    }

    public void ShowRightSide(bool cond)
    {
        if (R_Face != null && R_Bubble != null && R_Speech != null)
        {
            R_Face.SetActive(cond);
            R_Bubble.SetActive(cond);
            R_Speech.SetActive(cond);

            if(cond)
                StartCoroutine(OffDelay('r'));
        }
    }

    public void ShowLeftSide(bool cond)
    {
        if (L_Face != null && L_Bubble != null && L_Speech != null)
        {
            L_Face.SetActive(cond);
            L_Bubble.SetActive(cond);
            L_Speech.SetActive(cond);

            if(cond)
                StartCoroutine(OffDelay('l'));
        }
    }

    //Would like to set up a fade animation is time permits
    IEnumerator OffDelay(char side)
    {
        yield return new WaitForSecondsRealtime(6);
        if (side == 'r')
            ShowRightSide(false);
        else
            ShowLeftSide(false);


    }

}
