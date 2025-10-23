using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Character : MonoBehaviourPun
{
    [Header("Components")]
    private UICharacter uICharacter;
    private CombatCharacter combatCharacter;
    private AnimCharacter animCharacter;
    void Awake()
    {
        
        uICharacter = GetComponent<UICharacter>();
        combatCharacter = GetComponent<CombatCharacter>();
        animCharacter = GetComponent<AnimCharacter>();
        
    }

}
