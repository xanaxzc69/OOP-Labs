using System;
using ClinicApp.Utils;

namespace ClinicApp.Models;

// Задача 2. Рецепт: препарат, дозування і тривалість курсу
public class Prescription : MedicalRecord
{
    private string _medicationName;
    private string _dosage;
    private int _durationDays;

    public string MedicationName
    {
        get { return _medicationName; }
        set
        {
            ClinicValidator.ValidateName(value, "Назва препарату");
            _medicationName = value.Trim();
        }
    }

    public string Dosage
    {
        get { return _dosage; }
        set
        {
            ClinicValidator.ValidateName(value, "Дозування");
            _dosage = value.Trim();
        }
    }

    public int DurationDays
    {
        get { return _durationDays; }
        set
        {
            ClinicValidator.ValidatePositive(value, "Тривалість курсу");
            _durationDays = value;
        }
    }

    // Необов'язкове поле, тому не валідується
    public string Instructions { get; set; }

    // Обчислювана властивість: дата закінчення курсу
    public DateTime ExpiresAt => Date.AddDays(DurationDays);

    public Prescription(int patientId, int doctorId, DateTime date, string medicationName,
                        string dosage, int durationDays, string instructions = "", string notes = "")
        : base(patientId, doctorId, date, notes)
    {
        MedicationName = medicationName;
        Dosage = dosage;
        DurationDays = durationDays;
        Instructions = instructions == null ? "" : instructions;
    }

    public override string GetSummary()
    {
        string result = $"{MedicationName} {Dosage} × {DurationDays} днів";
        if (Instructions.Length > 0)
        {
            result += $" ({Instructions})";
        }
        return result;
    }

    public override string GetRecordType()
    {
        return "Рецепт";
    }

    // Рецепт активний, поки не закінчився курс, — правило 6 місяців тут не діє
    public override bool IsActive()
    {
        return ExpiresAt >= DateTime.Today;
    }
}
