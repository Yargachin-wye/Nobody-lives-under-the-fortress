using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    [SerializeField] private GameObject _textButton;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;
    [SerializeField] private Color _�olor�hoice, _�olorWin, _�olorLoss, _�olorTrial, _�olorEnd;

    int key;
    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            _text.fontSize = 100;
        }
        else
        {
            _text.fontSize = 50;
        }
    }
    public void Init(string text, string type, int Key, int Trial)
    {
        key = Key;

        _text.text = text;
        if (type == "trial")
        {
            _text.text = "���� �� ����� " + (100 - Trial).ToString() + "%\n" + text;
            _image.color = _�olorTrial;
        }
        else if (type == "trial_lose")
        {
            _image.color = _�olorLoss;
        }
        else if (type == "trial_win")
        {
            _image.color = _�olorWin;
        }
        else if (type == "end")
        {
            _image.color = _�olorEnd;
            _text.color = Color.white;
        }
        else 
        {
            _image.color = _�olor�hoice;
        }
    }
    public void Click()
    {
        GameObject go = Instantiate(_textButton, transform.parent);
        go.GetComponent<TextButton>().Init(_text.text, key, new Color(_image.color.r - 0.1f, _image.color.g - 0.1f, _image.color.b - 0.1f, 0.5f), _text.color);

        �ontrolerJson.instaince.SelectMessage(key);
    }
}
