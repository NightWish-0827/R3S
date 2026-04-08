using System;

namespace R3.Attributes
{
    /// <summary>
    /// When applied to a field, the Generator will generate the following:
    ///
    /// Generated result:
    ///   private readonly Subject<T> _field = new();
    ///   public Observable<T> Field => _field;
    ///
    /// Usage example:
    ///   [Subject]
    ///   private Subject<Unit> _onDead;
    ///
    ///   [Subject]
    ///   private Subject<int> _onDamaged;
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class SubjectAttribute : Attribute { }
}
