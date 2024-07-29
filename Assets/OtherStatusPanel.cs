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
    public PlayerControllerNetworked player;

    private void Update()
    {
        if (player)
        {
            player.OtherPanelUpdate();
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
        hpForeground.fillAmount = hp / maxHP;
    }
}