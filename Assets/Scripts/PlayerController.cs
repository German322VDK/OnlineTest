using Assets.Scripts.NetCode;
using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private float speed = 3.5f;
    private float jumpForce = 25f;
    private float pushForce = 20f;
    private int jumpCount = 2;

    [SerializeField]
    private NetworkVariable<float> healthPoint = new NetworkVariable<float>(50f,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField]
    private float mercyPoint = 3f;

    [SerializeField]
    private int godPoint = 5;

    [SerializeField]
    private Image _barHP;

    [SerializeField]
    private Image _barMP;

    private Rigidbody2D _rb;
    private SpriteRenderer _spite;
    private TextMeshPro _tm;
    private bool _isGrounded;
    private float _groundDist = 0.1f;
    private bool _mayPush = true;
    private float _pushTime = 1f;

    [Header("Player Animation Settings")]
    public Animator animator;

    public bool isMoving = true;


    public float footstep_timer;
    private float ftimer = 0.0f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        healthPoint.OnValueChanged += (float prevVal, float newVal) =>
        {
            _tm.text = $"{healthPoint.Value}";
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spite = GetComponentInChildren<SpriteRenderer>();

        _tm = GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;

        //UpdateHPBar();
        //UpdateMPBar();

        if (isMoving)
        {
            if (Input.GetButton($"{Axis.Horizontal}"))
            {
                Run();
                if (ftimer > footstep_timer)
                {
                    ftimer = 0.0f;

                }

                ftimer += Time.deltaTime;
            }

            if (jumpCount > 1 && Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                MPHeal();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Push();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
            }
        }
        //_tm.text = $"{healthPoint.Value}";

        animator.SetFloat("HorizontalMove", Mathf.Abs(Input.GetAxisRaw("Horizontal")));
    }

    public void ZeroForce()
    {
        _rb.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        _isGrounded = CheckGround();

        if (_isGrounded)
        {
            jumpCount = 2;
            animator.SetBool("Jumping", false);
        }
        else
            animator.SetBool("Jumping", true);

    }

    #region Transform Pos

    private void Run()
    {
        var getAxis = Input.GetAxis($"{Axis.Horizontal}");

        var rot = transform.rotation;

        Vector3 dir;

        if (getAxis < 0)
        {
            rot.y = 180;
            dir = -1 * transform.right * getAxis;
        }
        else
        {
            rot.y = 0;
            dir = transform.right * getAxis;
        }


        transform.rotation = rot;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

    }

    private void Jump()
    {
        //reset the forces to the jumps are the same
        ZeroForce();
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpCount--;
    }

    private bool CheckGround()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, _groundDist);

        return colliders.Length > 1;
    }

    private void Push()
    {
        if (_mayPush)
        {
            _mayPush = false;
            int mod = transform.rotation.w == 1 ? 1 : -1;
            _rb.AddForce(mod * Vector2.right * pushForce, ForceMode2D.Impulse);


            Invoke($"{nameof(PushWait)}", _pushTime);
            animator.SetTrigger("Dash");
        }
    }

    private void PushWait() =>
        _mayPush = true;

    #endregion

    #region Bar

    //private void UpdateHPBar()
    //{
    //    _barHP.fillAmount = healthPoint / 100;
    //}

    //private void UpdateMPBar()
    //{
    //    _barMP.fillAmount = mercyPoint / 3;
    //}

    #endregion

    #region Heal


    private void MPHeal()
    {
        var curTealthPoint = healthPoint.Value;
        if (mercyPoint > 0)
        {
            if (curTealthPoint < 100)
            {
                if (curTealthPoint < 50)
                    healthPoint.Value += 50;
                else
                    healthPoint.Value = 100;

                mercyPoint--;
                animator.SetTrigger("Heal_Cast");
            }
        }
    }

    private void HealAndDestroyOfHO(GameObject gameObject)
    {
        if (healthPoint.Value < 100)
        {
            if (healthPoint.Value < 80)
                healthPoint.Value += 20;
            else
                healthPoint.Value = 100;

            //Die
            Destroy(gameObject);
        }
    }

    #endregion

    #region TakeDamage

    public void TakeDamage(int damage)
    {
        healthPoint.Value -= damage;
        if (healthPoint.Value <= 0)
        {
            SpawnBulletServerRpc();
        }
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(ServerRpcParams param = default)
    {
        //print($"TestingServerRpc is executed by {OwnerClientId} : {message}");
        
        NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == $"{Tags.HealObj}")
            HealAndDestroyOfHO(collision.gameObject);
    }

}



enum Axis
{
    Horizontal,
    Vertical,
}


