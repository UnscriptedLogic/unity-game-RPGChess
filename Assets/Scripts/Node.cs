using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Node : MonoBehaviour
{
    [SerializeField] private Color highlightColor = Color.gray;
    
    private MeshRenderer meshRenderer;
    private Color originalColor;

    public static EventHandler OnAnyNodeSelected;

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void OnMouseEnter()
    {
        originalColor = meshRenderer.material.color;
        meshRenderer.material.color = highlightColor;
    }

    private void OnMouseExit()
    {
        meshRenderer.material.color = originalColor;
    }

    private void OnMouseDown()
    {
        OnAnyNodeSelected?.Invoke(this, EventArgs.Empty);
    }
}
