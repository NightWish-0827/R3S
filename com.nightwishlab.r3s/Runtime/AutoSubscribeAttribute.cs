using System;

namespace R3.Attributes
{
    /// <summary>
    /// When applied to a method, the Generator will generate Subscribe + AddTo statements inside R3Awake() / R3Initialize().
    /// Generate Subscribe + AddTo statements inside R3Awake() / R3Initialize().
    ///
    /// Example of generated result:
    ///   _hp.Subscribe(OnHpChanged).AddTo(_disposable);           // AddTo.Disposable
    ///   _hp.Subscribe(OnHpChanged).AddTo(destroyCancellationToken); // AddTo.CancellationToken
    ///   _hp.Subscribe(OnHpChanged).AddTo(this);                  // AddTo.MonoBehaviour
    ///
    /// Rules:
    ///   - AddTo.Disposable (Default) When used, [AutoDispose] is required (R3Gen005)
    ///   - AddTo.CancellationToken / MonoBehaviour When used, [AutoDispose] is not required
    ///   - If Awake() is not present, an error will occur (R3Gen007)
    ///   - If R3Awake() is not called inside Awake(), an error will occur (R3Gen008)
    ///   - If the target field is not present, an error will occur (R3Gen003)
    ///   - If the method parameter type does not match, an error will occur (R3Gen004)
    ///
    /// Usage example:
    ///   [AutoSubscribe(nameof(_hp))]
    ///   [AutoSubscribe(nameof(_hp), AddTo.Disposable)]
    ///   [AutoSubscribe(nameof(_hp), AddTo.CancellationToken)]
    ///   [AutoSubscribe(nameof(_hp), AddTo.MonoBehaviour)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AutoSubscribeAttribute : Attribute
    {
        /// <summary>The name of the ReactiveProperty or Subject field to subscribe to. Recommended to use nameof().</summary>
        public string TargetField { get; }

        /// <summary>AddTo target. The default is AddTo.Disposable.</summary>
        public AddTo AddTo { get; }

        public AutoSubscribeAttribute(string targetField, AddTo addTo = AddTo.Disposable)
        {
            TargetField = targetField;
            AddTo       = addTo;
        }
    }
}