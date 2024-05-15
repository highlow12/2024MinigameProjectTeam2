using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffIndicator : MonoBehaviour
{
    public List<DeBuff> deBuffs;
    public GameObject deBuffPrefab;

    public bool reqUpdated = false;

    // Start is called before the first frame update
    void Start()
    {
        deBuffs = new List<DeBuff>(); 
    }

    public void addBuff ()
    {
        DeBuffTypes type = DeBuffTypes.Blind;
        DeBuff debuff = deBuffs.Find(x => x.deBuffType == type);

        if (debuff == null) {
            // 동일한 종류의 디버프가 없다면 새로 생성
            GameObject n = Instantiate(deBuffPrefab, gameObject.transform);
            DeBuff nD = n.GetComponent<DeBuff>();
            nD.indicator = this;

            deBuffs.Add(nD);
            reqUpdated = true;
        } else if (debuff.stackable) {
            // 중첩이 가능한 경우 1스택 추가 후 레이블 업데이트
            debuff.stacks++;
            debuff.UpdateLabel();
        } else {
            // 지속시간 초기화
            debuff.startTime = Time.time;
        }
    }

    public void removeBuff (DeBuff _)
    {
        reqUpdated = deBuffs.Remove(_);
        if (reqUpdated) {
            Destroy(_.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 새로운 버프에 걸리거나 버프가 사라졌을 경우에만 위치 업데이트가 필요하다.
        if (!reqUpdated) return;
        int cur = 0;
        foreach (DeBuff deBuff in deBuffs) {
            float x = 16 + (cur * (16 + 16 + 2));
            deBuff.gameObject.transform.localPosition = new Vector2(x, 0);
            cur++;
        }

        reqUpdated = false;
    }
}
