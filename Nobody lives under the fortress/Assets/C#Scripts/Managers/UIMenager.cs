using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class UIMenager : MonoBehaviour
{
    [SerializeField] private GameObject _scrollbarAudio;

    [SerializeField] private RectTransform _trialAnimationRect;
    [SerializeField] private RectTransform _trialAnimationPanel;

    public static UIMenager instaince;
    [SerializeField] private RectTransform _mainCanvas;
    [SerializeField] private RectTransform _messagerRect;
    [SerializeField] private RectTransform _anactiveMessager;

    [SerializeField] private RectTransform _anactiveMessagesContainer;
    [SerializeField] private RectTransform _interactiveMessagesContainer;

    [SerializeField] private GameObject _anactiveMessagePrefab;
    [SerializeField] private GameObject _interactiveMessagePrefab;

    [SerializeField] private GameObject _scrollerView;

    [SerializeField] private Image _oldBG;
    [SerializeField] private Image _newBG;

    [SerializeField] private int fontSize = 50;

    [SerializeField] private AnimationMenager animationMenager;

    [SerializeField] public int maxAnactiveMessages = 15;

    private VerticalLayoutGroup messagerrLayout;
    private VerticalLayoutGroup interactiveLayout;

    private List<GameObject> anactiveMessages = new List<GameObject>();
    private List<GameObject> interactiveMessages = new List<GameObject>();


    // public static bool AndroidUI => Application.platform == RuntimePlatform.Android;
    private void Awake()
    {
        if (instaince != null)
        {
            Debug.LogError("instaince != null");
        }
        instaince = this;

        interactiveLayout = _interactiveMessagesContainer.GetComponent<VerticalLayoutGroup>();
        messagerrLayout = _messagerRect.GetComponent<VerticalLayoutGroup>();

        Debug.Log(_messagerRect.localPosition);
        Debug.Log(_messagerRect.sizeDelta);

        _trialAnimationRect.gameObject.SetActive(true);
        _trialAnimationPanel.gameObject.SetActive(false);
        animationMenager.oldImageBG = _oldBG;
        animationMenager.newImageBG = _newBG;
        _scrollbarAudio.SetActive(false);
    }
    public void BtnAudio() => _scrollbarAudio.SetActive(!_scrollbarAudio.active);
    public void OffActivButtons()
    {
        foreach (var obj in interactiveMessages)
        {
            obj.SetActive(false);
        }
    }
    public void DestroyActivButtons()
    {
        foreach (var obj in interactiveMessages)
        {
            Destroy(obj);
        }
        interactiveMessages = new List<GameObject>();
        UpdateHight();
    }
    public void SetInteractiveMessage(string Text, NodeType Type, int ID, int Trial)
    {
        GameObject obj = Instantiate(_interactiveMessagePrefab, _interactiveMessagesContainer);

        if (Type == NodeType.Trial)
        {
            Text = "Испытание: " + Trial.ToString() + "\n" + Text;
        }

        obj.GetComponent<SelectButton>().Init(Text, Type, ID, fontSize);

        interactiveMessages.Add(obj);
        UpdateHight();
    }
    public void SetAnactiveMessage(string Text, int ID, Color BGColor, Color TextColor)
    {
        GameObject obj = Instantiate(_anactiveMessagePrefab, _anactiveMessagesContainer);
        obj.GetComponent<TextButton>().Init(Text, ID, BGColor, TextColor, fontSize);

        anactiveMessages.Add(obj);

        if (_anactiveMessagesContainer.childCount >= maxAnactiveMessages)
        {
            anactiveMessages.Remove(anactiveMessages[0]);
            Destroy(_anactiveMessagesContainer.GetChild(0).gameObject);
        }
    }
    public void EyeButton()
    {
        if (_anactiveMessagesContainer.gameObject.activeInHierarchy)
        {
            _anactiveMessagesContainer.gameObject.SetActive(false);
        }
        else
        {
            _anactiveMessagesContainer.gameObject.SetActive(true);
        }
    }
    public void UpdateHight()
    {
        StartCoroutine(UpdateHightCoroutine());
    }
    IEnumerator UpdateHightCoroutine()
    {
        yield return null;

        interactiveLayout.SetLayoutVertical();
        messagerrLayout.SetLayoutVertical();

        Vector2 sizeDelta = _anactiveMessager.sizeDelta;
        sizeDelta.y = _mainCanvas.sizeDelta.y - _interactiveMessagesContainer.sizeDelta.y;
        _anactiveMessager.sizeDelta = sizeDelta;

        //layout.CalculateLayoutInputVertical();
        //layout.CalculateLayoutInputHorizontal();
        //layout.SetLayoutVertical();
        //layout.SetLayoutHorizontal();
    }
}
