using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DurationIndicator : MonoBehaviour
{
    private Image _durationIndicator;
    private TextMeshProUGUI _durationText;
    public string skillName;
    public int order;
    public float maxDuration;
    public float currentDuration;
    void Start()
    {
        _durationIndicator = GetComponent<Image>();
        _durationText = GetComponentInChildren<TextMeshProUGUI>();
        _durationText.text = $"{skillName} - {maxDuration}s";
        order = transform.GetSiblingIndex();
        SetPos(order);
        StartCoroutine(Duration());
    }

    void Update()
    {
        order = transform.GetSiblingIndex();
        _durationText.text = $"{skillName} - {Math.Round(currentDuration, 1)}s";
        SetPos(order);
    }

    private void SetPos(int order)
    {
        var pos = GetComponent<RectTransform>().anchoredPosition;
        pos.x = -200 + 180 * order;
        GetComponent<RectTransform>().anchoredPosition = pos;
    }

    IEnumerator Duration()
    {
        currentDuration = maxDuration;
        while (currentDuration > 0)
        {
            currentDuration -= Time.deltaTime;
            _durationIndicator.fillAmount = currentDuration / maxDuration;
            yield return null;
        }
        Destroy(gameObject);
    }
}
