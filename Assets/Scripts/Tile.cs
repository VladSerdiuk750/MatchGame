using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private static Color selectedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private static Tile previousSelected = null;

    private SpriteRenderer _spriteRenderer;
    private bool _isSelected = false;

    private Vector2[] _neighborDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    public SpriteRenderer TileRenderer
    {
        get => _spriteRenderer;
        set
        {
            if (value == null) return;
            _spriteRenderer = value;
        }
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Select()
    {
        _isSelected = true;
        _spriteRenderer.color = selectedColor;
        previousSelected = gameObject.GetComponent<Tile>();
        Debug.Log("selected");
    }

    public void Deselect()
    {
        _isSelected = false;
        _spriteRenderer.color = Color.white;
        previousSelected = null;
        Debug.Log("deselect");
    }

    private void OnMouseDown()
    {
        if(_spriteRenderer == null || BoardManager.Instance.IsShifting)
        {
            if(_isSelected)
            {
                Deselect();
            }
            else
            {
                if (previousSelected == null)
                {
                    Select();
                }
                else
                {
                    SwapSprite(previousSelected.TileRenderer);
                    previousSelected.Deselect();
                }
            }
        }
    }

    private void SwapSprite(SpriteRenderer renderer)
    {
        if(_spriteRenderer.sprite == renderer.sprite)
        {
            return;
        }

        Sprite tempSprite = renderer.sprite;
        renderer.sprite = _spriteRenderer.sprite;
        _spriteRenderer.sprite = tempSprite;
    }
}
