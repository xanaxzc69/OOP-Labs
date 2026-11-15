using System;

namespace ClinicApp.Attributes;

// Задача 1 (Lab11). Власний атрибут: «число не менше за задану межу».
//
// Тип параметра — double, бо в параметрах атрибута можна використовувати
// лише константні значення обмеженого набору типів, і decimal серед них
// немає. Тому межа зберігається як double, а при перевірці значення
// властивості (int, decimal чи double) приводиться до double.
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class MinValueAttribute : Attribute
{
    public double Minimum { get; }

    public MinValueAttribute(double minimum)
    {
        Minimum = minimum;
    }

    public string BuildError(string propertyName, double actualValue)
    {
        return $"Поле «{propertyName}»: значення не може бути меншим за {Minimum}, зараз {actualValue}.";
    }
}
