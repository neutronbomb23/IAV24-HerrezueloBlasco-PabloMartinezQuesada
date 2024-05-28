using UnityEngine;

public class EnemyHealthDisplay : MonoBehaviour
{
    private Transform enemyTransform; // Transform del enemigo
    public float heightOffset = 2f; // Desplazamiento en altura
    private Enemy enemy; // Referencia al componente Enemy
    private TextMesh healthText; // Componente de texto

    void Start()
    {
        enemy = GetComponent<Enemy>();
        enemyTransform = GetComponent<Transform>(); // Obtener el transform del enemigo

        if (enemy == null)
        {
            Debug.LogError("Enemy component not found on this GameObject.");
            enabled = false;
            return;
        }

        // Crear el TextMesh para mostrar la salud
        CreateHealthText();
    }

    void Update()
    {
        if (enemy != null && healthText != null)
        {
            healthText.text =  enemy.life.ToString();
            Vector3 worldPosition = enemyTransform.position + Vector3.up * heightOffset;
            healthText.transform.position = worldPosition;
            healthText.transform.rotation = Camera.main.transform.rotation; // Orientar hacia la cámara
        }
    }

    void CreateHealthText()
    {
        GameObject textObject = new GameObject("HealthText");
        textObject.transform.SetParent(this.transform);
        textObject.transform.localPosition = Vector3.up * heightOffset;

        healthText = textObject.AddComponent<TextMesh>();
        healthText.fontSize = 30; // Aumentar el tamaño de la fuente
        healthText.characterSize = 0.1f; // Ajustar el tamaño del carácter
        healthText.color = Color.magenta;
        healthText.alignment = TextAlignment.Center;
        healthText.anchor = TextAnchor.MiddleCenter;
        healthText.GetComponent<MeshRenderer>().sortingOrder = 10; // Asegurarse de que el texto esté delante de otros objetos
    }
}
