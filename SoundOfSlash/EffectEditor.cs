using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EditedEffect
{
    ParticleSystem target;

    bool isConstant = false;
    bool isTwoConstant = false;
    bool isStartSize3D = false;

    /* Start Size */
    float[] originScale;
    /* Life Time */
    float[] originLifeTime;

    /* Shape */
    bool hasCircleShape = false;
    float originShapeRadius;


    public EditedEffect(int operation, int type, ParticleSystem ps)
    {
        target = ps;
        switch(type)
        {
            case 0:
                isConstant = true;
                if (operation == 0)
                    originScale = new float[1];
                else
                    originLifeTime = new float[1];
                break;
            case 1:
                isTwoConstant = true;
                if (operation == 0)
                    originScale = new float[2];
                else
                    originLifeTime = new float[2];
                break;
            case 2: // scale만 존재
                isStartSize3D = true;
                originScale = new float[3];
                break;
        }
    }

    // option. circle shape
    void HandleCircleShape(float adjustVal)
    {
        hasCircleShape = true;
        var main = target.main;
        ParticleSystem.ShapeModule ps = target.shape;
        if (target.shape.shapeType == ParticleSystemShapeType.Circle)
        {
            originShapeRadius = target.shape.radius;            
            ps.radius= originShapeRadius + adjustVal;
        }
    }
    // Revert to origin Radius (option. circle shape)
    void BackToOriginalRadius()
    {
        ParticleSystem.ShapeModule ps = target.shape;
        ps.radius = originShapeRadius;
    }

    // Constant - EditScale
    public bool EditScale(float adjustVal, float a)
    {
        var main = target.main;      
        originScale[0] = a;
        if (isConstant)
        {
            main.startSize = originScale[0] / adjustVal;
        }
        HandleCircleShape(adjustVal);
        return true;
    }
    // Two Constant - EditScale
    public bool EditScale(float adjustVal, float a, float b)
    {
        var main = target.main;
        originScale[0] = a;
        originScale[1] = b;
        if(isTwoConstant)
        {
            main.startSize = new ParticleSystem.MinMaxCurve(originScale[0] / adjustVal, originScale[1] / adjustVal);
        }
        HandleCircleShape(adjustVal);
        return true;
    }
    // StartSize3D - EditScale
    public bool EditScale(float adjustVal, float a, float b, float c)
    {
        var main = target.main;
        originScale[0] = a;
        originScale[1] = b;
        originScale[2] = c;
        if(isStartSize3D)
        {
            main.startSizeXMultiplier = originScale[0] / adjustVal;
            main.startSizeYMultiplier = originScale[1] / adjustVal;
            main.startSizeZMultiplier = originScale[2] / adjustVal;
        }
        HandleCircleShape(adjustVal);
        return true;
    }
    // Constant - LifeTime 
    public void EditLifeTime(float adjustVal, float a)
    {
        originLifeTime[0] = a;
        var main = target.main;
        if(isConstant)
        {
            if (adjustVal < 0)
            {
                main.startLifetimeMultiplier *= Mathf.Abs(adjustVal);
            }
            else
            {
                main.startLifetimeMultiplier /= adjustVal;
            }
        }
    }
    // Two Constant - LiftTime
    public void EditLifeTime(float adjustVal, float a, float b)
    {
        originLifeTime[0] = a;
        originLifeTime[1] = b;
        var main = target.main;
        if (isTwoConstant)
        {
            if (adjustVal < 0)
            {
                main.startLifetime = new ParticleSystem.MinMaxCurve(originLifeTime[0] * Mathf.Abs(adjustVal), originLifeTime[1] * Mathf.Abs(adjustVal));
            }
            else
            {
                main.startLifetime = new ParticleSystem.MinMaxCurve(originLifeTime[0] / adjustVal, originLifeTime[1] / adjustVal);
            }
        }
    }

    // Revert to origin LiftTime 
    public void BackToOriginLifeTime()
    {
        var main = target.main;
        if(isConstant)
        {
            main.startLifetime = originLifeTime[0];
        }
        else if(isTwoConstant)
        {
            main.startLifetime = new ParticleSystem.MinMaxCurve(originLifeTime[0], originLifeTime[1]);
        }
    }
    // Revert to origin Size
    public void BackToOriginalSize()
    {
        var main = target.main;
        if (isConstant)
        {
            main.startSize = originScale[0];
        }
        else if (isTwoConstant)
        {
            main.startSize = new ParticleSystem.MinMaxCurve(originScale[0], originScale[1]);
        }
        else
        {
            main.startSizeXMultiplier = originScale[0];
            main.startSizeYMultiplier = originScale[1];
            main.startSizeZMultiplier = originScale[2];
        }

        if(hasCircleShape)
        {
            BackToOriginalRadius();
        }
    }

    public ParticleSystem GetTargetVFX()
    {
        return target;
    }
};


