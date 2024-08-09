using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
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
    Rigidbody2D rb;

    public Transform target;
    public Transform DiveFeedback;
    public IEnumerator performDiveAttack() 
    {
        float upspeed = 100;
        float StayWaitSeconds = 1;
        var gravityScale = rb.gravityScale;

        rb.gravityScale = 0;
        rb.velocity = Vector2.up * upspeed;

        yield return new WaitForSeconds(StayWaitSeconds/10);

        rb.velocity = Vector3.zero;
        
        transform.position = new(target.position.x + Random.onUnitSphere.x,transform.position.y, transform.position.z);

        yield return new WaitForSeconds(StayWaitSeconds);

        
        
        RaycastHit2D[] hitsArray;
        hitsArray = Physics2D.RaycastAll(transform.position, Vector3.down,10000);

        foreach (var hit in hitsArray) 
        {
            if (hit.transform.CompareTag("Ground"))
            {
                DiveFeedback.position = (Vector3)hit.point + (Vector3.up * 0.5f);
                Debug.Log((Vector3)hit.point + (Vector3.up * 0.5f));
                DiveFeedback.gameObject.SetActive(true);
            }
        }

        yield return new WaitForSeconds(StayWaitSeconds);

        rb.gravityScale = gravityScale;
        rb.velocity = Vector3.down * 100;
        DiveFeedback.gameObject.SetActive(false);
    }
    public IEnumerator slashForward() 
    { 
    yield return new WaitForFixedUpdate();}
    public IEnumerator slashBackward() 
    { 
    yield return new WaitForFixedUpdate();}
    public IEnumerator slashBothDir() 
    {
    yield return new WaitForFixedUpdate();}
    public IEnumerator slashWithEnerge() 
    {
    yield return new WaitForFixedUpdate();}
    public IEnumerator chargeAttack() 
    {
    yield return new WaitForFixedUpdate();}


    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 220, 220), "Boss Pattern");

        
        if (GUI.Button(new Rect(20, 40, 190, 20), "Fly_Attack"))
        {
            StopCoroutine("performDiveAttack");
            StartCoroutine( "performDiveAttack");
        }

        // 두번 째 버튼 만들기
        if (GUI.Button(new Rect(20, 70, 190, 20), "Cut_with_Gi"))
        {
            StartCoroutine("slashForward");
        }

        if (GUI.Button(new Rect(20, 100, 190, 20), "Cut_Forward"))
        {
            StartCoroutine("slashBackward");
        }

        if (GUI.Button(new Rect(20, 130, 190, 20), "Cut_Backward"))
        {
            StartCoroutine("slashBothDir");
        }

        if (GUI.Button(new Rect(20, 160, 190, 20), "Cut_Forward_and_Backward"))
        {
            StartCoroutine("slashWithEnerge");
        }

        if (GUI.Button(new Rect(20, 190, 190, 20), "DolJin"))
        {
            StartCoroutine("chargeAttack");
        }

    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
