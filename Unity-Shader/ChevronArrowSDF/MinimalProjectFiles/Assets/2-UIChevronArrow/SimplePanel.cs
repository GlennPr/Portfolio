using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimplePanel : MonoBehaviour
{
    private readonly int _progressProperty = Shader.PropertyToID("_Progress");

    [Header("Settings")]
    [SerializeField] private bool _horizontal;
    [SerializeField] private float _duration = 0.8f;
    [Header("References")]
    [SerializeField] private Button _button;
    [SerializeField] private RectTransform _panel;

    private float _panelProgress;
    private float _buttonProgress;
    private bool _isShowing;
    private Vector2 _restPosition = Vector2.zero;

    private void Awake()
    {
        _button.onClick.AddListener(OnHandleButton);

        if (_horizontal)
        {
            _restPosition.y = _panel.sizeDelta.y;
        }
        else
        {
            _restPosition.x = _panel.sizeDelta.x;
        }
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnHandleButton);
    }

    private void Update()
    {     
        _panelProgress += (_isShowing ? 1 : -1) * Time.deltaTime * (1f /  _duration);
        _panelProgress = Mathf.Clamp01(_panelProgress);

        _panel.anchoredPosition = Vector2.Lerp(_restPosition, Vector2.zero, EaseInOutCubic(_panelProgress));

        #region ButtonVisual
        _buttonProgress += (_isShowing ? 1 : -1) * Time.deltaTime * 4;
        _buttonProgress = Mathf.Clamp01(_buttonProgress);
        _button.image.material.SetFloat(_progressProperty, EaseInOutCubic(_buttonProgress));
        #endregion
    }

    private void OnHandleButton()
	{
        _isShowing = !_isShowing;
    }

    private float EaseInOutCubic(float value)
    {
        return value < 0.5 ? 4 * value * value * value : 1 - Mathf.Pow(-2 * value + 2, 3) / 2;
    }
}
