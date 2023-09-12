using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraTransform;

    private float turnSmoothVel;

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(x, 0f, y).normalized;

        if (dir.magnitude > 0f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, rotationSpeed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 movementDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(movementDir * movementSpeed * Time.deltaTime);
        }
    }
}
