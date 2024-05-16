using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurationIndicator : MonoBehaviour
{
    public List<GameObject> durationIndicators;
    GameObject _durationIndicator;
    void Start()
    {
        _durationIndicator = Resources.Load<GameObject>("Duration");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CreateDurationIndicator(float maxDuration, string name = "")
    {
        GameObject durationIndicator = Instantiate(_durationIndicator, transform); // 지속시간 표시 오브젝트 생성
        var indicatorComponent = durationIndicator.GetComponent<Duration>(); // 지속시간 표시 컴포넌트
        indicatorComponent.maxDuration = maxDuration; // 지속시간 설정
        indicatorComponent.skillName = name; // 이름 설정
        durationIndicators.Add(durationIndicator); // 리스트에 추가

    }
}
