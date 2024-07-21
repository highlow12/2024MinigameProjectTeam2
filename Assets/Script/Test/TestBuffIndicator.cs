using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public enum BuffTypes
{
    Burn = 1,
    Blind = 2,
    Undead = 3
}

public enum BuffEffects
{
    damage = 0 // (디버프) 받는 데미지
}

public struct Buff : INetworkStruct
{
    public int type;         // (디)버프 타입 (1: 화상 / 2: 암흑 / 3: 언데드...)
    public int stacks;       // 스택 수 (0: 중첩 불가)
    public float startTime;  // (디)버프가 시작된 시간
    public float duration;   // (디)버프가 지속되는 시간 (sec)
    public float tick;       // 아래의 디버프 이펙트가 시작되는 주기 (sec)
    public float lastTick;   // 마지막으로 틱이 작동한 시간
}

public class BuffIcons
{
    static public List<Sprite> EffectImages = new();
    static public Sprite GetIcon(BuffTypes type)
    {
        if (EffectImages.Count == 0)
        {
            EffectImages = Resources.LoadAll<Sprite>("Icons/Effects").ToList();
        }

        return EffectImages.Find(x => x.name == type.ToString());
    }
}

public class TestBuffIndicator : MonoBehaviour
{
    public PlayerBuffs playerBuffs;
    public TestBuffTooltip tooltip;
    public bool reqUpdated = false;

    public void ShowTooltip(Buff buff)
    {
        tooltip.buff = buff;
        // 버프 인디케이터 툴팁 위치 변경
        tooltip.deltaPos = new Vector2(0, playerBuffs.iconHalfSize + 10);
        tooltip.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }

    public void Test()
    {
        playerBuffs.Test();
    }

    void FixedUpdate()
    {
        if (!playerBuffs) return;
        // 새로운 버프에 걸릴 경우에만 위치 업데이트가 필요하다.
        if (!reqUpdated || playerBuffs.Length < 1) return;
        int cur = 0;

        foreach (GameObject buff in playerBuffs.buffObjects)
        {
            if (!buff) continue;
            float x = playerBuffs.iconHalfSize + (cur * ((2 * playerBuffs.iconHalfSize) + 4));
            buff.gameObject.transform.localPosition = new Vector2(x, 0);
            buff.SetActive(true);
            cur++;
        }

        reqUpdated = false;
    }
}
