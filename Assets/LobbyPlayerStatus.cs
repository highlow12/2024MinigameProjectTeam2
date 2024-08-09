using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerStatus : MonoBehaviour
{
    [SerializeField] bool isLeader = false;
    [SerializeField] bool isReady = false;
    [Space]
    [SerializeField] TMP_Text nickName;
    [SerializeField] TMP_Text className;
    [SerializeField] Image background;
    [SerializeField] Image classImage;
    [Space]
    [SerializeField] Sprite readySprite;
    [SerializeField] Sprite defaultSprite;
    [Space]
    [SerializeField] Sprite leaderReadySprite;
    [SerializeField] Sprite leaderDefaultSprite;

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        Debug.Log("callasdssadsda");
        isLeader = false;
        isReady = false;
        nickName.text = "빈 자리";
        className.text = "플레이어를 기다리는 중입니다...";
        classImage.sprite = null;
        UIUpdate();
    }

    public void SetNick(string nick)
    {
        nickName.text = nick;
    }

    public void SetClass(CharacterClassEnum _class)
    {
        Sprite classSprite = ClassIcons.GetIcon(_class);
        classImage.sprite = classSprite;
        if (_class == CharacterClassEnum.Knight) className.text = "전사";
        else if (_class == CharacterClassEnum.Archer) className.text = "궁수";
        else if (_class == CharacterClassEnum.Tank) className.text = "방패병";
    }

    public void SetClass(int _class)
    {
        SetClass((CharacterClassEnum)_class);
    }

    public void SetLeader(bool _isLeader = true)
    {
        isLeader = _isLeader;
        UIUpdate();
    }

    public void SetReady(bool _isReady = true)
    {
        isReady = _isReady;
        UIUpdate();
    }

    private void UIUpdate()
    {
        if (isLeader)
        {
            background.sprite = isReady ? leaderReadySprite : leaderDefaultSprite;
        }
        else
        {
            background.sprite = isReady ? readySprite : defaultSprite;
        }
    }
}
