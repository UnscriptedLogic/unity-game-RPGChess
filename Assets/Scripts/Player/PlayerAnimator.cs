using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovementScript;
    [SerializeField] private Animator animator;
    [SerializeField] private float referenceMaxSpeed = 6;
    [SerializeField] private float multiplier;
    [SerializeField] private float animTransitionTime;
    [SerializeField] private List<GameObject> weaponObjects;

    private Vector3 previousPosition;
    private bool canAct;
    private float calculatedAnimSpeed;

    private void Start()
    {
        previousPosition = transform.position;
        canAct = true;
    }

    private void Update()
    {

        float distance = Vector3.Distance(transform.position, previousPosition);

        calculatedAnimSpeed = Mathf.Lerp(calculatedAnimSpeed, (distance * multiplier) / referenceMaxSpeed, animTransitionTime);
        calculatedAnimSpeed = Mathf.Clamp01(calculatedAnimSpeed);
        animator.SetFloat("MovementSpeed", calculatedAnimSpeed);
    
        previousPosition = transform.position;

        if (!canAct) return;

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

    public void EnableMovement()
    {
        canAct = false;
        playerMovementScript.SetCanMove(false);
    }

    public void DisableMovement()
    {
        canAct = true;
        playerMovementScript.SetCanMove(true);
    }

    public void EnableWeapons()
    {
        for (int i = 0; i < weaponObjects.Count; i++)
        {
            weaponObjects[i].SetActive(true);
        }
    }

    public void DisableWeapons()
    {
        for (int i = 0; i < weaponObjects.Count; i++)
        {
            weaponObjects[i].SetActive(false);
        }
    }
}
