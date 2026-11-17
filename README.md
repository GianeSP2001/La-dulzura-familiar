# La Dulzura Familiar 🍬

Videojuego serio educativo en 2D con estética *pixel art* diseñado para enseñar a niños de primaria las **equivalencias matemáticas tanto con números naturales como con números fraccionarios** de forma visual e intuitiva. Ambientado en la preparación de dulces tradicionales peruanos, este proyecto demuestra una arquitectura limpia en Unity/C# desarrollada mediante un flujo de trabajo moderno asistido por Inteligencia Artificial para la optimización y escalabilidad del código.

🚧 **Estado del proyecto:** En desarrollo activo (Prototipo funcional).

## Sobre el juego
Una niña hereda la dulcería tradicional de su familia y debe desbloquear las recetas secretas del recetario de su abuela. Para preparar cada dulce correctamente, el jugador debe comprender y aplicar equivalencias de medidas, transformando conceptos matemáticos abstractos en acciones tangibles de cocina.

* **Mecánica principal:** Arrastrar tazas y cucharas medidoras y verter los ingredientes en la olla para alcanzar la cantidad exacta solicitada.
* **Retos pedagógicos:** Ejercicios dinámicos de composición de medidas, cálculo de cantidades faltantes y comparación de volúmenes (enteros y fracciones).
* **Retroalimentación y apoyo:** Sistema de pistas progresivas, animación en tiempo real del nivel de líquido y un recetario interactivo.

## Tecnologías y Flujo de Trabajo
* **Motor:** Unity (2D)
* **Lenguaje:** C#
* **Arte:** Pixel Art (Diseño original de escenarios y utensilios)
* **UI:** TextMeshPro
* **Desarrollo Asistido por IA:** Uso estratégico de herramientas de Inteligencia Artificial (como Claude) para acelerar el prototipado, refactorizar scripts y optimizar la lógica matemática del juego.

## Capturas

<img width="810" height="527" alt="JuegoRecetaA1" src="https://github.com/user-attachments/assets/babce277-9092-466d-a128-128189ca57a6" />

## Arquitectura

| Script | Responsabilidad |
| :--- | :--- |
| `GameModeManager` | Controla el modo de juego activo y la lógica de los retos (composición, cantidad faltante y comparación) |
| `PotController` | Lógica principal de la olla: recepción de ingredientes, validación de equivalencias y estados de mezcla |
| `DraggableMeasure` | Interacción de arrastrar y soltar (drag & drop) de las tazas y cucharas medidoras hacia la olla |
| `MilkLevel` | Animación y control visual del nivel de líquido dentro de la olla según la proporción vertida |
| `QuantityFormat` | Formateo y conversión matemática para representar correctamente números naturales y fracciones en pantalla |
| `RecipeData` | Estructura de datos (*ScriptableObject*) que define los ingredientes, proporciones y pasos de cada dulce |
| `RecipeCatalog` | Base de datos o catálogo central que almacena todas las recetas disponibles en el juego |
| `RecipeSession` | Gestiona el estado temporal de la receta que el jugador está preparando en el turno actual |
| `ProgressManager` | Guarda y administra el avance del jugador y las recetas desbloqueadas |
| `RecipeBookUI` | Interfaz gráfica del recetario interactivo de la abuela y despliegue de pistas progresivas |
| `Scene Loader` | Gestión de transiciones y carga entre las distintas escenas del videojuego |

## Desarrollo y Optimización con IA
Este repositorio refleja mi enfoque como Ingeniera de Software para integrar **Inteligencia Artificial en el ciclo de desarrollo de videojuegos**. Utilicé asistentes de IA como copiloto técnico para estructurar la lógica de validación de fracciones, aplicar buenas prácticas en C# y depurar comportamientos complejos de interacción, logrando un código modular, limpio y fácil de mantener.

## Autora
**Gianella Sandoval Palomino** · Ingeniera de Software | Desarrolladora de videojuegos  
[LinkedIn](https://linkedin.com) · [itch.io](https://gp-stars01-studio.itch.io/)
