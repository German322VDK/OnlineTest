using UnityEngine;

public class EndLevelPoint : MonoBehaviour
{
    [SerializeField]
    private int idNextLevel;

    private Conductor conductor;

    // Start is called before the first frame update
    void Start()
    {
        conductor = new Conductor();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == $"{Tags.Player}")
            conductor.showScene(idNextLevel);
    }
}
