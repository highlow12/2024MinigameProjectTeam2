using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeIndicator : MonoBehaviour
{
    [SerializeField] List<Image> lifes = new List<Image>();
    [SerializeField] Sprite heart;
    [SerializeField] Sprite brokenHeart;
    [SerializeField] Sprite darkHeart;

    public void SetLife(int life)
    {
        for (int i = 0; i < lifes.Count; i++)
        {
            Debug.Log($"{life} / {i}");
            if (life == 0)
            {
                lifes[i].sprite = brokenHeart;
            }
            else if (life - 1 < i)
            {
                lifes[i].sprite = darkHeart;
            }
            else
            {
                lifes[i].sprite = heart;
            }
        }
    }
}
