using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] List<Sprite> charactersSprites = new List<Sprite>();

    [SerializeField] private int _xSize;
    [SerializeField] private int _ySize;
    [SerializeField] private bool _isShifting;

    [SerializeField] private GameObject tile;

    public int xSize => _xSize;
    public int ySize => _ySize;
    public bool IsShifting => _isShifting;

    private GameObject[,] tiles;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }

    private void CreateBoard(float xOffset, float yOffset)
    {
        tiles = new GameObject[_xSize, _ySize];

        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[_ySize];
        Sprite previousBelow = null;

        for (int x = 0; x < _xSize; x++)
        {
            for (int y = 0; y < _ySize; y++)
            {
                Vector3 tilePosition = new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0);
                GameObject newTile = Instantiate(tile, tilePosition, tile.transform.rotation);
                tiles[x, y] = newTile;
                newTile.transform.parent = transform;

                Sprite newSprite = RandomizeSprite(previousLeft[y], previousBelow);
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;

                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }

    public IEnumerator FindNullTiles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }
    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
    {
        _isShifting = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++)
        {
            SpriteRenderer renderer = tiles[x,y].GetComponent<SpriteRenderer>();
            if (renderer.sprite is null)
            {
                nullCount++;
            }
            renders.Add(renderer);
        }

        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < renders.Count - 1; k++)
            {
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
            }
        }

        _isShifting = false;
    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(charactersSprites);

        if (x > 0)
        {
            possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }

        if (x < xSize - 1)
        {
            possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }

        if (y > 0)
        {
            possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }

    private Sprite RandomizeSprite(Sprite characterLeft, Sprite characterBelow)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(charactersSprites);
        possibleCharacters.Remove(characterLeft);
        possibleCharacters.Remove(characterBelow);

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }
}
