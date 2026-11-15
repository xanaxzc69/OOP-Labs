using System;

namespace ClinicApp.Attributes;

// Задача 1 (Lab11). Власний атрибут: «текст не довший за N символів».
//
// Обмеження передається параметром конструктора: [MaxLength(200)].
// Значення зберігається у властивості лише з get, тому після
// застосування атрибута змінити його неможливо.
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class MaxLengthAttribute : Attribute
{
    public int Length { get; }

    public MaxLengthAttribute(int length)
    {
        // Від'ємна межа не має сенсу — помилку видно одразу при запуску,
        // а не при валідації якогось далекого об'єкта
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length),
                "Максимальна довжина не може бути від'ємною.");

        Length = length;
    }

    public string BuildError(string propertyName, int actualLength)
    {
        return $"Поле «{propertyName}»: максимум {Length} символів, зараз {actualLength}.";
    }
}
