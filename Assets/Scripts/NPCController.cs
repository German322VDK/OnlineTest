using Assets.Scripts.NetCode;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NPCController : NetworkBehaviour
{
    private TextMeshPro _tm;

    private Canvas _canvas;

    [SerializeField]
    private NetworkVariable<float> healthPoint = new NetworkVariable<float>(20f,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner);

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
        _tm = GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        _tm.text = $"{healthPoint.Value + 1}";
    }


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
        NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
    }
}
enum DialogType
{
    trigger,
    standart,
}

enum Direction
{
    left,
    right,
}