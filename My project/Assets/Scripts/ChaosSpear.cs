using UnityEngine;
using Unity.Netcode;

public class ChaosSpear : NetworkBehaviour
{
    public float speed = 40f;
    public float lifeTime = 5f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Invoke(nameof(DestroySelf), lifeTime);
        }
    }

    void Update()
    {
        if (!IsServer) return; 

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void DestroySelf()
    {
        GetComponent<NetworkObject>().Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; 

        if (other.CompareTag("Enemy"))
        {
            var netObj = other.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Despawn(); 
            }
        }

        GetComponent<NetworkObject>().Despawn();
    }
}