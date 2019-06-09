using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public int color;
    public Block myBlock;
    SpriteRenderer spriteRenderer;
    Animator anim;
    public SwipeDirection swipeDirection
    {
        set
        {
            Block switchingBlock = null;
            if(value == SwipeDirection.DOWN)
            {
                switchingBlock = myBlock.GetNeighborBottom();
            }else if(value == SwipeDirection.UP)
            {
                switchingBlock = myBlock.GetNeighborTop();
            }
            else if(value == SwipeDirection.RIGHT)
            {
                switchingBlock = myBlock.GetNeighborRight();
            }
            else if(value == SwipeDirection.LEFT)
            {
                switchingBlock = myBlock.GetNeighborLeft();
            }
            if(switchingBlock != null)
            {
                StartCoroutine(Switching(switchingBlock));
            }
        }
    }
    IEnumerator Switching(Block neighborBlock)
    {
        GameManager.instance.dragBlocked = true;
        Item switchItem = neighborBlock.myItem;
        switchItem.myBlock = myBlock;
        myBlock.myItem = switchItem;

        myBlock = neighborBlock;
        neighborBlock.myItem = this;

        Vector3 startPos = transform.position;
        float progress = 0;
        float startTime = Time.time;
        while(progress < 1)
        {
            progress = (Time.time - startTime) * 5;
            transform.position = Vector3.Lerp(startPos, neighborBlock.transform.position, progress);
            switchItem.transform.position = Vector3.Lerp(neighborBlock.transform.position, startPos, progress);
            yield return new WaitForFixedUpdate();
        }
        List<Item> matchesInThisItem = GetMatchesArround();
        List<Item> MatchesInNeighborItem = switchItem.GetMatchesArround();
        if(matchesInThisItem.Count <2 && MatchesInNeighborItem.Count < 2)
        {
            Block temp = switchItem.myBlock;

            switchItem.myBlock = myBlock;
            myBlock.myItem = switchItem;

            myBlock = temp;
            temp.myItem = this;
            startPos = transform.position;
            progress = 0;
            startTime = Time.time;
            while (progress < 1)
            {
                progress = (Time.time - startTime) * 5;
                transform.position = Vector3.Lerp(startPos, myBlock.transform.position, progress);
                switchItem.transform.position = Vector3.Lerp(switchItem.transform.position, startPos, progress);
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            if(matchesInThisItem.Count >= 2)
            {
                matchesInThisItem.Add(this);
                for (int i = 0; i < matchesInThisItem.Count; i++)
                {
                    if (matchesInThisItem[i] != null)
                        matchesInThisItem[i].DestroyItem();
                }
            }
            if(MatchesInNeighborItem.Count >= 2)
            {
                MatchesInNeighborItem.Add(switchItem);
                for (int i = 0; i < MatchesInNeighborItem.Count; i++)
                {
                    if (MatchesInNeighborItem[i] != null)
                        MatchesInNeighborItem[i].DestroyItem();
                }
            }
            GameManager.instance.CheckFall();
        }
       
        GameManager.instance.dragBlocked = false;
    }


    public List<Item> GetMatchesArround()
    {
        List<Item> finalItmeList = new List<Item>();
        int row = myBlock.row;
        int col = myBlock.col;

        List<Item> HorizontalMatches = new List<Item>();
        if(col < GameManager.instance.maxCols-1)
        {
            HorizontalMatches.AddRange(GetMatchesOnLeftOrRight(1));
        }
        if (col > 0)
        {
            HorizontalMatches.AddRange(GetMatchesOnLeftOrRight(-1));
        }
        List<Item> verticalMatches = new List<Item>();
        if(row > 0)
        {
            verticalMatches.AddRange(GetMatchesTopOrBottom(-1));
        }
        if(row < GameManager.instance.maxRows-1)
        {
            verticalMatches.AddRange(GetMatchesTopOrBottom(1));
        }
        if(HorizontalMatches.Count >= 2)
        {
            finalItmeList.AddRange(HorizontalMatches);
        }
        if(verticalMatches.Count >= 2)
        {
            finalItmeList.AddRange(verticalMatches);
        }
        return finalItmeList;               
    }
   
    List<Item> GetMatchesOnLeftOrRight(int direction)
    {
        List<Item> itemList = new List<Item>();

        int row = myBlock.row;
        int col = myBlock.col;
        bool checkingFinished = false;
        while (!checkingFinished)
        {
            col += direction;
            Block b = GameManager.instance.gridBlocks[row, col];
            if (b.myItem.color == this.color)
            {
                itemList.Add(b.myItem);
            }
            else
            {
                checkingFinished = true;
            }
            if ((col <= 0 && direction == -1) || (col >= GameManager.instance.maxCols-1 && direction == 1))
            {
                checkingFinished = true;
            }
        }
        return itemList;
    }
    List<Item> GetMatchesTopOrBottom(int direction)
    {
        List<Item> itemList = new List<Item>();

        int row = myBlock.row;
        int col = myBlock.col;
        bool checkingFinished = false;
        while (!checkingFinished)
        {
            row += direction;
            Block b = GameManager.instance.gridBlocks[row, col];
            if (b.myItem.color == color)
            {
                itemList.Add(b.myItem);
            }
            else
            {
                checkingFinished = true;
            }
            if ((row <= 0 && direction == -1) || (row >= GameManager.instance.maxRows-1 && direction == 1))
            {
                checkingFinished = true;
            }
        }
        return itemList;
    }
    void Start() {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        color = GetRandomColor();
        spriteRenderer.sprite = GameManager.instance.itemSprites[color];
    }

    int GetRandomColor()
    {
        int row = myBlock.row;
        int col = myBlock.col;

        List<int> remainColors = new List<int>();
        for (int i = 0; i < GameManager.instance.maxColors; i++)
        {
            bool canGen = true;
            if (col > 1)
            {
                Block neighbor = GameManager.instance.gridBlocks[row, col - 1];
                if (neighbor != null)
                {
                    if (neighbor.myItem != null)
                    {
                        if (neighbor.myItem.color == i)
                            canGen = false;
                    }
                }
            }
            if (col < GameManager.instance.maxCols - 1)
            {
                Block neighbor = GameManager.instance.gridBlocks[row, col + 1];
                if (neighbor != null)
                {
                    if (neighbor.myItem != null)
                    {
                        if (neighbor.myItem.color == i)
                            canGen = false;
                    }
                }
            }
            if (row < GameManager.instance.maxRows-1)
            {
                Block neighbor = GameManager.instance.gridBlocks[row + 1, col];
                if (neighbor != null)
                {
                    if (neighbor.myItem != null)
                    {
                        if (neighbor.myItem.color == i)
                            canGen = false;
                    }
                }
            }
            if (canGen)
            {
                remainColors.Add(i);
            }
        }
        //  int randomNo = GetRandomColorWithProbabilty();
        int randomNo = Random.Range(0, GameManager.instance.maxColors);
        if (remainColors.Count > 0)
            randomNo = remainColors[Random.Range(0, remainColors.Count)];

        return randomNo;
    }
    int GetRandomColorWithProbabilty()
    {
        int clr = 0;
        if (GameManager.instance.maxColors == 3)
        {
            float rand = Random.value;
            if (rand <= .5f)                    // 50 %
                clr = 0;
            else if (rand <= .7f)               // 20 %
                clr = 1;
            else                                // 30 %
                clr = 2;
        }
        else
        {
            clr = Random.Range(0, GameManager.instance.maxColors);
        }
        return clr;
    }
    public void FallTo(Block empty)
    {
        myBlock.myItem = null;
        myBlock = empty;
        empty.myItem = this;
    }
    public bool fallingDown = false;
    public void Settle()
    {
        float dis = Vector3.Distance(transform.position, myBlock.transform.position);       
        if (!fallingDown && dis >0.5f)
            StartCoroutine(FaLL());
        else
            transform.position = myBlock.transform.position;
    }
    IEnumerator FaLL()
    {
        fallingDown = true;
        Vector3 startPos = transform.position;
        float progress = 0;
        float startTime = Time.time;
        while (progress < 1)
        {
            progress = (Time.time - startTime) * 5;
            transform.position = Vector3.Lerp(startPos, myBlock.transform.position, progress);
            yield return new WaitForFixedUpdate();
        }
        anim.SetTrigger("settle");
        fallingDown = false;
    }

    bool destroying = false;
    public void DestroyItem(bool animEffect = false)
    {
        if (destroying)
            return;
        destroying = true;
        myBlock.myItem = null;
        if (!animEffect)
        {
            GameManager.instance.PlayDestroyParticle(color, transform.position);
            Destroy(gameObject);
        }
        else
        {
            anim.SetTrigger("destroy");
        }
    }
    public bool destroyFinished = false;
    public void OnDestroyAnimFinished()
    {
        if (destroyFinished)
            return;
        destroyFinished = true;
        Destroy(gameObject);
    }
}
public enum SwipeDirection
{
    NONE,
    LEFT,
    RIGHT,
    UP,
    DOWN
}
