using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class UIMenager : MonoBehaviour
{
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
    [SerializeField] private GameObject _mobilePanel;
    [SerializeField] private GameObject _PCPanel;

    [SerializeField] private Image _oldMobileBG;
    [SerializeField] private Image _newMobileBG;
    [SerializeField] private Image _oldPCBG;
    [SerializeField] private Image _newPCBG;

    [SerializeField] private int _mobileFontSize;
    [SerializeField] private int _PCFontSize;

    [SerializeField] private AnimationMenager animationMenager;

    [SerializeField] public static int maxAnactiveMessages = 25;

    private VerticalLayoutGroup messagerrLayout;
    private VerticalLayoutGroup interactiveLayout;

    private List<GameObject> anactiveMessages = new List<GameObject>();
    private List<GameObject> interactiveMessages = new List<GameObject>();

    private int fontSize = 0;
    public static bool AndroidUI => Application.platform == RuntimePlatform.Android;
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

        if (AndroidUI)
        {
            _trialAnimationRect.localScale = Vector3.one * 2;
            _mobilePanel.SetActive(true);
            _PCPanel.SetActive(false);
            animationMenager.oldImageBG = _oldMobileBG;
            animationMenager.newImageBG = _newMobileBG;
            _messagerRect.localPosition = new Vector2(0, 0);
            _messagerRect.sizeDelta = new Vector2(-300.5f, 0);
            fontSize = _mobileFontSize;
        }
        else
        {
            _trialAnimationRect.localScale = Vector3.one * 1.5f;
            _mobilePanel.SetActive(false);
            _PCPanel.SetActive(true);
            animationMenager.oldImageBG = _oldPCBG;
            animationMenager.newImageBG = _newPCBG;
            _messagerRect.localPosition = new Vector2(-425, 0);
            _messagerRect.sizeDelta = new Vector2(-1150, 0);
            fontSize = _PCFontSize;
        }
    }
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

        if (interactiveMessages.Count >= maxAnactiveMessages)
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
