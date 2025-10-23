# Player Character Controller System - Setup Guide

## Архитектура системы

Система управления персонажем построена на основе **Quantum 3.0.8** с использованием аддонов **KCC** (Kinematic Character Controller) и **Animator**, следуя принципам **SOLID** и архитектуре **Quantum Predict/Rollback**.

---

## Структура проекта

```
Assets/QuantumUser/
├── Simulation/                          # Детерминированная логика (Quantum)
│   ├── Player/
│   │   ├── PlayerInput.cs              # Компонент: инпут игрока
│   │   ├── PlayerMovementConfig.cs     # Компонент: конфигурация движения
│   │   ├── PlayerState.cs              # Компонент: состояние персонажа
│   │   ├── PlayerLink.cs               # Компонент: связь с PlayerRef
│   │   ├── CameraReference.cs          # Компонент: направление камеры
│   │   └── Systems/
│   │       ├── PlayerInputSystem.cs    # Система: обработка инпута
│   │       ├── PlayerMovementSystem.cs # Система: управление движением
│   │       └── PlayerAnimationSystem.cs# Система: управление анимациями
│   └── Input.User.cs                   # Определение структуры Input
│
└── View/                                # Визуализация (Unity)
    ├── Player/
    │   ├── PlayerInputProvider.cs      # Сбор инпута из Unity Input System
    │   ├── LocalPlayerInputPoller.cs   # Передача инпута в Quantum
    │   ├── PlayerCameraController.cs   # Управление камерой Cinemachine
    │   └── PlayerAnimatorSync.cs       # Синхронизация Unity Animator
    └── Installers/
        └── PlayerInstaller.cs          # Zenject DI конфигурация
```

---

## Как это работает

### 1. **Input Flow (Поток инпута)**

```
Клавиатура/Геймпад
       ↓
Unity Input System (InputSystem_Actions)
       ↓
PlayerInputProvider (собирает инпут)
       ↓
LocalPlayerInputPoller (передает в Quantum)
       ↓
Quantum Input Polling
       ↓
PlayerInputSystem (обрабатывает в симуляции)
```

**Почему так:**
- **Детерминизм**: инпут собирается на клиенте, но обрабатывается в детерминированной симуляции
- **Predict/Rollback**: Quantum может откатить и пересчитать движение при рассинхронизации
- **Separation of Concerns**: Unity Input System отделен от логики Quantum

### 2. **Movement System (Система движения)**

```
PlayerInputSystem
       ↓ (записывает в PlayerInput компонент)
PlayerMovementSystem
       ↓ (управляет KCC компонентом)
KCC (Kinematic Character Controller)
       ↓ (обновляет Transform3D)
Unity Transform (визуализация)
```

**Почему так:**
- **KCC**: предоставляет детерминированную физику персонажа (прыжки, гравитация, коллизии)
- **Модульность**: движение отделено от инпута и анимаций
- **Конфигурируемость**: параметры движения в отдельном компоненте `PlayerMovementConfig`

### 3. **Animation System (Система анимаций)**

```
PlayerMovementSystem
       ↓ (обновляет PlayerState)
PlayerAnimationSystem
       ↓ (устанавливает параметры Quantum Animator)
Quantum AnimatorComponent
       ↓
PlayerAnimatorSync (View)
       ↓
Unity Animator (визуализация)
```

**Почему так:**
- **Двойная синхронизация**: 
  - Quantum Animator для детерминированных событий (например, урон во время атаки)
  - Unity Animator для плавной визуализации
- **State Machine**: состояния персонажа (Idle, Walking, Jumping, Falling) управляют анимациями

### 4. **Camera System (Система камеры)**

```
PlayerInputProvider (Look input)
       ↓
PlayerCameraController
       ↓ (поворачивает CameraTarget)
Cinemachine Virtual Camera
       ↓ (следует за CameraTarget)
Main Camera
```

**Дополнительно:**
```
PlayerCameraController
       ↓ (обновляет CameraReference в Quantum)
PlayerMovementSystem
       ↓ (использует направление камеры для движения)
```

**Почему так:**
- **View-only**: камера не влияет на симуляцию, только на визуализацию
- **Camera-relative movement**: персонаж движется относительно направления камеры
- **Cinemachine**: профессиональная система камер Unity

---

## Настройка префаба персонажа

### Шаг 1: Создать структуру GameObject

```
PlayerCharacter (root)
├── QuantumEntityView
├── QuantumEntityPrototype
├── LocalPlayerInputPoller
├── PlayerCameraController
├── CameraTarget (empty Transform)
│   └── Position: (0, 1.6, 0) - высота глаз
└── Visual (дочерний объект)
    ├── CharacterModel (меш)
    └── Animator (Unity Animator)
```

### Шаг 2: Настроить QuantumEntityPrototype

Добавить компоненты:
1. **Transform3D** - позиция и поворот
2. **KCC** - контроллер персонажа
   - Настроить KCCSettings asset
3. **AnimatorComponent** - аниматор Quantum
   - Создать AnimatorGraph asset
4. **PlayerInput** - инпут игрока
5. **PlayerMovementConfig** - конфигурация движения
   - WalkSpeed: 5.0
   - JumpImpulse: 8.0
6. **PlayerState** - состояние персонажа
7. **PlayerLink** - связь с игроком
   - Установить PlayerRef
8. **CameraReference** - направление камеры

### Шаг 3: Настроить Unity Animator

Создать параметры:
- **MoveX** (Float) - горизонтальное движение
- **MoveY** (Float) - вертикальное движение
- **MoveSpeed** (Float) - скорость движения
- **IsGrounded** (Bool) - на земле
- **IsJump** (Trigger) - прыжок
- **IsFall** (Bool) - падение
- **Locomotion** (Bool) - движется ли

