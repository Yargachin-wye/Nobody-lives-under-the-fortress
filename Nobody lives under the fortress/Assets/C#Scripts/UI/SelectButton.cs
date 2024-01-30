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

    int id;
    public void Init(string text, NodeType type, int ID, int FontSize)
    {
        _text.fontSize = FontSize;
        id = ID;
        _text.text = text;
        
        if (type == NodeType.Trial)
        {
            _image.color = _ÒolorTrial;
        }
        else if (type == NodeType.TrialLose)
        {
            _image.color = _ÒolorLoss;
        }
        else if (type == NodeType.TrialWin)
        {
            _image.color = _ÒolorWin;
        }
        else if (type == NodeType.End)
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
        UIMenager.instaince.SetAnactiveMessage(
            _text.text,
            id,
            new Color(_image.color.r - 0.1f, _image.color.g - 0.1f, _image.color.b - 0.1f, 0.5f),
            _text.color);

        —ontroler.instaince.SetMessage(id);
    }
}
