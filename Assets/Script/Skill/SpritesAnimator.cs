using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesAnimator : MonoBehaviour
{
    public Sprite[] Sprites = null;
    public bool IsLoop = false;
    public System.Action EventEnd = null;

    public static SpritesAnimator Play(Vector3 pos, Sprite[] sprites, bool isLoop = false)
    {
        if(sprites == null) return null;
        
        SpritesAnimator prefab = ResourcesCache.Load<SpritesAnimator>("Prefabs/SpritesAnimator");
        SpritesAnimator obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.Sprites = sprites;
        obj.IsLoop = isLoop;
        return obj;
    }

    public static SpritesAnimator Play(Vector3 startPos, Vector3 destPos, Sprite[] sprites, bool isLoop = false)
    {
        SpritesAnimator prefab = ResourcesCache.Load<SpritesAnimator>("Prefabs/SpritesAnimator");
        SpritesAnimator obj = Instantiate(prefab, startPos, Quaternion.identity);
        obj.Sprites = sprites;
        obj.IsLoop = isLoop;
        Vector3 dir = destPos - startPos;
        dir.z = 0;
        float dist = dir.magnitude;
        dir.Normalize();
        obj.transform.localScale = new Vector3(dist, 1, 1);
        obj.transform.right = dir;
        return obj;
    }

    void Start()
    {
        if(Sprites == null) return;

        if(IsLoop)
        {
            StartCoroutine(CoPlayLoop());
        }
        else
        {
            StartCoroutine(CoPlay());
        }
    }

    IEnumerator CoPlay()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        foreach(Sprite sprite in Sprites)
        {
            renderer.sprite = sprite;
            yield return new WaitForSeconds(1 / 12.0f);
        }
        EventEnd?.Invoke();
        Destroy(gameObject);
    }

    IEnumerator CoPlayLoop()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        int idx = 0;
        while(true)
        {
            renderer.sprite = Sprites[idx];
            yield return new WaitForSeconds(1 / 12.0f);
            idx = (idx + 1) % Sprites.Length;
        }
    }
}
