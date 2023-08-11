using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public int numberOfObstacles;
    public float spawnY;
    public float startX;
    public float endX;

    public GameObject character;

    private List<GameObject> spawnedObstacles = new List<GameObject>();
    public List<Vector3> obstaclePositions = new List<Vector3>();

    private bool hasJumped = false;

    private int obstacleCount = 0;
    private bool isObstacleFalling = false;
    private bool isLevelOver = false;

    // Character properties
    public Transform characterTransform;
    public float jumpForce = 5f;
    public float jumpThreshold = 0.1f; // Minimum distance to consider the jump complete

    private void Start()
    {
        CalculateSpacingAndSpawnObstacles();
    }

    private void Update()
    {
        GameObject fallingObstacle = spawnedObstacles[obstacleCount];

        if (Input.GetMouseButtonDown(0))
        {
            if (!isLevelOver)
            {
                if (!isObstacleFalling)
                {
                    fallingObstacle.GetComponent<Rigidbody2D>().isKinematic = false;
                    isObstacleFalling = true;
                }
                else if (isObstacleFalling)
                {
                    fallingObstacle.GetComponent<Rigidbody2D>().isKinematic = true;
                    fallingObstacle.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                    isObstacleFalling = false;
                    obstacleCount++;

                    if (obstacleCount == numberOfObstacles)
                    {
                        obstacleCount = 0;
                        isLevelOver = true;
                        StartMoving();
                    }
                }
            }
        }
    }

    void CalculateSpacingAndSpawnObstacles()
    {
        float totalWidth = endX - startX;
        float spacingX = totalWidth / (numberOfObstacles - 1);

        float currentX = startX;

        for (int i = 0; i < numberOfObstacles; i++)
        {
            Vector3 spawnPosition = new Vector3(currentX, spawnY, 0f);

            GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
            obstaclePositions.Add(spawnPosition);
            spawnedObstacles.Add(obstacle); // Add spawned obstacle to the list

            currentX += spacingX;
        }
    }

    private void StartMoving()
    {
        StartCoroutine(MoveCharacterToFirstObstacle());
    }

    private IEnumerator MoveCharacterToFirstObstacle()
    {
        Rigidbody2D characterRigidbody = characterTransform.GetComponent<Rigidbody2D>();

        for (int i = 0; i < obstaclePositions.Count; i++)
        {
            Vector3 targetPosition = obstaclePositions[i];
            Vector3 initialPosition = characterTransform.position;

            float jumpDistance = Vector2.Distance(initialPosition, targetPosition);
            float normalizedJumpForce = jumpForce * Mathf.Clamp01(1f / jumpDistance);

            Vector3 moveDirection = (targetPosition - initialPosition).normalized;

            while (Vector2.Distance(characterTransform.position, targetPosition) > jumpThreshold)
            {
                characterRigidbody.velocity = moveDirection * normalizedJumpForce;

                yield return null;
            }

            // Stop character's movement
            characterRigidbody.velocity = Vector2.zero;

            // Wait for 1 second
            yield return new WaitForSeconds(1f);
        }

        // All obstacles reached
        hasJumped = true;
    }
}
