using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.StatsInternal;
using UnityEngine;

public enum BuffTypes
{
    Burn = 1,
    Blind = 2,
    Undead = 3,
    Aura = 4,
}

public enum BuffEffects
{
    damage = 0,       // (디버프) 받는 데미지
    atkboost = 1,     // (버프) 데미지 증가량 (상수)
    atkspeed = 2,     // (버프) 공격 속도 증가량 (상수)
    movspeed = 3,     // (버프) 이동 속도 증가량 (상수)
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

public class BasicBuff
{
    public BuffTypes type;
    public string name;
    public string description;
    public Sprite icon;

    public float[] effects;
}

public class BurnDebuff : BasicBuff
{
    public BurnDebuff()
    {
        type = BuffTypes.Burn;
        name = "화상";
        description = "지속적으로 화염 피해를 입습니다.";
        icon = BuffIcons.GetIcon(type);

        effects = new float[4];
        effects[(int)BuffEffects.damage] = 2;
    }
}

public class BlindDebuff : BasicBuff
{
    public BlindDebuff()
    {
        type = BuffTypes.Blind;
        name = "실명";
        description = "시야가 좁아집니다.";
        icon = BuffIcons.GetIcon(type);

        effects = new float[4];
    }
}

public class UndeadDebuff : BasicBuff
{
    public UndeadDebuff()
    {
        type = BuffTypes.Undead;
        name = "언데드";
        description = "???";

        effects = new float[1];
    }
}

public class AuraBuff : BasicBuff
{
    public AuraBuff()
    {
        type = BuffTypes.Aura;
        name = "아우라";
        description = "아우라 안에 있으면 공격 속도, 이동 속도, 가하는 데미지가 상승하고 체력을 주기적으로 회복합니다.";

        effects = new float[4];
        effects[(int)BuffEffects.atkboost] = 1.3f;
        effects[(int)BuffEffects.atkspeed] = 1.2f;
        effects[(int)BuffEffects.movspeed] = 1.15f;
    }
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

    static public BasicBuff GetInfo(BuffTypes type)
    {
        if (type == BuffTypes.Burn) return new BurnDebuff();
        else if (type == BuffTypes.Blind) return new BlindDebuff();
        else if (type == BuffTypes.Undead) return new UndeadDebuff();
        else if (type == BuffTypes.Aura) return new AuraBuff();
        else return new BasicBuff();
    }

    static public BasicBuff GetInfo(int type)
    {
        return GetInfo((BuffTypes)type);
    }
}

public class TestBuffIndicator : MonoBehaviour
{
    public PlayerControllerNetworked player;
    public bool onOther = false;
    public PlayerBuffs playerBuffs;
    public TestBuffTooltip tooltip;
    public bool reqUpdated = false;
    public bool invert = false;

    float size = 32;
    float margin = 4;

    public void Start()
    {
        size = onOther ? 12 : 32;
        margin = onOther ? 2 : 4;
        tooltip = FindAnyObjectByType<TestBuffTooltip>();
    }

    public void ShowTooltip(Buff buff)
    {
        Debug.Log(tooltip);
        if (!tooltip) return;
        tooltip.buff = buff;
        // 버프 인디케이터 툴팁 위치 변경
        tooltip.deltaPos = new Vector2(0, size + 10);
        tooltip.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        if (!tooltip) return;
        tooltip.gameObject.SetActive(false);
    }

    public void Test()
    {
        playerBuffs.Test2();
    }

    void FixedUpdate()
    {
        if (!playerBuffs) return;
        // 새로운 버프에 걸릴 경우에만 위치 업데이트가 필요하다.
        if (!reqUpdated) return;
        int cur = 0;

        foreach (TestBuff buff in playerBuffs.buffObjects)
        {
            if (!buff) continue;
            float x = 0;
            float y = 0;
            if (cur > 0) x = cur * ((2 * size) + margin);
            if (onOther) y = - size - margin;
            if (invert) x *= -1;

            RectTransform rt = buff.gameObject.GetComponent<RectTransform>();
            rt.SetSizeDelta(size * 2, size * 2);

            buff.onOther = onOther;
            buff.gameObject.transform.localPosition = new Vector3(x, y, transform.position.z);
            buff.gameObject.SetActive(true);
            buff.UpdateLabel();
            cur++;
        }

        reqUpdated = false;
    }
}
