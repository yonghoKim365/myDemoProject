using UnityEngine;
using System.Collections;

public class DummyAniEvent : MonoBehaviour 
{
    public void Attack(float DamageRatio = 1.0f){
        Debug.Log("Attack");
    }
        
    public void AttackEnd(){
    }

    public void DieEnd(){
    }

    public void SkillUse(int skillID){
        //Debug.Log("SkillUse");
    }

    public void SkillUse_Child(int childGroupID){
    }

    public void SkillEnd(){
    }

    public void PlaySound(int soundID) { }

    public void CameraShake(float ShakePower){
        Debug.Log("CameraShake");
    }

    public void SkillMove(string SpawnProjectile)
    {
    }
    
    public void DoAction(int actionID) {}
    public void SpawnFx(string effectName) {}
}
