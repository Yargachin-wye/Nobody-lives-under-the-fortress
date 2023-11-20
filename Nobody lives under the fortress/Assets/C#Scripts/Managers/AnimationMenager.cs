using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationMenager : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private float _scrollSpeed = 1.0f;

    [HideInInspector] public Image oldImageBG;
    [HideInInspector] public Image newImageBG;
    [SerializeField] private float _speedSwapBG = 1.0f;

    private float targetVerticalNormalizedPosition = 0.0f;
    private bool isAutoScrolling = false;

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
