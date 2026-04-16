# PROMPT DEFINITIVO (v3): Rediseño total desde cero — Carrera a la cantina + NPCs + UI + QA

Actuá como un **equipo senior completo de Unity**:
- Lead Game Designer
- Level Designer
- Gameplay Programmer
- AI/NPC Programmer
- UI/UX Designer
- Technical Artist
- QA Lead

Tu misión es crear una **propuesta integral desde cero** para rehacer el minijuego de carrera.
Debés tomar como referencia conceptual la carrera anterior, pero **no reutilizar decisiones de diseño rotas**.
La solución final debe ser **jugable, divertida, intuitiva, estable y medible**.

---

## 0) Contexto completo del producto (usá esto como verdad base)
- Juego con múltiples minijuegos.
- Segundo minijuego: **Carrera a la cantina**.
- Fantasía narrativa: *“Si no llegás antes que los demás, te quedás sin comer.”*
- Ubicación: patio/interior del colegio.
- Tono visual: caricaturesco, exagerado, colorido, readable (inspiración Fall Guys, sin copiar).
- Estado actual: la versión presente no se siente jugable.

### Problemas reportados (asumir como requisitos de mejora)
1. Flujo de carrera confuso.
2. Falta de claridad visual del camino.
3. Obstáculos frustrantes o injustos.
4. NPCs poco creíbles o que se traban.
5. UI inconsistente y poco profesional.
6. Falta de pausa global consistente.
7. QUIZ con UX confusa (confirmación y pistas mejorables).

---

## 1) Restricciones y límites técnicos (obligatorio cumplir)
1. **Se puede y se debe usar assets ya existentes del proyecto** (prefabs, materiales, texturas, VFX, audio, fuentes, iconos, UI).
2. Evitar compras de assets nuevos.
3. Priorizar arquitectura simple y mantenible para equipo chico.
4. Priorizar rendimiento real sobre “efectos bonitos”.
5. Diseñar para iteración rápida (MVP primero, polish después).
6. Toda propuesta debe incluir criterios objetivos de calidad.

---

## 2) Objetivo de diseño (definición de “éxito”)
La nueva carrera será considerada exitosa si:
- Se entiende en menos de 15 segundos.
- Se puede completar sin frustración injusta.
- Tiene decisiones interesantes (ruta segura vs atajo riesgoso).
- Los NPCs compiten de forma coherente y entretenida.
- UI acompaña sin tapar el gameplay.
- El jugador quiere reintentar (alta rejugabilidad).

---

## 3) Lo que tenés que entregar (estructura exacta de respuesta)
Respondé **exactamente** con estas secciones:

### A. Diagnóstico técnico y de diseño
- Top 10 motivos por los que hoy “no es jugable”.
- Clasificación por severidad: crítico / alto / medio.
- Impacto en experiencia de jugador.

### B. Rediseño del gameplay desde cero
- Core loop en 1 frase.
- Reglas completas de partida.
- Condiciones de victoria y derrota.
- Duración ideal por ronda.
- Cómo escalar dificultad sin frustrar.

### C. Diseño de nivel (blueprint de carrera)
- Segmentación del mapa por tramos (inicio, caos controlado, decisión, sprint final).
- Ruta principal (siempre legible).
- Atajos con riesgo/recompensa reales.
- Obstáculos con telemetría visual y ventanas justas.
- Mecanismos anti-bloqueo y comeback.

### D. Sistema de NPCs (obligatorio, en detalle)
- Roles de NPC: competidores, ambience/crowd, apoyo visual.
- Arquitectura recomendada (FSM o Behavior Tree) + por qué.
- Estados mínimos sugeridos (Spawn, Navigate, Avoid, Recover, Sprint, Finish).
- Lógica de navegación en bifurcaciones.
- Sistema de recuperación cuando se traban.
- Parámetros editables (ideal ScriptableObject):
  - velocidad base,
  - agresividad,
  - tolerancia al riesgo,
  - precisión de salto/esquiva,
  - tiempo de reacción.
- Reglas anti-frustración para jugador humano.

### E. Controles y game feel
- Configuración inicial recomendada (aceleración, frenado, giro, salto/impulso si aplica).
- Feedback inmediato (VFX/SFX/UI/cámara).
- Medidas para sensación de control y respuesta.

### F. UI global + pausa en todos los minijuegos
- Arquitectura de HUD reutilizable.
- Botón de pausa persistente.
- Menú de pausa: Reanudar / Reiniciar / Salir.
- Información en pausa: puntaje, posición, estado de ronda.
- Navegación por mouse/teclado/gamepad.

### G. UI del QUIZ (rework de UX)
- Doble confirmación:
  1) click 1 = “¿Estás seguro?”
  2) click 2 = confirma y salta contador para mostrar resultado
- Panel de pistas:
  - draggable,
  - minimizable/maximizable,
  - no bloquear elementos críticos,
  - persistencia de estado durante la ronda.
- Microinteracciones recomendadas.

### H. Dirección artística y legibilidad
- Paleta y jerarquía de color para orientar al jugador.
- Iluminación caricaturesca (key/fill/rim) + sombras limpias.
- Uso de postproceso moderado.
- Cómo reutilizar assets existentes sin perder coherencia estética.

### I. Rendimiento y estabilidad
- Presupuesto base de rendimiento (objetivo FPS y límites razonables).
- Pooling de NPCs y efectos.
- Estrategias para evitar spikes (física, UI, IA).
- Priorización de optimizaciones por impacto.

### J. Plan de implementación por fases
- Fase 1: MVP jugable (2-3 días).
- Fase 2: mejora sistémica (1 semana).
- Fase 3: polish final y balance.
- Para cada fase: tareas, responsables y criterio de salida.

### K. Aseguramiento de calidad (QA) — obligatorio
- Checklist funcional.
- Checklist de usabilidad.
- Checklist de rendimiento.
- Checklist de estabilidad de NPCs.
- Casos borde (edge cases).
- Métricas con umbral de aprobación/rechazo.

### L. Riesgos técnicos + mitigaciones
- Top riesgos reales.
- Señales tempranas de falla.
- Plan de contingencia por riesgo.

---

## 4) Estándar de calidad que debe cumplir tu respuesta
Tu respuesta debe ser:
1. Accionable (pasos concretos, sin humo).
2. Priorizada (qué hacer primero y por qué).
3. Medible (métricas claras de éxito).
4. Implementable con assets existentes.
5. Clara para un equipo pequeño y con tiempos ajustados.

Si proponés código o pseudocódigo, que sea modular, corto y listo para integrar.

---

## 5) Instrucción final
No des una respuesta genérica.
Quiero un documento de ejecución real que permita pasar de un estado “no jugable” a una versión **estable, divertida, intuitiva y con calidad validada**.

