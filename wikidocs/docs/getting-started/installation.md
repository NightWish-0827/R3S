---
id: installation
title: 설치 및 환경 요구사항
sidebar_label: 설치 및 환경 요구사항 
---

# 설치 및 환경 요구사항 

:::info
개발 환경인 **Unity 2022.3 LTS에서 동작 무결이 완벽하게 보장됩니다**.  
해당 라이브러리는 **Roslyn Gen을 지원하는 Unity 2021.3 LTS 미만 버전 에서 동작하지 않습니다**.
:::

---

## 요구사항

| 항목 | 요구·전제 |
|------|-----------|
| **Unity 에디터** | **2021.3 LTS 이상** (이전 **레거시 버전을 지원하지 않습니다.**) |
| **컨벤션** | 후술할 **간단한 컨벤션**을 통해 **`R3S`의 모든 기능을 활용할 수 있습니다.** |
| **의존성 패키지** | **`R3S`는 원본 패키지인 `R3` 패키지를 필요로 합니다.** |

:::danger[반드시 설치해야 할 의존성]
**`R3S`는 반드시 `R3`가 설치된 환경에서 설치하십시오. `R3`가 없으면, 전체 기능이 작동하지 않습니다.**

**`R3 Git` : `https://github.com/Cysharp/R3`**
:::

---

## UPM으로 추가 (Git)

**Package Manager → Add package from git URL** 에 아래를 입력합니다.

```text
https://github.com/NightWish-0827/R3S.git?path=/com.nightwishlab.r3s
```

### 로컬/서브 모듈로 넣는 경우 

`Packages/manifest.json` 의 `dependencies` 에 **로컬 경로를 지정하는 방식도 동일하게 작동합니다**.  
이 경우에도 **`package.json` 이 있는 폴더가 패키지 루트**여야 합니다.

---

## 패키지 구성 확인

:::info
**`R3S` 는 아래 두 축을 통해 구동됩니다.**
:::

- **Runtime : `R3.Attributes` (Attribute 정의)**
  
- **Editor : `R3Generator.dll` (Roslyn Source Generator)**

---

## 설치 후 체크리스트


1. `R3Generator.dll`이 **Editor**에서 **Analyzer**로 **인식되는지 확인**
   
2. `R3.Attributes.asmdef`가 `com.cysharp.r3`를 **참조하는지 확인**

3. **임의 클래스**에서 `partial`, `Awake -> R3Awake()`, `OnDestroy -> R3OnDestroy()` **규칙이 지켜지는지 확인**

---

## 최소 검증 코드

:::tip
**설치 직후 아래 코드가 오류 없이 컴파일되면 기본 준비는 끝입니다.**
:::

```csharp
using R3;
using R3.Attributes;
using UnityEngine;

[AutoDispose]
public partial class R3SInstallCheck : MonoBehaviour
{
    [ReactiveProperty]
    private ReactiveProperty<int> _value = new(1);

    [AutoSubscribe(nameof(_value))]
    private void OnValueChanged(int v) => Debug.Log(v);

    private void Awake() => R3Awake();
    private void OnDestroy() => R3OnDestroy();
}
```

---

## 설치가 안 맞을 때 빠른 점검

:::info[Case 1]
- `R3` **네임스페이스를 찾지 못함**
  - `com.cysharp.r3` **참조 확인**
  - `R3` **설치 여부 확인**
:::

:::info[Case 2]
- `R3Awake`, `R3OnDestroy`**를 찾지 못함**
  - `partial` **누락 여부 확인**
  - [Attribute](/core/attributes) **적용 여부 확인**
:::

:::info[Case 3]
- `R3Gen` **오류 다수 발생**
  - `docs/core/diagnostics` **문서의 코드별 조치 순서대로 점검**
:::

---

## 라이선스

:::info

MIT License

Copyright (c) 2026 NightWish

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

:::