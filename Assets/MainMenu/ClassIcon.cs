using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassIcon : MonoBehaviour
{
    public Color hoverColor;
    public GameObject SummaryPannel;
    
    RawImage rawImage;
    Color orignalColor;
    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();
        orignalColor = rawImage.color;
    }

    public void OnPointerEnter()
    {
        rawImage.color = hoverColor;
    }

    public void OnPointerExit()
    {
        rawImage.color = orignalColor;
    }

    public void OnClick()
    {
        SummaryPannel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
