using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager _instance;

    public AudioSource MusicController;
    public AudioSource SFXController;
    public AudioSource SFXController2;
    public AudioSource SFXController3;

    public AudioClip _day;
    public AudioClip _night;
    public AudioClip[] _combat;
    public AudioClip _horn;

    public AudioClip _buttonClick;
    public AudioClip _construction;
    public AudioClip _demolish;
    public AudioClip _assign;
    public AudioClip _dismiss;
    public AudioClip _wilbur;

    public AudioClip[] _pickup;
    public AudioClip _crown;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<SoundManager>();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            PlayCombat();
        if (Input.GetKeyDown(KeyCode.O))
            PlayResource();
    }


    public void PlayDay()
    {
        if(MusicController)
        {
            MusicController.Stop();
            MusicController.clip = _day;
            MusicController.Play();
        }
    }
    public void PlayNight()
    {
        if (MusicController)
        {
            MusicController.Stop();
            MusicController.clip = _night;
            MusicController.Play();
        }
    }


    public void PlayCombat()
    {
        if (!SFXController || !SFXController2)
            return;

        if(SFXController.isPlaying==false)
        {
            int _choice = Random.Range(0, _combat.Length);
            SFXController.Stop();
            SFXController.clip = _combat[_choice];
            SFXController.Play();
        }
        else
        {
            int _choice = Random.Range(0, _combat.Length);
            SFXController2.Stop();
            SFXController2.clip = _combat[_choice];
            SFXController2.Play();
        }
    }

    public void PlayResource()
    {
        if (!SFXController || !SFXController2)
            return;

        if (SFXController.isPlaying == false)
        {
            int _choice = Random.Range(0, _pickup.Length);
            SFXController.Stop();
            SFXController.clip = _pickup[_choice];
            SFXController.Play();
        }
        else
        {
            int _choice = Random.Range(0, _pickup.Length);
            SFXController2.Stop();
            SFXController2.clip = _pickup[_choice];
            SFXController2.Play();
        }
    }
    public void PlayCrown()
    {
        if (!SFXController || !SFXController2)
            return;

        if (SFXController.isPlaying == false)
        {
            SFXController.Stop();
            SFXController.clip = _crown;
            SFXController.Play();
        }
        else
        {
            SFXController2.Stop();
            SFXController2.clip = _crown;
            SFXController2.Play();
        }
    }
    public void PlayAssign()
    {
        if (!SFXController || !SFXController2)
            return;

        if (SFXController.isPlaying == false)
        {
            SFXController.Stop();
            SFXController.clip = _assign;
            SFXController.Play();
        }
        else
        {
            SFXController2.Stop();
            SFXController2.clip = _assign;
            SFXController2.Play();
        }
    }
    public void PlayDismiss()
    {
        if (!SFXController || !SFXController2)
            return;

        if (SFXController.isPlaying == false)
        {
            SFXController.Stop();
            SFXController.clip = _dismiss;
            SFXController.Play();
        }
        else
        {
            SFXController2.Stop();
            SFXController2.clip = _dismiss;
            SFXController2.Play();
        }
    }
    public void PlayConstruction()
    {
        if (!SFXController || !SFXController2)
            return;

        if (SFXController.isPlaying == false)
        {
            SFXController.Stop();
            SFXController.clip = _construction;
            SFXController.Play();
        }
        else
        {
            SFXController2.Stop();
            SFXController2.clip = _construction;
            SFXController2.Play();
        }
    }
    public void PlayDemolish()
    {
        if (!SFXController || !SFXController2)
            return;

        if (SFXController.isPlaying == false)
        {
            SFXController.Stop();
            SFXController.clip = _demolish;
            SFXController.Play();
        }
        else
        {
            SFXController2.Stop();
            SFXController2.clip = _demolish;
            SFXController2.Play();
        }
    }
    public void PlayClick()
    {
        if (!SFXController || !SFXController2)
            return;

        if (SFXController.isPlaying == false)
        {
            SFXController.Stop();
            SFXController.clip = _buttonClick;
            SFXController.Play();
        }
        else
        {
            SFXController2.Stop();
            SFXController2.clip = _buttonClick;
            SFXController2.Play();
        }
    }
    public void PlayHorn()
    {
        if (!SFXController3)
            return;

            SFXController3.Stop();
            SFXController3.clip = _horn;
            SFXController3.Play();
    }
    public void PlayWilbur()
    {
        if (!SFXController || !SFXController2)
            return;

        if (SFXController.isPlaying == false)
        {
            SFXController.Stop();
            SFXController.clip = _wilbur;
            SFXController.Play();
        }
        else
        {
            SFXController2.Stop();
            SFXController2.clip = _wilbur;
            SFXController2.Play();
        }
    }
}
