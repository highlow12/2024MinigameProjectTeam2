// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;

// public class HealthBar : MonoBehaviour
// {
//     public float maxHP = 400f;
//     public float hp = 400f;
//     GameObject bar;
//     TMP_Text label;

//     public BuffIndicator buffIndicator;

//     void Awake()
//     {
//         Transform[] childrens = GetComponentsInChildren<Transform>();
//         foreach (Transform child in childrens)
//         {
//             if (child.name == "Bar") bar = child.gameObject;
//             else if (child.name == "Label") label = child.GetComponent<TMP_Text>();
//         }
//     }

//     void Update()
//     {
//         if (!buffIndicator) return;
//         if (buffIndicator.deBuffs.Count > 0)
//         {
//             // Debug.Log(buffIndicator)
//             foreach(DeBuff debuff in buffIndicator.deBuffs)
//             {
//                 if (debuff.deBuffType == DeBuffTypes.Burn)
//                 {
//                     hp -= debuff.info.effects[(int)EffectIndex.damage] * Time.deltaTime;
//                 }
//             }
//         }

//         bar.GetComponent<RectTransform>().offsetMax = new Vector2(-1 * (maxHP-hp), 0);
//         label.text = $"( {Mathf.Floor(hp)} / {maxHP} )";
//     }
// }

