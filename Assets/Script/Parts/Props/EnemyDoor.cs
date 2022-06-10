using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoor : MonoBehaviour, IMapEditorObject
{
    [SerializeField] BoxCollider DetectArea = null;
    [SerializeField] SpriteRenderer ImageTarget = null;
    [SerializeField] Sprite[] SpriteImages = null;

    void Start()
    {
        StartCoroutine(CoUpdate());
    }

    IEnumerator CoUpdate()
    {
        yield return new WaitForSeconds(1);

        while(true)
        {
            if(!IsEnemyAround())
                break;

            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(0.5f);

        PlayAnimation(0.3f);
        yield return new WaitForSeconds(0.3f);
        this.GetBaseObject().Body.Lock = true;
    }

    private bool IsEnemyAround()
    {
        Collider[] cols = Physics.OverlapBox(DetectArea.bounds.center, DetectArea.bounds.extents, Quaternion.identity, 1 << LayerID.Enemies);
        foreach(Collider col in cols)
        {
            if(col.GetComponentInBaseObject<AIBigMovingBotInput>() != null)
                return true;

            if(col.GetComponentInBaseObject<AIPillBugRobotInput>() != null)
                return true;
        }

        return false;
    }

    private void PlayAnimation(float duration)
    {
        StartCoroutine(CoPlayAnimation(duration));
    }
    IEnumerator CoPlayAnimation(float duration)
    {
        float interval = duration / SpriteImages.Length;
        for (int i = 0; i < SpriteImages.Length; ++i)
        {
            ImageTarget.sprite = SpriteImages[i];
            yield return new WaitForSeconds(interval);
        }
    }
    private void PlayAnimationReverse(float duration)
    {
        StartCoroutine(CoPlayAnimationReverse(duration));
    }
    IEnumerator CoPlayAnimationReverse(float duration)
    {
        float interval = duration / SpriteImages.Length;
        for (int i = SpriteImages.Length - 1; i >= 0; --i)
        {
            ImageTarget.sprite = SpriteImages[i];
            yield return new WaitForSeconds(interval);
        }
    }

    public void OnInitMapEditor()
    {
        enabled = false;
    }
}
