using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.DialogueSystem.DataContainers;
using Subtegral.DialogueSystem.Editor;

public class СontrolerJson : MonoBehaviour
{
    const string GRAPH_FILE_NAME = "GraphJ";

    [SerializeField] private AnimationMenager _animatorMenager;
    [SerializeField] private SoundMenager _soundMenager;

    [SerializeField] private Transform _messagesContainer;
    [SerializeField] private int _maxMessages = 25;
    [SerializeField] private GameObject _button;

    private List<GameObject> buttons = new List<GameObject>();

    private List<Message> messagesList = new List<Message>();

    List<DialogueNodeData> dataList = new List<DialogueNodeData>();

    public static СontrolerJson instaince;
    private void Awake()
    {
        if (instaince != null)
        {
            Debug.LogError("instaince != null");
        }
        instaince = this;
    }
    public void Load()
    {
        TextAsset textAsset = (TextAsset)Resources.Load(GRAPH_FILE_NAME);
        DataContainer dataContainer = JsonUtility.FromJson<DataContainer>(textAsset.text);
        dataList = dataContainer.dataList;
        foreach (var data in dataList)
        {
            messagesList.Add(new Message(data.Id, data.OutIds, data.DialogueText, data.Type, data.Trial, data.Bg, data.Sound, data.Music));
        }
        Debug.Log(messagesList[0].Text);
    }

    private void Start()
    {
        Load();
        SelectMessage(0);
    }
    public void SelectMessage(int id)
    {
        Message message = messagesList[id];
        bool lose = false;
        if (message.Type == "trial")
        {
            int trial = Random.Range(0, 100);
            if (trial >= message.Trial)
            {
                _soundMenager.PlayWin();
            }
            else
            {
                lose = true;
                _soundMenager.PlayLose();
            }
        }
        if (message.Sound != null)
        {
            _soundMenager.PlaySound(message.Sound);
        }
        if (message.Music != null)
        {
            _soundMenager.PlayMusic(message.Music);
        }
        if (message.Background != null)
        {
            _animatorMenager.SetBGSprite(message.Background);
        }

        SetNewMessages(message, lose);
        _animatorMenager.StartAutoScrollDown();
    }
    private void SetNewMessages(Message message, bool lose)
    {
        foreach (var obj in buttons)
        {
            Destroy(obj);
        }

        for (int i = 0; i < message.OutIds.Count; i++)
        {
            int tId = message.OutIds[i];
            var msg = messagesList[tId];
            if (message.Type == "trial")
            {
                if (!lose && msg.Type == "trial_lose" || lose && msg.Type == "trial_win")
                {
                    continue;
                }
            }

            GameObject obj = Instantiate(_button, _messagesContainer);
            obj.GetComponent<SelectButton>().Init(msg.Text, msg.Type, tId, msg.Trial);

            buttons.Add(obj);
            if (_messagesContainer.childCount > _maxMessages)
            {
                Destroy(_messagesContainer.GetChild(0).gameObject);
            }
        }
    }
}
public class Message
{
    public int Id;
    public List<int> OutIds;
    public string Text;
    public string Type;
    public int Trial;
    public Sprite Background;
    public AudioClip Sound;
    public AudioClip Music;

    public Message(int id, List<int> transitionIds, string text, string type, int trail, string background, string sound, string music)
    {
        Id = id;
        OutIds = transitionIds;
        Text = text;
        Type = type;
        Trial = trail;

        Background = Resources.Load<Sprite>(background);
        if (Background == null)
        {
            Debug.Log("Спрайт " + Background + " не найден.");
        }

        if (sound == "step")
        {
            Sound = SoundMenager.instaince.GetStep();
        }
        else
        {
            Sound = Resources.Load<AudioClip>(sound);
            if (Sound == null)
            {
                Debug.Log("Аудиоклип " + Sound + " не найден в ресурсах.");
            }
        }

        Music = Resources.Load<AudioClip>(music);
        if (Music == null)
        {
            Debug.Log("Аудиоклип " + Music + " не найдена.");
        }
    }
}
