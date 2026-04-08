using System;

namespace R3.Attributes
{
    /// <summary>
    /// When applied to a field, the Generator will generate the following:
    ///
    /// Generated result:
    ///   public ReadOnlyReactiveProperty<T> Field => _field;  (ReadOnly=true, Default)
    ///   public ReactiveProperty<T> Field => _field;           (ReadOnly=false)
    ///
    /// Usage example:
    ///   [ReactiveProperty]
    ///   private ReactiveProperty<int> _hp = new(100);
    ///
    ///   [ReactiveProperty(ReadOnly = false)]
    ///   private ReactiveProperty<int> _mp = new();
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ReactivePropertyAttribute : Attribute
    {
        /// <summary>
        /// true  → public ReadOnlyReactiveProperty<T> property exposed (Default)
        /// false → public ReactiveProperty<T> property exposed
        /// </summary>
        public bool ReadOnly { get; set; } = true;
    }
}