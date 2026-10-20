using System;
using ClinicApp.Models;

namespace ClinicApp.Managers;

// Задачі 2-3. Поліморфний масив: в одному масиві MedicalRecord[]
// живуть діагнози, аналізи й рецепти. Менеджер не знає, який саме
// об'єкт лежить у комірці, — кожен запис сам показує свій рядок.
public class MedicalRecordManager
{
    private const int MaxRecords = 1000;

    private MedicalRecord[] _records = new MedicalRecord[MaxRecords];
    private int _count = 0;

    public int Count => _count;

    // Індексатор — доступ до запису за порядковим номером
    public MedicalRecord this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) return null;
            return _records[index];
        }
    }

    // ---------- Create ----------
    public bool Add(MedicalRecord record)
    {
        if (record == null) return false;

        if (_count == MaxRecords)
        {
            Console.WriteLine($"Помилка: досягнуто ліміт у {MaxRecords} медичних записів.");
            return false;
        }

        _records[_count++] = record;
        Console.WriteLine($"Запис [{record.Id}] {record.GetRecordType()} додано.");
        return true;
    }

    // ---------- Read ----------
    public MedicalRecord FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].Id == id) return _records[i];
        }
        return null;
    }

    // Двопрохідний патерн: рахуємо збіги, потім заповнюємо масив точного розміру
    public MedicalRecord[] GetByPatient(int patientId)
    {
        int matches = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId) matches++;
        }

        MedicalRecord[] result = new MedicalRecord[matches];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId) result[index++] = _records[i];
        }
        return result;
    }

    public MedicalRecord[] GetByDoctor(int doctorId)
    {
        int matches = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].DoctorId == doctorId) matches++;
        }

        MedicalRecord[] result = new MedicalRecord[matches];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].DoctorId == doctorId) result[index++] = _records[i];
        }
        return result;
    }

    // ---------- Задача 3. Фільтрація за реальним типом об'єкта ----------
    // is — перевіряє тип, as — безпечно приводить (повертає null замість винятку)

    public Diagnosis[] GetDiagnoses(int patientId)
    {
        MedicalRecord[] records = GetByPatient(patientId);

        int matches = 0;
        for (int i = 0; i < records.Length; i++)
        {
            if (records[i] is Diagnosis) matches++;
        }

        Diagnosis[] result = new Diagnosis[matches];
        int index = 0;
        for (int i = 0; i < records.Length; i++)
        {
            Diagnosis diagnosis = records[i] as Diagnosis;
            if (diagnosis != null) result[index++] = diagnosis;
        }
        return result;
    }

    public LabResult[] GetLabResults(int patientId)
    {
        MedicalRecord[] records = GetByPatient(patientId);

        int matches = 0;
        for (int i = 0; i < records.Length; i++)
        {
            if (records[i] is LabResult) matches++;
        }

        LabResult[] result = new LabResult[matches];
        int index = 0;
        for (int i = 0; i < records.Length; i++)
        {
            LabResult labResult = records[i] as LabResult;
            if (labResult != null) result[index++] = labResult;
        }
        return result;
    }

    public Prescription[] GetPrescriptions(int patientId)
    {
        MedicalRecord[] records = GetByPatient(patientId);

        int matches = 0;
        for (int i = 0; i < records.Length; i++)
        {
            if (records[i] is Prescription) matches++;
        }

        Prescription[] result = new Prescription[matches];
        int index = 0;
        for (int i = 0; i < records.Length; i++)
        {
            Prescription prescription = records[i] as Prescription;
            if (prescription != null) result[index++] = prescription;
        }
        return result;
    }

    // Лише хронічні діагнози
    public Diagnosis[] GetChronicDiagnoses(int patientId)
    {
        Diagnosis[] diagnoses = GetDiagnoses(patientId);

        int matches = 0;
        for (int i = 0; i < diagnoses.Length; i++)
        {
            if (diagnoses[i].IsChronic) matches++;
        }

        Diagnosis[] result = new Diagnosis[matches];
        int index = 0;
        for (int i = 0; i < diagnoses.Length; i++)
        {
            if (diagnoses[i].IsChronic) result[index++] = diagnoses[i];
        }
        return result;
    }

    // Лише рецепти, курс яких ще не закінчився
    public Prescription[] GetActivePrescriptions(int patientId)
    {
        Prescription[] prescriptions = GetPrescriptions(patientId);

        int matches = 0;
        for (int i = 0; i < prescriptions.Length; i++)
        {
            if (prescriptions[i].IsActive()) matches++;
        }

        Prescription[] result = new Prescription[matches];
        int index = 0;
        for (int i = 0; i < prescriptions.Length; i++)
        {
            if (prescriptions[i].IsActive()) result[index++] = prescriptions[i];
        }
        return result;
    }

    // Зведена медична картка пацієнта
    public void DisplayPatientSummary(int patientId)
    {
        Diagnosis[] diagnoses = GetDiagnoses(patientId);
        LabResult[] labResults = GetLabResults(patientId);
        Prescription[] prescriptions = GetPrescriptions(patientId);

        Console.WriteLine($"=== Медична картка пацієнта #{patientId} ===");
        Console.WriteLine($"Діагнозів: {diagnoses.Length}, аналізів: {labResults.Length}, рецептів: {prescriptions.Length}");

        Diagnosis[] chronic = GetChronicDiagnoses(patientId);
        if (chronic.Length > 0)
        {
            Console.WriteLine("Хронічні діагнози:");
            for (int i = 0; i < chronic.Length; i++)
            {
                Console.WriteLine($"  - {chronic[i].GetSummary()}");
            }
        }

        Prescription[] active = GetActivePrescriptions(patientId);
        if (active.Length > 0)
        {
            Console.WriteLine("Активні рецепти:");
            for (int i = 0; i < active.Length; i++)
            {
                Console.WriteLine($"  - {active[i].GetSummary()} (до {active[i].ExpiresAt.ToString("dd.MM.yyyy")})");
            }
        }
    }

    // ---------- Вивід ----------

    // Поліморфізм у чистому вигляді: цикл один, а рядок кожного запису
    // формує той клас, об'єкт якого реально лежить у комірці
    public void DisplayAll()
    {
        Console.WriteLine($"=== Медичні записи ({_count} / {MaxRecords}) ===");
        if (_count == 0)
        {
            Console.WriteLine("Записів немає.");
            return;
        }
        for (int i = 0; i < _count; i++)
        {
            Console.WriteLine(_records[i]);
        }
    }

    public void DisplayList(MedicalRecord[] list)
    {
        if (list == null || list.Length == 0)
        {
            Console.WriteLine("Записів не знайдено.");
            return;
        }
        for (int i = 0; i < list.Length; i++)
        {
            Console.WriteLine(list[i]);
        }
    }
}
