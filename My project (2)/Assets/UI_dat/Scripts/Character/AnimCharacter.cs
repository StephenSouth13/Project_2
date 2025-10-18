using Photon.Pun;
using UnityEngine;

public class AnimCharacter : MonoBehaviourPun
{
    private Animator animator;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void SetBoolDefault()
    {
        animator.SetBool("5_Debuff", false);
        animator.SetBool("isDeath", false);
    }
    [PunRPC]
    public void PlayMove(bool isMoving)
    {
        animator.SetBool("1_Move", isMoving);
    }
    [PunRPC]
    public void PlayDebuff()
    {
        SetBoolDefault();
        animator.SetBool("5_Debuff", true);
    }
    [PunRPC]
    public void PlayTriggerAttack()
    {
        animator.SetTrigger("2_Attack");
    }
    [PunRPC]
    public void PlayTrigerDamaged()
    {
        animator.SetTrigger("3_Damaged");
    }
    [PunRPC]
    public void PlayTriggerDead()
    {
        animator.SetTrigger("4_Death");
    }
}
