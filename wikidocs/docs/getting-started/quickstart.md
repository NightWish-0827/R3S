---
id: quickstart
title: Quick Start
sidebar_label: 시작
---

# 시작

:::info
**아래 예제는 R3S의 대표 자동화 흐름을 한 번에 보여줍니다.**
:::

---

## 기초 예제 스크립트

```csharp
using R3;
using R3.Attributes;
using UnityEngine;

[AutoDispose]
public partial class TestViewModel : MonoBehaviour 
// partial 선언은 [AutoDispose]를 사용하기 위해서 선택이 아닌, 필수입니다.
{
    [ReactiveProperty]
    private ReactiveProperty<int> _hp = new(100);

    [ReactiveCommand]
    private ReactiveCommand<Unit> _attack = new();

    [AutoSubscribe(nameof(_hp))]
    private void OnHpChanged(int hp) => Debug.Log(hp);

    private void Awake() // 필수 펑션
    {
        R3Awake(); // Awake에 해당 멤버가 필수로 호출되어야만 합니다. (IDE 레벨에서 경고해줍니다.)
    }

    private void OnDestroy() //필수 펑션
    {
        R3OnDestroy(); // OnDestroy 또한 해당 멤버가 필수로 호출되어야만 합니다. (IDE 레벨에서 경고해줍니다.)
    }
}
```

---

## 이 코드에서 자동 생성되는 것

- **`_hp`를 외부에서 읽을 수 있는 프로퍼티**
  
- **`_attack` 실행용 메서드**

- **`OnHpChanged`를 `_hp.Subscribe(...)`로 연결하는 코드**

- **`CompositeDisposable` 및 정리 코드**

:::tip[그래서?]
**핵심**은 **직접 작성해야 할 반복 코드가 크게 줄어든다**는 점입니다.  
그리고 이는, **필드 선언이 많아질 수록 매우 강력하게 작동**합니다.  

**직관적인 어트리뷰트**와, **각종 엔트리를 자동화**하여 **스크립트를 얇게 유지해보세요**.
:::

---

## 1분 실습 시나리오

**아래 순서로 실행**하면 `R3S` **흐름을 바로 체감**할 수 있습니다.

:::note[실행 플로우]
1. **씬에 빈 GameObject를 만들고 `TestViewModel` 붙이기**

2. **플레이 모드 진입**

3. **`Hp.Value`를 바꾸는 코드를 한 줄 추가하고 로그 확인**
:::
```csharp
private void Start()
{
    Hp.Subscribe(v => Debug.Log($"HP stream: {v}")).AddTo(this);
    Hp.Value = 80;
    ExecuteAttack();
}
```

---

## 확장 예제 - Subject + Command + 여러 구독

```csharp
using R3;
using R3.Attributes;
using UnityEngine;

[AutoDispose]
public partial class ExtendedViewModel : MonoBehaviour
{
    [ReactiveProperty]
    private ReactiveProperty<int> _hp = new(100);

    [ReactiveProperty(ReadOnly = false)]
    private ReactiveProperty<int> _mp = new(30);

    [Subject]
    private Subject<Unit> _onDead = new();

    [ReactiveCommand]
    private ReactiveCommand<Unit> _attack = new();

    [AutoSubscribe(nameof(_hp))]
    private void OnHpChanged(int hp)
    {
        if (hp <= 0) _onDead.OnNext(Unit.Default);
    }

    [AutoSubscribe(nameof(_attack))]
    private void OnAttack(Unit _)
    {
        _hp.Value -= 25;
    }

    private void Awake() => R3Awake();
    private void OnDestroy() => R3OnDestroy();
}
```

---

## 자주 하는 실수

:::danger[깜빡하지 않으셨나요?]
- **`partial` 누락**

- **`Awake()`는 있는데 `R3Awake()` 호출 누락**

- **`OnDestroy()`는 있는데 `R3OnDestroy()` 호출 누락**

- **`AutoSubscribe` 대상 필드명 오타 (`nameof(_field)` 권장)**
:::