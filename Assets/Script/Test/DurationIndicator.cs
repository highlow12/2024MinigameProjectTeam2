using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurationIndicator : MonoBehaviour
{
    public List<GameObject> durationIndicators;
    public GameObject durationIndicator;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CreateDurationIndicator(float maxDuration, string name = "")
    {
        GameObject duration = Instantiate(durationIndicator, transform); // 지속시간 표시 오브젝트 생성
        var indicatorComponent = duration.GetComponent<Duration>(); // 지속시간 표시 컴포넌트
        indicatorComponent.maxDuration = maxDuration; // 지속시간 설정
        indicatorComponent.skillName = name; // 이름 설정
        durationIndicators.Add(duration); // 리스트에 추가

    }
}
