# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/) and this project adheres to [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [0.0.2] - 2026-02-14

### Added
- **DefaultTriggerPayloadBuilder** — Reflection-based fluent builder for creating `DefaultTriggerPayload` instances in tests, handling private `[SerializeField]` fields.
- **DefaultTriggerTests** — Comprehensive coverage across 8 categories: initialization, activation rules (Immediately, AfterFixedFrame), deactivation rules (Never, NextFrame, AfterFixedFrame), early returns, cancellation, disposal, event firing, and payload validation.

### Fixed
- **DefaultTriggerTests** — Rewrote the entire test suite (33 tests) to properly initialize triggers with `DefaultTriggerPayload`, fixing `NullReferenceException` failures caused by missing `InitializeAsync` calls.

## [0.0.1] - 2026-02-10

### Added

#### Runtime — Core
- **State Machine** — Generic `StateMachineBase<TState, TStateContext, TStateMachinePayload>` with full async lifecycle (`Initialize → Activate → MoveToState → Deactivate → Dispose`) powered by UniTask.
- **State** — Generic `StateBase<TContext, TPayload>` with async `Enter`/`Exit` transitions and `StateStatus` tracking.
- **Transition Rules** — Immutable `TransitionRule` with current state, target state, condition, and priority.
- **Transition Solver** — `DefaultTransitionSolver` with priority-based resolution, per-frame batching, and reactive condition subscription.
- **Triggers** — `DefaultTrigger` with configurable activation rules (Immediately, AfterFixedFrame, AfterTime, AfterTimeUnscaled) and deactivation rules (Never, NextFrame, AfterFixedFrame, AfterTime, AfterTimeUnscaled). `ManualTrigger` for programmatic control.
- **Conditions** — `StateBasedCondition`, `TriggerBasedCondition`, and `ConditionComposite` (All / Any filter modes).
- **Payload & Context System** — `IPayload` interface with validation; `EmptyPayload` and `EmptyContext` defaults.
- **Container & Registry** — `StateMachineContainerInstaller` for async install/uninstall; `StateMachineContainerRegistry` singleton; observer pattern via `ObserverTopic` and `IdentifiableObserver`.
- **MonoBehaviour Integration** — `StateMachineContainerInstallerMono` with install-on-awake and auto-activate options.
- **Factory** — `StateMachineFactory` for creating state machines, states, triggers, conditions, and transition rules from `ScriptableObject` definitions.
- **Reference Pickers** — Type-safe `ReferencePickerBase<T>` for state, state machine, and trigger cross-references.
- **Logging** — Pluggable logging subsystem (`ILogger`, `UnityLogger`, `NullLogger`, `LoggerFactory`, severity levels).

#### Editor
- **State Machine Editor** — Visual graph editor with node-based state visualization, transition edge elements, MVVM architecture, forms for creating/editing states, triggers, conditions, and transition rules, plus custom property drawers.
- **Debugger Window** — Live runtime debugger (`EditorWindow`) with searchable state machine and trigger inspection, event history tracking, and per-entity foldout drawers.
- **ScriptableObject Definitions** — `StateMachineDefinition`, `StateDefinition`, `TriggerDefinition`, `ConditionDefinition`, and `TransitionRuleDefinition` for data-driven configuration.

#### Tests
- Comprehensive unit test suite (45 test files) covering core runtime logic and editor view models.

#### Samples
- **Game Scenario** — Complete sample demonstrating multiple interconnected state machines (safe, door, key) with custom contexts, payloads, triggers, and an objective tracking system.

[Unreleased]: https://github.com/PiotrowskiPatryk/State-Machine-Lite-Unity-Package/compare/v0.0.2...HEAD
[0.0.2]: https://github.com/PiotrowskiPatryk/State-Machine-Lite-Unity-Package/releases/tag/v0.0.2
[0.0.1]: https://github.com/PiotrowskiPatryk/State-Machine-Lite-Unity-Package/releases/tag/v0.0.1
