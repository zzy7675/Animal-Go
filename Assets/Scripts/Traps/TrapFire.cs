using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapFire : MonoBehaviour
{
    private Animator anim;
    private CapsuleCollider2D fireCollider;
    private bool isActive;

    [SerializeField] private float offDuration;
    [SerializeField] private TrapFireButton fireButton;
    public void SwitchOffFire()
    {
        if (!isActive)
            return;
        StartCoroutine(FireRoutine());
    }
    private IEnumerator FireRoutine()
    {
        SetFire(false);
        yield return new WaitForSeconds(offDuration);
        SetFire(true);
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();
        fireCollider = GetComponent<CapsuleCollider2D>();
    }
    private void Start()
    {
        if (fireButton == null)
            Debug.LogWarning("You don't have fire button on " + gameObject.name + "!");

        SetFire(true);
    }

    private void SetFire(bool active)
    {
        anim.SetBool("active", active);
        fireCollider.enabled = active;
        isActive = active;
    }
}
