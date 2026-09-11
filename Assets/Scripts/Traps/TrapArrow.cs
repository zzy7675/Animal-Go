using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrapArrow : TrapTrampoline
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float cooldown;
    [SerializeField] private bool rotateClockwise;
    [SerializeField] private float scaleSpeed;
    [SerializeField] private Vector3 targetScale;
    private int rotateDirection = 1;
    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    private void Update()
    {
        HandleScaling();
        HandleRotation();
    }

    private void HandleScaling()
    {
        if (transform.localScale.x < targetScale.x)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        rotateDirection = rotateClockwise ? -1 : 1;
        transform.Rotate(0, 0, (rotateSpeed * rotateDirection) * Time.deltaTime);
    }

    private void DestroyMe() {
        GameObject arrowPrefab = ObjectCreator.instance.arrowPrefab;
        ObjectCreator.instance.CreateObject(arrowPrefab, transform, cooldown);
        Destroy(gameObject);
    }
}
