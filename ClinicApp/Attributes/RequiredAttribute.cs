using System;

namespace ClinicApp.Attributes;

// Задача 1 (Lab11). Власний атрибут: «поле обов'язкове до заповнення».
//
// Атрибут — це метадані, які чіпляються до елемента коду й зберігаються
// у збірці. Сам по собі він нічого не робить: правило лише ОПИСАНО,
// а виконує його той, хто прочитає атрибут через рефлексію
// (у цій роботі — ModelValidator із Задачі 3).
//
// [AttributeUsage] — це атрибут, що описує інший атрибут:
//   AttributeTargets.Property — ставити можна лише над властивостями;
//                               над класом чи методом компілятор не дозволить;
//   AllowMultiple = false     — не більше одного разу на одну властивість;
//   Inherited = true          — атрибут видно й у похідних класах.
//
// sealed — від атрибута не успадковуються: правило закінчене, і дозволяти
// створювати «Required з особливостями» не потрібно.
//
// За домовленістю класу дають суфікс Attribute, а в коді пишуть коротко:
// оголошення RequiredAttribute використовується як [Required].
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class RequiredAttribute : Attribute
{
    // Лише get: параметр задається один раз у квадратних дужках
    // при застосуванні атрибута і далі не змінюється
    public string Message { get; }

    public RequiredAttribute()
    {
        Message = "";
    }

    // Необов'язковий власний текст помилки: [Required("Діагноз обов'язковий")]
    public RequiredAttribute(string message)
    {
        Message = message ?? "";
    }

    // Текст помилки для конкретної властивості: або заданий автором,
    // або стандартний
    public string BuildError(string propertyName)
    {
        if (Message.Length > 0) return Message;
        return $"Поле «{propertyName}» обов'язкове до заповнення.";
    }
}
