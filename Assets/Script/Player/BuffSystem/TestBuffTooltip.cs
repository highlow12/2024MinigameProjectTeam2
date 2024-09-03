using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestBuffTooltip : MonoBehaviour
{
    public Buff buff;
    public Vector2 deltaPos;

    Image icon;
    TMP_Text stacks;
    TMP_Text buffName;
    TMP_Text type;
    TMP_Text desc;

    RectTransform rect;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
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
        BuffTypes buffType = (BuffTypes)buff.type;
        icon.sprite = BuffIcons.GetIcon(buffType);
        stacks.text = buff.stacks.ToString();
        buffName.text = "테스트";
        type.text = "디버프";
        desc.text = "테스트";
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        if (deltaPos.x == 0) deltaPos.x = 1;
        if (deltaPos.y == 0) deltaPos.y = 1;
        float invertX = deltaPos.x / Mathf.Abs(deltaPos.x);
        float invertY = deltaPos.y / Mathf.Abs(deltaPos.y);
        mousePos.x += invertX * (rect.sizeDelta.x * rect.lossyScale.x / 2) + deltaPos.x;
        mousePos.y += invertY * (rect.sizeDelta.y * rect.lossyScale.y / 2) + deltaPos.y;
        // 이 코드가 없으면 z 좌표가 -10800으로 고정되어 다른 UI 요소에 파묻힘
        mousePos.z += 10800 * 0.009259259f; // 캔버스의 스케일을 곱해서 Z 좌표를 0으로 만든다.
        transform.position = Vector3.Lerp(
            transform.position,
            cam.ScreenToWorldPoint(mousePos),
            0.3f
        );
    }
}
