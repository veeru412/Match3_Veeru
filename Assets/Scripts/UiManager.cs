
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject infoMenu;
    public InputField rowsTxt, columTxt, colorsTxt;

    public void Play()
    {
        int rows = int.Parse(rowsTxt.text);
        int colums = int.Parse(columTxt.text);
        int maxColors = int.Parse(colorsTxt.text);
        if(rows >= 3 && colums >= 3 && maxColors >= 3 && maxColors <= 6)
        {
            GameManager.instance.maxRows = rows;
            GameManager.instance.maxCols = colums;
            GameManager.instance.maxColors = maxColors;
            GameManager.instance.gameState = GameState.PrepareGame;
        }
        else
        {
            infoMenu.SetActive(true);
        }
    }

    public void Home()
    {
        GameManager.instance.gameState = GameState.Menu;
    }
}
