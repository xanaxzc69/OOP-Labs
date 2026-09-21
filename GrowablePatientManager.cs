using System;

namespace ClinicApp;

// Задача 8. Менеджер пацієнтів, у якому масив сам збільшується.
// Жодного наперед заданого ліміту немає: коли масив заповнений
// (_count == довжина масиву), створюємо новий удвічі більший,
// циклом копіюємо в нього всі елементи і далі працюємо з новим.
public class GrowablePatientManager
{
    private const int InitialCapacity = 4;

    private Patient[] _patients = new Patient[InitialCapacity];
    private int _count = 0;

    public int Count => _count;

    // Скільки комірок зараз виділено в пам'яті
    public int Capacity => _patients.Length;

    // ---------- Алгоритм зростання ----------
    private void Grow()
    {
        Patient[] bigger = new Patient[_patients.Length * 2];
        for (int i = 0; i < _count; i++)
        {
            bigger[i] = _patients[i];
        }
        Console.WriteLine($"  (масив збільшено: {_patients.Length} -> {bigger.Length})");
        _patients = bigger;
    }

    // ---------- Create ----------
    public void Add(Patient patient)
    {
        if (_count == _patients.Length)
        {
            Grow();
        }
        _patients[_count++] = patient;
    }

    // ---------- Read ----------
    public Patient[] GetAll()
    {
        Patient[] result = new Patient[_count];
        for (int i = 0; i < _count; i++)
        {
            result[i] = _patients[i];
        }
        return result;
    }

    public Patient FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].Id == id)
            {
                return _patients[i];
            }
        }
        return null;
    }

    public Patient[] FindByName(string query)
    {
        string q = query.ToLower();

        int matches = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].FirstName.ToLower().Contains(q) || _patients[i].LastName.ToLower().Contains(q))
            {
                matches++;
            }
        }

        Patient[] result = new Patient[matches];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].FirstName.ToLower().Contains(q) || _patients[i].LastName.ToLower().Contains(q))
            {
                result[index++] = _patients[i];
            }
        }
        return result;
    }

    // ---------- Delete ----------
    public bool Remove(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].Id == id)
            {
                for (int j = i; j < _count - 1; j++)
                {
                    _patients[j] = _patients[j + 1];
                }
                _patients[_count - 1] = null;
                _count--;
                return true;
            }
        }
        return false;
    }

    // ---------- Вивід ----------
    public void DisplayAll()
    {
        Console.WriteLine($"=== Пацієнти ({_count}, місткість масиву {_patients.Length}) ===");
        if (_count == 0)
        {
            Console.WriteLine("Список порожній.");
            return;
        }
        for (int i = 0; i < _count; i++)
        {
            Console.WriteLine(_patients[i]);
        }
    }
}
