using UnityEngine;

public class Jiggler : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float power = 0.1f;
    [SerializeField] private bool useUnscaledTime;
    [SerializeField] private bool stagger;

    [System.Serializable]
    private class JigSettings {
        public bool enabled = true;
        public Vector3 amount;
        [Range(0, 120)] public float frequency = 10;
        [HideInInspector] public float time;
    }

    [Header("Jiggle Settings")]
    [SerializeField] private JigSettings positionJig = new JigSettings();
    [SerializeField] private JigSettings rotationJig = new JigSettings();
    [SerializeField] private JigSettings scaleJig = new JigSettings { amount = new Vector3(0.1f, -0.1f, 0.1f) };

    private Vector3 basePosition;
    private Quaternion baseRotation;
    private Vector3 baseScale;

    private void Start()
    {
        basePosition = transform.localPosition;
        baseRotation = transform.localRotation;
        baseScale = transform.localScale;

        if (stagger)
        {
            positionJig.time = Random.Range(0, Mathf.PI * 2);
            rotationJig.time = Random.Range(0, Mathf.PI * 2);
            scaleJig.time = Random.Range(0, Mathf.PI * 2);
        }

        RandomizeJigAmount(ref positionJig.amount);
    }

    private void Update()
    {
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (positionJig.enabled) JigPosition(dt);
        if (rotationJig.enabled) JigRotation(dt);
        if (scaleJig.enabled) JigScale(dt);
    }

    private void RandomizeJigAmount(ref Vector3 jigAmount)
    {
        jigAmount = new Vector3(
            Random.Range(-jigAmount.x / 2, jigAmount.x / 2),
            Random.Range(-jigAmount.y / 2, jigAmount.y / 2),
            Random.Range(-jigAmount.z / 2, jigAmount.z / 2)
        );
    }

    private void JigPosition(float deltaTime)
    {
        positionJig.time += deltaTime * positionJig.frequency;
        transform.localPosition = basePosition + positionJig.amount * (Mathf.Sin(positionJig.time) * power);
    }

    private void JigRotation(float deltaTime)
    {
        rotationJig.time += deltaTime * rotationJig.frequency;
        transform.localRotation = baseRotation * Quaternion.Euler(rotationJig.amount * (Mathf.Sin(rotationJig.time) * power));
    }

    private void JigScale(float deltaTime)
    {
        scaleJig.time += deltaTime * scaleJig.frequency;
        transform.localScale = baseScale + scaleJig.amount * (Mathf.Sin(scaleJig.time) * power);
    }
}
