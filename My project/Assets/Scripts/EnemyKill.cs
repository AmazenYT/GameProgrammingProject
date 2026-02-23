using UnityEngine;

public class EnemyKill : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public AudioClip enemySound;
    public AudioSource enemySource;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy") && playerMovement.rb.linearVelocity.y <0)
        {
            Destroy(collision.gameObject);
        }
    }
}