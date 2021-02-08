using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MotionDisappear : MotionBase
{
    [SerializeField] private Material AppearMaterial = null;
    [SerializeField] private GameObject AppearParticle = null;
    private Material BackupMaterial;
    private float flashAmount = 0;
    private float lightAmount = 0;
    private Color Tint = Color.white;
    private GameObject particle;

    public override UnitState State { get { return UnitState.Disappear; } }
    public override bool IsReady() { return false; }

    public override void OnEnter()
    {
        Unit.Anim.SetTrigger("idle");
        particle = Instantiate(AppearParticle, Unit.Center, Quaternion.identity, transform);
        BackupMaterial = Unit.SR.material;
        Unit.SR.material = AppearMaterial;
        flashAmount = 0.0f;
        lightAmount = 1.0f;
        Tint = Color.white;
        Unit.SR.material.SetFloat("_FlashAmount", flashAmount);
        Unit.SR.material.SetFloat("_LightAmount", lightAmount);
        Unit.SR.material.SetColor("_Color", Tint);
        StartCoroutine(AnimateFlash());
    }
    private IEnumerator AnimateFlash()
    {
        float duration = 0.5f;
        float time = 0;
        while(time < duration)
        {
            float rate = time / duration;
            flashAmount = rate;
            lightAmount = 1 - rate;
            Tint.r = 1 - rate;
            Unit.SR.material.SetFloat("_FlashAmount", flashAmount);
            Unit.SR.material.SetFloat("_LightAmount", lightAmount);
            Unit.SR.material.SetColor("_Color", Tint);

            time += Time.deltaTime;
            yield return null;
        }
        Destroy(particle);
        Destroy(gameObject);
    }
}

