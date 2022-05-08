using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EffectEditor))]
public class EffectCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(1);

        EffectEditor effectEditor = (EffectEditor)target;
        
        GUILayout.Space(1);

        GUILayout.Space(1);
        
        effectEditor.adjustScaleValue = EditorGUILayout.Slider("Added to scale", effectEditor.adjustScaleValue, 0, 10);
        if (GUILayout.Button("Adjust Scale"))
        {
            effectEditor.AdjustParticleScale();
        }
        GUILayout.Space(1);
        if (GUILayout.Button("Back To Previous Scale"))
        {
            effectEditor.BackToOriginSize();
        }

        GUILayout.Space(1);

        effectEditor.adjustDurationValue = EditorGUILayout.Slider("Adjust Duration", effectEditor.adjustDurationValue, -5, 5);
        if (GUILayout.Button("Adjust Duration"))
        {
            effectEditor.AdjustDuration();
        }

        if(GUILayout.Button("Back to Previous Duration"))
        {
            effectEditor.BackToOriginDuration();
        }
    }
}
