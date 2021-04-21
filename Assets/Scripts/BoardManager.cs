using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] List<Sprite> charactersSprites = new List<Sprite>();

    [SerializeField] private int xSize;
    [SerializeField] private int ySize;
    [SerializeField] private bool isShifting;

    [SerializeField] private GameObject tile;

    public int XSize => xSize;
    public int YSize => ySize;
    public bool IsShifting => isShifting;

    private GameObject[,] _tiles;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }

    private void CreateBoard(float xOffset, float yOffset)
    {
        _tiles = new GameObject[xSize, ySize];

        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Vector3 tilePosition = new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0);
                GameObject newTile = Instantiate(tile, tilePosition, tile.transform.rotation);
                _tiles[x, y] = newTile;
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
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                if (_tiles[x, y].GetComponent<SpriteRenderer>().sprite is null)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }

        for (int x= 0; x< XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                _tiles[x, y].GetComponent<Tile>().ClearAllMatches();
            }
        }
    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
    {
        isShifting = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < YSize; y++)
        {
            SpriteRenderer spriteRenderer = _tiles[x,y].GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite is null)
            {
                nullCount++;
            }
            renders.Add(spriteRenderer);
        }

        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(shiftDelay);
            GUIManager.Instance.Score += 50;
            for (int k = 0; k < renders.Count - 1; k++)
            {
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, YSize - 1);
            }
        }

        isShifting = false;
    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(charactersSprites);

        if (x > 0)
        {
            possibleCharacters.Remove(_tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }

        if (x < XSize - 1)
        {
            possibleCharacters.Remove(_tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }

        if (y > 0)
        {
            possibleCharacters.Remove(_tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
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
