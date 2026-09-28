using System;

namespace ClinicApp;

// Задача 3. Колекція пацієнтів на масиві фіксованого розміру
public class PatientManager
{
    private const int MaxPatients = 100;

    private Patient[] _patients = new Patient[MaxPatients];
    private int _count = 0;

    public int Count => _count;

    // Задача 3. Індексатор: clinic.Patients[2] замість FindById(3).
    // Якщо індексу немає — повертаємо null (зручно для ?. та ??).
    public Patient this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) return null;
            return _patients[index];
        }
    }

    // ---------- Create ----------
    public bool Add(Patient patient)
    {
        if (_count == MaxPatients)
        {
            Console.WriteLine($"Помилка: досягнуто ліміт у {MaxPatients} пацієнтів.");
            return false;
        }

        _patients[_count++] = patient;
        Console.WriteLine($"Пацієнта [{patient.Id}] {patient.FullName} додано.");
        return true;
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

    // Лінійний пошук; null, якщо не знайдено
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

    // Двопрохідний патерн: 1) рахуємо збіги, 2) заповнюємо масив точного розміру
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
    // Знаходимо індекс, зсуваємо елементи ліворуч, обнуляємо останній слот
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
        Console.WriteLine($"=== Пацієнти ({_count} / {MaxPatients}) ===");
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

    // Уся статистика обчислюється за ОДИН прохід по масиву
    public void DisplayStats()
    {
        Console.WriteLine("=== Статистика пацієнтів ===");
        if (_count == 0)
        {
            Console.WriteLine("Список порожній.");
            return;
        }

        int totalAge = 0;
        int minIdx = 0;
        int maxIdx = 0;
        int adultsCount = 0;

        for (int i = 0; i < _count; i++)
        {
            int age = _patients[i].Age;
            totalAge += age;
            if (age < _patients[minIdx].Age) minIdx = i;
            if (age > _patients[maxIdx].Age) maxIdx = i;
            if (_patients[i].IsAdult) adultsCount++;
        }

        double averageAge = (double)totalAge / _count;

        Console.WriteLine($"Всього: {_count}");
        Console.WriteLine($"Середній вік: {averageAge:F1} р.");
        Console.WriteLine($"Наймолодший: {_patients[minIdx].FullName} ({_patients[minIdx].Age} р.)");
        Console.WriteLine($"Найстарший: {_patients[maxIdx].FullName} ({_patients[maxIdx].Age} р.)");
        Console.WriteLine($"Дорослих: {adultsCount} з {_count}");
    }
}
