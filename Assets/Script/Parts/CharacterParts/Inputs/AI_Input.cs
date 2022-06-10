using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 컴퓨터 인공지능 알고리즘에 기반하여 캐릭터(적몬스터)의 입력을 제어한다.
public class AI_Input : CharacterInput
{
    void Start()
    {
        StartCoroutine(CoAIAlgorithm());
    }

    // 테스트용 간단한 몬스터 AI 알고리즘
    IEnumerator CoAIAlgorithm()
    {
        while(true)
        {
            // idle상태에서 몇초간 대기 후 다음 동작 수행
            float waitSec = UnityEngine.Random.Range(1, 3);
            yield return new WaitForSeconds(waitSec);
        }
    }
}
