# R3S – Roslyn-Powered R3 Callback Wiring for Unity

![](https://img.shields.io/badge/unity-2021.3%2B-black)
![](https://img.shields.io/badge/license-MIT-blue)
[![Wiki](https://img.shields.io/badge/📖%20Wiki-blue?style=for-the-badge)](https://nightwish-0827.github.io/R3S/)


> **R3S** turns repetitive **R3 subscription and callback wiring** into **declarative attributes**.  
> The generator emits **`Subscribe` + `AddTo`**, **public accessors**, and **dispose glue** so your handwritten code stays focused on **what happens when values change** — not on **how** subscriptions are registered.

This README **does not introduce or teach R3 itself** (streams, operators, or the reactive model).  
It only documents **how R3 callback patterns change when you use R3S**: *manual wiring* **→** *annotated members + generated glue*.

---

# Table of Contents

* [What Changes in Your Code](#what-changes-in-your-code)

* [Core Features](#core-features)

* [Example Script & Runtime Effect](#example-script--runtime-effect)

* [Attribute Reference & Generated Shape](#attribute-reference--generated-shape)

* [Lifecycle You Must Implement](#lifecycle-you-must-implement)

* [Generator Rules & Diagnostics](#generator-rules--diagnostics)

* [Requirements](#requirements)

---

# What Changes in Your Code

Without R3S, **every** reactive sink tends to repeat the same ceremony: hold a disposable bag, call **`Subscribe`**, chain **`AddTo`**, expose **`ReactiveProperty` / `Subject` / `ReactiveCommand`** surface API, and **`Dispose`** on teardown.

**R3S collapses that ceremony into attributes.** You still write **the callback body** (the method that runs when a value arrives). The generator emits the **subscription line**, the **backing field accessors**, and — when you opt in — the **`CompositeDisposable` lifecycle**.

### Before → After (callback wiring)

**Before** (handwritten wiring):

```csharp
private void R3Awake()
{
    _hp.Subscribe(OnHpChanged).AddTo(_disposable);
}
```

**After** (you write the target + the handler; generator emits the line above):

```csharp
[AutoSubscribe(nameof(_hp))]
private void OnHpChanged(int value)
{
    // Your logic
}
```

**Before** (manual property surface):

```csharp
private readonly ReactiveProperty<int> _hp = new(100);
public ReadOnlyReactiveProperty<int> Hp => _hp;
```

**After**:

```csharp
[ReactiveProperty]
private ReactiveProperty<int> _hp = new(100);
// Generator adds: public ReadOnlyReactiveProperty<int> Hp => _hp;
```

**Before** (command + execute + observable exposure):

```csharp
private readonly ReactiveCommand<Unit> _attack = new();
public Observable<Unit> Attack => _attack;
public void ExecuteAttack() => _attack.Execute(Unit.Default);
```

**After**:

```csharp
[ReactiveCommand]
private ReactiveCommand<Unit> _attack;
// Generator fills initialization + public Observable + ExecuteAttack()
```

The focus is always the same: **your R3 callbacks and types stay; the boilerplate around them disappears.**

---

# Core Features

### AutoSubscribe — `Subscribe` + `AddTo` generated for your method

Apply **`[AutoSubscribe(nameof(field))]`** to a **method**. The generator emits a **`Subscribe`** to that **`ReactiveProperty` or `Subject` field**, chained to **`AddTo`** according to the **`AddTo` mode** you choose (`Disposable`, `CancellationToken`, or `MonoBehaviour`).

**Effect:** You no longer hand-author the repetitive one-liner per subscription; mismatched handler signatures fail at **generation time** instead of silently compiling wrong glue.

---

### AddTo modes — same callback, different lifetime anchor

* **`AddTo.Disposable` (default)** — **`AddTo(_disposable)`**. Requires **`[AutoDispose]`** on the class so **`CompositeDisposable`** exists.
* **`AddTo.CancellationToken`** — **`AddTo(destroyCancellationToken)`**. **`[AutoDispose]`** not required (Unity **`MonoBehaviour`** + **`destroyCancellationToken`** pattern).
* **`AddTo.MonoBehaviour`** — **`AddTo(this)`**. **`[AutoDispose]`** not required.

**Effect:** One attribute captures **both** “what to listen to” and “what cancels the subscription,” without spreading that choice across files.

---

### ReactiveProperty — public read surface without manual passthrough

**`[ReactiveProperty]`** on a **`ReactiveProperty<T>`** field generates a **`ReadOnlyReactiveProperty<T>`** accessor by default (`ReadOnly = true`). Set **`ReadOnly = false`** to expose the writable **`ReactiveProperty<T>`** directly.

**Effect:** View-models expose **bindable state** in one line; the generator keeps naming consistent with your **`_field` → `Field`** convention.

---

### ReactiveCommand — execute helpers + observable exposure

**`[ReactiveCommand]`** on **`ReactiveCommand<T>`** generates:

* For **`T == Unit`**: **`ExecuteField()`** and **`Observable<Unit>`** getter.
* For **`T != Unit`**: **`ExecuteField(T value)`** and **`Observable<T>`** getter.

**Effect:** UI and systems **subscribe** to “command fired” the same way as other streams, while call sites use a **simple `Execute…` method** instead of reaching into the command object repeatedly.

---

### Subject — public **`Observable<T>`** for internal **`Subject<T>`**

**`[Subject]`** on **`Subject<T>`** generates **`public Observable<T> {Name} => _field;`** (read-only stream surface).

**Effect:** You raise events through the **subject** privately but **expose** only **`Observable`**, matching typical R3 boundary style — without writing the property yourself.

---

### AutoDispose — `CompositeDisposable` + teardown hook

**`[AutoDispose]`** on the class generates:

* **`MonoBehaviour`:** **`CompositeDisposable _disposable`** and **`R3OnDestroy()`** that disposes it. You forward **`OnDestroy`** to **`R3OnDestroy()`**.
* **Non-`MonoBehaviour`:** **`IDisposable`** implementation + **`Dispose()`** that clears the bag.

**Effect:** **`AutoSubscribe(..., AddTo.Disposable)`** has a **correct, centralized disposal target** that the generator can rely on.

---

# Example Script & Runtime Effect

Below is a **single cohesive example** showing how attributes compose. **`partial`** is required so the generator can inject members into the same class.

```csharp
using System;
using R3;
using R3.Attributes;
using UnityEngine;

[AutoDispose]
public partial class CombatViewModel : MonoBehaviour
{
    [ReactiveProperty]
    private ReactiveProperty<int> _hp = new(100);

    [ReactiveCommand]
    private ReactiveCommand<Unit> _respawnRequested;

    [Subject]
    private Subject<int> _onDamaged;

    private void Awake()
    {
        R3Awake();
    }

    private void OnDestroy()
    {
        R3OnDestroy();
    }

    [AutoSubscribe(nameof(_hp))]
    private void OnHpChanged(int hp)
    {
        if (hp <= 0)
            ExecuteRespawnRequested();
    }

    [AutoSubscribe(nameof(_onDamaged))]
    private void OnDamaged(int amount)
    {
        Debug.Log($"Damaged: {amount}");
    }
}
```

### What you get at runtime (behavioral summary)

* **`OnHpChanged`** runs **whenever** `_hp` publishes — **without** you writing **`_hp.Subscribe(...).AddTo(...)`**.
* **`OnDamaged`** runs for **`_onDamaged`** notifications the same way.
* External code can **bind** to **`Hp`** (read-only reactive surface), **`OnDamaged`** as **`Observable<int>`**, and **`RespawnRequested`** as **`Observable<Unit>`**, and invoke **`ExecuteRespawnRequested()`** from UI.
* When the **`GameObject`** is destroyed, **`R3OnDestroy()`** **disposes** the **`CompositeDisposable`**, tearing down **`Disposable`-mode** subscriptions created by **`AutoSubscribe`**.

---

# Attribute Reference & Generated Shape

## `[AutoSubscribe(string targetField, AddTo addTo = AddTo.Disposable)]`

| You write | Generator emits (conceptually) |
|-----------|----------------------------------|
| Method + attribute | `targetField.Subscribe(YourMethod).AddTo(...)` |

**`AddTo` targets:**

* **`Disposable`** → **`AddTo(_disposable)`** (**`[AutoDispose]`** required)
* **`CancellationToken`** → **`AddTo(destroyCancellationToken)`**
* **`MonoBehaviour`** → **`AddTo(this)`**

## `[ReactiveProperty(ReadOnly = true)]`

| `ReadOnly` | Generated property |
|------------|-------------------|
| `true` (default) | `public ReadOnlyReactiveProperty<T> Name => _name;` |
| `false` | `public ReactiveProperty<T> Name => _name;` |

## `[ReactiveCommand]`

| Field type | Generated surface |
|------------|-------------------|
| `ReactiveCommand<Unit>` | `public Observable<Unit> Name => _name;` + `public void ExecuteName()` |
| `ReactiveCommand<T>` | `public Observable<T> Name => _name;` + `public void ExecuteName(T value)` |

## `[Subject]`

| Field type | Generated surface |
|------------|-------------------|
| `Subject<T>` | `public Observable<T> Name => _name;` |

## `[AutoDispose]`

| Host type | Generated pieces |
|-----------|-------------------|
| `MonoBehaviour` | `CompositeDisposable _disposable` + `R3OnDestroy()` |
| Other class | `CompositeDisposable` + `IDisposable` + `Dispose()` |

---

# Lifecycle You Must Implement

R3S **does not** magically hook Unity messages. For **`MonoBehaviour`**, you must provide the **bridges** the generator assumes:

```
Awake()
------
Call R3Awake() (or R3Initialize — per generator pipeline)

OnDestroy()   // required when using [AutoDispose] on MonoBehaviour
------------
Call R3OnDestroy()
```

**Concrete effect:** If **`Awake`** never calls **`R3Awake()`**, subscriptions never run (**diagnostic R3Gen008**). If **`OnDestroy`** does not call **`R3OnDestroy()`**, **`CompositeDisposable`** is not disposed (**diagnostics R3Gen009 / R3Gen010**).

---

# Generator Rules & Diagnostics

The analyzer/generator enforces **safe wiring**. Representative rules from the attribute contracts:

| Code | Meaning (short) |
|------|-----------------|
| **R3Gen001** | `partial` class required for **`[AutoDispose]`** **`MonoBehaviour`** |
| **R3Gen002** | **`[ReactiveCommand]`** field name must follow **`_camelCase`** |
| **R3Gen003** | **`[AutoSubscribe]`** target field missing |
| **R3Gen004** | Handler parameter type does not match stream item type |
| **R3Gen005** | **`AddTo.Disposable`** requires **`[AutoDispose]`** |
| **R3Gen006** | **`[ReactiveCommand]`** field must be **`ReactiveCommand<T>`** |
| **R3Gen007** | **`Awake()`** must exist when required |
| **R3Gen008** | **`Awake()`** must call **`R3Awake()`** |
| **R3Gen009** | **`OnDestroy()`** required for **`[AutoDispose]`** **`MonoBehaviour`** |
| **R3Gen010** | **`OnDestroy()`** must call **`R3OnDestroy()`** |

**Effect:** Subscription mistakes become **build-time failures** with **stable codes**, instead of subtle runtime leaks or double-subscribe bugs.

---

# Requirements

* **Unity 2021.3+** (package **`com.nightwishlab.r3s`** metadata)
* **R3** runtime package available to your project (R3S attributes **do not** replace R3 types — they **generate code** that **uses** them)
* **Roslyn source generator** delivered with this package (**`Editor/R3Generator.dll`**) — no manual step beyond importing the package

---

### In a word...

R3S is **not** a reactive framework and **not** an R3 tutorial.

It is a **compiler-assisted rewrite layer**: **your callbacks stay**, and **the wiring around them becomes generated, consistent, and diagnosable** — so R3 code reads like **intent**, not **ceremony**.

**LessSubscribeNoise.MoreSignal.**
