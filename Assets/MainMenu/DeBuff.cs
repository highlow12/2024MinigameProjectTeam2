using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DeBuffTypes
{
    Burn,
    Blind,
    Undead
}

public enum EffectIndex
{
    damage = 0 // (디버프) 받는 데미지
}

public class BasicBuff {
    public DeBuffTypes type;
    public string name;
    public string description;
    public Sprite icon;
    public float duration;
    public int stacks; // 0일 경우 스택을 쌓을 수 없음.

    public float[] effects;
}

public class BurnDebuff: BasicBuff {
    public BurnDebuff()
    {
        type = DeBuffTypes.Burn;
        name = "화상";
        description = "지속적으로 화염 피해를 입습니다.";
        icon = BuffIndicator.GetIcon(type);
        duration = 60f;
        stacks = 1;

        effects = new float[1];
        effects[(int)EffectIndex.damage] = 2;
    }
}

public class BlindDebuff: BasicBuff {
    public BlindDebuff()
    {
        type = DeBuffTypes.Blind;
        name = "실명";
        description = "시야가 좁아집니다.";
        icon = BuffIndicator.GetIcon(type);
        duration = 60f;
        stacks = 0;

        effects = new float[1];
    }
}

public class UndeadDebuff: BasicBuff {
    public UndeadDebuff()
    {
        type = DeBuffTypes.Undead;
        name = "언데드";
        description = "???";
        icon = BuffIndicator.GetIcon(type);
        duration = 60f;
        stacks = 0;

        effects = new float[1];
    }
}

public class DeBuff : MonoBehaviour
{
    public BuffIndicator indicator;
    public BasicBuff info;
    public DeBuffTypes deBuffType;

    public float startTime;

    Image activated;
    Image deActivated;
    TMP_Text stacksLabel;
    
    void Awake()
    {
        Transform[] childrensTransforms = GetComponentsInChildren<Transform>();

        foreach (Transform child in childrensTransforms)
        {
            if (child.name == "Activated") activated = child.GetComponent<Image>();
            else if (child.name == "DeActivated") deActivated = child.GetComponent<Image>();
            else if (child.name == "StacksLabel") stacksLabel = child.GetComponent<TMP_Text>();

            if (activated != null && deActivated != null && stacksLabel != null) break;
        }
    }

    void OnEnable()
    {
        startTime = Time.time;
        UpdateLabel();
    }

    void Start()
    {
        if (deBuffType == DeBuffTypes.Burn) info = new BurnDebuff();
        else if (deBuffType == DeBuffTypes.Blind) info = new BlindDebuff();
        else if (deBuffType == DeBuffTypes.Undead) info = new UndeadDebuff();

        startTime = Time.time;

        activated.sprite = info.icon;
        deActivated.sprite = info.icon;

        UpdateLabel();
    }

    public void UpdateLabel()
    {
        if (stacksLabel == null) return; // if not initialized
        stacksLabel.gameObject.SetActive(info.stacks != 0);
        stacksLabel.text = info.stacks.ToString();
    }

    public void OnPointerEnter()
    {
        if (indicator) indicator.ShowTooltip(this);
    }

    public void OnPointerExit()
    {
        if (indicator) indicator.HideTooltip();
    }

    void Update()
    {
        if (info == null) return;
        float now = Time.time;
        if ((now - startTime) > info.duration)
        {
            if (info.stacks > 1)
            {
                info.stacks--;
                UpdateLabel();
                startTime = now;
            }
            else
            {
                // call parent script
                if (indicator) indicator.RemoveBuff(this);
                else gameObject.SetActive(false);
            }
        }

        float imgScale = (now - startTime) / info.duration;

        deActivated.fillAmount = imgScale;
    }
}
