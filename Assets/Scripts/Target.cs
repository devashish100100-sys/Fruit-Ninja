using UnityEngine;

public class Target : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject spawnManager;

    [Header("Visual & Audio")]
    [SerializeField] private GameObject slicedPrefab;
    [SerializeField] private ParticleSystem sliceParticles;
    [SerializeField] private AudioClip[] sliceSFXs;
    [SerializeField] private AudioClip missedSFX;

    [Header("Gameplay")]
    [SerializeField] private int points = 1;
    [SerializeField] private bool isLethal;
    public bool IsLethal => isLethal;

    [SerializeField] private float destroyPosY = -6f;

    public Rigidbody MyRigidbody { get; private set; }

    void Awake()
    {
        MyRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        gameManager = GameManager.Instance;
        spawnManager = GameObject.Find("Spawn Managers");
    }

    void Update()
    {
        if (transform.position.y < destroyPosY)
        {
            gameObject.SetActive(false);
            if (!isLethal)
            {
                AudioSource.PlayClipAtPoint(missedSFX, transform.position);
                gameManager.RemoveLife();
            }
        }
    }

    public void OnTargetHit(Vector3 hitPosition)
    {
        if (gameObject == null) return;

        if (!isLethal)
        {
            GameObject sliced = Instantiate(slicedPrefab, transform.position, transform.rotation, spawnManager.transform);

            foreach (Rigidbody s in sliced.GetComponentsInChildren<Rigidbody>())
            {
                s.linearVelocity = MyRigidbody.linearVelocity;
                s.angularVelocity = MyRigidbody.angularVelocity;
            }

            var particles = Instantiate(sliceParticles, transform.position,
                Quaternion.LookRotation(-(hitPosition - transform.position).normalized));
            Destroy(particles.gameObject, 2f);
        }
        else
        {
            var particles = Instantiate(sliceParticles, transform.position, Quaternion.Euler(Vector3.up));
            Destroy(particles.gameObject, 5f);
        }

        AudioSource.PlayClipAtPoint(RandomSound(), transform.position);
        gameObject.SetActive(false);

        if (isLethal)
            gameManager.RemoveLife();
        else
            gameManager.AddScore(points);
    }

    private AudioClip RandomSound()
    {
        return sliceSFXs.Length > 0
            ? sliceSFXs[Random.Range(0, sliceSFXs.Length)]
            : null;
    }

    public void ResetForces()
    {
        MyRigidbody.linearVelocity = Vector3.zero;
        MyRigidbody.angularVelocity = Vector3.zero;
    }

    public void AddForce(Vector3 force)
    {
        MyRigidbody.AddForce(force, ForceMode.Impulse);
    }

    public void AddRandomForce(float minVerticalForce, float maxVerticalForce, float maxHorizontalForce)
    {
        AddRandomVerticalForce(minVerticalForce, maxVerticalForce);
        AddRandomHorizontalForce(maxHorizontalForce);
    }

    public void AddRandomVerticalForce(float minVerticalForce, float maxVerticalForce)
    {
        Vector3 randomForce = Vector3.up * Random.Range(minVerticalForce, maxVerticalForce);
        MyRigidbody.AddForce(randomForce, ForceMode.Impulse);
    }

    public void AddRandomHorizontalForce(float maxHorizontalForce)
    {
        Vector3 randomForce = Vector3.right * Random.Range(-maxHorizontalForce, maxHorizontalForce);
        MyRigidbody.AddForce(randomForce, ForceMode.Impulse);
    }

    public void AddRandomTorque(float maxTorque)
    {
        Vector3 randomTorque = new Vector3(
            Random.Range(-maxTorque, maxTorque),
            Random.Range(-maxTorque, maxTorque),
            Random.Range(-maxTorque, maxTorque)
        );
        MyRigidbody.AddTorque(randomTorque, ForceMode.Impulse);
    }
}
