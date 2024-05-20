using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HandleTitleBar : MonoBehaviour
{
    public HandleWindow parentHandler;
    public GameObject panel;
    Canvas parentCanvas;
    RectTransform rect;
    bool isDragging;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        parentCanvas = parentHandler.GetComponent<Canvas>();
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0)) {
            Vector2 titleBarPos = transform.position;
            // 화면 크기에 따라 UI 요소들의 크기도 비례하므로 캔버스의 스케일을 곱해준다.
            Vector2 titleBarSize = rect.sizeDelta * parentCanvas.scaleFactor;
            isDragging = titleBarPos.x - (titleBarSize.x / 2) < mousePos.x
                && titleBarPos.x + (titleBarSize.x / 2) > mousePos.x
                && titleBarPos.y + (titleBarSize.y / 2) > mousePos.y
                && titleBarPos.y - (titleBarSize.y / 2) < mousePos.y;
        } else if (isDragging) {
            Vector2 titleBarPos = transform.position;
            Vector2 panelPos = panel.transform.position;

            float y = titleBarPos.y - mousePos.y; // deltaY
            float x = titleBarPos.x - mousePos.x; // deltaX

            panel.transform.position = new Vector2(panelPos.x - x, panelPos.y - y);

            Debug.Log($"{x} {y}");
        }

        if (Input.GetMouseButtonUp(0)) {
            isDragging = false;
        }
    }
}
