using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;
    int key;
    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            _text.fontSize = 90;
        }
        else
        {
            _text.fontSize = 40;
        }
    }
    public void Init(string text, int Key, Color BGColor, Color TextColor)
    {
        key = Key;
        _text.text = text;
        _text.color = TextColor;
        _image.color = BGColor;
    }
    public void Click()
    {

    }
}
