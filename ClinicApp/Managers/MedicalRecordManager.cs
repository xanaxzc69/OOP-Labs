using System;
using ClinicApp.Models;

namespace ClinicApp.Managers;

// Задача 2. Поліморфний масив: в одному масиві MedicalRecord[]
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
