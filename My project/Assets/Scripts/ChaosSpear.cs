using UnityEngine;

public class ChaosSpear : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // destroy enemy
        }

        Destroy(gameObject); // destroy spear on impact
    }
}