using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Usage:
//    yield return Yielders.EndOfFrame;     =>      yield return Yielders.EndOfFrame;
//    yield return Yielders.FixedUpdate;    =>      yield return Yielders.FixedUpdate;
//    yield return Yielders.GetWaitForSeconds(1.0f);    =>      yield return Yielders.GetWaitForSeconds(1.0f);

public static class Yielders
{
    static Dictionary<float, WaitForSeconds> _waitForSecond = new Dictionary<float, WaitForSeconds>(300);
    //static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
    //static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();

    //public static WaitForEndOfFrame EndOfFrame
    //{
    //    get
    //    {
    //        return _endOfFrame;
    //    }
    //}

    //public static WaitForFixedUpdate FixedUpdate
    //{
    //    get
    //    {
    //        return _fixedUpdate;
    //    }
    //}

    public static WaitForSeconds GetWaitForSeconds(float time)
    {
        if (_waitForSecond.TryGetValue(time, out WaitForSeconds wfs) == false)
        {
            // Yield 캐싱량이 300개를 넘어가면 초기화한다. (소수점을 랜덤값으로 넘겨주는 경우에는 엄청나게 쌓인다..)
            if (_waitForSecond.Count >= 300)
            {
                _waitForSecond.Clear();
            }

            wfs = new WaitForSeconds(time);
            _waitForSecond.Add(time, wfs);
        }

        return wfs;
    }



    public static WaitForSeconds GetWaitForSeconds(int time)
    {
        return GetWaitForSeconds((float)time);
    }


    // 현재 사용량 확인용 함수
    public static int GetDicCount()
    {
        return _waitForSecond.Count;
    }
}

