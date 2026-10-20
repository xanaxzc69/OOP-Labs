using System;
using ClinicApp.Utils;

namespace ClinicApp.Models;

// Задача 1. Перший конкретний нащадок: діагноз
public class Diagnosis : MedicalRecord
{
    private string _diagnosisCode;
    private string _description;

    public string DiagnosisCode
    {
        get { return _diagnosisCode; }
        set
        {
            ClinicValidator.ValidateName(value, "Код діагнозу");
            _diagnosisCode = value.Trim();
        }
    }

    public string Description
    {
        get { return _description; }
        set
        {
            ClinicValidator.ValidateName(value, "Опис діагнозу");
            _description = value.Trim();
        }
    }

    public bool IsChronic { get; set; }

    // base(...) передає спільні поля батьківському конструктору,
    // власні поля присвоюються через властивості, тож валідація спрацьовує
    public Diagnosis(int patientId, int doctorId, DateTime date,
                     string diagnosisCode, string description, bool isChronic, string notes = "")
        : base(patientId, doctorId, date, notes)
    {
        DiagnosisCode = diagnosisCode;
        Description = description;
        IsChronic = isChronic;
    }

    public override string GetSummary()
    {
        string result = $"{DiagnosisCode}: {Description}";
        if (IsChronic)
        {
            result += " [хронічне]";
        }
        return result;
    }

    public override string GetRecordType()
    {
        return "Діагноз";
    }
}
