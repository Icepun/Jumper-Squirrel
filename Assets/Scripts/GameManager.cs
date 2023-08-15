using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Voodoo.Utils;

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
    public float objectGravityMultiplier;
    public float maxPrefabScale;

    [Header("---------- GAME OBJECTS ----------")]

    public GameObject obstaclePrefab;
    public GameObject character;
    public GameObject gameOver;
    public GameObject winLevel;
    public GameObject lastPlatform;
    public TextMeshProUGUI levelText;
    public AudioSource music;

    //LISTS//
    private List<GameObject> spawnedObstacles = new List<GameObject>();
    private List<Transform> spawnedObstaclesPos = new List<Transform>();
    private List<float> posDifferenceList = new List<float>();

    private int obstacleCount = 0;
    private bool isObstacleFalling = false;
    private bool isLevelOver = false;

    [Header("---------- CHARACTER PROPERTIES ----------")]

    public Transform characterTransform;
    public float jumpForce = 5f;
    public float jumpThreshold = 0.1f; // Minimum distance to consider the jump complete
    public Sprite idleSprite;
    public Sprite jumpSprite;


    [Header("---------- ENERGY ----------")]

    public float maxEnergy = 100f;
    private float currentEnergy;
    public Image energyProgress;

    private void Start()
    {
        if (startScene.isSoundOn == 0)
        {
            music.mute = true;
        }

        else if (startScene.isSoundOn == 1)
        {
            music.mute = false;
        }

        gameOver.SetActive(false);
        winLevel.SetActive(false);
        levelText.text = SceneManager.GetActiveScene().name;
        CalculateSpacingAndSpawnObstacles();
        currentEnergy = maxEnergy;
    }

    private void Update()
    {
        GameObject fallingObstacle = spawnedObstacles[obstacleCount];

        energyProgress.fillAmount = currentEnergy / maxEnergy;

        if (Input.GetMouseButtonDown(0))
        {
            if (startScene.isVibrationOn == 1)
            {
                Vibrations.Haptic(HapticTypes.MediumImpact);
            }

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
                            float firstYPos = Mathf.Abs(platformYPos + 4.105f - spawnedObstaclesPos[obstacleCount].position.y);
                            posDifferenceList.Add(firstYPos);
                        }

                        else
                        {
                            float YPos = Mathf.Abs(spawnedObstaclesPos[obstacleCount - 1].position.y - spawnedObstaclesPos[obstacleCount].position.y);
                            posDifferenceList.Add(YPos);
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

    float defaultObstacleWidth = 2;  // Varsayılan obje genişliği
    float totalObstaclesWidth = numberOfObstacles * defaultObstacleWidth;
    
    // Eksik veya fazla olan alanı hesaplama
    float widthDifference = totalWidth - totalObstaclesWidth;
    
    // Her bir objenin ölçeğinde yapılması gereken değişiklik
    float scaleAdjustment = widthDifference / numberOfObstacles;

    float currentX = startX;

    for (int i = 0; i < numberOfObstacles; i++)
    {
        Vector3 spawnPosition = new Vector3(currentX, spawnY, 0f);

        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        obstaclePositions.Add(spawnPosition);
        spawnedObstacles.Add(obstacle);

        // Objelerin x eksenindeki ölçeğini ayarlama
        Vector3 currentScale = obstacle.transform.localScale;
        currentScale.x += scaleAdjustment;

        // minimum olan değer döndürüyor maksimum genişliği ayarlamak için eklendi
        currentScale.x = Mathf.Min(currentScale.x, maxPrefabScale);

        obstacle.transform.localScale = currentScale;

        // spawnedObstaclesPos listesine yeni bir Transform ekle
        spawnedObstaclesPos.Add(obstacle.transform);

        // Rigidbody2D componentini al ve yerçekimi değerini ayarla
        Rigidbody2D rb = obstacle.GetComponent<Rigidbody2D>();
        if (rb != null)  // Eğer objede Rigidbody2D componenti varsa
        {
            rb.gravityScale *= objectGravityMultiplier;
        }

        currentX += spacingX;
    }

        // lastPlatform GameObject'inin transformunu listeye ekle
        spawnedObstacles.Add(lastPlatform);
        spawnedObstaclesPos.Add(lastPlatform.transform);
    }

    private void StartMoving()
    {
        if (startScene.isVibrationOn == 1)
        {
            Vibrations.Haptic(HapticTypes.Warning);
        }

        FireMovement.isFireAlive = false;
        StartCoroutine(MoveCharacterToFirstObstacle());
    }

    private IEnumerator MoveCharacterToFirstObstacle()
    {
        Rigidbody2D characterRigidbody = characterTransform.GetComponent<Rigidbody2D>();

        // Son platform için enerji hesaplaması yap
        float lastPlatformEnergy = 0;
        posDifferenceList.Add(lastPlatformEnergy);

        for (int i = 0; i < spawnedObstacles.Count; i++)
        {
            Vector3 start = characterTransform.position;
            Vector3 target = spawnedObstacles[i].transform.position;
            target.y += 2f; 

            float duration = 1f;  // Zıplama hareketinin süresi
            float elapsedTime = 0f;
            float height = 2f; // Zıplama hareketinin maksimum yüksekliği

            currentEnergy -= posDifferenceList[i] * 30f; // Enerji hesaplaması

            // Enerji bitip bitmediğini kontrol etme
            if (currentEnergy <= 0f)
            {
                gameOver.SetActive(true);

                if (startScene.isVibrationOn == 1)
                {
                    Vibrations.Haptic(HapticTypes.Failure);
                }

                isLevelOver = true;
                yield break; // Coroutine'i durdurma
            }

            SetCharacterSprite(jumpSprite); // Zıplama sprite'ını ayarla

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;  // Normalleştirilmiş süre
                float yOffset = height * Mathf.Sin(t * Mathf.PI);

                characterTransform.position = Vector3.Lerp(start, target, t) + new Vector3(0f, yOffset, 0f);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            characterTransform.position = target;

            SetCharacterSprite(idleSprite); // Standart sprite'a geri dön

            yield return new WaitForSeconds(1f);
        }

        winLevel.SetActive(true);

        if (startScene.isVibrationOn == 1)
        {
            Vibrations.Haptic(HapticTypes.Success);
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentLevel = int.Parse(currentSceneName.Replace("level", ""));
        PlayerPrefs.SetInt("LastCompletedLevel", currentLevel);
    }

    private void SetCharacterSprite(Sprite sprite)
    {
        characterTransform.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void CloseButton()
    {
        SceneManager.LoadScene(0);
    }
}