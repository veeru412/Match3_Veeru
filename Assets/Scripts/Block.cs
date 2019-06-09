using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public int row;
    public int col;
    public Item myItem;
    public bool isFallingDown;

    public void GenItem(bool falling = false)
    {
        if(myItem == null)
        {
            myItem = Instantiate(GameManager.instance.itemPrefab, GameManager.instance.gameField);
            myItem.myBlock = this;
            myItem.transform.position = transform.position;
            if (falling)
            {
                Vector3 pos = transform.position;
                pos.y = 5f;
                myItem.transform.position = pos;
                myItem.Settle();
            }
        }
    }

    public void CheckFallDown()
    {
        if (myItem != null)
        {
            Block nextBlock = null;
            if(row < GameManager.instance.maxRows - 1)
            {
                for (int i = row + 1; i < GameManager.instance.maxRows; i++)
                {
                    Block temp = GameManager.instance.GetBlock(i, col);
                    if (temp != null && temp.myItem == null)
                    {
                        nextBlock = GameManager.instance.GetBlock(i, col);
                        break;
                    }
                }
            }

            if (nextBlock != null && nextBlock.myItem == null)
            {
                myItem.FallTo(nextBlock);
            }
        }
    }
    public bool HaveNextMatch()
    {
        bool haveMatches = false;
        // will check right combinations first
        Block neighborBlock = GameManager.instance.GetBlock(row,col+1);
        if (neighborBlock != null)
        {
            if(neighborBlock.myItem.color == myItem.color)
            {
                // X X 0 X                  
                Block checkBlock = GameManager.instance.GetBlock(row, col + 3);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    return true;
                }
                //     X
                // X X 0
                checkBlock = GameManager.instance.GetBlock(row - 1, col + 2);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    return true;
                }

                // X X 0
                //     X
                checkBlock = GameManager.instance.GetBlock(row + 1, col + 2);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    return true;
                }
            }
            else
            {
                Block nextBlock = GameManager.instance.GetBlock(row, col + 2);
                if(nextBlock != null && nextBlock.myItem.color == myItem.color)
                {
                    //   X
                    // X 0 X
                    Block checkBlock = GameManager.instance.GetBlock(row - 1, col + 1);
                    if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                    {
                        return true;
                    }

                    // X   X
                    //   0 
                    checkBlock = GameManager.instance.GetBlock(row + 1, col + 1);
                    if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                    {
                        return true;
                    }
                }
            }
        }
        // checking for left combinations
        neighborBlock = GameManager.instance.GetBlock(row, col - 1);
        if (neighborBlock != null)
        {
            if (neighborBlock.myItem.color == myItem.color)
            {
                // X 0 X X                  
                Block checkBlock = GameManager.instance.GetBlock(row, col - 3);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    return true;
                }
                // X
                // 0 X X
                checkBlock = GameManager.instance.GetBlock(row - 1, col - 2);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    return true;
                }

                // 0 X X
                // X
                checkBlock = GameManager.instance.GetBlock(row + 1, col - 2);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    return true;
                }
            }
            else
            {
                Block nextBlock = GameManager.instance.GetBlock(row, col - 2);
                if (nextBlock != null && nextBlock.myItem.color == myItem.color)
                {
                    //   X
                    // X 0 X
                    Block checkBlock = GameManager.instance.GetBlock(row - 1, col - 1);
                    if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                    {
                        return true;
                    }

                    // X   X
                    //   0 
                    checkBlock = GameManager.instance.GetBlock(row + 1, col - 1);
                    if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                    {
                        return true;
                    }
                }
            }
        }

        // Checking For top Combinations

        neighborBlock = GameManager.instance.GetBlock(row - 1, col);
        if (neighborBlock != null)
        {
            if (neighborBlock.myItem.color == myItem.color)
            {
                // X
                // 0
                // X
                // X
                Block checkBlock = GameManager.instance.GetBlock(row - 3, col);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    return true;
                }
                // X 0
                //   X
                //   X
                checkBlock = GameManager.instance.GetBlock(row - 2, col - 1);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    print("row " + row + "Col " + col);
                    return true;
                }
                // 0 X
                // X
                // X
                checkBlock = GameManager.instance.GetBlock(row -2, col + 1);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    return true;
                }
            }
            else
            {
                Block nextBlock = GameManager.instance.GetBlock(row-2, col);
                if (nextBlock != null && nextBlock.myItem.color == myItem.color)
                {
                    // X
                    // 0 X
                    // X
                    Block checkBlock = GameManager.instance.GetBlock(row - 1, col + 1);
                    if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                    {
                        return true;
                    }

                    //   X
                    // X 0 
                    //   X
                    checkBlock = GameManager.instance.GetBlock(row - 1, col - 1);
                    if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                    {
                        return true;
                    }
                }
            }
        }

        // Check For Bottom Combinations

        neighborBlock = GameManager.instance.GetBlock(row + 1, col);
        if (neighborBlock != null)
        {
            if (neighborBlock.myItem.color == myItem.color)
            {
                // X
                // X
                // 0
                // X
                Block checkBlock = GameManager.instance.GetBlock(row + 3, col);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    return true;
                }
                // X 0
                //   X
                //   X
                checkBlock = GameManager.instance.GetBlock(row + 2, col - 1);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    return true;
                }
                // 0 X
                // X
                // X
                checkBlock = GameManager.instance.GetBlock(row + 2, col + 1);
                if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                {
                    return true;
                }
            }
            else
            {
                Block nextBlock = GameManager.instance.GetBlock(row + 2, col);
                if (nextBlock != null && nextBlock.myItem.color == myItem.color)
                {
                    // X
                    // 0 X
                    // X
                    Block checkBlock = GameManager.instance.GetBlock(row + 1, col + 1);
                    if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                    {
                        return true;
                    }

                    //   X
                    // X 0 
                    //   X
                    checkBlock = GameManager.instance.GetBlock(row + 1, col - 1);
                    if (checkBlock != null && checkBlock.myItem.color == myItem.color)
                    {
                        return true;
                    }
                }
            }
        }
        return haveMatches;
    }

    public bool RightMatchCombo2()
    {
        bool haveMatches = false;
        Block rightNeighbor = GetNeighborRight();
        if (rightNeighbor != null && rightNeighbor.myItem.color == myItem.color)
        {
           
        }
        return haveMatches;
    }
    public bool RightMatchCombo3()
    {
        bool haveMatches = false;
        Block rightNeighbor = GetNeighborRight();
        if (rightNeighbor != null && rightNeighbor.myItem.color == myItem.color)
        {
           
        }
        return haveMatches;
    }

    public Block GetNeighborLeft()
    {
        if (col == 0)
            return null;
        return GameManager.instance.gridBlocks[row, col - 1];
    }

    public Block GetNeighborRight()
    {
        if (col >= GameManager.instance.maxCols-1)
            return null;
        return GameManager.instance.gridBlocks[row, col + 1];
    }

    public Block GetNeighborTop()
    {
        if (row == 0)
            return null;
        return GameManager.instance.gridBlocks[row - 1,col];
    }

    public Block GetNeighborBottom()
    {
        if (row >= GameManager.instance.maxRows-1)
            return null;
        return GameManager.instance.gridBlocks[row + 1,col];
    }

}
