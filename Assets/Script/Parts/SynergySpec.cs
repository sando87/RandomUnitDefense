using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergySpec : MonoBehaviour
{
    BaseObject mBaseObj = null;
    BuffProperty mSynergySpec = null;

    public BuffProperty Spec { get { return mSynergySpec; } }
    private bool IsMerged = false;

    void Awake()
    {
        mBaseObj = this.GetBaseObject();
        mSynergySpec = GetComponent<BuffProperty>();
    }

    void Start()
    {
        if(!IsMerged)
        {
            float multiplier = Mathf.Pow(2, mBaseObj.SpecProp.Level - 1);
            mSynergySpec.MultiplyBuffProp(multiplier);
        }
            
        mBaseObj.BuffProp.AddBuffProp(mSynergySpec);
    }

    public void MergeSynergySpecs(SynergySpec specA, SynergySpec specB, SynergySpec synergySpecC)
    {
        mSynergySpec.AddBuffProp(specA.Spec);
        mSynergySpec.AddBuffProp(specB.Spec);
        mSynergySpec.MultiplyBuffProp(0.5f);
        mSynergySpec.AddBuffProp(synergySpecC.Spec);
        IsMerged = true;
    }
}

