---
id: attributes
title: Attributes
sidebar_label: Attributes
---

# Attributes

:::info
`R3S`는 **Attribute 선언을 기준**을 구별하여 자동으로 **코드를 생성**합니다.
:::
---

## 핵심 규칙 (필수)

- **클래스는** `partial`
  
- **`AutoSubscribe`를 쓴다면 `Awake()`에서 `R3Awake()` 호출**

- **`AutoDispose`를 쓴다면 `OnDestroy()`에서 `R3OnDestroy()` 호출**

---

## 필드 대상

- `[ReactiveProperty]`
  - `ReactiveProperty<T>` **필드를 외부 노출 프로퍼티로 생성**
  - **기본은 읽기 전용 노출**

- `[ReactiveCommand]`
  - `ReactiveCommand<T>`**를 실행하는 메서드 생성**

- `[Subject]`
  - `Subject<T>`**를 외부 구독 가능한 형태로 노출**

---

## 메서드 대상

- `[AutoSubscribe(nameof(_field))]`
  - **대상 리액티브 필드**에 대해 `Subscribe(...).AddTo(...)` **코드 자동 생성**
  - 기본 **AddTo는 `Disposable`**

---

## 클래스 대상

- `[AutoDispose]`
  - `CompositeDisposable` **생성 및 정리 코드 생성**
  - **MonoBehaviour**에서는 `OnDestroy -> R3OnDestroy()` **규칙 필요**

---

## AddTo 모드

:::info[기본 형태]
- `AddTo.Disposable` (기본)
:::

:::info[CTS 지원]
- `AddTo.CancellationToken` (MonoBehaviour 전용)
::: 

:::info[Mono]
- `AddTo.MonoBehaviour` (MonoBehaviour 전용)
:::

---

## 튜토리얼 1 - ReactiveProperty / Subject / ReactiveCommand

아래는 **가장 기본적인 필드 선언 패턴**입니다.

```csharp
using R3;
using R3.Attributes;
using UnityEngine;

[AutoDispose]
public partial class PlayerViewModel : MonoBehaviour
{
    [ReactiveProperty]
    private ReactiveProperty<int> _hp = new(100);

    [ReactiveProperty(ReadOnly = false)]
    private ReactiveProperty<int> _mp = new(50);

    [Subject]
    private Subject<Unit> _onDead = new();

    [ReactiveCommand]
    private ReactiveCommand<int> _useItem = new();

    private void Awake() => R3Awake();
    private void OnDestroy() => R3OnDestroy();
}
```

**생성 후 외부에서 사용**하는 형태 :

```csharp
// 읽기
var hp = Hp.CurrentValue;

// 쓰기 (ReadOnly = false 인 경우)
Mp.Value -= 5;

// 이벤트 발행
_onDead.OnNext(Unit.Default);

// 커맨드 실행 메서드
ExecuteUseItem(101);
```

---

## 튜토리얼 2 - AutoSubscribe 기본 패턴

```csharp
using R3;
using R3.Attributes;
using UnityEngine;

[AutoDispose]
public partial class HpView : MonoBehaviour
{
    [ReactiveProperty]
    private ReactiveProperty<int> _hp = new(100);

    [AutoSubscribe(nameof(_hp))]
    private void OnHpChanged(int hp)
    {
        Debug.Log($"HP: {hp}");
    }

    private void Awake() => R3Awake();
    private void OnDestroy() => R3OnDestroy();
}
```

:::info [흐름 요약]

1. `_hp` **값이 바뀌면**

2. 생성된 `R3Awake()` **내부에서 구독된** `OnHpChanged`가 호출되고

3. `OnDestroy` **시점에 구독이 정리**됩니다.
:::

---

## 튜토리얼 3 - AddTo 모드 선택

### 1) 기본값 `AddTo.Disposable`

**클래스 수명과 함께 구독 정리**. 팀 공통 기본값으로 권장합니다.

```csharp
[AutoDispose]
public partial class ShopViewModel : MonoBehaviour
{
    [ReactiveProperty] private ReactiveProperty<int> _gold = new(1000);

    [AutoSubscribe(nameof(_gold), AddTo.Disposable)]
    private void OnGoldChanged(int gold) => Debug.Log(gold);

    private void Awake() => R3Awake();
    private void OnDestroy() => R3OnDestroy();
}
```

### 2) `AddTo.CancellationToken`

**Unity 수명 토큰(`destroyCancellationToken`) 기준으로 정리하고 싶을 때** 사용합니다.

```csharp
public partial class ShopViewModel2 : MonoBehaviour
{
    [ReactiveProperty] private ReactiveProperty<int> _gold = new(1000);

    [AutoSubscribe(nameof(_gold), AddTo.CancellationToken)]
    private void OnGoldChanged(int gold) => Debug.Log(gold);

    private void Awake() => R3Awake();
}
```

### 3) `AddTo.MonoBehaviour`

`AddTo(this)` **패턴을 선호할 때 사용**합니다.

```csharp
public partial class ShopViewModel3 : MonoBehaviour
{
    [ReactiveProperty] private ReactiveProperty<int> _gold = new(1000);

    [AutoSubscribe(nameof(_gold), AddTo.MonoBehaviour)]
    private void OnGoldChanged(int gold) => Debug.Log(gold);

    private void Awake() => R3Awake();
}
```

---

## 튜토리얼 4 - 실전 조합 예제 (HUD)

아래 예제는 상태/이벤트/커맨드를 동시에 쓰는 전형적인 HUD 패턴입니다.

```csharp
using R3;
using R3.Attributes;
using UnityEngine;

[AutoDispose]
public partial class BattleHudViewModel : MonoBehaviour
{
    [ReactiveProperty] private ReactiveProperty<int> _hp = new(100);
    [ReactiveProperty(ReadOnly = false)] private ReactiveProperty<int> _mp = new(40);
    [Subject] private Subject<Unit> _onDead = new();
    [ReactiveCommand] private ReactiveCommand<Unit> _usePotion = new();

    [AutoSubscribe(nameof(_hp))]
    private void OnHpChanged(int hp)
    {
        if (hp <= 0) _onDead.OnNext(Unit.Default);
    }

    [AutoSubscribe(nameof(_usePotion))]
    private void OnUsePotion(Unit _)
    {
        _hp.Value += 30;
        _mp.Value -= 10;
    }

    private void Awake() => R3Awake();
    private void OnDestroy() => R3OnDestroy();
}
```

### 실전 체크리스트

:::info[확인해보고 넘어갈 것]

- **외부 읽기 전용 상태**는 `ReactiveProperty` **기본값(`ReadOnly=true`) 유지**

- **외부에서 값 변경이 필요한 상태만 `ReadOnly=false` 사용**

- **이벤트는 `Subject`, 사용자 액션은 `ReactiveCommand`로 구분**

:::

---

## 빠른 참조표

| 목표 | 추천 Attribute |
|---|---|
| 외부 읽기 전용 상태 노출 | `ReactiveProperty` |
| 외부에서도 값 변경 가능 상태 | `ReactiveProperty(ReadOnly = false)` |
| 단발 이벤트 스트림 | `Subject` |
| 사용자 액션/트리거 | `ReactiveCommand` |
| 자동 구독 연결 | `AutoSubscribe` |
| 자동 정리 | `AutoDispose` |
