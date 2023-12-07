using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;
    int id;
    public void Init(string text, int ID, Color BGColor, Color TextColor, int FontSize)
    {
        _text.fontSize = FontSize;
        id = ID;
        _text.text = text;
        _text.color = TextColor;
        _image.color = BGColor;
    }
    public void Click()
    {

    }
}