Создать состояния (по вашему скриншоту):
- **Idle** - стоит на месте
- **Locomotion** - ходьба/бег (Blend Tree с MoveX, MoveY)
- **Jump** - прыжок
- **Exit** - выход

### Шаг 4: Настроить Cinemachine

1. Создать **Cinemachine Virtual Camera**
2. Установить:
   - Follow: PlayerCharacter/CameraTarget
   - LookAt: PlayerCharacter/CameraTarget
3. Настроить Body: 3rd Person Follow
4. Настроить Aim: Composer

### Шаг 5: Настроить Zenject

Создать Scene Context:
1. Добавить на сцену GameObject "SceneContext"
2. Добавить компонент `SceneContext`
3. В Installers добавить `PlayerInstaller`

---

## Принципы SOLID в архитектуре

### **S - Single Responsibility Principle**
- `PlayerInputSystem` - только обработка инпута
- `PlayerMovementSystem` - только логика движения
- `PlayerAnimationSystem` - только управление анимациями
- `PlayerCameraController` - только управление камерой

### **O - Open/Closed Principle**
- Системы расширяемы через наследование `SystemMainThreadFilter`
- Компоненты расширяемы через `partial struct`
- Конфигурация вынесена в отдельные компоненты

### **L - Liskov Substitution Principle**
- Все системы наследуют базовые интерфейсы Quantum
- Можно заменить `PlayerMovementSystem` на другую реализацию

### **I - Interface Segregation Principle**
- `PlayerInputProvider` предоставляет только методы для инпута
- `PlayerCameraController` предоставляет только методы для камеры
- Системы используют только нужные компоненты через Filter

### **D - Dependency Inversion Principle**
- Зависимости инжектятся через Zenject
- `PlayerInputProvider` не зависит от конкретной реализации Input System
- Системы зависят от абстракций (компонентов), а не от конкретных классов

---

## Почему именно такая архитектура

### 1. **Separation of Concerns (Разделение ответственности)**
- **Simulation (Quantum)**: детерминированная логика, работает одинаково на всех клиентах
- **View (Unity)**: визуализация, может отличаться на разных клиентах

### 2. **Predict/Rollback совместимость**
- Вся логика движения в Simulation
- При рассинхронизации Quantum откатит и пересчитает
- Визуализация плавно интерполируется

### 3. **Модульность**
- Каждая система решает одну задачу
- Легко добавить новые системы (например, StaminaSystem)
- Легко заменить компоненты (например, другой контроллер камеры)

### 4. **Тестируемость**
- Системы можно тестировать независимо
- Инпут можно мокировать
- Логика отделена от Unity API

### 5. **Производительность**
- ECS архитектура Quantum оптимизирована для многопоточности
- Системы обрабатывают только нужные компоненты через Filter
- Минимум аллокаций памяти

---

## Использование

### В коде (Quantum Simulation):

```csharp
// Получить состояние персонажа
if (frame.TryGet<PlayerState>(entity, out var state))
{
    if (state.MovementState == PlayerMovementState.Jumping)
    {
        // Персонаж прыгает
    }
}

// Изменить конфигурацию движения
if (frame.TryGet<PlayerMovementConfig>(entity, out var config))
{
    config.WalkSpeed = FP._10; // Увеличить скорость
    frame.Set(entity, config);
}
```

### В Unity (View):

```csharp
// Получить компонент камеры
var cameraController = GetComponent<PlayerCameraController>();
cameraController.SetSensitivity(3.0f, 150.0f);

// Получить инпут провайдер
var inputProvider = GetComponent<PlayerInputProvider>();
Vector2 movement = inputProvider.GetMovementInput();
```

---

## Дополнительные возможности для расширения

1. **Система выносливости (Stamina)**
   - Добавить `StaminaComponent`
   - Создать `StaminaSystem`
   - Интегрировать с `PlayerMovementSystem`

2. **Система взаимодействия**
   - Добавить `InteractionComponent`
   - Создать `InteractionSystem`
   - Использовать инпут `Interact` из Input Actions

3. **Система боя**
   - Добавить `CombatComponent`
   - Создать `CombatSystem`
   - Использовать Quantum Animator для событий атак

4. **Multiplayer синхронизация**
   - Все уже готово! Quantum автоматически синхронизирует
   - Каждый клиент видит одинаковую симуляцию
   - Визуализация может отличаться (например, качество анимаций)

---

## Troubleshooting

### Персонаж не двигается
1. Проверьте, что `LocalPlayerInputPoller` на префабе
2. Проверьте, что `PlayerLink.PlayerRef` установлен
3. Проверьте, что Input Actions включены

### Камера не поворачивается
1. Проверьте, что `PlayerCameraController` на префабе
2. Проверьте, что `CameraTarget` создан
3. Проверьте, что Cinemachine Virtual Camera настроена

### Анимации не работают
1. Проверьте, что `AnimatorComponent` добавлен в прототип
2. Проверьте, что Unity Animator имеет все параметры
3. Проверьте, что `PlayerAnimatorSync` на префабе

---

## Заключение

Эта архитектура обеспечивает:
- ✅ Детерминированную симуляцию
- ✅ Плавную визуализацию
- ✅ Модульность и расширяемость
- ✅ Соответствие SOLID принципам
- ✅ Готовность к мультиплееру
- ✅ Легкость тестирования и отладки

Все компоненты работают вместе, но остаются независимыми и заменяемыми.
