using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public GameObject backGroundSpace;
    public GameObject backGroundGround;
    [SerializeField] private float moveSpeed = 0.0f;

    private void Start()
    {
        if (backGroundSpace == null)
        {
            backGroundSpace = Resources.Load("BackGround Space") as GameObject;
        }
        if (backGroundGround == null)
        {
            backGroundGround = Resources.Load("BackGround Ground") as GameObject;
        }

        backGroundSpace.SetActive(true);

        if (SceneController.GetInstace.IsSinglePlayScene())
        {
            backGroundGround.SetActive(true);
        }
        else
        {
            backGroundGround.SetActive(false);
        }

        this.transform.position = new Vector2(0, 0);

        backGroundSpace.transform.position = new Vector2(22, 0);
        backGroundGround.transform.position = new Vector2(77, 0);
    }
    private void Update()
    {
        if (SceneController.GetInstace.IsSinglePlayScene())
        {
            SinglePlayMoveBackGround();
        }
        else
        {
            MultiPlayMoveBackGround();
        }
    }

    private void SinglePlayMoveBackGround()
    {
        backGroundSpace.transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        backGroundGround.transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        if(!GameManager.GetInstance.isBossStage)
        {
            if (backGroundGround.transform.position.x <= -6.0f)
            {
                backGroundGround.SetActive(true);
                backGroundGround.transform.position = new Vector2(15, 0);

                backGroundSpace.SetActive(false);
                backGroundSpace.transform.position = new Vector2(22, 0);
            }
        }
        else
        {
            if (backGroundGround.transform.position.x <= -10.0f)
            {
                backGroundSpace.SetActive(true);
                backGroundSpace.transform.position = new Vector2(22, 0);

                if(backGroundSpace.transform.position.x <= -18.0f)
                {
                    backGroundGround.SetActive(false);
                    backGroundGround.transform.position = new Vector2(77, 0);
                }
            }
        }
    }

    private void MultiPlayMoveBackGround()
    {
        if (backGroundGround.transform.position.x <= -10.0f)
        {
            backGroundSpace.SetActive(true);
            backGroundSpace.transform.position = new Vector2(22, 0);

            if (backGroundSpace.transform.position.x <= -18.0f)
            {
                backGroundGround.SetActive(false);
                backGroundGround.transform.position = new Vector2(77, 0);
            }
        }
    }
}
