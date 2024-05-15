using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DeBuffTypes {
    Burn,
    Blind,
    Undead
}

public class DeBuff : MonoBehaviour
{
    public BuffIndicator indicator;
    public DeBuffTypes deBuffType;
    public float defaultDuration = 1f;
    public int stacks = 1;
    public bool stackable = true;

    public float startTime;

    Image activated;
    Image deActivated;
    TMP_Text stacksLabel;

    void OnEnable() {
        startTime = Time.time;
        UpdateLabel();
    }

    void Start()
    {
        startTime = Time.time;
        Transform[] childrensTransforms = GetComponentsInChildren<Transform>();

        foreach (Transform child in childrensTransforms) {
            if (child.name == "Activated") activated = child.GetComponent<Image>();
            else if (child.name == "DeActivated") deActivated = child.GetComponent<Image>();
            else if (child.name == "StacksLabel") stacksLabel = child.GetComponent<TMP_Text>();

            if (activated != null && deActivated != null && stacksLabel != null) break;
        }

        // if (deBuffType == DeBuffTypes.Burn)

        UpdateLabel();
    }

    public void UpdateLabel()
    {
        stacksLabel.gameObject.SetActive(stackable);
        stacksLabel.text = stacks.ToString();
    }

    void Update()
    {
        float now = Time.time;
        if ((now - startTime) > defaultDuration) {
            if (stacks > 1) {
                stacks--;
                UpdateLabel();
                startTime = now;
            } else {
                // call parent script
                if (indicator) indicator.removeBuff(this);
                else gameObject.SetActive(false);
            }
        }

        float imgScale = (now - startTime) / defaultDuration;

        deActivated.fillAmount = imgScale;
    }
}
