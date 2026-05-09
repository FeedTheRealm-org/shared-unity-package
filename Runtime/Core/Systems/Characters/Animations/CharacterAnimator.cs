using System;
using System.Collections.Generic;
using FTRShared.Runtime.Models;
using UnityEngine;

/// <summary>
/// Handles character animation logic, centralizing animator control.
/// </summary>
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject front;

    [SerializeField]
    private GameObject back;

    [SerializeField]
    private GameObject right;

    [SerializeField]
    private GameObject left;

    [SerializeField]
    private Logging.Logger logger;

    private Dictionary<FacingDirection, GameObject> spriteMap;

    private WeaponType currentWeaponType;
    private SubWeaponType currentSubWeaponType;
    private bool hasEquipment = false;

    public event Action OnUseAnimationEnd;

    public FacingDirection CurrentFacing { get; private set; }

    private void Start()
    {
        if (animator == null)
            throw new System.Exception("Animator reference is missing on CharacterAnimator.");

        spriteMap = new Dictionary<FacingDirection, GameObject>()
        {
            { FacingDirection.Front, front },
            { FacingDirection.Back, back },
            { FacingDirection.Right, right },
            { FacingDirection.Left, left },
        };

        SetFacing(FacingDirection.Front);
        PlayIdle();
    }

    /* --- Getters --- */

    public bool IsMoving()
    {
        return animator.GetBool("IsRunning");
    }

    public bool IsIdle()
    {
        return animator.GetInteger("State") == 0 || animator.GetInteger("State") == 1;
    }

    /* --- Setters --- */

    public void SetFacing(FacingDirection facing)
    {
        if (spriteMap == null)
            return;

        foreach (var kvp in spriteMap)
        {
            kvp.Value.SetActive(kvp.Key == facing);
        }

        CurrentFacing = facing;

        if (IsIdle())
            PlayIdle(); // To reset to correct idle
    }

    public void SetMoving(bool isMoving)
    {
        animator.SetBool("IsRunning", isMoving);
    }

    public void SetDashing(bool isDashing)
    {
        // animator.SetBool("IsDashing", isDashing);
    }

    public void SetAction(bool isAction)
    {
        animator.SetBool("Action", isAction);
    }

    public void SetEquipment(WeaponType weaponType, SubWeaponType subWeaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Melee:
                animator.SetInteger("WeaponType", 0);
                break;
            case WeaponType.Ranged:
                switch (subWeaponType)
                {
                    case SubWeaponType.HandHeld:
                        animator.SetInteger("WeaponType", 5);
                        break;
                    case SubWeaponType.Bow:
                        animator.SetInteger("WeaponType", 0);
                        break;
                }
                break;
            default:
                animator.SetInteger("WeaponType", 0);
                break;
        }
        this.currentWeaponType = weaponType;
        this.currentSubWeaponType = subWeaponType;
        this.hasEquipment = true;
        if (IsIdle())
            PlayIdle(); // To reset to correct idle
    }

    public void UnSetWeaponType()
    {
        animator.SetInteger("WeaponType", 0);
        this.currentWeaponType = default;
        this.currentSubWeaponType = default;
        hasEquipment = false;
        if (IsIdle())
            PlayIdle(); // To reset to correct idle
    }

    /* --- Players --- */
    public void PlayIdle()
    {
        if (
            this.currentWeaponType == WeaponType.Ranged
            && this.currentSubWeaponType == SubWeaponType.HandHeld
            && (
                this.CurrentFacing == FacingDirection.Front
                || this.CurrentFacing == FacingDirection.Back
            )
        )
            animator.SetInteger("State", 1);
        else
            animator.SetInteger("State", 0);
    }

    public void PlayUse()
    {
        if (!hasEquipment)
        {
            animator.SetTrigger("Jab");
            return;
        }

        switch (currentWeaponType)
        {
            case WeaponType.Melee:
                animator.SetTrigger("Slash1H");
                break;
            case WeaponType.Ranged:
                if (currentSubWeaponType == SubWeaponType.Bow)
                    animator.SetTrigger("ShotBow");
                else
                    animator.SetTrigger("Fire");
                break;
            default:
                Debug.LogWarning($"PlayUse called with unhandled weapon type: {currentWeaponType}");
                break;
        }
    }

    public void PlayDamaged()
    {
        animator.SetTrigger("Hit");
    }

    public void PlayDeath()
    {
        animator.SetInteger("State", 9);
    }

    /* --- Animator Hooks --- */

    public void PlayUseEndHook()
    {
        SetAction(false);
        OnUseAnimationEnd?.Invoke();
    }
}
