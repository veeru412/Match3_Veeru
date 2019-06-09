using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Block blockPrefab;
    public Item itemPrefab;
    public Block[,] gridBlocks;
    public int maxRows;
    public int maxCols;
    public int maxColors;
    public readonly float blockWidth = 1.0f;
    public readonly float blockHeight = 1.0f;
    public Sprite[] itemSprites;
    public static GameManager instance;
    public bool dragBlocked = false;  
    [HideInInspector]
    public Transform gameField;
    GameState _gameState;

    [Header("UI")]
    public GameObject MainMenu;
    public GameObject homeBtn;
    public GameObject noMoreMatches;
    [Header("VFX")]
    public ParticleSystem explosion;
    public Color[] explosionColor;
    List<ParticleSystem> explosionArray = new List<ParticleSystem>();
    int particleIndex;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            DestroyImmediate(gameObject);
        }
        gameField = new GameObject("GameField").transform;
        gameState = GameState.Menu;
        for(int i= 0; i< 20; i++)
        {
            explosionArray.Add(Instantiate(explosion));
        }
    }
    public GameState gameState
    {
        get
        {
            return _gameState;
        }
        set
        {
            _gameState = value;
            if(_gameState == GameState.Menu)
            {
                MainMenu.SetActive(true);
                homeBtn.SetActive(false);
                if (gameField.childCount > 0)
                {
                    foreach (Transform t in gameField)
                        Destroy(t.gameObject);
                }
                if (fallRoute != null)
                    StopCoroutine(fallRoute);
            }else if(_gameState == GameState.PrepareGame)
            {
                MainMenu.SetActive(false);
                homeBtn.SetActive(true);
                CreateGrid();
            }else if(_gameState == GameState.Playing)
            {

            }
        }
    }

    void GenerateNewItems()
    {
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = maxRows - 1; row >= 0; row--)
            {
                Block b = GetBlock(row, col);
                if(b != null && b.myItem == null)
                {
                    b.GenItem();
                }
            }
        }
        gameState = GameState.Playing;
    }
    public Block GetBlock(int row,int col)
    {
        if (row < 0 || col < 0 || row >= maxRows || col >= maxCols)
            return null;
        return gridBlocks[row, col];
    }
    private void CreateGrid()
    {
        float offsetX = (-maxCols / 2f) * blockWidth + blockWidth / 2f;
        float offsetY = (maxRows / 2f) * blockHeight - blockHeight / 2f;
        gridBlocks = new Block[maxRows, maxCols];
        Vector3 spawnPos = new Vector3(offsetX, offsetY, 1.0f);
        for(int row = 0; row < maxRows; row++)
        {
            for(int col = 0; col < maxCols; col++)
            {
                Block b = Instantiate(blockPrefab, gameField);
                b.transform.position = spawnPos;
                spawnPos.x += blockWidth;
                b.row = row;
                b.col = col;
                gridBlocks[row, col] = b;
            }
            spawnPos.x = offsetX;
            spawnPos.y -= blockHeight;
        }
        GenerateNewItems();
    }
    Vector3 mouseStartPos;
    bool isDragging = false;
    Item dragItem;
    Item switchItem;
    private void Update()
    {
        if (dragBlocked || gameState != GameState.Playing)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if(hit != null)
            {
                Item item = hit.GetComponent<Item>();
                if(item != null)
                {
                    dragItem = item;
                    mouseStartPos = GetMousePosition();
                    isDragging = true;
                }
            }
        }
        if (isDragging)
        {
            Vector3 deltaPos = mouseStartPos - GetMousePosition();
            if(Vector3.Magnitude(deltaPos) > 0.1f)
            {
                isDragging = false;
                if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y) && deltaPos.x > 0)
                    dragItem.swipeDirection = SwipeDirection.LEFT;
                else if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y) && deltaPos.x < 0)
                    dragItem.swipeDirection = SwipeDirection.RIGHT;
                else if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y) && deltaPos.y > 0)
                    dragItem.swipeDirection = SwipeDirection.DOWN;
                else if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y) && deltaPos.y < 0)
                    dragItem.swipeDirection = SwipeDirection.UP;
            }
        }
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
    Coroutine fallRoute;
    public void CheckFall()
    {
        fallRoute =  StartCoroutine(FallDown());
    }
    IEnumerator FallDown()
    {
        dragBlocked = true;
        yield return new WaitForFixedUpdate();
        
        while (true)
        {
            for (int i = 0; i < 20; i++)
            {
                for (int row = maxRows - 1; row >= 0; row--)   //need to Check rows from bottom to top
                {
                    for (int col = 0; col < maxCols; col++)
                    {
                        if (gridBlocks[row, col] != null)
                            gridBlocks[row, col].CheckFallDown();
                    }
                }
            }
            for (int row = maxRows - 1; row >= 0; row--)   //need to Check rows from bottom to top
            {
                for (int col = 0; col < maxCols; col++)
                {
                    if (gridBlocks[row, col].myItem != null)
                        gridBlocks[row, col].myItem.Settle();
                }
            }
            yield return new WaitForSeconds(0.1f);
            while (!isAllItemsSettled())
            {
                yield return new WaitForSeconds(0.1f);
            }
            for (int row = maxRows - 1; row >= 0; row--)   //need to Check rows from bottom to top
            {
                for (int col = 0; col < maxCols; col++)
                {
                    if (gridBlocks[row, col] != null)
                        gridBlocks[row, col].GenItem(true);
                }
            }
            yield return new WaitForFixedUpdate();
            while (!isAllItemsSettled())
            {
                yield return new WaitForSeconds(0.1f);
            }
            List<Item> allMaches = GetAllMatches();
            if (allMaches.Count <= 0)
                break;
            else
            {
                for(int i = 0; i < allMaches.Count; i++)
                {
                    allMaches[i].DestroyItem();
                }
            }
            yield return new WaitForFixedUpdate();
        }
        bool haveMatches = false;
        while (!haveMatches)
        {
            yield return StartCoroutine(CheckForPossibleMatches(result => haveMatches = result));
            if (!haveMatches)
            {
                yield return new WaitForSeconds(2.0f);
                noMoreMatches.gameObject.SetActive(true);
                yield return new WaitForSeconds(2.0f);
                yield return StartCoroutine(ReGenarateLevel());
            }
        }
        dragBlocked = false;
    }

    IEnumerator ReGenarateLevel()
    {
        for(int row = 0; row < maxRows; row++)
        {
            for(int col = 0; col< maxCols; col++)
            {
                Item it = gridBlocks[row, col].myItem;
                if(it != null)
                {
                    it.DestroyItem(true);
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
        while (!isAllItemsDestroyed())
        {
            yield return new WaitForSeconds(0.1f);
        }
        GenerateNewItems();
        yield return new WaitForSeconds(1f);
    }
    public bool isAllItemsDestroyed()
    {
        Item[] allItems = FindObjectsOfType<Item>();
        for(int i =0; i< allItems.Length; i++)
        {
            if (allItems[i].destroyFinished == false)
                return false;
        }
        return true;
    }
    IEnumerator CheckForPossibleMatches(System.Action<bool> haveMatches)
    {
        for(int row = 0; row < maxRows; row++)
        {
            for(int col = 0; col < maxCols; col++)
            {
                if (gridBlocks[row, col].HaveNextMatch())
                {
                    haveMatches(true);
                    yield break;
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }
    List<Item> GetAllMatches()
    {
        List<Item> allMatches = new List<Item>();
        for(int row = 0; row < maxRows; row++)
        {
            for(int col = 0; col < maxCols; col++)
            {
                allMatches.AddRange(gridBlocks[row, col].myItem.GetMatchesArround());
            }
        }
        return allMatches;
    }
    bool isAllItemsSettled()
    {
        bool settled = true;
        for (int row = 0; row < GameManager.instance.maxRows; row++)   //need to Check rows from bottom to top
        {
            for (int col = 0; col < maxCols; col++)
            {
                Item it = gridBlocks[row, col].myItem;
                if (it != null && it.fallingDown)
                    return false;
            }
        }
        return settled;
    }

    public void PlayDestroyParticle(int color,Vector3 pos)
    {
        if(color < itemSprites.Length)
        {
            ParticleSystem p = explosionArray[particleIndex];
            p.startColor = explosionColor[color];
            p.transform.position = pos;
            p.Play();
            particleIndex++;
            if (particleIndex >= explosionArray.Count)
                particleIndex = 0;
        }   
    }
    public Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
public enum GameState
{
    Menu,
    PrepareGame,
    Playing,
    GameOver
}
