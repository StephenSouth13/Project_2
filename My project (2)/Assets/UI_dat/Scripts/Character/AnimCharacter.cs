using Photon.Pun;
using UnityEngine;

public class AnimCharacter : MonoBehaviourPun
{

    private Animator animator;
    public string[] attackTriger;
    public string[] skillTriger;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    //=== ANIM CHUNG ===
    [PunRPC] public void PlayMove(bool isMoving) {   animator.SetBool("1_Move", isMoving); }
    [PunRPC] public void PlayTrigerDamaged(){ animator.SetTrigger("3_Damaged");}
    [PunRPC] public void PlayTriggerDead() { animator.SetTrigger("4_Death"); }
    //=== HẾT CHUNG ===

    //=== LOGIC GỌI ANIM RIÊNG ===
    [PunRPC]
    public void PlayAttack(int index)
    {
        if (attackTriger != null && index >= 0 && index < attackTriger.Length)
        {
            animator.SetTrigger(attackTriger[index]);
        }
        else
        {
            Debug.LogWarning($"Attack trigger index {index} không hợp lệ hoặc chưa được cấu hình.");
        }
    }
    [PunRPC]
    public void PlaySkill(int index)
    {
        if(skillTriger != null && index >= 0 && index < skillTriger.Length)
        {
            animator.SetTrigger(skillTriger[index]);
        }
        else
        {
            Debug.LogWarning($"skillTriger index {index} không hợp lệ hoặc chưa được cấu hình.");
        }
    }
}
