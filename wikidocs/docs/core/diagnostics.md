---
id: diagnostics
title: 문제 해결
sidebar_label: 문제 해결
---

# 문제 해결

:::info[안전망]
`R3S` 는 잘못된 사용을 `R3Gen` 진단으로 컴파일 단계에서 알려줍니다.
:::

---

## 빠른 분류

- **구조 문제**: `R3Gen001`, `R3Gen007`, `R3Gen008`, `R3Gen009`, `R3Gen010`
- **대상/타입 문제**: `R3Gen003`, `R3Gen004`, `R3Gen006`
- **수명/모드 문제**: `R3Gen005`, `R3Gen011`, `R3Gen012`

---

## 자주 보는 진단

:::tip[진단 리스트]

- **`R3Gen001` : 클래스는 `partial`이어야 함**

- **`R3Gen003` : `AutoSubscribe` 대상 필드 없음**

- **`R3Gen004` : 구독 메서드 파라미터 타입 불일치**

- **`R3Gen005` : `AddTo.Disposable` 사용 시 `AutoDispose` 필요**

- **`R3Gen007` : `Awake()` 누락**

- **`R3Gen008` : `Awake()`에서 `R3Awake()` 호출 누락**

- **`R3Gen009` : `OnDestroy()` 누락**

- **`R3Gen010` : `OnDestroy()`에서 `R3OnDestroy()` 호출 누락**

- **`R3Gen011` : 비 MonoBehaviour에서 MonoBehaviour 전용 AddTo 사용**

- **`R3Gen012` : `Dispose()` 중복 선언**

:::

---

## 코드별 빠른 해결 예제

### R3Gen001 - partial 누락

```csharp
// before
public class PlayerViewModel : MonoBehaviour
{
}
```

```csharp
// after
public partial class PlayerViewModel : MonoBehaviour
{
}
```

### R3Gen008 - Awake에서 R3Awake 누락

```csharp
private void Awake()
{
    // R3Awake(); 누락
}
```

```csharp
private void Awake()
{
    R3Awake();
}
```

### R3Gen010 - OnDestroy에서 R3OnDestroy 누락

```csharp
private void OnDestroy()
{
    // R3OnDestroy(); 누락
}
```

```csharp
private void OnDestroy()
{
    R3OnDestroy();
}
```

### R3Gen004 - Subscribe 시그니처 불일치

```csharp
[ReactiveProperty]
private ReactiveProperty<int> _hp = new(100);

[AutoSubscribe(nameof(_hp))]
private void OnHpChanged(string hpText) // 잘못된 타입
{
}
```

```csharp
[AutoSubscribe(nameof(_hp))]
private void OnHpChanged(int hp) // 올바른 타입
{
}
```

### R3Gen003 - AutoSubscribe 대상 필드 누락

```csharp
[AutoSubscribe(nameof(_hp))]
private void OnHpChanged(int hp) { } // _hp 필드가 실제로 없음
```

```csharp
[ReactiveProperty]
private ReactiveProperty<int> _hp = new(100);

[AutoSubscribe(nameof(_hp))]
private void OnHpChanged(int hp) { }
```

---

## 권장 대응 순서

1. **`partial`, `Awake`, `OnDestroy` 기본 규칙부터 맞춘다.**
   
2. **`AutoSubscribe`의 대상 필드명/타입을 확인한다.**

3. **AddTo 모드가 클래스 타입(MonoBehaviour 여부)과 맞는지 확인한다.**

---

## 운영 팁

:::note[자유롭게 운영해보세요]
- `nameof(_field)`를 고정 습관으로 사용하면 필드명 변경 시 오류를 줄일 수 있습니다.

- 신규 팀원이 합류하면 먼저 `R3Gen001`, `R3Gen008`, `R3Gen010` 세 개를 우선 안내하세요.
:::
---

## 튜토리얼 - 자주 터지는 조합 오류

### 케이스 A : AutoSubscribe는 있는데 AutoDispose가 없음

```csharp
public partial class WrongCaseA : MonoBehaviour
{
    [ReactiveProperty] private ReactiveProperty<int> _hp = new(100);

    [AutoSubscribe(nameof(_hp))]
    private void OnHpChanged(int hp) { }

    private void Awake() => R3Awake();
}
```

**`AddTo.Disposable`가 기본값일 때**는 **`AutoDispose`가 필요**합니다.

```csharp
[AutoDispose]
public partial class FixedCaseA : MonoBehaviour
{
    [ReactiveProperty] private ReactiveProperty<int> _hp = new(100);

    [AutoSubscribe(nameof(_hp))]
    private void OnHpChanged(int hp) { }

    private void Awake() => R3Awake();
    private void OnDestroy() => R3OnDestroy();
}
```

### 케이스 B : 비 MonoBehaviour에서 AddTo.CancellationToken 사용

```csharp
[AutoDispose]
public partial class WrongCaseB
{
    [ReactiveProperty] private ReactiveProperty<int> _value = new(1);

    [AutoSubscribe(nameof(_value), AddTo.CancellationToken)] // R3Gen011
    private void OnValue(int value) { }

    public WrongCaseB() => R3Initialize();
}
```

**해결 : 일반 클래스는 `AddTo.Disposable`로 사용합니다.**

```csharp
[AutoDispose]
public partial class FixedCaseB
{
    [ReactiveProperty] private ReactiveProperty<int> _value = new(1);

    [AutoSubscribe(nameof(_value), AddTo.Disposable)]
    private void OnValue(int value) { }

    public FixedCaseB() => R3Initialize();
}
```

---

## 튜토리얼 - 디버깅 순서 (30초 루틴)

1. **첫 에러 코드 하나만 본다** (`R3Genxxx`)

2. **`partial`, `Awake/OnDestroy`, `nameof` 세 가지를 먼저 확인한다**

3. **AutoSubscribe 대상 필드의 실제 타입을 확인**한다

4. **AddTo 모드**와 **클래스 타입(MonoBehaviour 여부)을 대조**한다

5. **수정 후 재컴파일**하여 다음 에러로 이동한다

---

## 튜토리얼 - 신규 팀원 온보딩용 점검 코드

아래 코드를 붙여 넣고 **일부를 의도적으로 깨뜨리면 진단 코드 학습이 빠릅니다.**

```csharp
using R3;
using R3.Attributes;
using UnityEngine;

[AutoDispose]
public partial class R3GenTraining : MonoBehaviour
{
    [ReactiveProperty]
    private ReactiveProperty<int> _value = new(1);

    [AutoSubscribe(nameof(_value))]
    private void OnValueChanged(int value) { }

    private void Awake() => R3Awake();
    private void OnDestroy() => R3OnDestroy();
}
```

:::note[연습 방법]

- **`partial` 제거 → `R3Gen001`**

- **`R3Awake()` 호출 삭제 → `R3Gen008`**

- **`OnDestroy`에서 `R3OnDestroy()` 삭제 → `R3Gen010`**

- **`OnValueChanged(string value)`로 변경 → `R3Gen004`**

:::