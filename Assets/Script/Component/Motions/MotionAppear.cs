using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionAppear : MotionBase
{
    [SerializeField] private Material AppearMaterial = null;
    [SerializeField] private GameObject AppearParticle = null;
    private Material BackupMaterial;
    private float flashAmount = 0;
    private float lightAmount = 0;
    private Color Tint = Color.white;
    private GameObject particle;

    public override bool IsReady() { return false; }

    public override void OnEnter()
    {
        particle = Instantiate(AppearParticle, Unit.Center, Quaternion.identity, transform);
        BackupMaterial = Unit.SR.material;
        Unit.SR.material = AppearMaterial;
        flashAmount = 1.0f;
        lightAmount = 0;
        Tint = Color.cyan;
        Unit.SR.material.SetFloat("_FlashAmount", flashAmount);
        Unit.SR.material.SetFloat("_LightAmount", lightAmount);
        Unit.SR.material.SetColor("_Color", Tint);
        StartCoroutine(AnimateFlash());
    }
    public override void OnLeave()
    {
        base.OnLeave();
        Destroy(particle);
        Unit.SR.material = BackupMaterial;
    }
    private IEnumerator AnimateFlash()
    {
        float duration = 0.5f;
        float time = 0;
        while(time < duration)
        {
            float rate = time / duration;
            flashAmount = 1 - rate;
            lightAmount = rate;
            Tint.r = rate;
            Unit.SR.material.SetFloat("_FlashAmount", flashAmount);
            Unit.SR.material.SetFloat("_LightAmount", lightAmount);
            Unit.SR.material.SetColor("_Color", Tint);

            time += Time.deltaTime;
            yield return null;
        }
        Unit.FSM.ChangeState(UnitState.Idle);
    }
}

