using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic.Experimental.Generation;

[RequireComponent(typeof(BoxCollider))]
public class Node : MonoBehaviour
{
    [SerializeField] private Color highlightColor = Color.gray;
    
    private MeshRenderer meshRenderer;
    private Color originalColor;
    private Cell cell;

    public Cell Cell => cell;

    public static event EventHandler OnAnyNodeSelected;

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Initialize(Cell cell)
    {
        this.cell = cell;
    }

    private void OnMouseDown()
    {
        OnAnyNodeSelected?.Invoke(this, EventArgs.Empty);
    }
}
