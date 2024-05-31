using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace IAV24.Final
{
    public class LevelManager : MonoBehaviour
    {
        // Ciclo de dia
        private int days;
        [SerializeField]
        private float initTime = 6.0f;

        private float prevTime = 0.0f,
                      hoursPassed = 0.0f,
                      hour = 0.0f;

        [SerializeField]
        private float speedFactor = 2.0f;

        [SerializeField]
        private Gradient lightGradient;

        private Light lightSource;
        private Transform lightTr;

        [SerializeField]
        private TextMeshProUGUI dayText;


        // Enemigos
        private Transform[] spawnpoints = null;

        [SerializeField]
        private GameObject[] enemies = null;

        private Transform enemiesGroup;

        private float spawnTimer;

        [SerializeField]
        private float spawnStartHour = 20.0f;
        [SerializeField]
        private float spawnEndHour = 5.0f;


        [SerializeField]
        private float minSpawnDelay = 2.0f;
        [SerializeField]
        private float maxSpawnDelay = 5.0f;
        [SerializeField]
        private int maxEnemies = 10;

        private float spawnCooldown;

        [SerializeField]
        private float spawnOffset = 1.0f;

        [SerializeField]
        private TextMeshProUGUI _interactionInfo;
        public string interactionInfoText
        {
            private get { return _interactionInfo.text; }
            set { _interactionInfo.text = value; }
        }

        // Calcular FPS
        private TextMeshProUGUI fpsInfoText;
        // numero de frames que han pasado
        private int frameCounter = 0;
        // tiempo que ha tardado en pasar el numero de frames anterior
        private float timeCounter = 0.0f;
        // ultimo framerate calculado
        private float lastFrameRate = 0.0f;
        // cada cuanto se calcula un nuevo framerate
        private float refreshTime = 0.5f;

        public bool night { get; private set; } = false;

        public static LevelManager Instance { get; private set; } = null;

        private void calculateFPS()
        {
            if (timeCounter < refreshTime)
            {
                // aumenta el contador de tiempo
                timeCounter += Time.deltaTime;
                // aumenta el contador de frames transcurridos
                // como se ejecuta en el update, cada vuelta es un frame
                frameCounter++;
            }
            else
            {
                // frames por segundo
                lastFrameRate = (float)(frameCounter) / timeCounter;
                // se reinician
                frameCounter = 0;
                timeCounter = 0.0f;
            }
            if (fpsInfoText != null)
            {
                // se muestra con dos decimales
                fpsInfoText.text = (((int)(lastFrameRate * 100 + .5) / 100.0)).ToString();
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            prevTime = hoursPassed = hour = initTime / speedFactor;

            days = 1;

            lightSource = GameObject.Find("Light").GetComponent<Light>();
            lightTr = lightSource.transform;

            dayText = GameObject.Find("DayText").GetComponent<TextMeshProUGUI>();
            dayText.text = "Day " + days;

            changeLight(hour / 24.0f);

            enemiesGroup = GameObject.Find("Enemies").transform;
            GameObject sp = GameObject.Find("Spawnpoints");
            spawnpoints = new Transform[sp.transform.childCount];
            for (int i = 0; i < sp.transform.childCount; i++)
            {
                spawnpoints[i] = sp.transform.GetChild(i);
            }
            spawnCooldown = Random.Range(minSpawnDelay, maxSpawnDelay);

            fpsInfoText = GameObject.Find("FPSInfo").GetComponent<TextMeshProUGUI>();
        }

        private void updateDayTime()
        {
            // Actualiza la hora y las horas que han
            // pasado desde el inicio de la simulacion
            hour += Time.deltaTime * speedFactor;
            hoursPassed += Time.deltaTime * speedFactor;

            // El tiempo que ha pasado desde el ultimo guardado
            // supera las 24 horas, ha pasado un dia completo
            if ((hoursPassed - prevTime) * speedFactor >= 24)   // (hoursPassed - prevTime) * >= 24/speedFactor
            {
                // Se actualiza el dia y el texto
                days++;
                dayText.text = "Day " + days;

                // Se reinician las horas
                hoursPassed = prevTime = initTime;
            }
            hour %= (24 / speedFactor);
            changeLight(hour / (24.0f / speedFactor));
            //Debug.Log(hour * speedFactor);

            // Si la hora se encuentra entre las horas en las que pueden spawnear enemigos
            if (hour * speedFactor > spawnStartHour || hour * speedFactor < spawnEndHour)
            {
                night = true;
                //Debug.Log("Spawneable");

                // Se va actualizando el contador para spawnear enemigos
                spawnTimer += Time.deltaTime;

                // Si se ha pasado el cooldown de spawneo y no se supera el maximo
                // numero de enemigos, spawnea un enemigo, cambia el cooldown de 
                // spawneo, y reinicia el contador
                if (spawnTimer > spawnCooldown && enemiesGroup.childCount < maxEnemies)
                {
                    spawnEnemy();
                    spawnCooldown = Random.Range(minSpawnDelay, maxSpawnDelay);
                    spawnTimer = 0;
                }
            }
            else
            {
                night = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            calculateFPS();
            updateDayTime();

            if (Input.GetKeyDown(KeyCode.L)) spawnEnemy();
            else if (Input.GetKeyDown(KeyCode.K)) removeEnemy();
        }

        // Cambia el color de la luz dependiendo de la hora del dia.
        // El dia va progresando desde las 00 hasta las 23:59. A las
        // 00 el dia estara un 0% completo y a las 23:59 al 99% (aprox).
        // El color de la luz es % del degradado correspondiente al %
        // de que tan completo esta el dia
        private void changeLight(float timePercent)
        {
            lightSource.color = lightGradient.Evaluate(timePercent);
            lightTr.rotation = Quaternion.Euler(new Vector3((timePercent * 360.0f) - 90.0f, -90.0f, 0.0f));
        }

        // Instancia un enemigo aleatorio en un punto aleatorio de los
        // colocados en el mapa y lo mete al grupo de enemigos
        private void spawnEnemy()
        {
            Transform tr = spawnpoints[Random.Range(0, spawnpoints.Length)];
            GameObject prefab = enemies[Random.Range(0, enemies.Length)];

            GameObject enemy = Instantiate(prefab, tr);
            enemy.transform.position += new Vector3(Random.Range(-spawnOffset, spawnOffset), 0, Random.Range(-spawnOffset, spawnOffset));
            enemy.transform.parent = enemiesGroup;
        }

        private void removeEnemy()
        {
            if (enemiesGroup.childCount > 0) Destroy(enemiesGroup.GetChild(0).gameObject);
        }

        public int getDays() { return days; }

        public void resetLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}