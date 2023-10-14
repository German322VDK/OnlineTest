using Assets.Scripts.NetCode;
using Unity.Netcode;
using UnityEngine;

public class AttackController : NetworkBehaviour
{
    [SerializeField]
    private LayerMask enemyLayerMask;

    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private float _rangeMelee = 0.5f;

    [SerializeField]
    private int _weakDamage = 5;

    [SerializeField]
    private int _strongDamage = 15;

    private float _timerWeak = 1f;
    private float _timerStrong;
    private float _timerShot;

    private int _weakAttackCount = 3;
    [SerializeField] private int _shotCount = 12;

    private bool _mayWeakAttakTimer;
    private bool _mayShotTimer;

    public Animator animator;


    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetMouseButtonDown(0) && _weakAttackCount > 0)
        {
            WeakHit();
            animator.SetTrigger("Melee_Attack");
        }


        if (_timerStrong <= 0)
        {
            if (Input.GetMouseButtonDown(1))
            {
                StrongHit();
                animator.SetTrigger("Hard_Attack");
            }

        }
        else
            _timerStrong -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E) && _shotCount > 0)
        {
            Shot();
            animator.SetTrigger("Range_Attack");
        }

    }

    private void FixedUpdate()
    {
        WeakTimer();

        ShotTimer();
    }

    #region Attack

    private void WeakHit()
    {
        var enemies = Physics2D.OverlapCircleAll(attackPoint.position, _rangeMelee, enemyLayerMask);

        if (enemies.Length > 0)
            for (int i = 0; i < enemies.Length; i++)
                enemies[i].GetComponent<EnemyController>().TakeDamage(_weakDamage);

        _weakAttackCount--;

        _mayWeakAttakTimer = _weakAttackCount == 0;

        if (_mayWeakAttakTimer)
        {
            _timerWeak = 1f;
        }

    }

    private void StrongHit()
    {
        var enemies = Physics2D.OverlapCircleAll(attackPoint.position, _rangeMelee, enemyLayerMask);

        if (enemies.Length > 0)
            for (int i = 0; i < enemies.Length; i++)
                enemies[i].GetComponent<EnemyController>().TakeDamage(_strongDamage);

        _timerStrong = 1f;

    }

    private void Shot()
    {
        SpawnBulletServerRpc(attackPoint.transform.position, transform.rotation);
        _shotCount--;

        _mayShotTimer = _shotCount == 0;

        if (_mayShotTimer)
            _timerShot = 2f;


    }

    [ServerRpc]
    private void SpawnBulletServerRpc( Vector3 position,
            Quaternion rotation,
            ServerRpcParams param = default)
    {
        //print($"TestingServerRpc is executed by {OwnerClientId} : {message}");

        NetWorkSpawner.SpawnNewNetworkObject(bullet, position, rotation);
    }

    #endregion

    #region TimeFixed

    private void WeakTimer()
    {
        if (_mayWeakAttakTimer)
        {
            if (_timerWeak <= 0)
                _weakAttackCount = 3;
            else
                _timerWeak -= Time.deltaTime;
        }
    }

    private void ShotTimer()
    {
        if (_mayShotTimer)
        {
            if (_timerShot <= 0)
                _shotCount = 12;
            else
                _timerShot -= Time.deltaTime;
        }
    }

    #endregion

}
