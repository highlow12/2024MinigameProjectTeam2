using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class OtherStatusPanel : MonoBehaviour
{
    [SerializeField] Image classIcon;
    [SerializeField] TMP_Text className;
    [SerializeField] Image hpForeground;
    public TestBuffIndicator buffIndicator;
    public PlayerControllerNetworked player;

    [SerializeField] List<Image> lifeForeground;

    public void Start()
    {
        float[] yPoses = {60f, -60f};
        OtherStatusPanel[] panels = FindObjectsOfType<OtherStatusPanel>();
        
        for (int i = 0; i < panels.Length; i++)
        {
            Transform rt = panels[i].GetComponent<Transform>();
            Vector3 pos = new (rt.localPosition.x, yPoses[i], rt.localPosition.z);
            rt.localPosition = pos;
        }
    }

    public void SetClass(int classTypeInt)
    {
        CharacterClassEnum classType = (CharacterClassEnum)classTypeInt;
        classIcon.sprite = ClassIcons.GetIcon(classType);
    }

    public void SetName(string name)
    {
        className.text = name;
    }

    public void SetHP(float hp, float maxHP)
    {
        // Debug.Log($"{hp} / {maxHP} = {hp / maxHP}");
        hpForeground.fillAmount = hp / maxHP;
    }

    public void SetLife(int life)
    {
        for (int i = 0; i < lifeForeground.Count; i++)
        {
            if (life < i - 1) lifeForeground[i].gameObject.SetActive(false);
            else lifeForeground[i].gameObject.SetActive(true);
        }
    }
}
