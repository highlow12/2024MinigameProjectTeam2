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
        if (buff.type == (int)BuffTypes.Burn) info = new BurnDebuff();
        else if (buff.type == (int)BuffTypes.Blind) info = new BlindDebuff();
        else if (buff.type == (int)BuffTypes.Undead) info = new UndeadDebuff();

        activated.sprite = info.icon;
        deActivated.sprite = info.icon;

        UpdateLabel();
    }

    public void UpdateLabel()
    {
        if (stacksLabel == null) return; // if not initialized
        stacksLabel.gameObject.SetActive(buff.stacks != 0);
        stacksLabel.text = buff.stacks.ToString();
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
        if (info == null) return;
        float now = Time.time;
        if ((now - buff.startTime) > buff.duration)
        {
            if (buff.stacks > 1)
            {
                buff.stacks--;
                UpdateLabel();
                buff.startTime = now;
                indicator.playerBuffs.SetBuff(buff);
            }
            else
            {
                // call parent script
                if (indicator) indicator.playerBuffs.RemoveBuff(buff.type);
                else gameObject.SetActive(false);
            }
        }

        float imgScale = (now - buff.startTime) / buff.duration;

        deActivated.fillAmount = imgScale;
    }
}
