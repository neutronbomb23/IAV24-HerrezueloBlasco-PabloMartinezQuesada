# IAV - PROYECTO FINAL DE SHOOTER EN TERCERA PERSONA CON INTELIGENCIA ADAPTATIVA

## Autores
- Dorjee Khampa Herrezuelo Blasco (neutronbomb23)
- Pablo Martínez Quesada (Ares75643)

## Propuesta
Este proyecto implementa inteligencia artificial avanzada en un juego de disparos en tercera persona. La IA será capaz de adaptar dinámicamente su estrategia usando Apex Utility AI para evaluar probabilidades y el impacto de diferentes acciones en función del estado cambiante del juego. Las estrategias pueden incluir la gestión eficiente de munición, elegir los momentos adecuados para curarse y decidir cuándo cubrirse.

### Escenario del Juego
El juego se desarrolla en un entorno de combate donde los personajes de la IA enfrentan a un oponente que puede ser otro jugador o una segunda IA con estrategias distintas. El objetivo es demostrar la capacidad de adaptación de la IA en un entorno competitivo y dinámico.

### Mecánica de Juego
- **Tiempo Real:** El juego opera en tiempo real, donde las decisiones estratégicas de la IA se evalúan continuamente.
- **Aprendizaje y Adaptación de la IA:** La IA evalúa su rendimiento y adapta su estrategia en función del análisis en tiempo real del estado del juego y las acciones del oponente.

### Humanización de la IA
La IA se ha diseñado para comportarse de manera más humana, incluyendo la capacidad de fallar disparos y rotar de manera natural. Además, los enemigos tienen distintas cantidades de vida, añadiendo variedad al enfrentamiento.

### **Modos de Ataque del Jugador**
El jugador puede alternar entre dos modos de ataque presionando la tecla "L":
- Ataque al enemigo con menos vida.
- Ataque al enemigo más cercano.

## Plataforma de Desarrollo
Se utilizará Unity 2022.3.5f1 junto con Apex Utility AI para desarrollar un entorno interactivo y visualmente atractivo. El código y los recursos estarán disponibles en un repositorio de GitHub proporcionado por el profesor.

## Estructura del Proyecto
### Clase `Player`
```mermaid
classDiagram
    class Player {
        +float moveSpeed
        +PlayerController controller
        +GunController gunController
        +int startingLifePacks
        +int currentLifePacks
        +Vector3 spawnPoint
        +float thresholdCursorDistanceSquared
        +bool isInvincible
        +float invincibilityCoolDown
        +NavMeshAgent agent
        +event System.Action~int~ OnChangeLPValue
        +void Start()
        +void Awake()
        +void TakeDamage(float damage)
        +void InvincibilityTimer(float coolDownInSecs)
        +void EndInvincibility()
        +void AddLifePack()
        +void UseLifePack()
        +void OnNewWave(int waveNumber)
        +void ResetPosition()
        +void Update()
        +void Die()
        +bool HasAnyBulletInCharger()
        +bool HasAnyChargers()
        +void Shoot()
        +void AimAndShoot(float coolDownToShoot)
    }
```

```mermaid
classDiagram
    class PlayerController {
        +Vector3 velocity
        +Rigidbody myRigidbody
        +NavMeshAgent agent
        +Vector3 desiredPositionByAI
        +float rotationSpeed
        +void Awake()
        +void FixedUpdate()
        +void Move(Vector3 _velocity)
        +void LookAt(Transform target)
        +void MoveAgentTo(Vector3 position)
        +void MoveAgent()
    }

```

</details>

## Estrategias de la IA
La IA puede alternar entre tres estrategias principales, que se evalúan y ajustan continuamente:
1. **Ammo Conservation (AC):** Gestiona la munición de forma eficiente, priorizando disparos precisos.
2. **Health Management (HM):** Prioriza curarse cuando la salud es baja.
3. **Cover Utilization (CU):** Utiliza la cobertura de manera efectiva para minimizar el daño recibido.

## Diagrama de Clases
### Diagrama de Funcionamiento del Enemigo
```mermaid
graph TD
    A[Inicio] --> B[Awake]
    B --> C{¿Jugador encontrado?}
    C -- Sí --> D[Configurar objetivos]
    C -- No --> E[Estado: Idle]
    D --> F[Start]
    F --> G{¿Tiene objetivo?}
    G -- Sí --> H[Estado: Chasing]
    H --> I[OnTargetDeath]
    I --> E
    G -- No --> E
    E --> J[Update]
    J --> K{¿Tiene objetivo?}
    K -- Sí --> L[Actualizar comportamiento]
    L --> M{¿Dentro de rango de ataque?}
    M -- Sí --> N[Iniciar ataque]
    N --> O[PerformAttack]
    O --> P{¿Ataque completado?}
    P -- Sí --> Q[EndAttack]
    Q --> H
    P -- No --> O
    M -- No --> H
    K -- No --> E

    subgraph Configurar objetivos
        D1[Obtener Transform del jugador]
        D2[Obtener LivingEntity del jugador]
        D3[Calcular radios de colisión]
        D1 --> D2 --> D3 --> D
    end

    subgraph Actualizar comportamiento
        L1[Estado: Chasing]
        L2[Actualizar ruta hacia el jugador]
        L1 --> L2
    end

    subgraph Iniciar ataque
        N1[Estado: Attacking]
        N2[Deshabilitar NavMeshAgent]
        N3[Calcular posición de ataque]
        N4[Interpolar hacia posición de ataque]
        N1 --> N2 --> N3 --> N4
    end

    subgraph Realizar ataque
        O1[Incrementar porcentaje de ataque]
        O2[Interpolar posición]
        O3{¿Ha aplicado daño?}
        O4[Aplicar daño al jugador]
        O5{¿Ataque completado?}
        O1 --> O2 --> O3
        O3 -- No --> O4
        O3 -- Sí --> O5
        O4 --> O5
        O5 -- No --> O2
    end

    subgraph Terminar ataque
        Q1[Estado: Chasing]
        Q2[Habilitar NavMeshAgent]
        Q3[Restaurar color de material]
        Q1 --> Q2 --> Q3
    end
```


### Pruebas y Evaluación
Las pruebas se centrarán en la efectividad de las estrategias de IA, midiendo la capacidad de adaptación y la toma de decisiones en situaciones de combate variadas. Se utilizarán métricas como tasa de victoria, uso eficiente de recursos (munición y vida) y cambios de estrategia efectuados.

### Vídeo Demostrativo
Un vídeo demostrativo del proyecto está disponible en el siguiente enlace: [link VIDEO](https://youtu.be/kJgEEXlvNuI)


### Documentación y Repositorio
La documentación completa del proyecto, incluyendo código, recursos y un vídeo demostrativo, estará disponible en un repositorio de GitHub. Esto permitirá una revisión detallada del enfoque y la implementación de las estrategias de IA en un entorno de combate dinámico.
