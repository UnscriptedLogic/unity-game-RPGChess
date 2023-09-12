using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float referenceMaxSpeed = 6;
    [SerializeField] private float multiplier;
    [SerializeField] private float animTransitionTime;
    private Vector3 previousPosition;

    private float calculatedAnimSpeed;

    private void Start()
    {
        previousPosition = transform.position;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, previousPosition);

        calculatedAnimSpeed = Mathf.Lerp(calculatedAnimSpeed, (distance * multiplier) / referenceMaxSpeed, animTransitionTime);
        animator.SetFloat("MovementSpeed", calculatedAnimSpeed);
    
        previousPosition = transform.position;
    }
}
