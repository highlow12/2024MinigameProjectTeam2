using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandleWindow : MonoBehaviour
{
    GameObject titleBar;
    GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] childrensTransforms = GetComponentsInChildren<Transform>();
        // GameObject[] childrens = new GameObject[childrensTransforms.Length];

        foreach (Transform child in childrensTransforms) {
            if (child.name == "Panel") panel = child.gameObject;
            else if (child.name == "TitleBar") titleBar = child.gameObject;
        }

        HandleTitleBar titleBarHandler = titleBar.AddComponent<HandleTitleBar>();
        titleBarHandler.parentHandler = this;
        titleBarHandler.panel = panel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
