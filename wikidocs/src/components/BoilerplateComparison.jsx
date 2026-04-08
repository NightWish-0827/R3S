import React from 'react';
import CodeBlock from '@theme/CodeBlock';
import styles from './BoilerplateComparison.module.css';

const legacyCode = `public class PlayerViewModel : MonoBehaviour
{
    private readonly ReactiveProperty<int> _hp = new(100);
    public ReadOnlyReactiveProperty<int> Hp => _hp;

    private readonly ReactiveProperty<int> _mp = new();
    public ReactiveProperty<int> Mp => _mp;

    private readonly Subject<Unit> _onDead = new();
    public Observable<Unit> OnDead => _onDead;

    private readonly ReactiveCommand<Unit> _attack = new();
    public Observable<Unit> Attack => _attack;
    public void ExecuteAttack() => _attack.Execute(Unit.Default);

    private CompositeDisposable _disposable = new();

    private void Awake()
    {
        _hp.Subscribe(OnHpChanged).AddTo(_disposable);
    }

    private void OnHpChanged(int hp) => Debug.Log(hp);
    private void OnDestroy() => _disposable.Dispose();
}`;

const r3sCode = `[AutoDispose]
public partial class PlayerViewModel : MonoBehaviour
{
    [ReactiveProperty]
    private ReactiveProperty<int> _hp = new(100);

    [ReactiveProperty(ReadOnly = false)]
    private ReactiveProperty<int> _mp = new();

    [Subject]
    private Subject<Unit> _onDead = new();

    [ReactiveCommand]
    private ReactiveCommand<Unit> _attack = new();

    [AutoSubscribe(nameof(_hp))]
    private void OnHpChanged(int hp) => Debug.Log(hp);

    private void Awake() => R3Awake();
    private void OnDestroy() => R3OnDestroy();
}`;

const generatedCode = `// PlayerViewModel.R3Generated.cs (auto-generated)
public partial class PlayerViewModel
{
    private CompositeDisposable _disposable = new();

    public ReadOnlyReactiveProperty<int> Hp => _hp;
    public ReactiveProperty<int> Mp => _mp;
    public Observable<Unit> OnDead => _onDead;
    public Observable<Unit> Attack => _attack;
    public void ExecuteAttack() => _attack.Execute(Unit.Default);

    private void R3Awake()
    {
        _hp.Subscribe(OnHpChanged).AddTo(_disposable);
    }

    private void R3OnDestroy() => _disposable.Dispose();
}`;

export default function BoilerplateComparison() {
  return (
    <section className={styles.wrap}>
      <h3 className={styles.title}>동일한 ViewModel 코드 비교</h3>

      <div className={styles.cols}>
        <article className={styles.panel}>
          <div className={styles.head}>
            <span className={`${styles.badge} ${styles.old}`}>기존</span>
            보일러플레이트 방식
          </div>
          <div className={styles.codeWrap}>
            <CodeBlock language="csharp" title="Before.cs">
              {legacyCode}
            </CodeBlock>
          </div>
        </article>

        <article className={styles.panel}>
          <div className={styles.head}>
            <span className={`${styles.badge} ${styles.new}`}>R3S</span>
            Attribute 선언 방식
          </div>
          <div className={styles.codeWrap}>
            <CodeBlock language="csharp" title="After.cs">
              {r3sCode}
            </CodeBlock>
          </div>
        </article>
      </div>

      <h3 className={styles.title}>Generator 자동 생성 코드</h3>
      <div className={`${styles.panel} ${styles.full}`}>
        <div className={styles.codeWrap}>
          <CodeBlock language="csharp" title="PlayerViewModel.R3Generated.cs">
            {generatedCode}
          </CodeBlock>
        </div>
      </div>

      <h3 className={styles.title}>효율 지표</h3>
      <div className={styles.stats} role="list">
        <div className={styles.statCard}>
          <div className={`${styles.num} ${styles.red}`}>38</div>
          <div className={styles.label}>기존 방식 라인 수</div>
        </div>
        <div className={styles.statCard}>
          <div className={`${styles.num} ${styles.green}`}>16</div>
          <div className={styles.label}>R3S 작성 라인 수</div>
        </div>
        <div className={styles.statCard}>
          <div className={`${styles.num} ${styles.green}`}>-58%</div>
          <div className={styles.label}>코드 감소율</div>
        </div>
        <div className={styles.statCard}>
          <div className={`${styles.num} ${styles.green}`}>12</div>
          <div className={styles.label}>진단 코드 수 (R3Gen)</div>
        </div>
      </div>
    </section>
  );
}
