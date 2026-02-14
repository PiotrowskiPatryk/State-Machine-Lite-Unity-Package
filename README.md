# State Machines Lite

![Version](https://raw.githubusercontent.com/PiotrowskiPatryk/State-Machine-Lite-Unity-Package/badges/version.svg)
![Coverage](https://raw.githubusercontent.com/PiotrowskiPatryk/State-Machine-Lite-Unity-Package/badges/coverage.svg)
![Preview](https://img.shields.io/badge/status-preview-orange)
![Unity](https://img.shields.io/badge/unity-6000.0%2B-black?logo=unity)
![License](https://img.shields.io/badge/license-MIT-green)

A lightweight, extensible framework for building **data-driven finite state machines** in Unity. Define states, triggers, conditions, and transitions as ScriptableObject assets, wire them together in a visual graph editor, and let the async runtime handle the rest — no boilerplate, no manual wiring.

---

## Key Features

### Runtime
- **Fully Async Lifecycle** — Every operation (`Initialize → Activate → MoveToState → Deactivate → Dispose`) is `async` via [UniTask](https://github.com/Cysharp/UniTask), with first-class `CancellationToken` support.
- **Generic Base Classes** — `StateMachineBase<TState, TContext, TPayload>` and `StateBase<TContext, TPayload>` let you define custom contexts and payloads while keeping the framework unopinionated.
- **Trigger System** — `DefaultTrigger` supports configurable activation rules (*Immediately*, *AfterFixedFrame*, *AfterTime*, *AfterTimeUnscaled*) and deactivation rules (*Never*, *NextFrame*, *AfterFixedFrame*, *AfterTime*, *AfterTimeUnscaled*). `ManualTrigger` is available for programmatic control.
- **Condition System** — Built-in `StateBasedCondition`, `TriggerBasedCondition`, and `ConditionComposite` (with **All** / **Any** filter modes) cover common transition guard patterns out of the box.
- **Priority-Based Transition Solver** — `DefaultTransitionSolver` batches condition changes per frame, resolves conflicts by priority and registration order, and fires a single winning transition.
- **Container & Registry** — `StateMachineContainerInstaller` groups multiple state machines and triggers into a single installable unit. The `StateMachineContainerRegistry` singleton tracks all active containers at runtime.
- **MonoBehaviour Integration** — Drop `StateMachineContainerInstallerMono` on a GameObject to install on `Awake` and optionally auto-activate all state machines.
- **Factory** — `StateMachineFactory` creates state machines, states, triggers, conditions, and transition rules from ScriptableObject definitions.
- **Reference Pickers** — Type-safe `ReferencePickerBase<T>` for cross-referencing states, state machines, and triggers across definitions.
- **Pluggable Logging** — `ILogger` abstraction with `UnityLogger` and `NullLogger` implementations; configurable severity levels.

### Editor
- **Visual Graph Editor** — Node-based state machine editor with draggable state nodes, transition edge elements, and dedicated forms for creating/editing states, triggers, conditions, and transition rules.
- **Runtime Debugger** — Live `EditorWindow` for inspecting active state machines and triggers at runtime, with search/filter, per-entity foldout drawers, and event history tracking.
- **Custom Property Drawers** — Inspector drawers for trigger payloads, reference pickers, state/trigger/state machine selectors, and more.
- **MVVM Architecture** — Editor code follows a clean ViewModel layer for testability and separation of concerns.

### Testing
- **45 unit test files** covering core runtime logic (state machines, states, triggers, conditions, transition solver, registry, factory) and editor view models.

---

## Architecture

The package follows a layered architecture:

```
Interfaces          Contracts for all core types (IState, IStateMachine, ITrigger, ICondition, …)
    ↓
Abstractions        Generic base classes (StateMachineBase, StateBase, TriggerBase, ConditionBase)
    ↓
Implementations     Concrete types (DefaultTransitionSolver, DefaultTrigger, StateBasedCondition, …)
    ↓
Configuration       ScriptableObject definitions (StateMachineDefinition, StateDefinition, …)
    ↓
Factory             StateMachineFactory — constructs runtime instances from definitions
    ↓
Registry            StateMachineContainerInstaller → StateMachineContainerRegistry (singleton)
```

---

## Core Concepts

| Concept | Description |
|---------|-------------|
| **State Machine** | Manages an ordered set of states and transitions between them based on conditions. |
| **State** | Represents a discrete mode with async `Enter` / `Exit` lifecycle hooks. |
| **Trigger** | An activatable signal (manual or rule-based) that drives condition evaluation. |
| **Condition** | A boolean guard that determines whether a transition can fire. Can be state-based, trigger-based, or a composite. |
| **Transition Rule** | Links a *current state* → *target state* with a condition and a priority. |
| **Transition Solver** | Evaluates all active transition rules per frame and picks the highest-priority satisfied rule. |
| **Payload** | Serializable configuration data injected into states, triggers, and conditions during initialization. |
| **Container** | Groups one or more state machines and triggers into a single installable unit managed by a `MonoBehaviour`. |

---

## Installation

### From Disk (local development)

1. Open **Window → Package Manager**.
2. Click **+** → **Add package from disk…**
3. Navigate to `Packages/dev.cortez.state-machines/package.json`.

### From Git URL

1. Open **Window → Package Manager**.
2. Click **+** → **Add package from git URL…**
3. Enter:
   ```
   https://github.com/PiotrowskiPatryk/State-Machine-Lite-Unity-Package.git
   ```

> **Note:** [UniTask](https://github.com/Cysharp/UniTask) must be installed in your project. It is listed as a package dependency and will be resolved automatically when using UPM.

---

## Quick Start

1. **Create a State Machine Container** — `Assets → Create → State Machines → Configuration → State Machine Container`.
2. **Open the State Machine Editor** — Double-click the container asset or use the Editor window.
3. **Define Triggers** — Add triggers (e.g., `DefaultTrigger` with desired activation/deactivation rules).
4. **Define State Machines and States** — Add state machines and states.
5. **Define Transition Rules** — Connect states with conditions (state-based, trigger-based, or composite).
6. **Drop the MonoBehaviour** — Add `StateMachineContainerInstallerMono` to a GameObject, assign your container, and enable *Install on Awake*.
7. **Play** — The state machine initializes, activates, and transitions automatically at runtime.

> **💡 Tip:** Import the **Game Scenario** sample via **Window → Package Manager → State Machines Lite → Samples** to see a fully configured, runnable example with multiple interconnected state machines, triggers, and conditions. It's the fastest way to learn the workflow.

---

## Extending the Framework

The package is designed for extensibility. Create your own **states**, **state machines**, **triggers**, and **conditions** by inheriting from the provided generic base classes. All custom types are **automatically discovered** at editor-time by the built-in reflection system — no manual registration required.

> **How it works:** The editor scans all loaded assemblies for non-abstract, concrete classes that inherit from the framework's base types (e.g., `StateBase`, `StateMachineBase`, `TriggerBase`, `ConditionBase`). These types are automatically populated in the type picker dropdowns throughout the State Machine Editor. The framework also resolves the associated `IPayload` type from your generic arguments, so the inspector renders the correct payload fields for each type.

### Custom State

Inherit from `StateBase` (no payload) or `StateBase<TContext, TPayload>` for states that need custom data:

```csharp
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;

// Simple state with no payload and no context
public sealed class IdleState : StateBase
{
    protected override UniTask DoEnterAsync(EmptyContext context, CancellationToken cancellationToken)
    {
        // Called when the state machine transitions INTO this state
        return UniTask.CompletedTask;
    }

    protected override UniTask DoExitAsync(EmptyContext context, CancellationToken cancellationToken)
    {
        // Called when the state machine transitions OUT of this state
        return UniTask.CompletedTask;
    }
}
```

For states with custom payloads (serialized configuration injected via the editor):

```csharp
using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

// 1. Define a serializable payload
[Serializable]
public sealed class PatrolPayload : IPayload
{
    [SerializeField] private float _patrolSpeed;
    [SerializeField] private float _patrolRadius;

    public float PatrolSpeed => _patrolSpeed;
    public float PatrolRadius => _patrolRadius;

    public bool IsValid() => _patrolSpeed > 0f && _patrolRadius > 0f;
}

// 2. Inherit from StateBase<TContext, TPayload>
public sealed class PatrolState : StateBase<EmptyContext, PatrolPayload>
{
    private PatrolPayload _payload;

    protected override UniTask<bool> DoInitializeAsync(PatrolPayload payload, CancellationToken cancellationToken)
    {
        _payload = payload;
        return UniTask.FromResult(true);
    }

    protected override UniTask DoEnterAsync(EmptyContext context, CancellationToken cancellationToken)
    {
        // Use _payload.PatrolSpeed, _payload.PatrolRadius
        return UniTask.CompletedTask;
    }

    protected override UniTask DoExitAsync(EmptyContext context, CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }
}
```

### Custom State Machine

Inherit from `StateMachineBase<TState>` for simple state machines, or use the full generic form for custom contexts and payloads:

```csharp
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;

// Simple state machine with default context and payload
public sealed class EnemyStateMachine : StateMachineBase<StateBase>
{
}
```

#### Shared Context

A **context** is a plain C# object created and owned by the state machine and automatically passed to every state on `DoEnterAsync` / `DoExitAsync`. Use it to share runtime data (references, services, configuration) across all states without coupling them to each other.

```csharp
// 1. Define a context — any class works
public sealed class EnemyContext
{
    public Transform Transform { get; }
    public NavMeshAgent Agent { get; }

    public EnemyContext(Transform transform, NavMeshAgent agent)
    {
        Transform = transform;
        Agent = agent;
    }
}

// 2. Create a state machine that builds and exposes the context
public sealed class EnemyStateMachine
    : StateMachineBase<EnemyStateBase, EnemyPayload, EnemyContext>
{
    private EnemyContext _context;

    protected override EnemyContext StateContext => _context;

    protected override UniTask<bool> DoInitializeStateMachineAsync(
        EnemyPayload payload, CancellationToken cancellationToken)
    {
        // Build context once during initialization
        _context = new EnemyContext(payload.Transform, payload.Agent);
        return UniTask.FromResult(true);
    }
}

// 3. States receive the context automatically
public sealed class ChaseState : StateBase<EnemyContext, EmptyPayload>
{
    protected override UniTask DoEnterAsync(
        EnemyContext context, CancellationToken cancellationToken)
    {
        context.Agent.SetDestination(/* ... */);
        return UniTask.CompletedTask;
    }

    protected override UniTask DoExitAsync(
        EnemyContext context, CancellationToken cancellationToken)
    {
        context.Agent.ResetPath();
        return UniTask.CompletedTask;
    }
}
```

### Custom Trigger

Inherit from `TriggerBase<TPayload>` to create triggers with custom activation logic:

```csharp
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

[Serializable]
public sealed class ProximityTriggerPayload : IPayload
{
    [SerializeField] private float _activationDistance;

    public float ActivationDistance => _activationDistance;

    public bool IsValid() => _activationDistance > 0f;
}

public sealed class ProximityTrigger : TriggerBase<ProximityTriggerPayload>
{
    private ProximityTriggerPayload _payload;

    public ProximityTrigger(string id, string name, string description)
        : base(id, name, description) { }

    protected override UniTask<bool> InitializeAsync(
        ProximityTriggerPayload payload, CancellationToken cancellationToken)
    {
        _payload = payload;
        return UniTask.FromResult(true);
    }

    public override UniTask<bool> TriggerValueAsync(
        bool targetValue, CancellationToken cancellationToken)
    {
        IsTriggered = targetValue;
        return UniTask.FromResult(true);
    }
}
```

### Custom Condition

Inherit from `ConditionBase` (no payload) or `ConditionBase<TPayload>`:

```csharp
using Dev.Cortez.StateMachines.Core.Abstraction;

public sealed class HealthBelowThresholdCondition : ConditionBase
{
    public override bool IsSatisfied => /* your custom logic */;
}
```

### Excluding Types from the Editor

To hide internal, test, or abstract-helper types from the editor's type picker, apply the `[ExcludeFromTypePicker]` attribute:

```csharp
using Dev.Cortez.StateMachines.Core.Attributes;

[ExcludeFromTypePicker]
public sealed class MockState : StateBase
{
    // Won't appear in the State Machine Editor type dropdowns
}
```

---

## Editor Tooling

### State Machine Editor

The visual graph editor provides a drag-and-drop interface for designing state machines. Each state is rendered as a node, and transitions are visualized as directed edges. Dedicated forms allow you to configure:

- State name, type, and payload
- Trigger definitions and activation/deactivation rules
- Condition type (state-based, trigger-based, composite) and parameters
- Transition rule priority and linked condition

### Runtime Debugger

Open via the custom inspector on `StateMachineContainerInstallerMono`. The debugger window shows:

- All registered **triggers** — current triggered state, event history
- All registered **state machines** — active state, state list, transition rules
- **Search & filter** by name or ID
- **Event history** timeline per entity

---

## Samples

### Game Scenario

A complete, runnable sample demonstrating multiple interconnected state machines (safe, door, key) with custom contexts, payloads, triggers, and an objective tracking system.

Import via **Window → Package Manager → State Machines Lite → Samples → Game Scenario**.

---

## Requirements

| Requirement | Version |
|-------------|---------|
| Unity | **6000.0** or newer |
| [UniTask](https://github.com/Cysharp/UniTask) | **2.5.10** or newer |

---

## Known Limitations

> [!WARNING]
> This is an early **preview** release. The following limitations apply to v0.0.1:

| Area | Limitation |
|------|------------|
| **Dependency Injection** | No integration with Zenject or VContainer. DI support is planned for a future release. |
| **Serialization** | No JSON serialization/deserialization of state machine definitions. Configuration is ScriptableObject-only. |
| **FSM Type** | Only flat Finite State Machines are supported. Hierarchical (HFSM) and nested sub-state machines are not yet available. |
| **Global Transitions** | No support for "Any State" transitions (like Unity Animator's *Any State* node). All transitions must originate from a specific state. |
| **Graph Editor** | The visual node graph editor is functional but early-stage — it was prototyped rapidly and may lack polish. Migration to Unity's Graph Toolkit is under consideration. |
| **Instancing** | No support for instanced or template state machines. Each `StateMachineContainer` produces a single runtime instance and cannot be reused across multiple GameObjects simultaneously. |

---

## Roadmap

Planned improvements for future releases:

- [ ] Dependency injection support (Zenject / VContainer)
- [ ] JSON serialization/deserialization of definitions
- [ ] Hierarchical / nested state machines (HFSM)
- [ ] Global "Any State" transitions
- [ ] Graph editor rewrite (Graph Toolkit)
- [ ] Instanced / template state machines (multi-instance containers)
- [ ] Additional built-in condition types
- [ ] Runtime performance profiling tools
- [ ] Documentation site with API reference

---

## License

This package is licensed under the MIT License. See [LICENSE.md](LICENSE.md) for details.
