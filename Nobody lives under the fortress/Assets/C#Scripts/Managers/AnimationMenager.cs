using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class AnimationMenager : MonoBehaviour
{
    
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private float _scrollSpeed = 1.0f;
    [SerializeField] private GameObject _TrialPanel;
    [SerializeField] private RectTransform[] firstTwisterTextSlots;
    [SerializeField] private RectTransform[] secondTwisterTextSlots;

    [HideInInspector] public Image oldImageBG;
    [HideInInspector] public Image newImageBG;
    [SerializeField] private float _speedSwapBG = 1.0f;
    [SerializeField] private AnimationCurve twisterAnimationCurve;

    private float targetVerticalNormalizedPosition = 0.0f;
    private bool isAutoScrolling = false;
    public int trialNum = -1;

    public static AnimationMenager instaince;
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
        _scrollRect.verticalNormalizedPosition = targetVerticalNormalizedPosition;
        _TrialPanel.SetActive(false);
    }
    public void SetBGSprite(Sprite bg)
    {
        StopCoroutine(SwapAnimationBG(bg));
        StartCoroutine(SwapAnimationBG(bg));
    }
    public void StartAutoScrollDown()
    {
        if (!isAutoScrolling)
        {
            isAutoScrolling = true;
            StartCoroutine(AutoScroll());
        }
    }
    public void StartTwister()
    {
        trialNum = -1;
        StartCoroutine(TwistAnimation());
    }
    private IEnumerator TwistAnimation()
    {
        yield return null;
        _TrialPanel.SetActive(true);

        int trial = -1;
        float animationTime = 0;
        int num1 = Random.Range(0, 10);
        int num2 = Random.Range(0, 10);
        int[] firstNums = new int[10];
        int[] secondNums = new int[10];

        for (int i = 0; i < firstTwisterTextSlots.Length; i++)
        {
            firstNums[i] = num1;
            secondNums[i] = num2;
            firstTwisterTextSlots[i].GetComponent<TextMeshProUGUI>().text = num1.ToString();
            secondTwisterTextSlots[i].GetComponent<TextMeshProUGUI>().text = num2.ToString();
            num1 = num1 < 9 ? num1 + 1 : 0;
            num2 = num2 < 9 ? num2 + 1 : 0;
        }

        while (animationTime < 0.95f || (-1 * firstTwisterTextSlots[0].anchoredPosition.y % 50 > 0.1f))
        {
            for (int i = 0; i < firstTwisterTextSlots.Length; i++)
            {
                float curveValue = twisterAnimationCurve.Evaluate(animationTime);
                float speed = Time.fixedDeltaTime * curveValue * 500;

                firstTwisterTextSlots[i].anchoredPosition = TwistMove(firstTwisterTextSlots[i].anchoredPosition, speed);

                secondTwisterTextSlots[i].anchoredPosition = TwistMove(secondTwisterTextSlots[i].anchoredPosition, -speed);
            }
            if (animationTime < 0.96f) animationTime += 0.005f;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        while (secondTwisterTextSlots[0].anchoredPosition.y % 50 > 0.1f)
        {
            for (int i = 0; i < secondTwisterTextSlots.Length; i++)
            {

                float curveValue = twisterAnimationCurve.Evaluate(animationTime);
                float speed = Time.fixedDeltaTime * curveValue * 500;
                secondTwisterTextSlots[i].anchoredPosition = TwistMove(secondTwisterTextSlots[i].anchoredPosition, -speed);
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        for (int i = 0; i < firstTwisterTextSlots.Length; i++)
        {
            if (firstTwisterTextSlots[i].anchoredPosition.y < -248 && firstTwisterTextSlots[i].anchoredPosition.y > -252)
            {
                trial = firstNums[i] * 10;
                break;
            }
        }
        for (int i = 0; i < secondTwisterTextSlots.Length; i++)
        {
            if (secondTwisterTextSlots[i].anchoredPosition.y < -248 && secondTwisterTextSlots[i].anchoredPosition.y > -252)
            {
                trial += secondNums[i];
                break;
            }
        }

        trialNum = trial;

        Debug.Log(trialNum);

        yield return new WaitForSeconds(1);
        _TrialPanel.SetActive(false);
    }
    private Vector2 TwistMove(Vector2 vector2, float speed)
    {
        Vector2 pose = vector2;
        pose.y += speed;
        if (pose.y >= -50)
        {
            pose.y -= 500;
        }
        if (pose.y <= -500)
        {
            pose.y += 500;
        }
        return pose;
    }
    private IEnumerator SwapAnimationBG(Sprite loadedSprite)
    {
        Color color = new Color(1, 1, 1, 0);
        newImageBG.color = color;
        newImageBG.sprite = loadedSprite;

        while (Color.white.a > color.a)
        {
            color = new Color(1, 1, 1, newImageBG.color.a + Time.fixedDeltaTime * _speedSwapBG);
            newImageBG.color = color;
            yield return null;
        }
        oldImageBG.sprite = loadedSprite;
    }
    private IEnumerator AutoScroll()
    {
        yield return new WaitForSeconds(0.03f);

        while (_scrollRect.verticalNormalizedPosition > targetVerticalNormalizedPosition)
        {
            _scrollRect.verticalNormalizedPosition -= Time.fixedDeltaTime * _scrollSpeed;
            yield return null;
        }
        isAutoScrolling = false;
    }
}
