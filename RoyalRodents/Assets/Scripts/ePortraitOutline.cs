using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ePortraitOutline : MonoBehaviour
{
    public bool _RectangleMode;
    [SerializeField] private Sprite _Img;

    // Start is called before the first frame update
    void Start()
    {
        if (!_RectangleMode)
            _Img = Resources.Load<Sprite>("TmpAssests/Blank_Portrait");
        else
            _Img = Resources.Load<Sprite>("TmpAssests/Blank_Portrait_Rect");

       this.GetComponent<SpriteRenderer>().sprite = _Img;
       
        // Debug.Log("start Over");
    }

    public void SetRectMode(bool cond)
    {
        _RectangleMode = cond;
    }

}
