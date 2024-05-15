using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffIndicator : MonoBehaviour
{
    static public List<Sprite> EffectImages = new List<Sprite>();
    static public Sprite GetIcon(DeBuffTypes type)
    {
        if (EffectImages.Count == 0) {
            EffectImages = Resources.LoadAll<Sprite>("Icons/Effects").ToList();
        }

        return EffectImages.Find(x => x.name == type.ToString());
    }

    public List<DeBuff> deBuffs;    
    public GameObject deBuffPrefab;

    public bool reqUpdated = false;

    float iconHalfSize;

    // Start is called before the first frame update
    void Start()
    {
        iconHalfSize = deBuffPrefab.gameObject.GetComponent<RectTransform>().sizeDelta.x / 2;
        deBuffs = new List<DeBuff>();
    }

    public void AddBuff(DeBuffTypes type)
    {
        DeBuff debuff = deBuffs.Find(x => x.deBuffType == type);

        if (debuff == null)
        {
            // 동일한 종류의 디버프가 없다면 새로 생성
            GameObject n = Instantiate(deBuffPrefab, gameObject.transform);
            DeBuff nD = n.GetComponent<DeBuff>();
            nD.indicator = this;
            nD.deBuffType = type;
            deBuffs.Add(nD);
            reqUpdated = true;
        }
        else if (debuff.info.stacks != 0)
        {
            // 중첩이 가능한 경우 1스택 추가 후 레이블 업데이트
            debuff.info.stacks++;
            debuff.UpdateLabel();
        }
        else
        {
            // 지속시간 초기화
            debuff.startTime = Time.time;
        }
    }

    public void test()
    {
        AddBuff(DeBuffTypes.Burn);
    }

    public void RemoveBuff(DeBuff _)
    {
        reqUpdated = deBuffs.Remove(_);
        if (reqUpdated)
        {
            Destroy(_.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 새로운 버프에 걸릴 경우에만 위치 업데이트가 필요하다.
        if (!reqUpdated || deBuffs.Count < 1) return;
        int cur = 0;
        

        foreach (DeBuff deBuff in deBuffs)
        {
            float x = iconHalfSize + (cur * ((2 * iconHalfSize) + 4));
            deBuff.gameObject.transform.localPosition = new Vector2(x, 0);
            cur++;
        }

        reqUpdated = false;
    }
}
