using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [System.Serializable]
    private class ObjectInfo
    {
        // 오브젝트 이름
        public string ObjectName;
        // 오브젝트 풀에서 관리할 오브젝트
        public GameObject Prefab;
        // parent of the object that will be created
        public Transform ObjectParent;
        // 몇개를 미리 생성 해놓을건지
        public int Count;
    }
    public static ObjectPoolManager Instance;

    // 오브젝트풀 매니저 준비 완료표시
    public bool IsReady { get; private set; }

    [SerializeField]
    private ObjectInfo[] objectInfos;

    // 생성할 오브젝트의 key값지정을 위한 변수
    private string objectName;
    // Parent of the object that will be created
    private Transform objectParent;

    // 오브젝트풀들을 관리할 딕셔너리
    private readonly Dictionary<string, IObjectPool<GameObject>> objectPoolDic = new();
    // 오브젝트풀에서 오브젝트를 새로 생성할때 사용할 딕셔너리
    private Dictionary<string, GameObject> goDic = new();
    private Dictionary<string, GameObject> goRoot = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
        }

        else
        {
            Destroy(this.gameObject);
        }

    }


    private void Init()
    {
        IsReady = false;

        for (int idx = 0; idx < objectInfos.Length; idx++)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
            OnDestroyPoolObject, true, objectInfos[idx].Count, objectInfos[idx].Count);

            if (goDic.ContainsKey(objectInfos[idx].ObjectName))
            {
                Debug.LogFormat("{0} 이미 등록된 오브젝트입니다.", objectInfos[idx].ObjectName);
                return;
            }

            goDic.Add(objectInfos[idx].ObjectName, objectInfos[idx].Prefab);
            goRoot.Add(objectInfos[idx].ObjectName, objectInfos[idx].ObjectParent.gameObject);
            objectPoolDic.Add(objectInfos[idx].ObjectName, pool);

            // 미리 오브젝트 생성 해놓기
            for (int i = 0; i < objectInfos[idx].Count; i++)
            {
                objectName = objectInfos[idx].ObjectName;
                objectParent = objectInfos[idx].ObjectParent;
                Debug.Log($"Created {objectName}, {objectParent}");
                PoolAble poolAbleGo = CreatePooledItem().GetComponent<PoolAble>();
                poolAbleGo.Pool.Release(poolAbleGo.gameObject);
            }
        }

        Debug.Log("오브젝트풀링 준비 완료");
        IsReady = true;
    }

    // 생성
    private GameObject CreatePooledItem()
    {
        GameObject poolGo = Instantiate(goDic[objectName], goRoot[objectName].transform);
        poolGo.GetComponent<PoolAble>().Pool = objectPoolDic[objectName];
        return poolGo;
    }

    // 대여
    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    // 반환
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    // 삭제
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }

    public GameObject GetGo(string goName)
    {
        objectName = goName;

        if (goDic.ContainsKey(goName) == false)
        {
            Debug.LogFormat("{0} 오브젝트풀에 등록되지 않은 오브젝트입니다.", goName);
            return null;
        }

        return objectPoolDic[goName].Get();
    }


}
