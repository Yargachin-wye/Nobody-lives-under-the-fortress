using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenager : MonoBehaviour
{
    [SerializeField] private RectTransform _scroller;
    [SerializeField] private GameObject _scrollerView;
    [SerializeField] private GameObject _mobilePanel;
    [SerializeField] private GameObject _PCPanel;

    [SerializeField] private Image _oldMobile;
    [SerializeField] private Image _newMobile;
    [SerializeField] private Image _oldPC;
    [SerializeField] private Image _newPC;

    [SerializeField] private AnimationMenager animationMenager;
    private void Awake()
    {
        Debug.Log(_scroller.localPosition);
        Debug.Log(_scroller.sizeDelta);
        if (Application.platform == RuntimePlatform.Android)
        {
            _mobilePanel.SetActive(true);
            _PCPanel.SetActive(false);
            animationMenager.oldImageBG = _oldMobile;
            animationMenager.newImageBG = _newMobile;
            _scroller.localPosition = new Vector2(0, 190);
            _scroller.sizeDelta = new Vector2(-370, -370);
        }
        else
        {
            _mobilePanel.SetActive(false);
            _PCPanel.SetActive(true);
            animationMenager.oldImageBG = _oldPC;
            animationMenager.newImageBG = _newPC;
            _scroller.localPosition = new Vector2(-400, 0);
            _scroller.sizeDelta = new Vector2(-1000, 0);
        }
    }
    public void EyeButton()
    {
        if (_scrollerView.activeInHierarchy)
        {
            _scrollerView.SetActive(false);
        }
        else
        {
            _scrollerView.SetActive(true);
        }
    }
}
