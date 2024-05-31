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

### **Modos de Ataque de la IA**
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
### AI MANAGER

### Características Principales
- **Administración de comportamientos:** Permite alternar entre dos comportamientos principales (ataque por proximidad y prioridad a enemigos con baja salud) mediante la tecla "L".
- **Escaneo y detección:** Periodicamente escanea el entorno en busca de enemigos y objetos útiles dentro del rango especificado.
- **Movimiento táctico:** Decidir si moverse hacia un objeto útil o reposicionarse estratégicamente en el campo de batalla según la situación.
- **Acciones de combate:** Decide cuándo recargar el arma, usar botiquines de salud, o iniciar un ataque según el comportamiento configurado.
- **Selección de objetivos y ataque:** Calcula el objetivo más adecuado y realiza el ataque cuando es oportuno.

### Diagrama de Flujo del `AIManager`
```mermaid
graph TD
    A(Awake) --> B(Start)
    B --> C(Update)
    C -->|Cada Intervalo| D(Scan)
    D --> E{Tiene Enemigos?}
    E -->|No| C
    E -->|Sí| F(TacticalMovement)
    F --> G(PlayerAction)
    G --> C

    subgraph "Procesos Internos de Scan"
        D --> D1(ScanForEnemies)
        D --> D2(ScanForPickups)
        D --> D3(ScanForPositions)
    end

    subgraph "Acciones Tácticas"
        F -->|Encuentra Objeto Útil| F1(MoveToPickup)
        F -->|Recargar Arma| F2(ReloadGun)
        F --> F3(GetBestPosition)
    end

    subgraph "Acciones del Jugador"
        G -->|Usar Botiquín| G1(UseLifePack)
        G -->|Ataque por Proximidad| G2(ProximityAttack)
        G -->|Prioridad a Baja Salud| G3(LowHealthPriorityAttack)
    end
```
Este diagrama visualiza cómo el AIManager interactúa con sus subcomponentes y gestiona el flujo de decisión para controlar las acciones del personaje de IA en tiempo real. Cada función y decisión se realiza en base a la situación actual del juego, lo que permite una adaptación y respuesta dinámica a las circunstancias cambiantes del entorno y las tácticas del enemigo.
--- 

### Pruebas y Evaluación
Las pruebas se centrarán en la efectividad de las estrategias de IA, midiendo la capacidad de adaptación y la toma de decisiones en situaciones de combate variadas. Se utilizarán métricas como tasa de victoria, uso eficiente de recursos (munición y vida) y cambios de estrategia efectuados.

### Vídeo Demostrativo
Un vídeo demostrativo del proyecto está disponible en el siguiente enlace: [link VIDEO](https://youtu.be/pM_U1lsDRuc)


### Documentación y Repositorio
La documentación completa del proyecto, incluyendo código, recursos y un vídeo demostrativo, estará disponible en un repositorio de GitHub. Esto permitirá una revisión detallada del enfoque y la implementación de las estrategias de IA en un entorno de combate dinámico.
