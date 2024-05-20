using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffTooltip : MonoBehaviour
{
    public DeBuff buff;
    public Vector2 deltaPos;

    Image icon;
    TMP_Text stacks;
    TMP_Text buffName;
    TMP_Text type;
    TMP_Text desc;

    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name == "BuffIcon") icon = t.GetComponent<Image>();
            else if (t.name == "BuffStacks") stacks = t.GetComponent<TMP_Text>();
            else if (t.name == "BuffName") buffName = t.GetComponent<TMP_Text>();
            else if (t.name == "BuffType") type = t.GetComponent<TMP_Text>();
            else if (t.name == "BuffDesc") desc = t.GetComponent<TMP_Text>();
        }
    }

    void OnEnable()
    {
        icon.sprite = buff.info.icon;
        stacks.text = buff.info.stacks.ToString();
        buffName.text = buff.info.name;
        type.text = "디버프";
        desc.text = buff.info.description;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        if (deltaPos.x == 0) deltaPos.x = 1;
        if (deltaPos.y == 0) deltaPos.y = 1;
        float invertX = deltaPos.x / Mathf.Abs(deltaPos.x);
        float invertY = deltaPos.y / Mathf.Abs(deltaPos.y);
        mousePos.x += invertX * (rect.sizeDelta.x * rect.lossyScale.x / 2) + deltaPos.x;
        mousePos.y += invertY * (rect.sizeDelta.y * rect.lossyScale.y / 2) + deltaPos.y;
        transform.position = mousePos;
    }
}
