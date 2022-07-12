using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSwapper_MasterSystems : MonoBehaviour
{

    public int modeNumber = 0;
    [Tooltip("Keyboard button used to swap between Realistic and Gamified Mode")]
    [SerializeField] private KeyCode swapModeKeyCode = KeyCode.G;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Pressing the SwapMode Key will add 1 to the Mode Number variable for the swapping process.
        if (Input.GetKeyDown(swapModeKeyCode))
        {
            modeNumber++;
        }

        switch (modeNumber)
        {
            //Case 2 is used to reset the Mode Number to 0 and switch to Realistic Mode.
            case 2:
                modeNumber = 0;
                break;
        }
    }
}
