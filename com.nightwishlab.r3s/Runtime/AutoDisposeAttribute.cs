using System;

namespace R3.Attributes
{
    /// <summary>
    /// 클래스에 붙이면 Generator가 다음을 생성합니다.
    ///
    /// 생성 결과 (MonoBehaviour):
    ///   private CompositeDisposable _disposable = new();
    ///   private void R3OnDestroy() { _disposable.Dispose(); }
    ///
    /// 생성 결과 (일반 클래스):
    ///   private CompositeDisposable _disposable = new();
    ///   public void Dispose() { _disposable.Dispose(); }
    ///   + 클래스 선언에 : IDisposable 추가
    ///
    /// MonoBehaviour 사용 규칙:
    ///   - partial 클래스 필수 (R3Gen001)
    ///   - OnDestroy()를 반드시 선언해야 함 (R3Gen009)
    ///   - OnDestroy() 안에서 R3OnDestroy()를 반드시 호출해야 함 (R3Gen010)
    ///   - [AutoSubscribe] 사용 시 이 어트리뷰트가 함께 있어야 함 (R3Gen005)
    ///
    /// Usage example:
    ///   [AutoDispose]
    ///   public partial class PlayerViewModel : MonoBehaviour
    ///   {
    ///       private void OnDestroy() => R3OnDestroy();
    ///   }
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AutoDisposeAttribute : Attribute { }
}