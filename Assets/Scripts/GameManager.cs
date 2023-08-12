using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("---------- LEVEL VALUES ----------")]

    public int numberOfObstacles;
    public float spawnY;
    public float startX;
    public float endX;
    public List<Vector3> obstaclePositions = new List<Vector3>();
    public float platformYPos;
    public float maxObstacleYPos;

    [Header("---------- GAME OBJECTS ----------")]

    public GameObject obstaclePrefab;
    public GameObject character;

    //LISTS//
    private List<GameObject> spawnedObstacles = new List<GameObject>();
    private List<Transform> spawnedObstaclesPos = new List<Transform>();
    private List<float> posDifferenceList = new List<float>();

    private bool hasJumped = false;

    private int obstacleCount = 0;
    private bool isObstacleFalling = false;
    private bool isLevelOver = false;

    [Header("---------- CHARACTER PROPERTIES ----------")]

    public Transform characterTransform;
    public float jumpForce = 5f;
    public float jumpThreshold = 0.1f; // Minimum distance to consider the jump complete

    [Header("---------- ENERGY ----------")]

    public float maxEnergy = 100f;
    private float currentEnergy;
    public Image energyProgress;

    private void Start()
    {
        CalculateSpacingAndSpawnObstacles();
        currentEnergy = maxEnergy;
    }

    private void Update()
    {
        GameObject fallingObstacle = spawnedObstacles[obstacleCount];

        energyProgress.fillAmount = currentEnergy / maxEnergy;

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
                    if (spawnedObstaclesPos[obstacleCount].position.y > maxObstacleYPos)
                    {
                        if (obstacleCount == 0)
                        {
                            float firstYPos = Mathf.Abs(platformYPos + 0.71f - spawnedObstaclesPos[obstacleCount].position.y);
                            posDifferenceList.Add(firstYPos);
                            Debug.Log("ilk pos eklendi : " + firstYPos);
                        }

                        else
                        {
                            float YPos = Mathf.Abs(spawnedObstaclesPos[obstacleCount - 1].position.y - spawnedObstaclesPos[obstacleCount].position.y);
                            posDifferenceList.Add(YPos);
                            Debug.Log("pos eklendi : " + YPos);
                        }

                        fallingObstacle.GetComponent<Rigidbody2D>().isKinematic = true;
                        fallingObstacle.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                        isObstacleFalling = false;
                        spawnedObstaclesPos[obstacleCount] = spawnedObstacles[obstacleCount].transform;
                        obstacleCount++;

                        if (obstacleCount == numberOfObstacles)
                        {
                            obstacleCount = 0;
                            isLevelOver = true;
                            StartMoving();
                        }
                    }

                    else
                    {
                        isLevelOver = true;
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
            spawnedObstacles.Add(obstacle);

            // spawnedObstaclesPos listesine yeni bir Transform ekle
            spawnedObstaclesPos.Add(obstacle.transform);

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

            currentEnergy -= posDifferenceList[i] * 30f;

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

            if (currentEnergy < 0f)
            {
                isLevelOver = true;
                Debug.Log("bitti");
            }
        }

        // All obstacles reached
        hasJumped = true;
    }
}