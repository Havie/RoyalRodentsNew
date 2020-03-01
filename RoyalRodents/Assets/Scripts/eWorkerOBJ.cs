using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eWorkerOBJ : MonoBehaviour
{
    public bool _RectangleMode;
    public bool _Locked;
    [SerializeField] private Sprite _Img;

    // Start is called before the first frame update
    void Start()
    {
        if (!_RectangleMode)
            _Img = Resources.Load<Sprite>("TmpAssests/Locked");
        else
            _Img = Resources.Load<Sprite>("TmpAssests/Locked");

        if(_Locked)
            this.GetComponent<SpriteRenderer>().sprite = _Img;
    }

    public void SetRectMode(bool cond)
    {
        _RectangleMode = cond;
    }
    public void Locked(bool cond)
    {
        _Locked = cond;
    }
}
