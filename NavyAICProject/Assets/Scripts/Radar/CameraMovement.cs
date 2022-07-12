using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class CameraMovement : MonoBehaviour
{
    [Tooltip("Camera that is rendering the radar screen")]
    private Camera radarCamera;
    [Tooltip("Clipping mask for radar")]
    private Transform radarMask;

    [Header("Zoom")]
    [Tooltip("Cooldown between zooms")]
    [SerializeField]
    private float zoomCooldownMax = 0.1f;
    [SerializeField]
    private float zoomCooldown = 0.0f;

    [Tooltip("orthographicSize options for Camera this is attatched to")]
    private float[] zooms = { 1.5f, 5.0f, 7.5f, 20f };
    [Tooltip("Current index of zooms[]")]
    private int incrIndex = 1;

    [Header("Movement")]
    [Tooltip("Speed at which the camera can move")]
    [SerializeField]
    private float moveSpeed = 1.0f;
    [Tooltip("Max amount of time after a mouse click to check for a double click")]
    [SerializeField]
    private float DoubleClickDelaymax = 0.2f;
    private float DoubleClickDelay = 0.0f;
    [Tooltip("Which layers to check for entities to track")]
    [SerializeField]
    LayerMask mask;
    [Tooltip("Max range the camera can move")]
    [SerializeField]
    private Vector2 cameraClamp;

    [Header("Array of ID Tags")]
    [Tooltip("An array for storing all of the ID Tags found in the scenario")]
    [SerializeField] private GameObject[] idTags;

    [Header("Zoom Slider")]
    [Tooltip("Slider to show the distance of the zoom in terms of nautical miles")]
    [SerializeField] private Slider zoomSlider;

    [Tooltip("Text used to show the current zoom in distance of the screen")]
    [SerializeField] private Text zoomInDistanceText;

    //Used to get the State Machine for the different Tile Maps for the Grid Background
    private TileMap_StateMachine tileMapStateMachine;

    private void Awake()
    {
        radarCamera = GameObject.Find("Radar Camera").GetComponent<Camera>();
        radarMask = transform.Find("Radar Mask");
        idTags = GameObject.FindGameObjectsWithTag("IDTag");

        //Setting tag localScale to match 5.0f zoom
        foreach (GameObject tag in idTags)
        {
            tag.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        tileMapStateMachine = FindObjectOfType<TileMap_StateMachine>();
    }

    private void Update()
    {
        zoomInDistanceText.text = $"{zoomSlider.value} " + "NM";
        // Zoom
        if (zoomCooldown == 0 /*&& Time.timeScale >= 0.1f*/)
        {
            if (Input.mouseScrollDelta.y > 0 || Input.GetKeyDown(KeyCode.Q))
            { Zoom(true); }
            if (Input.mouseScrollDelta.y < 0 || Input.GetKeyDown(KeyCode.E))
            { Zoom(false); }
        }
        zoomCooldown = Mathf.Max(0.0f, zoomCooldown - Time.unscaledDeltaTime);

        // Move
        Vector2 moveDir = Vector2.zero;
        //if (Time.timeScale >= 0.1f)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                BreakFocus();
                moveDir.y = 1.0f;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                BreakFocus();
                moveDir.y = -1.0f;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                BreakFocus();
                moveDir.x = 1.0f;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                BreakFocus();
                moveDir.x = -1.0f;
            }
        }
        MoveCamera(moveDir);

        // Follow Target
        if (Input.GetMouseButtonDown(0))
        {
            BreakFocus();
            if (DoubleClickDelay > 0)
            {
                RaycastHit2D hit2D = Physics2D.Raycast(radarCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0.0f, mask);
                if (hit2D)
                {
                    //transform.parent = hit2D.collider.transform;
                    StartCoroutine(FocusOnTarget(hit2D.collider.transform));
                }
            }
            else
            {
                DoubleClickDelay = DoubleClickDelaymax;
            }
        }
        DoubleClickDelay = Mathf.Max(0.0f, DoubleClickDelay - Time.deltaTime);
    }

    private void Zoom(bool zoomingIn)
    {
        float priorZoom = zooms[incrIndex];
        incrIndex += zoomingIn ? -1 : 1;
        incrIndex = Mathf.Clamp(incrIndex, 0, zooms.Length - 1);
        GetComponent<Camera>().orthographicSize = zooms[incrIndex];
        zoomCooldown = zoomCooldownMax;

        // Adjust mask size
        radarMask.localScale *= zooms[incrIndex] / priorZoom;

        //Switch function used to change the sizes of the ID Tags in the scenario based on the Zoom distance of the camera
        foreach (GameObject tag in idTags)
        {
            if (tag != null)
            {
                switch (zooms[incrIndex])
                {
                    case 20f:
                        tag.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        tag.transform.parent.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                        tag.transform.parent.GetComponent<LineRenderer>().startWidth = 0.038f;
                        zoomSlider.value = 200;
                        tileMapStateMachine.ChangingTileMapState(TileMap_StateMachine.TileMapState.TileMap200NMi);
                        break;
                    case 7.5f:
                        tag.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        tag.transform.parent.localScale = new Vector3(0.45f, 0.45f, 0.45f);
                        tag.transform.parent.GetComponent<LineRenderer>().startWidth = 0.038f;
                        zoomSlider.value = 100;
                        tileMapStateMachine.ChangingTileMapState(TileMap_StateMachine.TileMapState.TileMap100NMi);
                        break;
                    case 5.0f:
                        tag.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        tag.transform.parent.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                        tag.transform.parent.GetComponent<LineRenderer>().startWidth = 0.038f;
                        zoomSlider.value = 50;
                        tileMapStateMachine.ChangingTileMapState(TileMap_StateMachine.TileMapState.TileMap50NMi);
                        break;
                    case 1.5f:
                        tag.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        tag.transform.parent.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                        tag.transform.parent.GetComponent<LineRenderer>().startWidth = 0.015f;
                        zoomSlider.value = 20;
                        tileMapStateMachine.ChangingTileMapState(TileMap_StateMachine.TileMapState.TileMap20NMi);
                        break;
                }
            }
        }
    }

    private void MoveCamera(Vector2 moveDir)
    {
        Vector3 position = transform.position + (Vector3)moveDir * Time.unscaledDeltaTime * moveSpeed * (incrIndex * incrIndex + 0.5f);
        position.x = Mathf.Clamp(position.x , - cameraClamp.x, cameraClamp.x);
        position.y = Mathf.Clamp(position.y , - cameraClamp.y, cameraClamp.y);
        transform.position = position;
    }

    IEnumerator FocusOnTarget(Transform target)
    {
        PositionConstraint pc = GetComponent<PositionConstraint>();
        ConstraintSource constraintSource = pc.GetSource(0);
        constraintSource.sourceTransform = target;
        pc.SetSource(0, constraintSource);
        pc.constraintActive = true;

        //while (Vector2.Distance(transform.position, target.position) > 0.05f)
        //{
        //    Vector2 pos2D = Vector2.MoveTowards(transform.position, target.position, 8.0f * Time.deltaTime);
        //    transform.position = new Vector3(pos2D.x, pos2D.y, transform.position.z);
        //    yield return new WaitForEndOfFrame();
        //}

        yield return null;
    }

    private void BreakFocus()
    {
        PositionConstraint pc = GetComponent<PositionConstraint>();
        pc.constraintActive = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = Vector3.zero;
        Gizmos.DrawLine(new Vector3(position.x - cameraClamp.x, position.y + cameraClamp.y), new Vector3(position.x + cameraClamp.x, position.y + cameraClamp.y));
        Gizmos.DrawLine(new Vector3(position.x + cameraClamp.x, position.y + cameraClamp.y), new Vector3(position.x + cameraClamp.x, position.y - cameraClamp.y));
        Gizmos.DrawLine(new Vector3(position.x + cameraClamp.x, position.y - cameraClamp.y), new Vector3(position.x - cameraClamp.x, position.y - cameraClamp.y));
        Gizmos.DrawLine(new Vector3(position.x - cameraClamp.x, position.y - cameraClamp.y), new Vector3(position.x - cameraClamp.x, position.y + cameraClamp.y));
    }
}
