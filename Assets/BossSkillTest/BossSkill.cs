using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *### 보스 패턴
 *- 1페이즈
 *  - 보스가 날아올랐다가 내려 찍기
 *      - 안 보이는 곳으로 이동 후, 내려 찍기
 *      - 내려 찍기 직전에 유저 피드백 줌
 *      - 파회 방법: 구르기, 범위 밖에 존재
 *  - 베기
 *      - 앞으로 베기
 *      - 뒤로 베기
 *      - 앞/뒤로 베기
 *      - 베면서 검기 나가기
 *      - 파회 방법: 구르기, 범위 밖에 존재, 패링
 *  - 돌진
 *      - 돌진 범위 내의 캐릭터에게 피해
 *      - 파회 방법: 점프, 패링
 *          - 패링 시 캐릭터가 약간 뒤로 밀려남
 */
public class BossSkill : MonoBehaviour
{
    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 100, 200), "Boss Pattern");

        
        if (GUI.Button(new Rect(20, 40, 80, 20), "Fly Attack"))
        {
            //Application.LoadLevel(1);
        }

        // 두번 째 버튼 만들기
        if (GUI.Button(new Rect(20, 70, 80, 20), "DolJin"))
        {
            //Application.LoadLevel(2);
        }

        if (GUI.Button(new Rect(20, 90, 80, 20), "Cut Forward"))
        {
            //Application.LoadLevel(2);
        }

        if (GUI.Button(new Rect(20, 110, 80, 20), "Cut Backward"))
        {
            //Application.LoadLevel(2);
        }

        if (GUI.Button(new Rect(20, 130, 80, 20), "Cut Forward and Backward"))
        {
            //Application.LoadLevel(2);
        }

        if (GUI.Button(new Rect(20, 150, 80, 20), "Cut with Gi"))
        {
            //Application.LoadLevel(2);
        }

    }
}
