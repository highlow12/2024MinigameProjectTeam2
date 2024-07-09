using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuStatue : MonoBehaviour
{
    Camera main;
    SpriteRenderer spriteRenderer;

    float moveSpeed = -3f;
    public Sprite nornalSprite;
    public Sprite brokenSprite;

    int count; // 오브젝트가 맵 바깥으로 나간 횟수 (MainMenuForegroundController에서 제어)

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = nornalSprite;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        Vector3 _pos = main.WorldToViewportPoint(pos);

        float dX = moveSpeed * Time.deltaTime;

        if (_pos.x < -1)
        {
            transform.position = new Vector3(11 + dX, pos.y, pos.z);
            AddCount();
        }
        else
        {
            transform.position = new Vector3(pos.x + dX, pos.y, pos.z);
        }
    }

    void AddCount()
    {
        count++;
        if (count > 1) {
            spriteRenderer.sprite = brokenSprite;
        }
    }
}
