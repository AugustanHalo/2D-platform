using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private float baseHealth = 100f;
    public virtual float Speed => baseSpeed;
    public virtual float MaxHealth => baseHealth;

    protected float currentHealth;
    protected Vector3[] path;
    protected int currentPathIndex = 0;
 
    public static List<Enemy> enemiesList = new List<Enemy>();
    [SerializeField] private GameObject healthBarUIPrefab;
    protected GameObject healthBarInstance;
    protected Image healthBar;
    protected Image healthBarFill;
    protected Canvas healthBarCanvas;
    protected Vector3 offset;
    protected float MaxYOffset = -4.5f;
    protected float MinYOffset = -5.75f;

    [Header("Pop up damage")]
    [SerializeField] private float popupDuration = 1f;
    private float popupInitalSpeed = 1f;
    [SerializeField] private Color popupTextColor = Color.red;
    private float popupSpreadAngle = 60f;

    [Header("Special Effect")]
    public bool isHitRed = false;
    public bool isHitBlue = false;
    [SerializeField] AudioClip hollowPurpleSound;

    [Header("Hollow Purple Effect")]
    [SerializeField] private Color blueColor = Color.blue;
    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private Color purpleColor = new Color(0.5f, 0, 1f);
    [SerializeField] private float glowIntensity = 0.5f;
    private float circleRadius = 10f;
    private float spinDuration = 3f;
    private float explosionDuration = 1f;
    public bool isHollowPurple = false;

    private GameObject blueCircle;
    private GameObject redCircle;
    private GameObject purpleCircle;

    [Header("Cutting Effect")]
    private Color bladeColor = new Color(0f, 0f, 0f, 1);
    private Color bladeGlowColor = new Color(0.75f,0.75f,0.75f,0.75f);
    [SerializeField] private float cuttingEffectDuration = 1f;
    private int bladeCount = 7;
    private float bladeSize = 0.25f;
    private float bladeSpread = 0.1f;
    private float bladeCutDistance = 0.25f; 
    private float bladeCutSpeed = 25f;
    private float bladeIntensity = 0.75f;
    [SerializeField] AudioClip cuttingSound;
    private bool isCutting = false;
    public bool isKitchenOn = false;
    private List<GameObject> blades = new List<GameObject>();

    protected virtual void Awake()
    {
        currentHealth = MaxHealth;
        SetupHealthBar();
    }

    private void SetupHealthBar()
    {
        // Instantiate the HealthBarUI prefab
        healthBarInstance = Instantiate(GameAssets.i.pfHealthBar, transform.position , Quaternion.identity);
        healthBarInstance.transform.SetParent(transform);

        // Set the local position of the health bar
        // Calculate lerped y-offset based on spawn position
        float t = Mathf.InverseLerp(-65, 65f, transform.position.y); // Assuming spawn height ranges from 0 to 10
        float lerpedYOffset = Mathf.Lerp(MinYOffset, MaxYOffset, t);

        // Set the lerped position
        healthBarInstance.transform.localPosition = new Vector3(-10.75f, lerpedYOffset, 0f);

        // Find the Canvas component
        healthBarCanvas = healthBarInstance.GetComponentInChildren<Canvas>();
        if (healthBarCanvas == null)
        {
            Debug.LogError("Canvas not found in HealthBarUI prefab");
            return;
        }

        // Set the canvas to face the camera
        healthBarCanvas.renderMode = RenderMode.WorldSpace;
        healthBarCanvas.worldCamera = Camera.main;
        healthBarCanvas.sortingOrder = 2;
        healthBarCanvas.sortingLayerName = "Foreground";

        // Find the HealthBarFill image
        healthBar = healthBarInstance.GetComponentInChildren<Image>();
        if (healthBar == null)
        {
            Debug.LogError("HealthBar image not found in HealthBarUI prefab");
            return;
        }
        healthBarFill = healthBar.transform.Find("HealthBarFill").GetComponent<Image>();
        if (healthBarFill == null)
        {
            Debug.LogError("HealthBarFill image not found in HealthBarUI prefab");
            return;
        }

        //Scale the health bar
        healthBarInstance.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // Adjust scale as needed
        healthBarCanvas.transform.localScale = new Vector3(1f, 1f, 0.01f); // Adjust scale as needed

 
        UpdateHealthBar();
        UpdateHealthBarPosition();
    }

    protected virtual void UpdateHealthBar()
    {
        healthBarFill.fillAmount = currentHealth / MaxHealth;
    }

    private void UpdateHealthBarPosition()
    {
        //if(healthBarInstance != null)
        //{
        //    healthBarInstance.transform.localPosition = new Vector3(-10.75f,-5.5f);
        //}
    }
    protected virtual void Update()
    {
        if(path != null && path.Length > 0 && !isHollowPurple)
        {
            Move();
        }
    }

    private void LateUpdate()
    {
        UpdateHealthBarPosition();
        // Make the health bar face the camera
        if (healthBarCanvas != null)
        {
            healthBarCanvas.transform.forward = Camera.main.transform.forward;
        }
    }

    public void SetPath(Vector3[] newPath)
    {
        path = newPath;
        transform.position = path[0];
        enemiesList.Add(this);
    }


    protected virtual void Move()
    {
        if(IsDead()) return;
        if (currentPathIndex < path.Length)
        {
            Vector3 targetPosition = path[currentPathIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                currentPathIndex++;
            }
        }
        else
        {
            // Enemy has reached the end of the path
            ReachedEnd();
        }
    }

    protected virtual void ReachedEnd()
    {
        Destroy(gameObject);
        enemiesList.Remove(this);
        FindObjectOfType<Testing>().Health -= 20;
    }

    public bool IsDead()
    {
        return currentHealth <= 0 || !isActiveAndEnabled;
    }

    public virtual void TakeDamage(float damage)
    {
        CreateDamagePopup(damage, popupTextColor);
        if (IsDead()) return;
        currentHealth -= damage;
        UpdateHealthBar();
        
        if (IsDead())
        {
            Die();
        }
        if(isKitchenOn)
        {
            TriggerCuttingEffect();
        }
        
    }

    public void InKitchen(bool state)
    {
        isKitchenOn = state;
    }

    public void CreateDamagePopup(float damage, Color color)
    {
        GameObject popupObj = new GameObject("DamagePopup");
        popupObj.transform.SetParent(transform);
        popupObj.transform.localPosition = Vector3.up * 0.25f; // Adjust this to position the popup above the enemy

        TextMesh textMesh = popupObj.AddComponent<TextMesh>();
        textMesh.text = damage.ToString("F0");
        textMesh.fontSize = 40;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.LowerCenter;
        textMesh.color = color;

        MeshRenderer meshRenderer = popupObj.GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = "Foreground";
        meshRenderer.sortingOrder = 1;

        StartCoroutine(AnimatePopup(popupObj, color));
    }
    private Vector3 GenerateRandomDirection()
    {
        // Generate a random angle within the spread range
        float randomAngle = Random.Range(-popupSpreadAngle, popupSpreadAngle);

        // Convert the angle to a direction vector
        return Quaternion.Euler(0, 0, randomAngle) * Vector3.up;
    }

    protected IEnumerator AnimatePopup(GameObject popupObj, Color color)
    {
        float elapsedTime = 0f;
        Color startColor = color;
        Vector3 randomDirection = GenerateRandomDirection();

        while (elapsedTime < popupDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / popupDuration;
            float speed = popupInitalSpeed * (1 - t);
            // Move the popup upwards
            popupObj.transform.localPosition += randomDirection * Time.deltaTime * speed * 0.25f; 
            popupObj.transform.localPosition += Vector3.up * 0.25f * Time.deltaTime;
            

            // Fade out the text
            TextMesh textMesh = popupObj.GetComponent<TextMesh>();
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, 1 - t);

            yield return null;
        }

        Destroy(popupObj);
    }


    protected virtual void Die()
    {
        enemiesList.Remove(this);
        FindObjectOfType<Testing>().Money += (10);
        StartCoroutine(WaitForDestroy());
    }

    private IEnumerator WaitForDestroy()
    {
        if (isHollowPurple)
        {
            yield return new WaitForSeconds(10f);
            Destroy(gameObject);
            yield break;
        }
        GetComponent<SpriteRenderer>().enabled = false;
        healthBarInstance.SetActive(false);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    public virtual void SetProperties(float speedMultiplier, float healthMultiplier)
    {
        baseSpeed *= speedMultiplier;
        baseHealth *= healthMultiplier;
        currentHealth = MaxHealth;
        UpdateHealthBar();
    }

    public static Enemy GetClosetEnemy(Vector3 position, float maxDistance)
    {
        Enemy closestEnemy = null;
        float closestDistance = maxDistance;
        foreach (Enemy enemy in enemiesList)
        {
            if (enemy == null || enemy.IsDead()) continue;
            float distance = Vector3.Distance(enemy.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    public static Enemy Create(Vector3 position, string name)
    {
        switch (name)
        {
            case "FastEnemy":
                return Instantiate(GameAssets.i.pfFastEnemy, position, Quaternion.identity).GetComponent<Enemy>();
            case "TankEnemy":
                return Instantiate(GameAssets.i.pfTankEnemy, position, Quaternion.identity).GetComponent<Enemy>();
            case "ExplodingEnemy":
                return Instantiate(GameAssets.i.pfExplodingEnemy, position, Quaternion.identity).GetComponent<Enemy>();
            case "HealingEnemy":
                return Instantiate(GameAssets.i.pfHealingEnemy, position, Quaternion.identity).GetComponent<Enemy>();
            default:
                return Instantiate(GameAssets.i.pfEnemy, position, Quaternion.identity).GetComponent<Enemy>();
        }
    }


    private void OnDestroy()
    {
        Destroy(healthBarInstance);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        UpdateHealthBar();
    }

    public void ApplyRed()
    {
        isHitRed = true;
    }

    public void ApplyBlue()
    {
        isHitBlue = true;
    }

    public void TriggerHollowPurpleEffect()
    {
        if(isHollowPurple)
        {
            return;
        }
        StartCoroutine(HollowPurpleEffectCoroutine());
    }

    private IEnumerator HollowPurpleEffectCoroutine()
    {
        isHollowPurple = true;
        AudioManager.instance.PlayAudioClip(hollowPurpleSound);
        // Create circles
        blueCircle = CreateCircle(blueColor);
        redCircle = CreateCircle(redColor);

        blueCircle.transform.position = transform.position + Vector3.left * 10;
        redCircle.transform.position = transform.position + Vector3.right * 10;

        // Spin and merge
        yield return StartCoroutine(SpinAndMergeCircles());

        // Explode
        yield return StartCoroutine(ExplodeEffect());

        // Clean up
        Destroy(purpleCircle);

        // Call the HollowPurple method
        HollowPurple();

    }
    private IEnumerator SpinAndMergeCircles()
    {
        float elapsedTime = 0f;
        Vector3 centerPosition = transform.position;

        while (elapsedTime < spinDuration)
        {
            float t = elapsedTime / spinDuration;
            float angle = t * 360f;
            float distance = Mathf.Lerp(circleRadius, 0, t);

            if (blueCircle != null)
            {
                Vector3 bluePosition = centerPosition + Quaternion.Euler(0, 0, angle) * Vector3.right * distance;
                blueCircle.transform.position = bluePosition;
                UpdateGlowIntensity(blueCircle, t);
            }

            if (redCircle != null)
            {
                Vector3 redPosition = centerPosition + Quaternion.Euler(0, 0, angle + 180) * Vector3.right * distance;
                redCircle.transform.position = redPosition;
                UpdateGlowIntensity(redCircle, t);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Merge circles
        purpleCircle = CreateCircle(purpleColor);
        Destroy(blueCircle);
        Destroy(redCircle);
    }

    private IEnumerator ExplodeEffect()
    {
        float elapsedTime = 0f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, circleRadius * 5);
        foreach (Collider2D collider in colliders)
        {
            Debug.Log(collider.name);
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null && collider.gameObject != gameObject)
            {
                damageable.TakeDamage(MaxHealth);
            }
        }
        Vector3 up = new Vector3(transform.position.x, transform.position.y + circleRadius * 5, transform.position.z);
        Vector3 down = new Vector3(transform.position.x, transform.position.y - circleRadius * 5, transform.position.z);
        Vector3 left = new Vector3(transform.position.x - circleRadius * 5, transform.position.y, transform.position.z);
        Vector3 right = new Vector3(transform.position.x + circleRadius * 5, transform.position.y, transform.position.z);
        Debug.DrawLine(transform.position, up, Color.red, 5f);
        Debug.DrawLine(transform.position, down, Color.white, 5f);
        Debug.DrawLine(transform.position, left, Color.green, 5f);
        Debug.DrawLine(transform.position, right, Color.blue, 5f);
        while (elapsedTime < explosionDuration && purpleCircle != null)
        {
            float t = elapsedTime / explosionDuration;
            float scale = Mathf.Lerp(1f, 5f, t);
            purpleCircle.transform.localScale = Vector3.one * scale; // Adjust for initial scale

            Color fadeColor = purpleColor;
            fadeColor.a = 1 - t;
            UpdateCircleColor(purpleCircle, fadeColor);
            UpdateGlowIntensity(purpleCircle, 1 - t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
       
    }
    private GameObject CreateCircle(Color color)
    {
        GameObject circle = new GameObject("Circle");
        circle.transform.SetParent(transform);
        circle.transform.localPosition = Vector3.zero;
        circle.transform.localScale = Vector3.one * 0.2f; // Adjust size as needed

        SpriteRenderer spriteRenderer = circle.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite();
        spriteRenderer.color = color;
        spriteRenderer.sortingLayerName = "Foreground";
        spriteRenderer.sortingOrder = 3;
        GameObject glowObject = new GameObject("Glow");
        glowObject.transform.SetParent(circle.transform);
        glowObject.transform.localPosition = Vector3.zero;
        glowObject.transform.localScale = Vector3.one * 1.2f; // Slightly larger for glow effect

        SpriteRenderer glowRenderer = glowObject.AddComponent<SpriteRenderer>();
        glowRenderer.sprite = CreateCircleSprite(true); // Create a blurred version for glow
        glowRenderer.color = new Color(color.r, color.g, color.b, glowIntensity);
        glowRenderer.sortingLayerName = "Foreground";
        glowRenderer.sortingOrder = 2; // Behind the main circle

        return circle;
    }

    private Sprite CreateCircleSprite(bool blurred = false)
    {
        int size = blurred ? 128 : 64; // Larger texture for glow to allow for blur
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[texture.width * texture.height];

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(texture.width / 2, texture.height / 2));
                if (distance <= texture.width / 2)
                {
                    float alpha = blurred ? Mathf.Clamp01(1 - (distance / (texture.width / 2))) : 1f;
                    colors[y * texture.width + x] = new Color(1, 1, 1, alpha);
                }
                else
                {
                    colors[y * texture.width + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(colors);
        if (blurred)
        {
            // Apply a simple blur effect
            for (int i = 0; i < 3; i++) // Adjust the number of iterations for more/less blur
            {
                colors = texture.GetPixels();
                for (int y = 64; y < texture.height - 1; y++)
                {
                    for (int x = 64; x < texture.width - 1; x++)
                    {
                        Color sum = Color.clear;
                        for (int oy = -1; oy <= 1; oy++)
                        {
                            for (int ox = -1; ox <= 1; ox++)
                            {
                                sum += colors[(y + oy) * texture.width + (x + ox)];
                            }
                        }
                        colors[y * texture.width + x] = sum / 9f;
                    }
                }
                texture.SetPixels(colors);
            }
        }
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    private void UpdateGlowIntensity(GameObject circle, float intensity)
    {
        SpriteRenderer glowRenderer = circle.GetComponentInChildren<SpriteRenderer>();
        if (glowRenderer != null && glowRenderer.gameObject != circle)
        {
            Color glowColor = glowRenderer.color;
            glowColor.a = intensity * glowIntensity;
            glowRenderer.color = glowColor;
        }
    }

    private void UpdateCircleColor(GameObject circle, Color color)
    {
        SpriteRenderer mainRenderer = circle.GetComponent<SpriteRenderer>();
        if (mainRenderer != null)
        {
            mainRenderer.color = color;
        }

        SpriteRenderer glowRenderer = circle.GetComponentInChildren<SpriteRenderer>();
        if (glowRenderer != null && glowRenderer.gameObject != circle)
        {
            glowRenderer.color = new Color(color.r, color.g, color.b, color.a * glowIntensity);
        }
    }
    private void HollowPurple()
    {
        isHollowPurple = false;

        Die();

        // Implement the effects of Hollow Purple here
       
        Debug.Log("Hollow Purple activated!");
        // For example, you could apply damage, status effects, or trigger other game events
    }

    public void TriggerCuttingEffect()
    {
        if (!isCutting)
        {
            //AudioManager.instance.PlaySlashes(cuttingSound);
            StartCoroutine(CuttingEffectCoroutine());
        }
    }


    private IEnumerator CuttingEffectCoroutine()
    {
        isCutting = true;
        
        List<GameObject> blades = CreateBlades();

        yield return StartCoroutine(AnimateBlades(blades));

        foreach (GameObject blade in blades)
        {
            blade.SetActive(false);
        }
        if (enemiesList.Count == 0)
        {
            AudioManager.instance.StopSlashes();
        }
        else
        {
            if (!AudioManager.instance.IsSlashPlaying() && isKitchenOn)
            {
                AudioManager.instance.PlaySlashes(cuttingSound);
            }
        }
        isCutting = false;
    }

    private List<GameObject> CreateBlades()
    {
        if(blades.Count > 0)
        {
            foreach (GameObject blade in blades)
            {
                blade.SetActive(true);
            }
            return blades;
        }
        for (int i = 0; i < bladeCount; i++)
        {
            GameObject bladeObject = new GameObject($"Blade_{i}");
            bladeObject.transform.SetParent(transform);
            bladeObject.transform.localPosition = Random.insideUnitCircle * bladeSpread;
            bladeObject.transform.localScale = Vector3.one * bladeSize;
            bladeObject.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            // Create main blade sprite
            GameObject bladeSprite = CreateSpriteObject(bladeObject, "BladeSprite", CreateBladeSprite(), bladeColor, 5);

            // Create glow sprite
            GameObject glowSprite = CreateSpriteObject(bladeObject, "GlowSprite", CreateGlowSprite(), bladeGlowColor, 4);
            glowSprite.transform.localScale = Vector3.one * bladeIntensity; //  Make glow larger than the blade

            // Store the initial position and references for oscillation
            BladeData bladeData = bladeObject.AddComponent<BladeData>();
            bladeData.InitialPosition = bladeObject.transform.localPosition;
            bladeData.BladeSprite = bladeSprite.GetComponent<SpriteRenderer>();
            bladeData.BladeGlowSprite = glowSprite.GetComponent<SpriteRenderer>();
            blades.Add(bladeObject);
        }

        return blades;
    }

    private Sprite CreateGlowSprite()
    {
        int width = 18; // Wider than the blade sprite
        int height = 128; // Taller than the blade sprite
        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            int temp = width;
            if (y < 19)
            {
                temp = width / 2;
            }
            if (y > 107)
            {
                temp = width / 2;
            }
            for (int x = 0; x < temp; x++)
            {
                float distanceFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(width / 2f, height / 2f));
                float maxDistance = Mathf.Max(width, height) / 2f;
                float alpha = 1f - (distanceFromCenter / maxDistance);
                alpha = Mathf.Pow(alpha, 2); // Square alpha for a sharper falloff
                colors[y * width + x] = new Color(1, 1, 1, alpha);
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
    private GameObject CreateSpriteObject(GameObject parent, string name, Sprite sprite, Color color, int sortingOrder)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
        spriteRenderer.sortingLayerName = "Foreground";
        spriteRenderer.sortingOrder = sortingOrder;

        return obj;
    }

    private Sprite CreateBladeSprite()
    {
        int width = 16;
        int height = 128;
        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            int temp = width;
            if(y < 19)
            {
                temp = width / 2;
            }
            if(y > 108)
            {
                temp = width / 2;
            }
            for (int x = 0; x < temp; x++)
            {
                float distanceFromCenter = Mathf.Abs(x - width / 2f) / (width / 2f);
                float lengthFade = 1f - (float)y / height;
                float alpha = (1f - distanceFromCenter) * lengthFade;
                colors[y * width + x] = new Color(1, 1, 1, alpha);
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.1f));
    }

    private IEnumerator AnimateBlades(List<GameObject> blades)
    {
        float elapsedTime = 0f;

        while (elapsedTime < cuttingEffectDuration)
        {
            float t = elapsedTime / cuttingEffectDuration;

            foreach (GameObject blade in blades)
            {
                if(blade == null)
                {
                    continue;
                }
                BladeData bladeData = blade.GetComponent<BladeData>();

                // Scale up and fade out
                float scale = Mathf.Lerp(1f, 1.5f, t);
                blade.transform.localScale = Vector3.one * bladeSize * scale;

                // Fade out blade and glow
                Color fadedBladeColor = new Color(bladeColor.r, bladeColor.g, bladeColor.b, 1f - t/1.5f);
                Color fadedGlowColor = new Color(bladeGlowColor.r, bladeGlowColor.g, bladeGlowColor.b, (1f - t/1.25f) * bladeGlowColor.a);
                //bladeData.BladeSprite.color = fadedBladeColor;
                bladeData.BladeGlowSprite.color = fadedGlowColor;

                // Oscillate the blade back and forth
                float oscillation = Mathf.Sin(elapsedTime * bladeCutSpeed) * bladeCutDistance;
                Vector3 oscillationOffset = blade.transform.up * oscillation;
                blade.transform.localPosition = bladeData.InitialPosition + oscillationOffset;

                // Slightly rotate the blade for added dynamism
                blade.transform.Rotate(Vector3.forward, 30f * Time.deltaTime);

                // Pulse the glow
                float pulse = 1f + 0.2f * Mathf.Sin(elapsedTime * 20f);
                bladeData.BladeGlowSprite.transform.localScale = Vector3.one * bladeIntensity * pulse;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Clean up
        foreach (GameObject blade in blades)
        {
            blade.SetActive(false);
        }
    }
}