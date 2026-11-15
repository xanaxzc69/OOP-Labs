using System;
using System.Collections.Generic;
using ClinicApp.Models;
using ClinicApp.Enums;

namespace ClinicApp.Managers;

// Задача 1 (Lab09). Внутрішнє сховище — узагальнена колекція List<Patient>
// замість масиву фіксованого розміру з ручним лічильником.
//
// Що це змінило всередині:
//   - зник штучний ліміт MaxPatients: список росте сам;
//   - зник лічильник _count: список сам знає свій розмір;
//   - у Remove() більше немає ручного зсуву елементів ліворуч;
//   - пошукові методи більше не роблять два проходи (порахувати збіги,
//     потім заповнити масив точного розміру) — вони накопичують результат
//     у проміжний List<Patient> і в кінці віддають ToArray().
//
// Зовнішній API не змінився: Add, FindById, TryFindById, GetAll, FindByName,
// FindByBloodType, Remove, Count, індексатор, DisplayAll, DisplayStats
// працюють так само, тому решта програми заміни навіть не помічає.
public class PatientManager
{
    // List<T> — типізована колекція: у неї не можна покласти нічого,
    // крім Patient, і при читанні не потрібне приведення типу.
    private List<Patient> _patients = new List<Patient>();

    public int Count => _patients.Count;

    // Індексатор: clinic.Patients[2]. Поведінка збережена —
    // при виході за межі повертаємо null, а не кидаємо виняток.
    public Patient this[int index]
    {
        get
        {
            if (index < 0 || index >= _patients.Count) return null;
            return _patients[index];
        }
    }

    // ---------- Create ----------
    // Перевірки ліміту більше немає: колекція розширюється автоматично
    public bool Add(Patient patient)
    {
        if (patient == null) return false;

        _patients.Add(patient);
        Console.WriteLine($"Пацієнта [{patient.Id}] {patient.FullName} додано.");
        return true;
    }

    // ---------- Read ----------
    // Копію назовні віддаємо масивом, щоб зовнішній код не міг
    // змінити внутрішній список клініки
    public Patient[] GetAll()
    {
        return _patients.ToArray();
    }

    // Лінійний пошук; null, якщо не знайдено
    public Patient FindById(int id)
    {
        foreach (Patient patient in _patients)
        {
            if (patient.Id == id) return patient;
        }
        return null;
    }

    // Патерн TryXxx: метод повертає bool (успіх/невдача),
    // а сам знайдений об'єкт віддає через параметр out.
    public bool TryFindById(int id, out Patient patient)
    {
        foreach (Patient candidate in _patients)
        {
            if (candidate.Id == id)
            {
                patient = candidate;
                return true;
            }
        }
        patient = null;   // out-параметр треба присвоїти на всіх шляхах виконання
        return false;
    }

    // Пошук за групою крові: один прохід, результат накопичується у списку
    public Patient[] FindByBloodType(BloodType bloodType)
    {
        List<Patient> found = new List<Patient>();
        foreach (Patient patient in _patients)
        {
            if (patient.BloodType == bloodType) found.Add(patient);
        }
        return found.ToArray();
    }

    // Пошук за частиною імені або прізвища — теж один прохід
    public Patient[] FindByName(string query)
    {
        List<Patient> found = new List<Patient>();
        if (query == null) return found.ToArray();

        string q = query.ToLower();
        foreach (Patient patient in _patients)
        {
            if (patient.FirstName.ToLower().Contains(q) || patient.LastName.ToLower().Contains(q))
            {
                found.Add(patient);
            }
        }
        return found.ToArray();
    }

    // ---------- Delete ----------
    // Ручний зсув елементів більше не потрібен — список робить це сам
    public bool Remove(int id)
    {
        Patient patient = FindById(id);
        if (patient == null) return false;

        return _patients.Remove(patient);
    }

    // ---------- Вивід ----------
    public void DisplayAll()
    {
        Console.WriteLine($"=== Пацієнти ({_patients.Count}) ===");
        if (_patients.Count == 0)
        {
            Console.WriteLine("Список порожній.");
            return;
        }
        foreach (Patient patient in _patients)
        {
            Console.WriteLine(patient);
        }
    }

    // Уся статистика обчислюється за ОДИН прохід по колекції
    public void DisplayStats()
    {
        Console.WriteLine("=== Статистика пацієнтів ===");
        if (_patients.Count == 0)
        {
            Console.WriteLine("Список порожній.");
            return;
        }

        int totalAge = 0;
        Patient youngest = _patients[0];
        Patient oldest = _patients[0];
        int adultsCount = 0;

        foreach (Patient patient in _patients)
        {
            int age = patient.Age;
            totalAge += age;
            if (age < youngest.Age) youngest = patient;
            if (age > oldest.Age) oldest = patient;
            if (patient.IsAdult) adultsCount++;
        }

        double averageAge = (double)totalAge / _patients.Count;

        Console.WriteLine($"Всього: {_patients.Count}");
        Console.WriteLine($"Середній вік: {averageAge:F1} р.");
        Console.WriteLine($"Наймолодший: {youngest.FullName} ({youngest.Age} р.)");
        Console.WriteLine($"Найстарший: {oldest.FullName} ({oldest.Age} р.)");
        Console.WriteLine($"Дорослих: {adultsCount} з {_patients.Count}");
    }
}
