using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAssignmentVFX : MonoBehaviour
{

    public GameObject _clickedVFXPrefab;
    public GameObject _glowVFXPrefab;

    public ParticleSystem[] _onClicked;
    public ParticleSystem[] _glowVFX;

    public void Start()
    {
        setUpVFX();
       
    }

    public void setUpVFX()
    {
        if(_clickedVFXPrefab==null)
        {
            _clickedVFXPrefab = Resources.Load<GameObject>("UI/vfx_onClick");
            if (_clickedVFXPrefab)
            {
                var vfx = GameObject.Instantiate(_clickedVFXPrefab, this.transform.position, this.transform.rotation);
                vfx.transform.SetParent(this.transform);
                vfx.transform.localScale = new Vector3(1, 1, 1);
                _onClicked = vfx.GetComponentsInChildren<ParticleSystem>();
            }
            else
                Debug.LogError("Cant find clicked VFX");
        }

        if (_glowVFXPrefab == null)
        {
            _glowVFXPrefab = Resources.Load<GameObject>("UI/vfx_glow");
            if (_glowVFXPrefab)
            {
                var vfx = GameObject.Instantiate(_glowVFXPrefab, this.transform.position, this.transform.rotation);
                vfx.transform.SetParent(this.transform);
                vfx.transform.localScale = new Vector3(1, 1, 1);
                _glowVFX = vfx.GetComponentsInChildren<ParticleSystem>();
            }
            else
                Debug.LogError("Cant find glow VFX");
        }

        UIAssignmentMenu.Instance.setVFX(this);
        PlayGlowAnim(false);
    }



    //This way that spawns it via prefab over and over seems bad
    public void SpawnClickedVFX()
    {
        if(_clickedVFXPrefab)
        {
            var vfx = GameObject.Instantiate(_clickedVFXPrefab, this.transform.position, this.transform.rotation);
            vfx.transform.SetParent(this.transform);
            vfx.transform.localScale = new Vector3(1, 1, 1);
            var particleSys = vfx.GetComponent<ParticleSystem>();
            Destroy(vfx, particleSys.main.duration + particleSys.main.startLifetime.constantMax);
        }
        else
            Debug.LogError("SpawnClicked Failed");
    }

    public void PlayClickAnim()
    {
        foreach (ParticleSystem p in _onClicked)
        {
            p.Stop();
            p.Play();
        }
    }

    public void PlayGlowAnim(bool b)
    {
        foreach (ParticleSystem p in _glowVFX)
        {
            p.gameObject.SetActive(b);
        }
    }


    //No Better script to put this on sadly
    public void OnMouseEnter()
    {
        MVCController.Instance.CheckClicks(false);
        MVCController.Instance.SetAssignmentDummy(true);
    }
    public void OnMouseExit()
    {
        MVCController.Instance.CheckClicks(true);
        MVCController.Instance.SetAssignmentDummy(false);
    }
}
