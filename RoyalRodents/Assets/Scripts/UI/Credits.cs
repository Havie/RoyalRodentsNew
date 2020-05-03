using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public Sprite _img;
    public GameObject _object;

    public bool _on;

    private void Start()
    {
        _img = Resources.Load<Sprite>("UI/CREDITS");
        if (_object)
        {
            Image sp = _object.GetComponent<Image>();
            sp.sprite = _img;
        }
    }

    public void ShowCredits()
    {
        _on = !_on;
        if (_object)
            _object.gameObject.SetActive(_on);
    }
}
