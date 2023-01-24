using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
    public static int borderWidth = 11; 
    public static int borderHeight = 24;  
    public static int borderLimit = 21;  

    public static Transform[,] border = new Transform[borderWidth, borderHeight];

    public int scoreOneLine = 40;
    public int scoreTwoLine = 100;
    public int scoreThreeLine = 300;
    public int scoreFourLine = 1200;

    private int numberOfRowsThisTurn = 0;

    public Text hud_score;
    public Text hud_level;

    public static int currentScore;

    float fallSpeedGame;

    void Start()
    {
        currentScore = 0;
        SpawnNextTetramino();
    }

    public void UpdateUI()
    {
        hud_score.text = currentScore.ToString();

        if(fallSpeedGame == 0.5f)
        {
            hud_level.text = 1.ToString();
        }
        else if(fallSpeedGame == 0.3f)
        {
            hud_level.text = 2.ToString();
        }
        else if (fallSpeedGame == 0.1f)
        {
            hud_level.text = 3.ToString();
        }
    }

    public void UpdateScore()
    {
        if(numberOfRowsThisTurn > 0)
        {
            switch (numberOfRowsThisTurn)
            {
                case 1:
                    ClearedOneLine();
                    break;

                case 2:
                    ClearedTwoLines();
                    break;

                case 3:
                    ClearedThreeLines();
                    break;

                case 4:
                    ClearedFourLines();
                    break;
            }

            numberOfRowsThisTurn = 0;
        }
    }

    void Update()
    {
        if (currentScore < 1000)
        {
            FindObjectOfType<TetraminoScript>().fallSpeed = 1f;
            fallSpeedGame = 1f;
        }

        if (currentScore > 1000)
        {
            FindObjectOfType<TetraminoScript>().fallSpeed = 0.5f;
            fallSpeedGame = 0.5f;
        }

        if (currentScore > 5000)
        {
            FindObjectOfType<TetraminoScript>().fallSpeed = 0.3f;
            fallSpeedGame = 0.3f;
        }

        if (currentScore > 15000)
        {
            FindObjectOfType<TetraminoScript>().fallSpeed = 0.1f;
            fallSpeedGame = 0.1f;
        }

        UpdateUI();

        UpdateScore();
    }

    public void ClearedOneLine()
    {
        currentScore += (scoreOneLine * ((int)(1 / fallSpeedGame)));
    }

    public void ClearedTwoLines()
    {
        currentScore += (scoreTwoLine * ((int)(1 / fallSpeedGame)));
    }

    public void ClearedThreeLines()
    {
        currentScore += (scoreThreeLine * ((int)(1 / fallSpeedGame)));
    }

    public void ClearedFourLines()
    {
        currentScore += (scoreFourLine * ((int)(1 / fallSpeedGame)));
    }

    public bool CheckIsAboveBorder(TetraminoScript tetramino)
    {
        for(int x = 1; x < borderWidth; ++x)
        {
            foreach (Transform mino in tetramino.transform)
            {
                Vector3 pos = Round(mino.position);

                if(pos.y > borderLimit - 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsFullRowAt(int y) 
    { 
        for(int x = 1; x < borderWidth; ++x)
        {
            if (border[x, y] == null)
            {
                return false;
            }
        }

        numberOfRowsThisTurn++;
        
        return true;
    }

    public void DeleteMinoAt(int y)
    {
        for (int x = 1; x < borderWidth; ++x)
        {
            Destroy(border[x, y].gameObject);

            border[x, y] = null;
        }
    }

    public void MoveRowDown(int y)
    {
        for (int x = 1; x < borderWidth; ++x)
        {
            if (border[x, y] != null)
            {
                border[x, y - 1] = border[x, y];

                border[x, y] = null;

                border[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < borderHeight; ++i)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow()
    {
        for(int y = 1; y < borderHeight; ++y)
        {
            if(IsFullRowAt(y))
            {
                DeleteMinoAt(y);

                MoveAllRowsDown(y + 1);

                --y;
            }
        }
    }

    public void UpdateBorder(TetraminoScript tetramino)
    {
        for(int y = 1; y < borderHeight; ++y)
        {
            for (int x = 1; x < borderWidth; ++x)
            {
                if (border[x, y] != null)
                {
                    if (border[x, y].parent == tetramino.transform)
                    {
                        border[x, y] = null; 
                    }
                }
            }
        }

        foreach(Transform mino in tetramino.transform)
        {
            Vector2 pos = Round(mino.position);

            if(pos.y < borderHeight)
            {
                border[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtBorderPosition(Vector2 pos)
    {
        if(pos.y > borderHeight)
        {
            return null;
        }
        else
        {
            return border[(int)pos.x, (int)pos.y];
        }
    }

    public void SpawnNextTetramino()
    {
        GameObject nextTetramino = (GameObject)Instantiate(Resources.Load(GetRandomTetramino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
    }

    public bool CheckIsInsideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 1  && (int)pos.x < borderWidth && (int)pos.y >= 1);
    }

    public Vector2 Round (Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    string GetRandomTetramino()
    {
        int randomTetramino = Random.Range(0, 6);

        string randomTetraminoName = "Prefabs/Tetraminos/TetraminoT";

        switch(randomTetramino)
        {
            case 0:
                randomTetraminoName = "Prefabs/Tetraminos/TetraminoI";
                break;

            case 1:
                randomTetraminoName = "Prefabs/Tetraminos/TetraminoJ";
                break;

            case 2:
                randomTetraminoName = "Prefabs/Tetraminos/TetraminoL";
                break;

            case 3:
                randomTetraminoName = "Prefabs/Tetraminos/TetraminoO";
                break;

            case 4:
                randomTetraminoName = "Prefabs/Tetraminos/TetraminoT";
                break;

            case 5:
                randomTetraminoName = "Prefabs/Tetraminos/TetraminoZ";
                break;
        }

        return randomTetraminoName;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}
