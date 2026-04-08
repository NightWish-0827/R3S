using System;

namespace R3.Attributes
{
    /// <summary>
    /// Specify the AddTo target for the [AutoSubscribe] attribute.
    /// </summary>
    public enum AddTo
    {
        /// <summary>
        /// AddTo(_disposable) — Use CompositeDisposable (Default)
        /// [AutoDispose] Attribute must be on the class.
        /// </summary>
        Disposable,

        /// <summary>
        /// AddTo(destroyCancellationToken) — Only for Unity 2022+ MonoBehaviour
        /// [AutoDispose] is not required.
        /// </summary>
        CancellationToken,

        /// <summary>
        /// AddTo(this) — Pass the MonoBehaviour instance directly
        /// [AutoDispose] is not required.
        /// </summary>
        MonoBehaviour,
    }
}
