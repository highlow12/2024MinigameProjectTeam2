using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuStatue : MonoBehaviour
{
    Camera main;
    SpriteRenderer spriteRenderer;
    List<GameObject> childrens = new List<GameObject>();

    public float moveSpeed = -3f;
    public int durability = 1;
    public Sprite nornalSprite;
    public Sprite brokenSprite;

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = nornalSprite;

        foreach (Transform tf in GetComponentsInChildren<Transform>())
        {
            if (tf.gameObject.name == name) continue;
            Debug.Log(tf.gameObject.name);
            childrens.Add(tf.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moveSpeed == 0) return;
        Vector3 pos = transform.position;
        Vector3 _pos = main.WorldToViewportPoint(pos);

        float dX = moveSpeed * Time.deltaTime;

        if (_pos.x < -0.5)
        {
            transform.position = new Vector3(12 + dX, pos.y, pos.z);
            AddCount();
        }
        else
        {
            transform.position = new Vector3(pos.x + dX, pos.y, pos.z);
        }
    }

    void AddCount()
    {
        durability--;
        if (durability < 1)
        {
            if (brokenSprite) spriteRenderer.sprite = brokenSprite;
            foreach (GameObject child in childrens)
            {
                child.SetActive(false);
            }
        }
    }
}
