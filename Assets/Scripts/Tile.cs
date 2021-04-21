using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private static Color _selectedColor = Color.grey;
    private static Tile _previousSelected = null;

    private SpriteRenderer _spriteRenderer;
    private bool _isSelected;

    private Vector2[] adjacentDirections = new Vector2[] { Vector2.down, Vector2.up, Vector2.left, Vector2.right };

    public SpriteRenderer TileRenderer
    {
        get => _spriteRenderer;
        set
        {
            if (value == null) return;
            _spriteRenderer = value;
        }
    }

    private bool matchFound = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Select()
    {
        _isSelected = true;
        _spriteRenderer.color = _selectedColor;
        _previousSelected = gameObject.GetComponent<Tile>();
    }

    public void Deselect()
    {
        _isSelected = false;
        _spriteRenderer.color = Color.white;
        _previousSelected = null;
    }

    private void OnMouseDown() {
        // Not Selectable conditions
        if (_spriteRenderer.sprite == null || BoardManager.Instance.IsShifting) 
        {
            return;
        }

        if (_isSelected) 
        { // Is it already selected?
            Deselect();
        } 
        else 
        {
            if (_previousSelected == null) 
            { // Is it the first tile selected?
                Select();
            } 
            else 
            {
                if (GetAllAdjacentTiles().Contains(_previousSelected.gameObject)) 
                { 
                    SwapSprite(_previousSelected.TileRenderer);
                    _previousSelected.ClearAllMatches();
                    _previousSelected.Deselect();
                    ClearAllMatches();
                } 
                else 
                {
                    _previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
            }
        }
    }

    public void SwapSprite(SpriteRenderer render2) 
    {
        if (_spriteRenderer.sprite == render2.sprite) 
        {
            return;
        }

        Sprite tempSprite = render2.sprite;
        render2.sprite = _spriteRenderer.sprite;
        _spriteRenderer.sprite = tempSprite;
    }

    private GameObject GetAdjacent(Vector2 castDir) 
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null) 
        {
            Debug.Log(hit.collider.gameObject.transform.position);
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles() {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++) {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == _spriteRenderer.sprite)
        {
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }

        return matchingTiles;
    }

    private void ClearMatch(Vector2[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();

        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }

        if (matchingTiles.Count >= 2)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }

            matchFound = true;
        }
    }

    public void ClearAllMatches()
    {
        if (_spriteRenderer.sprite == null) return;

        ClearMatch(new Vector2[2] {Vector2.left, Vector2.right});
        ClearMatch(new Vector2[2] {Vector2.up, Vector2.down});

        if (matchFound)
        {
            _spriteRenderer.sprite = null;
            matchFound = false;
        }
    }
}
