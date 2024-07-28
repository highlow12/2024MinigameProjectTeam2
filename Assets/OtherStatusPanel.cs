using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OtherStatusPanel : MonoBehaviour
{
    [SerializeField] Image classIcon;
    [SerializeField] TMP_Text className;
    [SerializeField] Image hpForeground;
    
    public void SetClass(int classTypeInt)
    {
        CharacterClassEnum classType = (CharacterClassEnum)classTypeInt;
        classIcon.sprite = ClassIcons.GetIcon(classType);
    }

    public void SetName(string name)
    {
        className.text = name;
    }

    public void SetHP(int hp, int maxHP)
    {
        hpForeground.fillAmount = hp / maxHP;
    }
}
