using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    private static int gridWidth = 10;
    private static int gridHeight = 20;
    private const string path = "Prefabs/";
    private int score = 0;

    public Text scoreText;
    public Text pauseText;

    private Transform[,] grid = new Transform[gridWidth, gridHeight];

    private enum Figures
    {
        FIGURE_j,
        FIGURE_L,
        FIGURE_S,
        FIGURE_Z,
        FIGURE_T,
        FIGURE_LONG,
        FIGURE_SQUARE
    }

    // Use this for initialization
    void Start () {
        SpawnFigure();
        SetScoreText();
    }

	// Update is called once per frame
	void Update () {
		
	}

    public bool IsInsideGrid(Transform obj)
    {
        Vector3 position = RoundPosition(obj.position);
        return ((int)position.x >= 0 && (int)position.x < gridWidth && (int)position.y >= 0);
    }

    public Vector3 RoundPosition(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
    }

    public void SpawnFigure()
    {
        string[] names = System.Enum.GetNames(typeof(Figures));
        int index = Random.Range(0, names.Length);
        Instantiate(Resources.Load(path + names[index]), new Vector3(5, gridHeight, 0), Quaternion.identity);
    }

    public void UpdateGrid(Figure figure)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null && grid[x, y].parent == figure.transform)
                {
                    grid[x, y] = null;
                }
            }
        }
        foreach (Transform child in figure.transform)
        {
            Vector3 position = RoundPosition(child.position);
            if (position.y < gridHeight)
            {
                grid[(int)position.x, (int)position.y] = child;
            }
        }
    }

    public Transform GetObject(Vector3 position)
    {
        Vector3 pos = RoundPosition(position);
        if (position.y < gridHeight)
        {
            return grid[(int)pos.x, (int)pos.y];
        }
        return null;
    }

    private bool IsFullRow(int index)
    {
        for (int i = 0; i < gridWidth; i++)
        {
            if (grid[i, index] == null)
            {
                return false;
            }
        }
        return true;
    }

    private void DeleteObjectsInRow(int index)
    {
        for (int i = 0; i < gridWidth; i++)
        {
            Destroy(grid[i, index].gameObject);
            grid[i, index] = null;
        }
    }

    private void MoveRowDown(int index)
    {
        for (int i = 0; i < gridWidth; i++)
        {
            if (grid[i, index] != null)
            {
                grid[i, index - 1] = grid[i, index];
                grid[i, index - 1].position += new Vector3(0, -1, 0);
                grid[i, index] = null;
            }
        }
    }

    private void MoveAllRowsDonw(int index)
    {
        for (int i = index; i < gridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    public void ClearRows()
    {
        for (int i = 0; i < gridHeight; i++)
        {
            if (IsFullRow(i))
            {
                DeleteObjectsInRow(i);
                MoveAllRowsDonw(i + 1);
                i--;
                score++;
                DeleteFigures();
            }
        }
        SetScoreText();
    }

    private void DeleteFigures()
    {
        Figure[] figs = FindObjectsOfType<Figure>();
        foreach (Figure fig in figs)
        {
            if (fig.transform.childCount == 0)
            {
                Destroy(fig);
            }
        }
    }

    private void SetScoreText()
    {
        scoreText.text = score.ToString();
    }

    public bool IsAboveGrid(Figure figure)
    {
        foreach (Transform child in figure.transform)
        {
            Vector3 position = RoundPosition(child.position);
            if (position.y > gridHeight)
            {
                return true;
            }
        }
        return false;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public int GetScore()
    {
        return score;
    }

    public void SetPauseText(bool visible)
    {
        if (visible)
        {
            pauseText.text = "Pause";
        }
        else
        {
            pauseText.text = "";
        }
    }
}
