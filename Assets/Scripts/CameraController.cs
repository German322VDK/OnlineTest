using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float aboveDistance;
    [SerializeField] private float cameraSpeed;

    public Animator animator;

    // Êàìåðà
    public Camera cam;

    public bool scroll = true;
    private float maxZoom = 3;
    private float minZoom = 3.7f;

    private float sensitivity = 1;
    private float rememberAbove;
    private float rememberZoom;
    private float cameraZoom;

    private void Awake()
    {
        if (!player)
            player = FindObjectOfType<PlayerController>().transform;
    }

    void Start()
    {
        cameraZoom = cam.orthographicSize;
    }

    void Update()
    {
        if (scroll)
        {
            // Çóì, ïðîñòî àõóåííûé çóì 
            cameraZoom -= Input.mouseScrollDelta.y * sensitivity;
        }
        cameraZoom = Mathf.Clamp(cameraZoom, maxZoom, minZoom);
        float newSize = Mathf.MoveTowards(cam.orthographicSize, cameraZoom, cameraSpeed * Time.deltaTime);
        cam.orthographicSize = newSize;

        transform.position = new Vector3(player.position.x, player.position.y + aboveDistance, transform.position.z);
    }

    public void ChangeZoom(TriggerCam trigger)
    {
        switch (trigger)
        {
            case TriggerCam.NPCon:
                {
                    scroll = false;
                    rememberZoom = cameraZoom;
                    rememberAbove = aboveDistance;
                    aboveDistance = 1;
                    cameraZoom = 3;
                    animator.enabled = false;
                    break;
                }
            case TriggerCam.NPCoff:
                {
                    scroll = true;
                    cameraZoom = rememberZoom;
                    aboveDistance = rememberAbove;
                    animator.enabled = true;
                    break;
                }
        }
    }
}

public enum TriggerCam
{
    NPCon,
    NPCoff,
}