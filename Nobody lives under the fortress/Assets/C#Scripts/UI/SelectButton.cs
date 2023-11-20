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
    [SerializeField] private Color _Òolor—hoice, _ÒolorWin, _ÒolorLoss, _ÒolorTrial, _ÒolorEnd;

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
            _text.text = "ÿ‡ÌÒ Ì‡ ÛÒÔÂı " + (100 - Trial).ToString() + "%\n" + text;
            _image.color = _ÒolorTrial;
        }
        else if (type == "trial_lose")
        {
            _image.color = _ÒolorLoss;
        }
        else if (type == "trial_win")
        {
            _image.color = _ÒolorWin;
        }
        else if (type == "end")
        {
            _image.color = _ÒolorEnd;
            _text.color = Color.white;
        }
        else 
        {
            _image.color = _Òolor—hoice;
        }
    }
    public void Click()
    {
        GameObject go = Instantiate(_textButton, transform.parent);
        go.GetComponent<TextButton>().Init(_text.text, key, new Color(_image.color.r - 0.1f, _image.color.g - 0.1f, _image.color.b - 0.1f, 0.5f), _text.color);

        —ontrolerJson.instaince.SelectMessage(key);
    }
}