public class EffectEditor : MonoBehaviour
{
    private const int OP_SCALE = 0;
    private const int OP_DURATION = 1;
    private const int STARTSIZE_CONS = 0;
    private const int STARTSIZE_CONS2 = 1;
    private const int STARTSIZE_3D = 2;
    // Effect Editor
    // 1. Scale 조정
    // - 입력된 값만큼 기존 Paritcle system start size에 나누어 줌
    //   (constant / two constant / 3d start size를 구분)
    // - shape이 circle인 경우 radius에도 조절 값을 반영
    // 2. Duration 조정
    // - 입력된 값만큼 기존 Particle system start life time에 반영
    // ex) Input이 -2이면 2배 느리게, 2이면 2배 빠르게 조절
    public ParticleSystem targetParticleSystem = null;
    [HideInInspector]
    public float adjustScaleValue;
    [HideInInspector]
    public float adjustDurationValue;

    private List<EditedEffect> childVFXList_scale;
    private List<EditedEffect> childVFXList_duration;
    // ================================ VFX Scale ==============================
    public void AdjustParticleScale()
    {
        childVFXList_scale = new List<EditedEffect>();
        childVFXList_scale.Clear();

        if (adjustScaleValue == 0)
        {
            Debug.Log("0은 입력할 수 없습니다.");
            return;
        }

        PerformResizing(targetParticleSystem, adjustScaleValue);
    }

    private void PerformResizing(ParticleSystem targetVFX, float adjustVal)
    {
        for (int i = 0; i < targetVFX.transform.childCount; i++)
        {
            ParticleSystem childVFX = targetVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            var main = childVFX.main;

            if (main.startSize.mode == ParticleSystemCurveMode.Constant)
            {
                if (main.startSize3D)
                {
                    //Debug.Log("3d start size");
                    EditedEffect eef = new EditedEffect(OP_SCALE, STARTSIZE_3D, childVFX);
                    childVFXList_scale.Add(eef);                  
                    eef.EditScale(adjustScaleValue, main.startSizeXMultiplier, main.startSizeYMultiplier, main.startSizeZMultiplier);
                }
                else
                {
                    //Debug.Log("Constant");
                    EditedEffect eef = new EditedEffect(OP_SCALE, STARTSIZE_CONS, childVFX);
                    childVFXList_scale.Add(eef);
                    eef.EditScale(adjustScaleValue, main.startSize.constant);
                }
            }
            else if (main.startSize.mode == ParticleSystemCurveMode.TwoConstants)
            {
                //Debug.Log("2 Constant");
                EditedEffect eef = new EditedEffect(OP_SCALE, STARTSIZE_CONS2, childVFX);
                childVFXList_scale.Add(eef);
                eef.EditScale(adjustScaleValue, main.startSize.constantMin, main.startSize.constantMax);
            }
        }
    }

    // Revert to origin size
    public void BackToOriginSize()
    {
        for (int i = 0; i < targetParticleSystem.transform.childCount; i++)
        {
            ParticleSystem childVFX = targetParticleSystem.transform.GetChild(i).GetComponent<ParticleSystem>();
            if(childVFX == childVFXList_scale[i].GetTargetVFX())
            {
                childVFXList_scale[i].BackToOriginalSize();
            }
        }
    }

    // ================================ VFX Duration ==============================
    // Adjust vfx duration(life time)
    public void AdjustDuration()
    {
        childVFXList_duration = new List<EditedEffect>();
        childVFXList_duration.Clear();
        if (adjustDurationValue == 0)
        {
            Debug.Log("0은 입력할 수 없습니다.");
            return;
        }
        for (int i = 0; i < targetParticleSystem.transform.childCount; i++)
        {
            ParticleSystem childVFX = targetParticleSystem.transform.GetChild(i).GetComponent<ParticleSystem>();
            var main = childVFX.main;
            if (main.startLifetime.mode == ParticleSystemCurveMode.Constant)
            {
                EditedEffect eff = new EditedEffect(OP_DURATION, STARTSIZE_CONS, childVFX);
                childVFXList_duration.Add(eff);
                eff.EditLifeTime(adjustDurationValue, main.startLifetime.constant);
            }
            else if (main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
            {
                EditedEffect eff = new EditedEffect(OP_DURATION, STARTSIZE_CONS2, childVFX);
                eff.EditLifeTime(adjustDurationValue, main.startLifetime.constantMin, main.startLifetime.constantMax);
                childVFXList_duration.Add(eff);
            }
        }
    }
    // Revert to original duration
    public void BackToOriginDuration()
    {
        for (int i = 0; i < targetParticleSystem.transform.childCount; i++)
        {
            ParticleSystem childVFX = targetParticleSystem.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (childVFX == childVFXList_duration[i].GetTargetVFX())
            {
                childVFXList_duration[i].BackToOriginLifeTime();
            }
        }
    }

}