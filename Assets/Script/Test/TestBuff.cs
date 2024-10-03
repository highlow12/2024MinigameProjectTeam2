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
<<<<<<< HEAD
    
=======

>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c
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
<<<<<<< HEAD
        } else
=======
        }
        else
>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c
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
<<<<<<< HEAD
        float now = Time.time;
=======
        float now = indicator.GetCurrentTime();
>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c
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
<<<<<<< HEAD
                // call parent script
=======
>>>>>>> ff96bb64b01cb27b7fe339336ad1d1fb380b4d1c
                if (indicator) indicator.playerBuffs.RemoveBuff(buff.type);
                else gameObject.SetActive(false);
            }
        }

        float imgScale = (now - buff.startTime) / buff.duration;

        deActivated.fillAmount = imgScale;
    }
}
