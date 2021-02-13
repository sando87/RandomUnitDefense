using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionDeathMob : MotionBase
{
    [SerializeField] private AudioClip DeathSound = null;
    public override bool IsReady() { return false; }

    public override void OnEnter()
    {
        RGame.Get<RGameSystemManager>().DeathLineMob();
        ((UnitMob)Unit).HPBar.HideBar();
        Unit.Anim.SetTrigger("death");
        StartCoroutine(FadeOut());
        RGame.Get<RSoundManager>().PlaySFX(DeathSound);
    }

    private IEnumerator FadeOut()
    {
        //점점 희미하게 사라져가는 효과
        yield return Yielders.GetWaitForSeconds(2.0f);
        Color color = Unit.SR.color;
        float duration = 1;
        float time = 0;
        while (time < duration)
        {
            color.a = 1 - time / duration;
            Unit.SR.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}

