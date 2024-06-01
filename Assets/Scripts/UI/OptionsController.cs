using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    
    private bool open;
    
    // field values
    public bool _portalsActive;
    public bool _characterWordsActive;
    public bool _flyActive;
    public Single _textSpeed;

    private Player _player;
    
    private void Start()
    {
        _player = FindObjectOfType<Player>();
    }

    public void OpenToggle()
    {
        if (open)
            Exit();
        else
            Open();
    }
    
    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
        Time.timeScale = 0;        
        Cursor.lockState = CursorLockMode.None;
    }
    
    public void Exit()
    {
        open = false;
        gameObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnRestart()
    {
        FindObjectOfType<RouteManager>().RestartAnimation();
    }
    
    public void OnSkip()
    {
        FindObjectOfType<RouteManager>().SkipToEnd();
    }

    public void OnPortalChanges(bool portalsActive)
    {
        _portalsActive = portalsActive;
    }
    
    public void OnCharacterWordsChanged(bool characterWordsActive)
    {
        _characterWordsActive = characterWordsActive;
    }

    public void OnTextSpeedChanged(Single textSpeed)
    {
        _textSpeed = textSpeed;
        FindObjectOfType<RouteManager>().SetSpeed(textSpeed);
    }

    public void OnFlyChanged(bool flyActive)
    {
        _flyActive = flyActive;
    }
}
