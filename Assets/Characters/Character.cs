using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;
//using UnityEngine.UIElements;

/// <summary>
/// Mainly used as a data container to define a character. This script is attached to the prefab
/// (found in the Bundles/Characters folder) and is to define all data related to the character.
/// </summary>
public class Character : MonoBehaviour
{
    public Animator animator;

    [Header("Debug")]
    [ReadOnly] [SerializeField] private float _animatorSpeed;

    public void SetVelocity(float value)
    {
        value = Mathf.Clamp01(value);

        animator.SetFloat("Speed", value);

        _animatorSpeed = value;
    }

    public void SetRunning(bool value)
    {
        animator.SetBool("Running", value);
    }
}
