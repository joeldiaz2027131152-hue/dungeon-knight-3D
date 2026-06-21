# Dungeon Knight 3D

Proyecto nuevo basado en una copia segura de `/home/jowy/Developer/dungeon-knight-retro/dungeon-knight-retro-unity`.

El original retro no se modifica. Esta carpeta contiene la version 3D inicial y mantiene los scripts 2D copiados como referencia para seguir migrando sistemas.

## Como abrirlo

1. Abre Unity Hub.
2. Usa **Add project from disk**.
3. Selecciona la carpeta `/home/jowy/Documents/dungeon-knight-3D`.
4. Abre el proyecto con Unity 6000.4.8f1.
5. Crea o abre una escena vacia y pulsa Play.

El script `WorldOneOneBootstrap` redirige el arranque hacia `DungeonKnight3DBootstrap`, que construye una escena 3D automaticamente al iniciar Play.

## Controles

- `WASD` o flechas: moverse
- `Space`: saltar
- `Tab`: fijar/liberar al enemigo mas cercano
- `J`: atacar; mantener para golpe cargado
- `K`: escudo
- `L`: rodar
- `E`: interactuar con cofres, hogueras, tablillas y puertas
- `Q`: usar pocion

## Que incluye esta base 3D

- Player 3D con movimiento, salto, ataque, escudo, rodada, stamina y pociones.
- Camara 3D con seguimiento suave.
- World 1-1 generado con piso, muros, columnas, antorchas, plataformas y porton.
- World 1-2 con patio exterior, pasarelas, escaleras, plataforma movil, trampas de fuego, cuchilla y cofres.
- Torre 1-3 con pisos verticales, hoguera, plataforma movil, guardian final y salida.
- Enemigos 3D que persiguen y hacen dano al jugador.
- Guardian que suelta la llave del porton.
- Cofres, monedas, pociones, hogueras/checkpoints, tablillas de ayuda y salida.

La idea es iterar aqui hasta que se sienta bien y luego reemplazar placeholders por modelos, animaciones y assets finales.
