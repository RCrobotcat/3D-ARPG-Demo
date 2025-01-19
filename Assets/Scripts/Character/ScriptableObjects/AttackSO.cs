using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "Attacks/Normal Attack")]
public class AttackSO : ScriptableObject
{
    public AnimatorOverrideController animatorOV;
    public float damage;

    public List<VisualEffect> attackVFXs;
    public VFXType vfxType;
}

public enum VFXType
{
    None,
    Left,
    Right,
    Center
}
