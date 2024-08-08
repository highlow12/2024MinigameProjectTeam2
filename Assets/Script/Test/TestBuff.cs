using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestBuff : MonoBehaviour
{
    public TestBuffIndicator indicator;
    public Buff buff;
    public BasicBuff info;
    public bool onOther = false;

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
        UpdateLabel();
    }

    void Start()
    {
        info = BuffIcons.GetInfo(buff.type);
        activated.sprite = info.icon;
        deActivated.sprite = info.icon;
        UpdateLabel();
    }

    public void UpdateLabel()
    {
        if (stacksLabel == null) return;
        if (onOther)
        {
            stacksLabel.gameObject.SetActive(false);
        }
        else
        {
            stacksLabel.gameObject.SetActive(buff.stacks != 0);
            stacksLabel.text = buff.stacks.ToString();
        }
    }

    public void OnPointerEnter()
    {
        if (indicator) indicator.ShowTooltip(buff);
        if (indicator)
        {
            indicator.Test();
        }
    }

    public void OnPointerExit()
    {
        if (indicator) indicator.HideTooltip();
    }

    void FixedUpdate()
    {
        if (onOther) return;
        if (info == null) return;
        float now = Time.time;
        if ((now - buff.startTime) > buff.duration)
        {
            if (buff.stacks > 1)
            {
                buff.stacks--;
                buff.startTime = now;
                indicator.playerBuffs.SetBuff(buff);
                UpdateLabel();
            }
            else
            {
                if (indicator) indicator.playerBuffs.RemoveBuff(buff.type);
                else gameObject.SetActive(false);
            }
        }

        float imgScale = (now - buff.startTime) / buff.duration;

        deActivated.fillAmount = imgScale;
    }
}
