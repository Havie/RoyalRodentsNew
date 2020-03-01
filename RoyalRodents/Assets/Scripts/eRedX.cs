using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eRedX : MonoBehaviour
{
    public bool _RectangleMode;
    [SerializeField] private Sprite _Img;

    // Start is called before the first frame update
    void Start()
    {
        if (!_RectangleMode)
            _Img = Resources.Load<Sprite>("TmpAssests/Redx2");
        else
            _Img = Resources.Load<Sprite>("TmpAssests/Redx2");

        this.GetComponent<SpriteRenderer>().sprite = _Img;
    }

    public void SetRectMode(bool cond)
    {
        _RectangleMode = cond;
    }

}
