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
        Debug.Log($"{hp} / {maxHP} = {hp / maxHP}");
        hpForeground.fillAmount = hp / maxHP;
    }
}
