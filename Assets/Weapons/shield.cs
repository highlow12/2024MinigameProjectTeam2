using UnityEngine;

public class Shield : MonoBehaviour
{
    public float attackSpeed = 1.0f; 
    public float moveSpeed = 5.0f; 
    public float range = 2.0f; 
    public int defense = 100; 

    public int healingAmount = 50; 

    
    public void HealAlly(PlayerController ally)
    {
        
        ally.Heal(healingAmount);
    }
}

