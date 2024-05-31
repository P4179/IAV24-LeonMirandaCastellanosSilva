# IAV24 Proyecto final (Enchanted town) (¡¡CORREGIR DE NUEVO!!)

## Índice
- [Autores](#autores)
- [Propuesta](#propuesta)
    - [Elementos](#elementos)
        - [Mundo](#mundo)
        - [Personaje](#personaje)
        - [Objetos](#objetos)
        - [Enemigos](#enemigos)
        - [Interfaz](#interfaz-de-usuario)
    - [Apartados](#apartados)
        - [Apartado A](#apartado-a-matt)
        - [Apartado B](#apartado-b-matt)
        - [Apartado C](#apartado-c-pedro)
        - [Apartado D](#apartado-d-pedro)
        - [Apartado E](#apartado-e-matt)
        - [Ampliaciones](#ampliaciones)
- [Punto de partida](#punto-de-partida)
- [Diseño de la solución](#diseño-de-la-solución)
    - [Solución A](#solución-a)
    - [Solución B](#solución-b)
    - [Solución C](#solución-c)
    - [Solución D](#solución-d)
    - [Solución E](#solución-e)
- [Pruebas y métricas](#pruebas-y-métricas)
    - [Prueba A](#prueba-a)
    - [Prueba B](#prueba-b)
    - [Prueba C](#prueba-c)
    - [Prueba D](#prueba-d)
    - [Prueba E](#prueba-e)
- [Ampliaciones](#ampliaciones)
- [Producción](#producción)
- [Licencia](#licencia)
- [Referencias](#referencias)
    - [Assets](#assets)
    - [Herramientas](#herramientas)
    - [Insipraciones](#inspiraciones)
    - [Programación](#programación)

## Autores
- Matt Castellanos ([MattCastUCM](https://github.com/MattCastUCM))
- Pedro León ([P4179](https://github.com/P4179))

## Propuesta

Este proyecto forma parte de la asignatura de Inteligencia Artificial para Videojuegos del Grado en Desarrollo de Videojuegos de la UCM.

El proyecto consiste en un ***life simulator***, en el que el protagonista deberá ir satisfaciendo sus necesidades a través de la interacción con distintos objetos. El ambiente está basado en un pueblo medieval, en el que nuestro protagonista, un mago, debe hacer su día a día, ya sea alimentándose y durmiendo, o tratando de sobrevivir a hordas de enemigos que invadirán el pueblo. 

En este contexto se pretende implementar el uso de **árboles de comportamiento**, con los que el personaje decide qué hacer y adónde ir en cada momento, y de ***smart objects***, que indican al personaje qué es lo que tiene que hacer con ellos.

El objetivo principal es sobrevivir el mayor números de días posibles. Para ello, el protagonista tiene que atender a sus necesidades y evitar el daño de los enemigos. De lo contrario, irá perdiendo vida y si llega a 0, perderá.

### Elementos

De forma más detallada, el contenido del juego se puede dividir en los siguientes apartados:

#### Mundo
Hay un mundo virtual (pueblo medieval) con un esquema de división de malla navegación donde sucede toda la acción descrita anteriormente.

Además, como se ha indicado, el juego está dividido en días. Cada día dispone de un ciclo de día y noche en el que, durante la mañana, el personaje debe cubrir sus necesidades, mientras que por la noche tiene tanto que huir de los enemigos, como seguir cubriendo sus necesidades.

#### Personaje
El personaje se desplaza alrededor del mapa usando la navegación automática programada con un árbol de comportamiento. En su movimiento trata de realizar sus tareas diarias (mantener sus necesidades altas) y elimina a los enemigos que amenazan el pueblo. Dispone de un área de percepción que le permite conocer los enemigos que tiene a su alrededor y actuar en consecuencia.

Para enfrentarse a los enemigos, utiliza su poder mágico. Cada punto de poder mágico equivale a un proyectil y cuando un enemigo entra en su radio de acción de lucha y tiene poder disponible, lo dispara y lo elimina. Sin embargo, los enemigos también pueden contraatacar y si entran en contacto con él, le inflingen daño. Otra forma de que pierda vida es si no atiende sus necesidades y llegan a cero.

Las necesidades que tiene son las siguientes:
- Hambre: baja de forma pasiva.
- Sed: baja de forma pasiva.
- Energía: baja de forma pasiva. Durante la noche esta bajada es mucho mayor.

#### Objetos
Existen diferentes objetos interactuables del entorno, que le permiten al personaje aumentar alguno de sus parámetros. Están programados como *smart objects* y ninguno de ellos es consumible. Además, para utilizarlos se requiere de un tiempo. Los elementos disponibles son los siguientes:
- Víveres: reducen el hambre del personaje. A pesar de que todos los objetos de este tipo tienen la misma apariencia visual, cada uno ofrece un número diferente de puntos.
- Barriles: reducen la sed del personaje. A pesar de que todos objetos de este tipo tienen la misma apariencia visual, cada uno ofrece un número diferente de puntos.
- Torre del personaje: aumenta la energía del personaje completamente. El personaje se mete dentro de la torre a descansar y no puede sufrir daño. Sin embargo, si hay muchos enemigos alrededor del edificio, no puede seguir durmiendo por el ruido y sale de la torre.
- Libro del mago
    - Inicialmente todos los libros están cerrado y debe abrirlos para activar su poder mágico
    - Una vez abiertos, puede utilizarlos para recargar su poder. Cada uno ofrece un número diferente de puntos.

#### Enemigos
Los enemigos aparecen en los límites del poblado durante la noche. Su movimiento sencillo está programa a través de un árbol de comportamiento. Consiste en merodear por todo el mapa hasta encontrarse con el personaje. Entonces, lo persiguen. Sin embargo, si este sale de su área de detección, dejan de perseguirlo y vuelven a merodear.

Los enemigos mueren de una bola de poder y realizan cierto daño al personaje al entrar en contacto con él.

#### Interfaz de usuario
- FPS y controles: arriba a la derecha. Se usa para testear el juego más adelante.
- Día actual: arriba en el centro.
- Vida del personaje: arriba a la izquierda. Está representado con una barra.
- Poder mágico: arriba a la izquierda debajo de la vida. Está representado con un número, que indica el número de bolas de poder que todavía puede lanzar el personaje.
- Necesidades: se muestran en la parte inferior de la pantalla en forma de barras: hambre, sed y energía.
- Flujo de interacciones: Encima de la barra de energía. Indica qué interacción de que *smart object* se está realizando.


### Apartados

Una vez explicados todos los elementos del juego, siguiendo el [guión del proyecto](https://narratech.com/es/docencia/prueba/), se estructura de la siguiente manera:

#### Apartado A (Matt)
Hay un **mundo virtual** (el pueblo) con un esquema de división por **malla de navegación** generado con la herramienta ***AI Navigation*** de Unity, en el que se encuentran todos los elementos descritos anteriormente. La cámara se puede acercar y alejar usando la rueda del ratón, mover dentro de ciertos límites arrastrando mientras se presiona `click izquierdo`, y reiniciar la posición y zoom haciendo `click derecho`. 

#### Apartado B (Matt)
Hay un **ciclo de día y noche** con el que se van contando los días que lleva vivo el personaje. El sol irá saliendo y poniéndose según la hora del día, y por la noche aparecerán **enemigos** cada cierto tiempo a las afueras del pueblo.

#### Apartado C (Pedro)
El personaje cuenta con una **barra de vida**, que disminuirá si los enemigos entran en contacto con él. Por otro lado, también cuenta con **poder mágico**, que le permitirá **dispararle** a los enemigos que se acerquen a él a cierta distancia. El **poder mágico** restante estará indicado por el número restante de proyectiles que puede disparar. Se activa y se recarga usando un ***smart object***.

#### Apartado D (Pedro)
El personaje cuenta con unas **necesidades**, indicando qué tan satisfechas están mediante unas barras. Estas barras se van vaciando con el tiempo, por lo que tendrá que usar los ***smart objects*** correspondientes para satisfacerlas. Si es de noche, la barra de energía se vaciará más rápidamente, y el personaje no podrá dormir si hay demasiados enemigos cerca de la torre.

#### Apartado E (Matt)
Tanto los enemigos como los personajes están controlados por **árboles de comportamiento** complejos, programados mediante ***Behavior Designer***. El personaje tratará de huir de los enemigos que se acerquen a él, evadiéndolos tanto a ellos como a los obstáculos del mapa. Por otro lado, los enemigos merodearán por el mapa hasta que encuentren con la vista al personaje, comenzando a perseguirlo una vez lo detecten y volviendo a merodear si lo pierde. Además, el **árbol de comportamiento** del personaje **se comunicará con sus necesidades**, que le harán saber qué ***smart objects*** debe utilizar para satisfacerlas para posteriormente acercarse a ellos y usarlos.

#### Ampliaciones
- Existen grupos de enemigos que realizan su movimiento correspondiente, pero en vez de ir en solitario, van en grupo, en bandada.
- Movimiento manual con el clic derecho usando la malla de navegación.
- Se puede asignar “objetos” a los *smart objects*, de modo que cuando el personaje los utiliza, selecciona uno y sirven como multiplicadores para la estadística base del *smart object*.
- A partir de los objetos cogidos (descritos en el punto anterior), el personaje crea una memoria de corto plazo y una permanente.

<br>

## Punto de partida
Se parte de un proyecto de Unity 2022.3.5.f1 proporcionado por el profesor que contiene la herramienta ***[Behavior Designer](https://assetstore.unity.com/packages/tools/visual-scripting/behavior-designer-behavior-trees-for-everyone-15277)***, que sirve para crear árboles de comportamiento.
Los árboles de comportamiento surgen como una mejora de las máquinas finitas de estados. Representan la ejecución de un plan (secuencia de acciones) y consiste en un árbol binario dirigido con un nodo raíz, nodos de control de flujo y nodos de ejecución (tareas). Los nodos principales de flujo son:
- Nodo selector: tiene éxito cuando uno de los hijos tiene éxito. Prueba de izquierda derecha.
- Nodo secuencia: tiene éxito cuando todos los hijos tienen éxito. Se ejecutan de izquierda a derecha.

<br>

## Diseño de la solución

### Solución A
La creación del mundo se ha hecho en dos partes. Mientras que el terreno se ha creado con la herramienta Tiles de Unity, los objetos del pueblo, tanto los que funcionan como obstáculos como los interactuables, se han dispuesto de forma estratégica para que el personaje no realiza sus tareas en un intervalo de tiempo ni muy corto ni muy largo. Luego, se ha configurado la malla de navegación, que representa el terreno real por el que pueden caminar los enemigos y el personaje.

Además, se ha utilizado un borde para los objetos interactuables para que el usuario pueda reconocerlos fácilmente y conozca perfectamente lo que está haciendo el personaje.

![](mapOverview.png)

### Solución B
El `LevelManager` es el gestor encargado de:
- Ciclo de día y noche &rarr; para hacer un cambio visual, se hace que la luz cambie de color según un grandiente y que gire alrededor del mundo como si de un sol se tratase, de modo que las sombras cambian de posición dependiendo de la hora del día.
- Spawn de los enemigos &rarr; existen varios puntos en los que que a partir de una horas establecidas en el día aparecen los tipos de enemigos asignados. Además, se lleva un conteo de los enemigos que hay en el mapa para que no se generen más si ya hay demasiados.

### Solución C

### Solución D

### Solución E
El movimiento del enemigo responde al siguiente diagrama:
```mermaid
stateDiagram-v2
    [*] --> Aparece
    Aparece --> Merodeo
    Merodeo --> Persecucion : Detecta al personaje
    Persecucion --> Merodeo : Deja de detectar al personaje
    Persecucion --> Pierde_vida : Disparado por el personaje
    Pierde_vida --> Persecucion
    Merodeo --> Pierde_vida : Disparado por el personaje
    Pierde_vida --> Merodeo
    Persecucion --> Ataque : El personaje entra en su area de ataque
    Ataque --> Persecucion : El personaje sale de su area de ataque
    Pierde_vida --> [*] : Vida llega a 0
```

Además, durante la persecución también se realiza un control de llegada, implementado de manera que funcione como un nodo de **Behavior Designer**. El pseudocódigo del algoritmo es el siguiente:
```
class Arrival:
    # Target object to arrive at
    target : GameObject
    # Distance at which to start slowing down.
    slowDist: float
    # Distance at which the object is considered to have reached the target
    stopDist : float

    # Components that belong to the object
    transform: Transform
    agent: NavMeshAgent
    lastVel: Vector3
    latVel = Vector3(0, 0, 0)

    function update() -> TaskStatus:
        # Calculate distance to target
        dist : float 
        dist = Distance(target.position, transform.position)

        # If the distance is enough to start slowing down
        if dist <= slowDist:
            # If the distance is enough to stop, the node returns success
            if dist <= stopDist:
                agent.ResetPath()
                return Success
            # Slows down the agent depending on the distance to the target. Returns running because the node isn't finished yet
            else:
                ratio : float
                ratio = dist / slowDist
                agent.velocity = lastVel * ratio
                agent.velocity.Normalize()
                return Running
            
        # If the distance is not enough, the agent's last velocity keeps updating and the node returns failure
        lastVel = navMesh
        return Failure

```
<br>


El movimiento del personaje responde al siguiente diagrama:
```mermaid
stateDiagram-v2
    [*] --> Aparece
    Aparece --> Merodeo
    
    Merodeo --> Ir_hacia_smart_object : Necesidad que rellenar
    Ir_hacia_smart_object --> Merodeo : Termina de usar el smart object o hay enemigo bloqueando el paso
    
    Merodeo --> Evasion : Se encuentra con algun obstaculo o enemigo
    Evasion --> Merodeo : Deja de haber obstaculos o enemigos en medio

    Evasion --> Ataque : Enemigo entra en su area de ataque
    Ataque --> Evasion 

    Merodeo --> Pierde_vida : Golpeado por enemigo
    Evasion --> Pierde_vida : Golpeado por enemigo
    Ir_hacia_smart_object --> Pierde_vida : Golpeado por enemigo
    Pierde_vida --> Merodeo
    Pierde_vida --> Evasion
    Pierde_vida --> Ir_hacia_smart_object

    Pierde_vida --> [*] : Vida llega a 0
```

<br>

## Pruebas y métricas
Se ha creado un plan de pruebas para comprobar el correcto funcionamiento del prototipo creado.

A la hora de la medición se especifica el número de FPS a los que se ejecutaba el programa, para comprobar que el prototipo creado no se ha basado en ninguna práctica de programación errónea que empeora el rendimiento, y cuales han sido los resultados esperados.

[Vídeo con la batería de pruebas](enlace)

### Prueba A
Este apartado está enfocado en probar el correcto funcionamiento de la cámara y del mundo, sobre todo que la malla de navegación está bien creada y se pueden llegar todos los lugares.

<ins>Especificaciones de la máquina</ins>
- Sistema operativo: Window 10 64 bits (compilación 19045)
- Procesador: Intel Core i5-11600k 3.90GHz (12CPUs)
- RAM: 16GB
- Tarjeta gráfica: NVIDIA GeForce RTX 3060
- VRAM: 12GB

| Prueba | Descripción | Atributos | Resultados esperados | Resultados | FPS |
|:-:|:-:|:-:|:-:|:-:|:-:|
| A1 | Hacer que el personaje se dirija a cada uno de los extremos del mapa desde el centro del pueblo pulsando `click izquierdo` sobre ellos | - Árbol de comportamiento del personaje desactivado | Se espera que el personaje pueda llegar a cada uno de los extremos y no se quede atascado en ningún sitio usando el movimiento manual | El comportamiento es el esperado y el personaje se mueve hacia los lugares sobre los que se ha hecho click. | 300 |
| A2 | Hacer zoom con la `rueda del ratón` y mover la cámara con `click izquierdo` |  | Mover la rueda hacia arriba hace zoom in y hacia abajo zoom out. Arrastrar el ratón por la pantalla con un zoom distinto del original hace que la cámara se mueva, sin superar unos límites | El comportamiento es el esperado. Al coincidir el botón para el movimiento con el de arrastrar la cámara, si el movimiento manual está activado, mover la cámara también hará que se mueva el personaje | 300 |
| A3 | Reiniciar la cámara con `click derecho` después de modificar su posición y zoom |  | La cámara vuelve a su posición y zoom originales | El comportamiento es el esperado | 300 |

### Prueba B
El objetivo de esta prueba es testear los cambios en el mundo y cómo estos afectan al spawn de enemigos, de modo que se puedan generar enemigos a lo largo de todos los días sin que afecte en ningún momento al flujo del juego.

<ins>Especificaciones de la máquina</ins>
- Sistema operativo: Window 10 64 bits (compilación 19045)
- Procesador: Intel Core i5-11600k 3.90GHz (12CPUs)
- RAM: 16GB
- Tarjeta gráfica: NVIDIA GeForce RTX 3060
- VRAM: 12GB

| Prueba | Descripción | Atributos | Resultados esperados | Resultados | FPS |
|:-:|:-:|:-:|:-:|:-:|:-:|
| B1 | Comprobar que el ciclo de día y noche funciona correctamente y que los enemigos spawnean durante la noche | - Velocidad x2 <br> - Personaje desactivado <br> - Esperar a que pasen 10 días | Se espera que los 10 días sucedan con normalidad, alternándose el ciclo día-noche perfectamente. Además, como los enemigos merodean, terminarán llegando al pueblo y no se impedirá el avance de los nuevos enemigos que spawneeen por llenar los puntos de spawn | El comportamiento es el esperado y los enemigos se mueven por todo el mapa | 300 |

### Prueba C
El objetivo de está prueba está divido en dos partes:
- Probar el *smart object* del libro: primero se usa para activar el poder mágico del personaje y luego, para recargarlo.
- Interacciones personaje: se prueba tanto que puede usar el poder mágico para dañar a los enemigos como que puede sufrir daño.

<ins>Especificaciones de la máquina:</ins>
- Sistema operativo: Windows 11 Home 64 bits (10.0, compilación 22631)
- Procesador: Intel Core i7-11800H 2.30 GHz (16 CPUs)
- RAM: 16GB
- Tarjeta gráfica: Nvidia Geforce RTX 3050
- VRAM: 4GB

| Prueba | Descripción | Atributos | Resultados esperados | Resultados | FPS |
|:-:|:-:|:-:|:-:|:-:|:-:|
| C1 | Iniciar la simulación y observar como el personaje inicialmente abre todos los libros para activar su poder mágico y luego, los vuelve a utilizar para recarlo. Después, spawnear enemigos y hacer que el personaje los dispare por proximidad y los elimine. También se debe probar como los enemigos hacen daño al personaje al chochar con él. | - Se desactivan todos los *smart objects* menos los libros | Se espera que el personaje sea capaz de realizar de forma secuencial las tareas para activar y cargar su poder mágico y que luego pueda utilizarlo para eliminar a los enemigos. También se espera que sea capaz de sufrir daño y que todos estos cambios se reflejen en la UI | El comportamiento es el esperado. El personaje se dirige a los libros y una vez abiertos se muestra en la UI el texto del poder mágico del personaje a modo de feedback. Entonces, el personaje utiliza ambos libros para recargar su poder. En la segunda parte del vídeo se puede observar como el personaje mata a dos enemigos lanzándoles una bola de poder y además, como sufre cierto daño de un enemigo que lo persigue y lo golpea, viéndose modificada su barra de vida | 150, a excepción de cuando había enemigos que funcionaba a 70 |

### Prueba D
El objetivo de esta prueba es probar el comportamiento de los tres *smart object* que afectan a las necesidades:
- Torre: aumenta la energía y si hay muchos enemigos alrededor, el personaje deja de dormir.
- Víveres: disminuye el hambre del personaje.
- Barriles: disminuye la sed del personaje.

<ins>Especificaciones de la máquina:</ins>
- Sistema operativo: Windows 11 Home 64 bits (10.0, compilación 22631)
- Procesador: Intel Core i7-11800H 2.30 GHz (16 CPUs)
- RAM: 16GB
- Tarjeta gráfica: Nvidia Geforce RTX 3050
- VRAM: 4GB

| Prueba | Descripción | Atributos | Resultados esperados | Resultados | FPS |
|:-:|:-:|:-:|:-:|:-:|:-:|
| D1 | Iniciar la simulación y observar como el personaje debido a la falta de energía se dirige a la torre a dormir, interacción que dura cierto tiempo. Durante este tiempo, el contorno de la torre se torna de color rojo para indica que se está usando. Luego, spawnear enemigos y esperar que se acerquen a la torre para que debido al ruido que producen interrumpan la interacción de dormir del personaje | - Se desactivan todos los *smart objects* a excepción de la torre <br> - Bajar la barra de vida de energía del personaje a 0.5  | El personaje es capaz de dirigirse a la torre y dormir en ella. Una vez durmiendo, si hay muchos enemigos cerca, se despierta | El resultado es el esperado. Nada más comenzar la simulación el personaje debido a la falta de energía se dirige a la torre a descansar, cuyo borde se pone de color rojo. Después, se observa como el personaje ve interrumpida su interacción de dormir debido a que hay muchos enemigos alrededor. Esta información también se puede apreciar en el texto del flujo de interacciones, situado abajo a la izquierda | 170, a excepción de cuando había enemigos que funcionaba a 70 |
| D2 | Iniciar la simulación y observar como el personaje debido al hambre se dirige a uno de los víveres a comer. Esta interacción tarda un tiempo en realizarse | - Se desactivan todos los *smart objects* menos las víveres <br> - Bajar la barra de vida de hambre del personaje a 0.5  | El personaje se dirige a uno de los víveres, el cual elige en función del hambre faltante y la cantidad que le aporta el objeto. Una vez allí, lo utiliza para comer y reducir su hambre. | El resultado es el esperado. El personaje debido al hambre que tiene se dirige al objeto situada en la parte inferior derecha del mapa y lo utiliza para alimentarse. En el vídeo también se explica como este víver está configurado para tener diferentes elementos (comida buena, comida mala y comida promedio), que multiplican la reducción de hambre que otorga y crean una memoria que afecta en las necesidades del personaje | 150 |
| D3 | Iniciar la simulación y observar como el personaje debido a la sed que tiene se dirige a uno de los barriles para rellenar en base al método explicado en el apartado anterior | - Se desactivan todos los *smart objects* a excepción de los barriles <br> - Bajar la barra de sed del personaje a 0.5  | El personaje es capaz de dirigirse a uno de los barriles existentes en el mapa para rellenar su sed. La interacción de beber dura un tiempo y mientras la está realizando el contorno del objeto se torna de color rojo para indicar que se está usando. | El resultado es el esperado. Como el jugador tiene sed se dirige a uno de los barriles y la rellena parcialmente. Luego, al seguir teniendo más sed se dirige a otro de los barriles para terminar de llenarla. | 170, a excepción de cuando había enemigos que funcionaba a 70 |

### Prueba E

El objetivo de esta prueba es comprobar el correcto funcionamiento de los árboles de comportamiento, tanto del personaje como de los enemigos.

<ins>Especificaciones de la máquina</ins>
- Sistema operativo: Window 10 64 bits (compilación 19045)
- Procesador: Intel Core i5-11600k 3.90GHz (12CPUs)
- RAM: 16GB
- Tarjeta gráfica: NVIDIA GeForce RTX 3060
- VRAM: 12GB

| Prueba | Descripción | Atributos | Resultados esperados | Resultados | FPS |
|:-:|:-:|:-:|:-:|:-:|:-:|
| E1 | Acercar y alejar al personaje de los enemigos para que lo vean y lo persigan | - Árbol de comportamiento del personaje desactivado <br> - Nodo del merodeo desactivado <br> - Nodo del merodeo activado | Con el merodeo desactivado, los enemigos no se moverán hasta que vean al personaje, volviendo a quedarse quietos si lo pierden de vista. <br> Con el merodeo activado, se moverán alrededor del mapa de manera aleatoria hasta encontrarse con el personaje, volviendo a merodear si lo pierden de vista. | El comportamiento es el esperado en ambos casos. Además, como la persecución se hace con predicción de movimiento, el más difícil que pierdan al personaje de vista | 300 |
| E2 | Dejar que el personaje se mueva libremente por el mapa | - Evasión de enemigos desactivada <br> - Necesidades desactivadas <br> - Merodeo desactivado | Con la evasión de enemigos desactivada, el personaje se moverá merodeando alrededor del mapa, evitando obstáculos si se los encuentra, o yendo hacia los *smart objects* si necesita rellenar alguna necesidad, pero sin evitar a los enemigos que vayan acercándose <br> Desactivando las necesidades, el personaje merodeará todo el rato, evitando obstáculos y enemigos si los detecta <br> Con el merodeo desactivado, el personaje estará quieto hasta que necesite ir hacia un *smart object* para rellenar sus necesidades o evitar enemigos, volviendo a quedarse quieto una vez termine de realizar cualquiera de esas tareas | El comportamiento es el esperado en todos los casos, aunque con las necesidades desactivadas, el personaje tenderá a moverse más por una parte del mapa, ya que al haber tantos obstáculos alrededor del pueblo, al salir de él le costará volver a entrar al estar evitando obstáculos constantemente. Por otro lado, desactivar el merodeo apenas se aprecia, ya que el personaje estará tratando de satisfacer sus necesidades o evitando enemigos la mayoría de las veces | 300 |

<br>

## Ampliaciones
Se han realizado las siguientes ampliaciones:
- Movimiento manual del personaje con clic derecho usando la malla de navegación. Se ha lanzado un raycast con la posición del mouse para saber a que punto tiene que dirigirse.
- Se pueden asignar "objetos" a las diferentes interacciones que puede tener un *smart objet*. Estos objetos funcionan como multiplicadores en base a una probabilidad asignada, que indica cual es más probable que aparezca o puede incluso darse el caso de que no salga ninguno. El multiplicador afecta a la estadística asignada del objeto, es decir, si el objeto modifica el hambre del jugador, se puede asignar un multiplicador que afecte a este característica.
- A los "objetos" explicados en el punto anterior también se les puede asignar fragmentos de memoria, que funcionan con *scriptable objets* de Unity (scripts que son assets contenedores de datos). Por ejemplo, un fragmento de memoria que haga recordar al jugador que debe ir a beber con más frecuencia porque ha tomado una comida salada. A partir de estos fragmentos se crean dos tipos de memoria: una memoria a corto plazo, cuyos fragmentos desaparecen al poco tiempo y una memoria permanente, que está formada por fragmentos que se encontraban en la memoria a corto plazo y que se habían repetido con frecuencia. De este modo, el jugador puede aprender en base a los *smart objects* que ha usado y tener un comportamiento personalizado.

## Producción
Las tareas se han realizado y el esfuerzo ha sido repartido entre los autores. La cronología de los objetivos del grupo está documentada en la tabla situada más abajo. Para obtener más información sobre la organización y distribución de tareas, puede consultarse el desglose exhaustivo de estas en la sección de Proyectos en GitHub.

| Estado  |  Objetivo  |  Fecha  |  
|:-:|:-:|:-:|
| ✔ | Presentación y resolución de dudas | 07-05-2024 | 
| ✔ | Documentación final | 16-05-2024 |
| ✔ | Presentación | 28-05-2024 |
| :x: | Entrega final | 31-05-2024 |

<br>

## Licencia
Matt Castellanos y Pedro León, autores de la documentación, código y recursos de este trabajo, concedemos permiso permanente a los profesores de la Facultad de Informática de la Universidad Complutense de Madrid para utilizar nuestro material, con sus comentarios y evaluaciones, con fines educativos o de investigación; ya sea para obtener datos agregados de forma anónima como para utilizarlo total o parcialmente reconociendo expresamente nuestra autoría.

Una vez superada con éxito la asignatura se prevee publicar todo en abierto (la documentación con licencia Creative Commons Attribution 4.0 International (CC BY 4.0) y el código con licencia GNU Lesser General Public License 3.0).

<br>

## Referencias
Los recursos de terceros utilizados son de uso público.

### Assets
- [Personaje](https://kaylousberg.itch.io/kaykit-adventurers)
- [Enemigos](https://kaylousberg.itch.io/kaykit-skeletons)
- [Entorno](https://kaylousberg.itch.io/kaykit-medieval-builder-pack)
- [Objetos](https://kaylousberg.itch.io/kaykit-dungeon-remastered)
- [Entorno y objetos](https://kaylousberg.itch.io/kaykit-medieval-hexagon)
- [Fuente](https://fonts.google.com/specimen/Montserrat)
- [Meters and Levels](https://gamedeveloperstudio.itch.io/meters-and-levels)

### Herramientas
- [Behavior Designer](https://assetstore.unity.com/packages/tools/visual-scripting/behavior-designer-behavior-trees-for-everyone-15277)
- Unity AI Navigation
- Unity Shader Graph


### Inspiraciones
- Los Sims Medieval (2011, EA)
- Los Sims 2 (2004, EA)
- Los Sims 3 (2010, EA)
- Los Sims 4 (2014, EA)

### Programación
- *AI for Games*, Ian Millington. Capitulo 5, apartados 5.1, 5.3, 5.4 y 5.9.
- [Shader outline](https://www.youtube.com/watch?v=d89qqVGUHtA)
- [Shader always on top](https://forum.unity.com/threads/shader-drawing-over-everything.1041850/)
- [Day & Night Cycle](https://www.youtube.com/watch?v=m9hj9PdO328)
- [Camera Movement](https://www.youtube.com/watch?v=pJQndtJ2rk0&t=835s)
- Unity AI Tutorial: Sims-Style Videos (Iain McManus)
    - [Part 1 - Smart Objects](https://www.youtube.com/watch?v=gh5PNt6sD_M&list=PLkBiJgxNbuOXBAN5aJnMVkQ9yRSB1UYrG&index=15)
    - [Part 2 - Satisfying Needs](https://www.youtube.com/watch?v=zGCe8vOHRqg&list=PLkBiJgxNbuOXBAN5aJnMVkQ9yRSB1UYrG&index=16)
    - [Part 3 - AI Blackboard](https://www.youtube.com/watch?v=4HWH1QsvwOs&list=PLkBiJgxNbuOXBAN5aJnMVkQ9yRSB1UYrG&index=17)
    - [Part 4 - Traits and Polish](https://www.youtube.com/watch?v=rDYGRseVdt4&list=PLkBiJgxNbuOXBAN5aJnMVkQ9yRSB1UYrG&index=20)
    - [Part 5 - Modular Stats](https://www.youtube.com/watch?v=N6W1T92gS-8&list=PLkBiJgxNbuOXBAN5aJnMVkQ9yRSB1UYrG&index=21)
    - [Part 6 - Memories](https://www.youtube.com/watch?v=_4RTyc9vows&list=PLkBiJgxNbuOXBAN5aJnMVkQ9yRSB1UYrG&index=22)
- [How the Sims make Decisions (Kyle Martin)](https://www.youtube.com/watch?v=Jm1F6UaMtY4)
- [The Genious AI Behind The Sims (Game Maker's Toolkit)](https://www.youtube.com/watch?v=9gf2MT-IOsg)
- [Smart Objects - GameDev Pensieve](https://www.gamedevpensieve.com/ai/ai_knowledge/ai_knowledge_smart-objects)
