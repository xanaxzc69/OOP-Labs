using System;
using ClinicApp.Attributes;
using ClinicApp.Enums;

namespace ClinicApp.Models;

// Задача 2 (Lab11). План лікування пацієнта.
//
// Головна відмінність від попередніх моделей: правила перевірки полів
// тут НЕ написані в сеттерах, а описані атрибутами над властивостями.
// Сам клас не містить жодного рядка валідації — її виконує зовнішній
// ModelValidator, читаючи ці атрибути через рефлексію. Завдяки цьому
// правило видно одразу поруч із полем, а не десь у тілі сеттера.
//
// Стан плану змінюється лише методами переходу: властивість Status
// має приватний сеттер, тому ззовні призначити довільний стан неможливо.
public class TreatmentPlan
{
    private static int _nextId = 1;

    // Id призначається лічильником у конструкторі без параметрів
    public int Id { get; private set; }

    // Для числа правило «обов'язкове» безглузде: int завжди має значення.
    // Осмислене обмеження тут — нижня межа: ID пацієнта починається з 1.
    [MinValue(1)]
    public int PatientId { get; set; }

    [Required("Діагноз обов'язковий.")]
    [MaxLength(200)]
    public string Diagnosis { get; set; } = "";

    [MaxLength(500)]
    public string Description { get; set; } = "";

    [MinValue(1)]
    public int DurationDays { get; set; } = 1;

    [MinValue(0)]
    public decimal Cost { get; set; }

    // Дата початку лікування; атрибутів не має — будь-яка дата допустима
    public DateTime StartDate { get; set; } = DateTime.Today;

    // Змінюється лише через Activate/Complete/Cancel
    public TreatmentStatus Status { get; private set; } = TreatmentStatus.Planned;

    // Конструктор без параметрів обов'язковий: саме його вимагає
    // обмеження where T : new() у FormBuilder із Задачі 4
    public TreatmentPlan()
    {
        Id = _nextId++;
    }

    // Зручний конструктор для початкових даних
    public TreatmentPlan(int patientId, string diagnosis, int durationDays, decimal cost) : this()
    {
        PatientId = patientId;
        Diagnosis = diagnosis;
        DurationDays = durationDays;
        Cost = cost;
    }

    public DateTime EndDate => StartDate.AddDays(DurationDays);

    // ---------- Переходи стану ----------
    //
    // Кожен метод повертає bool — чи справді перехід відбувся.
    // Недопустимий перехід не є аварією: план просто лишається
    // в попередньому стані, а викликач отримує false і сам вирішує,
    // що про це сказати користувачеві.

    // Почати лікування можна лише з плану, який ще не розпочато
    public bool Activate()
    {
        if (Status != TreatmentStatus.Planned) return false;

        Status = TreatmentStatus.Active;
        return true;
    }

    // Завершити можна лише те, що вже триває
    public bool Complete()
    {
        if (Status != TreatmentStatus.Active) return false;

        Status = TreatmentStatus.Completed;
        return true;
    }

    // Скасувати можна і ще не розпочатий, і активний план,
    // але не завершений і не скасований раніше
    public bool Cancel()
    {
        if (Status == TreatmentStatus.Completed) return false;
        if (Status == TreatmentStatus.Cancelled) return false;

        Status = TreatmentStatus.Cancelled;
        return true;
    }

    public override string ToString()
    {
        string line = $"[{Id}] Пацієнт #{PatientId} | {Diagnosis} | {Status} | " +
                      $"{StartDate.ToString("dd.MM.yyyy")}–{EndDate.ToString("dd.MM.yyyy")} " +
                      $"({DurationDays} дн.) | {Cost} грн";

        if (Description.Length > 0)
        {
            line += " | " + Description;
        }
        return line;
    }
}
