using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private static Color _selectedTileColor = Color.grey;
    private static Tile _previousSelectedTile;

    private SpriteRenderer _spriteRenderer;
    private bool _isSelected;

    private Vector2[] _neighborDirections = new Vector2[] { Vector2.down, Vector2.up, Vector2.left, Vector2.right };

    public SpriteRenderer TileRenderer
    {
        get => _spriteRenderer;
        set
        {
            if (value == null) return;
            _spriteRenderer = value;
        }
    }

    private bool _matchFound;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _previousSelectedTile = null;
        _matchFound = false;
    }

    public void Select()
    {
        _isSelected = true;
        _spriteRenderer.color = _selectedTileColor;
        _previousSelectedTile = gameObject.GetComponent<Tile>();
    }

    public void Deselect()
    {
        _isSelected = false;
        _spriteRenderer.color = Color.white;
        _previousSelectedTile = null;
    }

    private void OnMouseDown() 
    {
        if (_spriteRenderer.sprite == null || BoardManager.Instance.IsShifting) 
        {
            return;
        }

        if (_isSelected) 
        {
            Deselect();
        } 
        else 
        {
            if (_previousSelectedTile == null) 
            {
                Select();
            } 
            else 
            {
                if (GetAllNeighborTiles().Contains(_previousSelectedTile.gameObject)) 
                { 
                    SwapSprite(_previousSelectedTile.TileRenderer);
                    _previousSelectedTile.ClearAllMatches();
                    _previousSelectedTile.Deselect();
                    ClearAllMatches();
                } 
                else 
                {
                    _previousSelectedTile.GetComponent<Tile>().Deselect();
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

    private GameObject GetNeighbor(Vector2 castDir) 
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null) 
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllNeighborTiles() 
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < _neighborDirections.Length; i++) 
        {
            adjacentTiles.Add(GetNeighbor(_neighborDirections[i]));
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
            
            _matchFound = true;
        }
    }

    public void ClearAllMatches()
    {
        if (_spriteRenderer.sprite is null) return;

        ClearMatch(new[] {Vector2.left, Vector2.right});
        ClearMatch(new[] {Vector2.up, Vector2.down});

        if (_matchFound)
        {
            _spriteRenderer.sprite = null;
            _matchFound = false;
            StopCoroutine(BoardManager.Instance.FindNullTiles());
            StartCoroutine(BoardManager.Instance.FindNullTiles());
        }
    }
}
