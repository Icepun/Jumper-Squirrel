using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMovement : MonoBehaviour
{
    [SerializeField]
    private List<Transform> waypoints = new List<Transform>();

    [SerializeField]
    private float speed = 1.0f;

    public GameObject gameOver;

    private int currentWaypointIndex = 0;

    // Başka scriptlerden erişilebilir bir boolean değişkeni
    public static bool isFireAlive = true;

    void Update()
    {
        if (!isFireAlive)
        {
            gameObject.SetActive(false);
            return;
        }

        // Eğer hiç waypoint yoksa, hareket etme
        if (waypoints.Count == 0)
            return;

        // Hedef waypoint'e olan mesafeyi kontrol et
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);

        // Eğer hedef waypoint'e çok yakınsak
        if (distanceToWaypoint < 0.1f)
        {
            // Sonraki waypoint'e geç
            currentWaypointIndex++;

            // Eğer son waypoint'e ulaştıysak, ilk waypoint'e dön
            if (currentWaypointIndex >= waypoints.Count)
                currentWaypointIndex = 0;
        }
        else
        {
            // Hedef waypoint'e doğru hareket et
            Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    // Bu fonksiyon, objenin başka bir objeye temas ettiğinde otomatik olarak çağrılır.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameOver.SetActive(true);
        gameObject.SetActive(false);
        // Temas edilen objeyi yok eder.
        Destroy(collision.gameObject);
    }
}
