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

    int id;
    public void Init(string text, NodeType type, int ID, int FontSize)
    {
        _text.fontSize = FontSize;
        id = ID;
        _text.text = text;
        
        if (type == NodeType.Trial)
        {
            _image.color = _�olorTrial;
        }
        else if (type == NodeType.TrialLose)
        {
            _image.color = _�olorLoss;
        }
        else if (type == NodeType.TrialWin)
        {
            _image.color = _�olorWin;
        }
        else if (type == NodeType.End)
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
        UIMenager.instaince.SetAnactiveMessage(
            _text.text,
            id,
            new Color(_image.color.r - 0.1f, _image.color.g - 0.1f, _image.color.b - 0.1f, 0.5f),
            _text.color);

        �ontroler.instaince.SetMessage(id);
    }
}
