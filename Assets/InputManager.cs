using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool upButtonUp, rightButtonUp, downButtonUp, leftButtonUp;
    public bool aButtonUp, bButtonUp, lButtonUp, rButtonUp;
    public bool startButtonUp, selectButtonUp;

    public bool upButtonDown, rightButtonDown, downButtonDown, leftButtonDown;
    public bool aButtonDown, bButtonDown, lButtonDown, rButtonDown;
    public bool startButtonDown, selectButtonDown;

    public bool upButtonGet, rightButtonGet, downButtonGet, leftButtonGet;
    public bool aButtonGet, bButtonGet, lButtonGet, rButtonGet;
    public bool startButtonGet, selectButtonGet;

    public KeyCode upCode = KeyCode.UpArrow;
    public KeyCode rightCode = KeyCode.RightArrow;
    public KeyCode downCode = KeyCode.DownArrow;
    public KeyCode leftCode = KeyCode.LeftArrow;

    public KeyCode aCode = KeyCode.Z;
    public KeyCode bCode = KeyCode.X;
    public KeyCode lCode = KeyCode.A;
    public KeyCode rCode = KeyCode.S;

    public KeyCode startCode = KeyCode.Return;
    public KeyCode selectCode = KeyCode.Space;

    public static InputManager Instance = null;

    void Awake()
    {
        if (Instance != null) // meaning there's already an instance
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Get();
        GetDown();
        GetUp();
    }

    void GetUp()
    {
        upButtonUp = Input.GetKeyUp(upCode);
        rightButtonUp = Input.GetKeyUp(rightCode);
        downButtonUp = Input.GetKeyUp(downCode);
        leftButtonUp = Input.GetKeyUp(leftCode);

        aButtonUp = Input.GetKeyUp(aCode);
        bButtonUp = Input.GetKeyUp(bCode);
        lButtonUp = Input.GetKeyUp(lCode);
        rButtonUp = Input.GetKeyUp(rCode);

        startButtonUp = Input.GetKeyUp(startCode);
        selectButtonUp = Input.GetKeyUp(selectCode);
    }

    void GetDown()
    {
        upButtonDown = Input.GetKeyDown(upCode);
        rightButtonDown = Input.GetKeyDown(rightCode);
        downButtonDown = Input.GetKeyDown(downCode);
        leftButtonDown = Input.GetKeyDown(leftCode);

        aButtonDown = Input.GetKeyDown(aCode);
        bButtonDown = Input.GetKeyDown(bCode);
        lButtonDown = Input.GetKeyDown(lCode);
        rButtonDown = Input.GetKeyDown(rCode);

        startButtonDown = Input.GetKeyDown(startCode);
        selectButtonDown = Input.GetKeyDown(selectCode);
    }

    void Get()
    {
        upButtonGet = Input.GetKey(upCode);
        rightButtonGet = Input.GetKey(rightCode);
        downButtonGet = Input.GetKey(downCode);
        leftButtonGet = Input.GetKey(leftCode);

        aButtonGet = Input.GetKey(aCode);
        bButtonGet = Input.GetKey(bCode);
        lButtonGet = Input.GetKey(lCode);
        rButtonGet = Input.GetKey(rCode);

        startButtonGet = Input.GetKey(startCode);
        selectButtonGet = Input.GetKey(selectCode);
    }
}