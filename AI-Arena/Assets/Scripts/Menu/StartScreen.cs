using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    [SerializeField][Range(0.1f, 1f)] private float _fadeSpeed;
    [SerializeField][Range(0.1f, 3f)] private float _screenTime;
    private CanvasGroup _screen;
    private bool _isTimeToFadeOut;
    private float _timer;

    void Start()
    {
        _screen = GetComponent<CanvasGroup>();
        _screen.alpha = 0;
        _isTimeToFadeOut = false;
        _timer = 0;
    }

    private void Update()
    {
        if (_screen.alpha < 1 && !_isTimeToFadeOut)
        {
            _screen.alpha += Time.deltaTime * _fadeSpeed;
        }

        else if (_screen.alpha >= 1 && !_isTimeToFadeOut)
        {
            _timer += Time.deltaTime;
            if (_timer >= _screenTime)
            {
                _isTimeToFadeOut = true;
            }
        }

        if (!_isTimeToFadeOut) return;
        
        _screen.alpha -= Time.deltaTime * _fadeSpeed;
        if (_screen.alpha <= 0)
        {
            SceneManager.LoadScene(1);
        }
    }
}
