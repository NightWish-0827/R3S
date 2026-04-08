using System;

namespace R3.Attributes
{
    /// <summary>
    /// When applied to a field, the Generator will generate the following:
    ///
    /// Generated result (T == Unit):
    ///   private readonly ReactiveCommand<Unit> _field = new();
    ///   public Observable<Unit> Field => _field;
    ///   public void ExecuteField() => _field.Execute(Unit.Default);
    ///
    /// Generated result (T != Unit):
    ///   private readonly ReactiveCommand<T> _field = new();
    ///   public Observable<T> Field => _field;
    ///   public void ExecuteField(T value) => _field.Execute(value);
    ///
    /// Rules:
    ///   - If the field type is not ReactiveCommand<T>, an error will occur (R3Gen006)
    ///   - The field name must follow the _camelCase convention (R3Gen002)
    ///
    /// Usage example:
    ///   [ReactiveCommand]
    ///   private ReactiveCommand<Unit> _attack;
    ///
    ///   [ReactiveCommand]
    ///   private ReactiveCommand<int> _useItem;
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ReactiveCommandAttribute : Attribute { }
}
