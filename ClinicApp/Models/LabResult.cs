using System;
using ClinicApp.Utils;

namespace ClinicApp.Models;

// Задача 2. Результат аналізу: числове значення, одиниці виміру,
// норма й ознака «в нормі»
public class LabResult : MedicalRecord
{
    private string _testName;
    private string _unit;
    private string _referenceRange;

    public string TestName
    {
        get { return _testName; }
        set
        {
            ClinicValidator.ValidateName(value, "Назва аналізу");
            _testName = value.Trim();
        }
    }

    public string Unit
    {
        get { return _unit; }
        set
        {
            ClinicValidator.ValidateName(value, "Одиниця виміру");
            _unit = value.Trim();
        }
    }

    public string ReferenceRange
    {
        get { return _referenceRange; }
        set
        {
            ClinicValidator.ValidateName(value, "Норма");
            _referenceRange = value.Trim();
        }
    }

    // Автовластивості без валідації: значення може бути будь-яким,
    // у тому числі від'ємним або нульовим
    public double Value { get; set; }
    public bool IsNormal { get; set; }

    public LabResult(int patientId, int doctorId, DateTime date, string testName,
                     double value, string unit, string referenceRange, bool isNormal, string notes = "")
        : base(patientId, doctorId, date, notes)
    {
        TestName = testName;
        Value = value;
        Unit = unit;
        ReferenceRange = referenceRange;
        IsNormal = isNormal;
    }

    public override string GetSummary()
    {
        string result = $"{TestName}: {Value} {Unit} (норма: {ReferenceRange})";
        if (!IsNormal)
        {
            result += " ⚠ поза нормою";
        }
        return result;
    }

    public override string GetRecordType()
    {
        return "Аналіз";
    }
}
