---
id: generated-code
title: Generated Code
sidebar_label: Generated Code
---

# Generated Code

:::info[FACT]
**`R3S`** 의 가장 큰 장점은 **보일러플레이트 제거**입니다.
:::
---

## 핵심 관점

- **개발자는 선언과 로직에 집중**
  
- **반복 연결/노출/정리 코드는 생성기로 이동**

- **코드 리뷰가 “반복 구현 확인”에서 “비즈니스 로직 검증”으로 이동**

---

## Before (직접 작성 방식)

```csharp
private readonly ReactiveProperty<int> _hp = new(100);
public ReadOnlyReactiveProperty<int> Hp => _hp;

private readonly ReactiveCommand<Unit> _attack = new();
public Observable<Unit> Attack => _attack;
public void ExecuteAttack() => _attack.Execute(Unit.Default);

private CompositeDisposable _disposable = new();
private void Awake() => _hp.Subscribe(OnHpChanged).AddTo(_disposable);
private void OnDestroy() => _disposable.Dispose();
```

:::note[기본 `R3` 방식]
개발자가 직접 작성하면 다음 코드가 반복됩니다.

- 공개용 프로퍼티 래퍼
  
- `Subscribe(...).AddTo(...)` 연결

- `CompositeDisposable` 생성/정리

- 커맨드 실행 메서드
:::

---

## After (R3S 사용)

```csharp
[AutoDispose]
public partial class PlayerViewModel : MonoBehaviour
{
    [ReactiveProperty]
    private ReactiveProperty<int> _hp = new(100);

    [ReactiveCommand]
    private ReactiveCommand<Unit> _attack = new();

    [AutoSubscribe(nameof(_hp))]
    private void OnHpChanged(int hp) => Debug.Log(hp);

    private void Awake() => R3Awake();
    private void OnDestroy() => R3OnDestroy();
}
```

:::note[`R3S`의 방식]

개발자가 유지하는 코드는 다음처럼 짧아집니다.

- Attribute 선언

- 비즈니스 로직 메서드

- `Awake` / `OnDestroy`에서 `R3Awake`, `R3OnDestroy` **오토 멤버** 호출

:::

---

## 실제 생성 코드 예시

아래는 **위 사용자 클래스를 기반**으로 **`R3S`** 에서 **자동으로 생성된 코드 형태**입니다.

```csharp
public partial class PlayerViewModel
{
    private CompositeDisposable _disposable = new();

    public ReadOnlyReactiveProperty<int> Hp => _hp;
    public Observable<Unit> Attack => _attack;
    public void ExecuteAttack() => _attack.Execute(Unit.Default);

    private void R3Awake()
    {
        _hp.Subscribe(OnHpChanged).AddTo(_disposable);
    }

    private void R3OnDestroy() => _disposable.Dispose();
}
```

:::info[잠깐, 짚고 넘어가시면 좋습니다.]

해당 스크립트는 **`R3S`** 의 **Roslyn Source Generator가 "자동"으로 생성**합니다.  

**사용자는 볼 수도, 조작할 수도 없습니다.** 

:::

---

## 튜토리얼 1 - MonoBehaviour 클래스 기준

### 작성 코드

```csharp
using R3;
using R3.Attributes;
using UnityEngine;

[AutoDispose]
public partial class LobbyViewModel : MonoBehaviour
{
    [ReactiveProperty] private ReactiveProperty<int> _playerCount = new(1);
    [ReactiveCommand] private ReactiveCommand<Unit> _startGame = new();

    [AutoSubscribe(nameof(_playerCount))]
    private void OnPlayerCountChanged(int count)
    {
        Debug.Log($"players: {count}");
    }

    private void Awake() => R3Awake();
    private void OnDestroy() => R3OnDestroy();
}
```

:::tip[생성 코드에서 확인할 포인트]

- `PlayerCount` 노출 프로퍼티
  
- `StartGame` Observable 노출 + `ExecuteStartGame()` 생성

- `R3Awake()` 내부 구독 연결

- `R3OnDestroy()` 내부 dispose

:::

---

## 튜토리얼 2 - 일반 클래스 기준

일반 클래스는 생성자에서 `R3Initialize()` 진입 패턴을 사용합니다.

```csharp
using R3;
using R3.Attributes;

[AutoDispose]
public partial class QuestService
{
    [ReactiveProperty]
    private ReactiveProperty<int> _activeQuestCount = new(0);

    [AutoSubscribe(nameof(_activeQuestCount))]
    private void OnQuestCountChanged(int count)
    {
        // logging
    }

    public QuestService()
    {
        R3Initialize();
    }
}
```

:::tip[일반 클래스에서 확인할 포인트]

- `IDisposable` 구현 및 `Dispose()` 생성 여부

- 생성자에서 `R3Initialize()` 호출 여부

- AddTo 모드가 `Disposable`인지 확인
:::

---

## 튜토리얼 3 - 리뷰/리팩토링 루틴

:::note[다형성을 존중합니다.]
아래는 간단한 예시이며, 필요에 따라 설계 후 진행하시면 됩니다.
:::

1. **수동 프로퍼티 래퍼/수동 Execute 메서드 제거 가능성 확인**

2. **수동 Subscribe 연결을 `AutoSubscribe`로 치환**

3. **수동 Dispose 로직을 `AutoDispose`로 치환**

4. **클래스에 `partial` 선언 추가**

5. **진입점(`Awake/OnDestroy` 또는 생성자) 확인**

---

## 결과

:::info[FACT]

- 클래스당 수십 줄의 반복 코드 감소
  
- 코드 리뷰 포인트가 “반복 구현”에서 “도메인 로직”으로 이동

- 실수하기 쉬운 수동 연결 코드 제거

:::