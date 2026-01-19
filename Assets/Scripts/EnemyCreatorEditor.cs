using UnityEngine;
using UnityEditor;
using System.IO;

public class EnemyCreatorEditor : EditorWindow
{
    private const string EnemyPrefabPath = "Assets/Resources/Enemies/";
    private const string RequiredObjectsPath = "Assets/Resources/RequiredObjects/";

    private string enemyName = "New Enemy";
    private int damage;
    private float health;
    private float speed;
    private int detectionRange;
    private float animationSpeed;
    private Sprite enemySprite;

    private GameObject experienceObject;
    private GameObject explosionObject;
    private GameObject textObject;

    [MenuItem("Enemies/Enemy Creator")]
    public static void ShowWindow()
    {
        GetWindow<EnemyCreatorEditor>("Enemy Creator");
    }

    private void OnEnable()
    {
        LoadRequiredObjects();
    }

    private void LoadRequiredObjects()
    {
        experienceObject =
            AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(RequiredObjectsPath, "ExpPill.prefab"));
        explosionObject =
            AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(RequiredObjectsPath, "Explosion.prefab"));
        textObject = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(RequiredObjectsPath, "DamageText.prefab"));

        if (experienceObject == null || explosionObject == null || textObject == null)
        {
            Debug.LogError(
                "One or more required objects are missing. Please ensure all required prefabs are in the correct folder.");
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Enemy Creator", EditorStyles.boldLabel);

        enemyName = EditorGUILayout.TextField("Enemy Name", enemyName);

        damage = EditorGUILayout.IntSlider("Damage", damage, 1, 30);
        health = EditorGUILayout.Slider("Health", health, 10f, 1000f);
        speed = EditorGUILayout.Slider("Speed", speed, 1f, 5f);
        detectionRange = EditorGUILayout.IntSlider("Detection Range", detectionRange, 20, 50);
        animationSpeed = EditorGUILayout.Slider("Rotation Speed", animationSpeed, 200f, 1500f);

        enemySprite = (Sprite)EditorGUILayout.ObjectField("Enemy Sprite", enemySprite, typeof(Sprite), false);

        if (GUILayout.Button("Create Enemy"))
        {
            CreateEnemy();
        }
    }

    private void CreateEnemy()
    {
        GameObject enemyObject = new GameObject(enemyName);
        Enemy enemy = enemyObject.AddComponent<Enemy>();
        SpriteRenderer spriteRenderer = enemyObject.AddComponent<SpriteRenderer>();
        if (experienceObject == null || explosionObject == null || textObject == null)
        {
            Debug.LogError("Required objects are missing. Cannot create enemy.");
            DestroyImmediate(enemyObject);
            return;
        }

        if (enemySprite)
        {
            spriteRenderer.sprite = enemySprite;
        }
        else
        {
            Debug.LogWarning("No sprite selected for the enemy. The enemy will be invisible.");
            DestroyImmediate(enemyObject);
            return;
        }


        // Add Circle Collider 2D
        CircleCollider2D circleCollider = enemyObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f;

        // Add Rigidbody 2D
        Rigidbody2D rigidbody = enemyObject.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        rigidbody.simulated = true;
        rigidbody.mass = 1f;
        rigidbody.drag = 0f;
        rigidbody.angularDrag = 0.05f;
        rigidbody.gravityScale = 0f;
        rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rigidbody.sleepMode = RigidbodySleepMode2D.StartAwake;
        rigidbody.interpolation = RigidbodyInterpolation2D.None;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        SerializedObject serializedObject = new SerializedObject(enemy);
        serializedObject.FindProperty("_damage").intValue = damage;
        serializedObject.FindProperty("_health").floatValue = health;
        serializedObject.FindProperty("_speed").floatValue = speed;
        serializedObject.FindProperty("_detectionRange").intValue = detectionRange;
        serializedObject.FindProperty("_animationSpeed").floatValue = animationSpeed;
        serializedObject.FindProperty("_expierenceObject").objectReferenceValue = experienceObject;
        serializedObject.FindProperty("_explosionObject").objectReferenceValue = explosionObject;
        serializedObject.FindProperty("_textObject").objectReferenceValue = textObject;
        serializedObject.ApplyModifiedProperties();

        if (!AssetDatabase.IsValidFolder(EnemyPrefabPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Enemies");
        }

        string prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(EnemyPrefabPath, $"{enemyName}.prefab"));
        PrefabUtility.SaveAsPrefabAsset(enemyObject, prefabPath);
        DestroyImmediate(enemyObject);

        Debug.Log($"Enemy '{enemyName}' created and saved as prefab at {prefabPath}");

        ResetFields();
    }

    private void ResetFields()
    {
        enemyName = "New Enemy";
        damage = 0;
        health = 10f;
        speed = 10f;
        detectionRange = 10;
        animationSpeed = 1f;
        enemySprite = null;
    }
}