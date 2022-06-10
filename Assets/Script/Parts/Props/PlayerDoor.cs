using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoor : MonoBehaviour, IMapEditorObject
{
    [SerializeField] BoxCollider PlayerDetectArea = null;
    [SerializeField] SpriteRenderer ImageTarget = null;
    [SerializeField] Sprite[] SpriteImages = null;

    void Start()
    {
        StartCoroutine(CoUpdate());
    }

    IEnumerator CoUpdate()
    {
        while(true)
        {
            yield return new WaitUntil(() => IsPlayerAround());

            yield return new WaitForSeconds(0.5f);

            PlayAnimation(0.3f);
            yield return new WaitForSeconds(0.3f);
            this.GetBaseObject().Body.Lock = true;

            yield return new WaitForSeconds(2.0f);

            yield return new WaitUntil(() => !IsPlayerAround());

            PlayAnimationReverse(0.3f);
            this.GetBaseObject().Body.Lock = false;

            yield return new WaitForSeconds(1);
        }
    }

    private bool IsPlayerAround()
    {
        Collider[] cols = Physics.OverlapBox(PlayerDetectArea.bounds.center, PlayerDetectArea.bounds.extents, Quaternion.identity, 1 << LayerID.Player);
        return cols.Length > 0;
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
