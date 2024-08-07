using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeComponent : MonoBehaviour
{
    // Start is called before the first frame update

    List<GameObject> elements = new List<GameObject>();
    Color32 ActiveColor = new Color32(124, 155, 123, 255);
    Color32 DefaultColor = new Color32(45, 65, 44, 255);
    public int value = 5;

    void Awake()
    {
        Transform[] childrens = GetComponentsInChildren<Transform>();
        foreach (Transform t in childrens)
        {
            if (int.TryParse(t.name, out int _val))
            {
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                List<EventTrigger.Entry> triggers = trigger.triggers;
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.callback.AddListener((eventData) => { SetVolume(_val); });
                triggers.Add(entry);
                trigger.triggers = triggers;
                elements.Add(t.gameObject);
            }
        }

        SetVolume(value);
    }

    void SetVolume(int volume)
    {
        for (int i = 0; i < elements.Count; i++)
        {
            if (i <= volume)
            {
                elements[i].GetComponent<Image>().color = ActiveColor;
            }
            else
            {
                elements[i].GetComponent<Image>().color = DefaultColor;
            }
        }
    }
}
