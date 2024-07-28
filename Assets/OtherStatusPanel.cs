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
    public NetworkObject player;
    public PlayerRef playerRef;

    void Start()
    {
        PlayerControllerNetworked playerController = player.GetComponent<PlayerControllerNetworked>();
        CharacterClassEnum classType = (CharacterClassEnum)playerController.characterClass;
        classIcon.sprite = ClassIcons.GetIcon(classType);
        className.text = (string)PlayerInfosProvider.Instance.PlayerInfos[playerRef.PlayerId - 1].nickName;
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
