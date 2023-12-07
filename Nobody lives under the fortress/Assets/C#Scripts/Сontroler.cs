using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Сontroler : MonoBehaviour
{
    const string GRAPH_FILE_NAME = "Graph";

    [SerializeField] private AnimationMenager _animatorMenager;
    [SerializeField] private SoundMenager _soundMenager;

    [SerializeField] private Transform _sentMessagesContainer;
    [SerializeField] private Transform _interactiveMessagesContainer;

    [SerializeField] private GameObject _button;

    private List<Message> messagesList = new List<Message>();

    List<DialogueNodeData> dataList = new List<DialogueNodeData>();

    public static Сontroler instaince;
    private int ID = 0;
    private void Awake()
    {
        if (instaince != null)
        {
            Debug.LogError("instaince != null");
        }
        instaince = this;
    }
    private void Start()
    {
        Load();
        SetMessage(0);
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
    }
    public void SetMessage(int id)
    {
        ID = id;
        StartCoroutine(SelectMessage());
    }
    public IEnumerator SelectMessage()
    {
        yield return null;
        //UIMenager.instaince.OffActivButtons();
        UIMenager.instaince.DestroyActivButtons();
        Message message = messagesList[ID];
        bool lose = false;
        if (message.Type == NodeType.Trial)
        {
            AnimationMenager.instaince.StartTwister();

            yield return new WaitUntil(() => AnimationMenager.instaince.trialNum > -1);

            Debug.Log(AnimationMenager.instaince.trialNum);

            int trial = AnimationMenager.instaince.trialNum;

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
        yield return null;
    }
    private void SetNewMessages(Message message, bool lose)
    {
        
        for (int i = 0; i < message.OutIds.Count; i++)
        {
            int tId = message.OutIds[i];
            if (messagesList.Count == 0)
            {
                return;
            }

            var msg = messagesList[tId];
            if (message.Type == NodeType.Trial)
            {
                if (!lose && msg.Type == NodeType.TrialLose || lose && msg.Type == NodeType.TrialWin)
                {
                    continue;
                }
            }

            UIMenager.instaince.SetInteractiveMessage(msg.Text, msg.Type, tId, msg.Trial);
        }
        UIMenager.instaince.UpdateHight();
    }
}
public class Message
{
    public int Id;
    public List<int> OutIds;
    public string Text;
    public NodeType Type;
    public int Trial;
    public Sprite Background;
    public AudioClip Sound;
    public AudioClip Music;

    public Message(int id, List<int> transitionIds, string text, NodeType type, int trail, string background, string sound, string music)
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
