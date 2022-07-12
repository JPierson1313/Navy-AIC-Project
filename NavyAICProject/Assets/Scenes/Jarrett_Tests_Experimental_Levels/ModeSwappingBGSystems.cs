using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSwappingBGSystems : MonoBehaviour
{
    [Tooltip("Sprite Renderer that will house the BackGround Sprite for Gamified Mode")]
    [SerializeField] private SpriteRenderer backGroundSprite;

    [Tooltip("Keyboard button used to swap between Realistic and Gamified Mode")]
    [SerializeField] private KeyCode swapModeKeyCode = KeyCode.G;

    private int modeNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        backGroundSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(swapModeKeyCode) && Time.timeScale != 0)
        {
            modeNumber++;
        }

        switch (modeNumber)
        {
            case 0:
                backGroundSprite.enabled = false;
                break;

            case 1:
                backGroundSprite.enabled = true;
                break;

            case 2:
                modeNumber = 0;
                break;
        }
    }
}
