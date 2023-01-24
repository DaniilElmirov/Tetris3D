using System;
using UnityEngine;

public class TetraminoScript : MonoBehaviour
{
    public static event OnSwipeInput SwipeEvent;
    public delegate void OnSwipeInput(Vector2 direction);

    private Vector2 tapPosition;
    private Vector2 swipeDelta;

    private float deadZone = 80;

    private bool isSwiping;
    private bool isMobile;

    float fall = 0;
    public float fallSpeed = 1;

    void Start()
    {
        isMobile = Application.isMobilePlatform;

        SwipeEvent += OnSwipe;
    }

    private void OnSwipe(Vector2 direction)
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        if (!isMobile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isSwiping = true;
                tapPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ResetSwipe();
            }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    isSwiping = true;
                    tapPosition = Input.GetTouch(0).position;
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Canceled ||
                    Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    ResetSwipe();
                }
            }
        }

        CheckSwipe();

        MoveDown();
    }

    private void CheckSwipe()
    {
        swipeDelta = Vector2.zero;

        if (isSwiping)
        {
            if (!isMobile && Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - tapPosition;
            }
            else if (Input.touchCount > 0)
            {
                swipeDelta = Input.GetTouch(0).position - tapPosition;
            }
        }

        if (swipeDelta.magnitude > deadZone)
        {
            if (SwipeEvent != null)
            {
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    if (swipeDelta.x > 0)
                    {
                        transform.position += new Vector3(1, 0);

                        if (CheckIsValidPosition())
                        {
                            FindObjectOfType<GameScript>().UpdateBorder(this);
                        }
                        else
                        {
                            transform.position += new Vector3(-1, 0, 0);
                        }
                    }
                    else
                    {
                        transform.position += new Vector3(-1, 0, 0);

                        if (CheckIsValidPosition())
                        {
                            FindObjectOfType<GameScript>().UpdateBorder(this);
                        }
                        else
                        {
                            transform.position += new Vector3(1, 0, 0);
                        }
                    }
                }
                else
                {
                    if (swipeDelta.y > 0)
                    {
                        transform.Rotate(0, 0, 90);
                        
                        if (CheckIsValidPosition())
                        {
                            FindObjectOfType<GameScript>().UpdateBorder(this);
                        }
                        else
                        {
                            transform.Rotate(0, 0, -90);
                        }
                    }
                    else if (swipeDelta.y < 0)
                    {
                        transform.position += new Vector3(0, -1, 0);

                        if (CheckIsValidPosition())
                        {
                            FindObjectOfType<GameScript>().UpdateBorder(this);
                        }
                        else
                        {
                            transform.position += new Vector3(0, 1, 0);

                            FindObjectOfType<GameScript>().DeleteRow();

                            if (FindObjectOfType<GameScript>().CheckIsAboveBorder(this))
                            {
                                FindObjectOfType<GameScript>().GameOver();
                            }

                            FindObjectOfType<GameScript>().SpawnNextTetramino();

                            enabled = false;
                        }

                        fall = Time.time;
                    }
                }
            }
            ResetSwipe();
        }
    }

    private void ResetSwipe()
    {
        isSwiping = false;

        tapPosition = Vector2.zero;
        swipeDelta = Vector2.zero;
    }

    void MoveDown()
    {
        if(Time.time - fall >= fallSpeed)
        {
            transform.position += new Vector3(0, -1, 0);

            if (CheckIsValidPosition())
            {
                FindObjectOfType<GameScript>().UpdateBorder(this);
            }
            else
            {
                transform.position += new Vector3(0, 1, 0);

                FindObjectOfType<GameScript>().DeleteRow();

                if (FindObjectOfType<GameScript>().CheckIsAboveBorder(this))
                {
                    FindObjectOfType<GameScript>().GameOver();
                }

                FindObjectOfType<GameScript>().SpawnNextTetramino();

                enabled = false;
            }

            fall = Time.time;
        }
    }

    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<GameScript>().Round(mino.position);

            if (FindObjectOfType<GameScript>().CheckIsInsideBorder(pos) == false)
            {
                return false;
            }

            if (FindObjectOfType<GameScript>().GetTransformAtBorderPosition(pos) != null && FindObjectOfType<GameScript>().GetTransformAtBorderPosition(pos).parent != transform)
            {
                return false;
            }
        }

        return true;
    }
}
