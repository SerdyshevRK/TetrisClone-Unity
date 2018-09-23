using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figure : MonoBehaviour {

    private float timer = 1;
    private float gameSpeed = 1;
    private int score;
    private bool IsPaused = false;
    
    public bool allowRotation = true;
    public bool limitRotation = false;

    Dictionary<string, Vector3> direction = new Dictionary<string, Vector3>();
    Dictionary<string, Vector3> rotation = new Dictionary<string, Vector3>();

    // Use this for initialization
    void Start () {
        direction.Add("left", new Vector3(-1, 0, 0));
        direction.Add("right", new Vector3(1, 0, 0));
        direction.Add("down", new Vector3(0, -1, 0));
        direction.Add("up", new Vector3(0, 1, 0));
        rotation.Add("forward", new Vector3(0, 0, 90));
        rotation.Add("backward", new Vector3(0, 0, -90));
        if (!IsPaused)
        {
            FindObjectOfType<Game>().SetPauseText(false);
        }
        CheckScore();
    }
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        CheckPauseButton();
        if (!IsPaused)
        {
            CheckUserInput();
        }
	}

    private void CheckPauseButton()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsPaused = !IsPaused;
            if (IsPaused)
            {
                Time.timeScale = 0;
                FindObjectOfType<Game>().SetPauseText(true);
            }
            else
            {
                Time.timeScale = 1;
                FindObjectOfType<Game>().SetPauseText(false);
            }
        }
    }

    private void CheckScore()
    {
        score = FindObjectOfType<Game>().GetScore();
        gameSpeed = (float)((score / 10) * 0.1);
    }

    private void CheckUserInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveFigure("left");
            if (!IsValidPosition())
            {
                MoveFigure("right");
            }
            else
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveFigure("right");
            if (!IsValidPosition())
            {
                MoveFigure("left");
            }
            else
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
        }
        if (Input.GetKey(KeyCode.DownArrow) || timer < 0)
        {
            timer = 1 - gameSpeed;
            MoveFigure("down");
            if (!IsValidPosition())
            {
                MoveFigure("up");
                enabled = false;
                if (FindObjectOfType<Game>().IsAboveGrid(this))
                {
                    FindObjectOfType<Game>().GameOver();
                }
                FindObjectOfType<Game>().ClearRows();
                FindObjectOfType<Game>().SpawnFigure();
            }
            else
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (allowRotation)
            {
                if (limitRotation && transform.rotation.eulerAngles.z > 0)
                {
                    RotateFigure("backward");
                    if (!IsValidPosition())
                    {
                        RotateFigure("forward");
                    }
                    else
                    {
                        FindObjectOfType<Game>().UpdateGrid(this);
                    }
                    return;
                }
                RotateFigure("forward");
                if (!IsValidPosition())
                {
                    RotateFigure("backward");
                }
                else
                {
                    FindObjectOfType<Game>().UpdateGrid(this);
                }
            }            
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void RotateFigure(string rot)
    {
        transform.Rotate(rotation[rot]);
    }

    private void MoveFigure(string dir)
    {
        transform.position += direction[dir];
    }

    private bool IsValidPosition()
    {
        foreach (Transform child in transform)
        {
            if (!FindObjectOfType<Game>().IsInsideGrid(child))
            {
                return false;
            }
            if (FindObjectOfType<Game>().GetObject(child.position) != null && 
                FindObjectOfType<Game>().GetObject(child.position).parent != transform)
            {
                return false;
            }
        }
        return true;
    }
}
