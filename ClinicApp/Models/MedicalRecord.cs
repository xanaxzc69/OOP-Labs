using System;
using ClinicApp.Utils;

namespace ClinicApp.Models;

// Задача 1. Спільний контракт для всіх медичних записів.
// Клас абстрактний: створити new MedicalRecord(...) неможливо —
// існують лише конкретні нащадки (діагноз, аналіз, рецепт).
public abstract class MedicalRecord
{
    private static int _nextId = 1;

    public int Id { get; }
    public int PatientId { get; }
    public int DoctorId { get; }
    public DateTime Date { get; }
    public string Notes { get; set; }

    // protected: конструктор доступний лише нащадкам — ззовні його не викликати
    protected MedicalRecord(int patientId, int doctorId, DateTime date, string notes)
    {
        ClinicValidator.ValidatePositive(patientId, "Код пацієнта");
        ClinicValidator.ValidatePositive(doctorId, "Код лікаря");

        PatientId = patientId;
        DoctorId = doctorId;
        Date = date;
        Notes = notes == null ? "" : notes;

        // Id присвоюється останнім: якщо валідація вище кине виняток,
        // номер не витратиться на об'єкт, який так і не створився
        Id = _nextId++;
    }

    // Кожен нащадок зобов'язаний описати свій зміст по-своєму
    public abstract string GetSummary();

    // Базова реалізація, яку нащадок може перевизначити
    public virtual string GetRecordType()
    {
        return "Медичний запис";
    }

    // Запис вважається активним, якщо він не старший за 6 місяців
    public virtual bool IsActive()
    {
        return Date >= DateTime.Today.AddMonths(-6);
    }

    public override string ToString()
    {
        return $"[{Id}] {GetRecordType()}: {GetSummary()} | {Date.ToString("dd.MM.yyyy")}";
    }
}
