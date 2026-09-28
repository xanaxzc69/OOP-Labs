using System;

namespace ClinicApp;

// Задача 4. Колекція лікарів — той самий шаблон, що й PatientManager
public class DoctorManager
{
    private const int MaxDoctors = 50;

    private Doctor[] _doctors = new Doctor[MaxDoctors];
    private int _count = 0;

    public int Count => _count;

    // Задача 3. Індексатор — доступ до лікаря за порядковим номером
    public Doctor this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) return null;
            return _doctors[index];
        }
    }

    // ---------- Create ----------
    public bool Add(Doctor doctor)
    {
        if (_count == MaxDoctors)
        {
            Console.WriteLine($"Помилка: досягнуто ліміт у {MaxDoctors} лікарів.");
            return false;
        }

        _doctors[_count++] = doctor;
        Console.WriteLine($"Лікаря [{doctor.Id}] {doctor.FullName} додано.");
        return true;
    }

    // ---------- Read ----------
    // Новий масив розміром _count, копіюємо перші _count елементів
    public Doctor[] GetAll()
    {
        Doctor[] result = new Doctor[_count];
        for (int i = 0; i < _count; i++)
        {
            result[i] = _doctors[i];
        }
        return result;
    }

    public Doctor FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Id == id)
            {
                return _doctors[i];
            }
        }
        return null;
    }

    // Пошук за рядком (часткове співпадіння з назвою спеціальності).
    // Двопрохідний патерн: 1) рахуємо збіги, 2) заповнюємо масив точного розміру
    public Doctor[] FindBySpeciality(string query)
    {
        string q = query.ToLower();

        int matches = 0;
        for (int i = 0; i < _count; i++)
        {
            if (MatchesQuery(_doctors[i].Speciality, q))
            {
                matches++;
            }
        }

        Doctor[] result = new Doctor[matches];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (MatchesQuery(_doctors[i].Speciality, q))
            {
                result[index++] = _doctors[i];
            }
        }
        return result;
    }

    // Задача 4. Перевантаження: та сама назва методу, але параметр — enum.
    // Компілятор сам обирає потрібний варіант за типом аргументу.
    // Тут співпадіння точне, без пошуку за частиною рядка.
    public Doctor[] FindBySpeciality(Speciality speciality)
    {
        int matches = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Speciality == speciality) matches++;
        }

        Doctor[] result = new Doctor[matches];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Speciality == speciality) result[index++] = _doctors[i];
        }
        return result;
    }

    // Задача 4. Патерн TryXxx з параметром out
    public bool TryFindById(int id, out Doctor doctor)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Id == id)
            {
                doctor = _doctors[i];
                return true;
            }
        }
        doctor = null;
        return false;
    }

    // Збіг за назвою enum ("Cardiologist") або за українською назвою ("Кардіологія")
    private static bool MatchesQuery(Speciality speciality, string loweredQuery)
    {
        return speciality.ToString().ToLower().Contains(loweredQuery)
            || ClinicFormatter.FormatSpeciality(speciality).ToLower().Contains(loweredQuery);
    }

    // ---------- Delete ----------
    public bool Remove(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Id == id)
            {
                for (int j = i; j < _count - 1; j++)
                {
                    _doctors[j] = _doctors[j + 1];
                }
                _doctors[_count - 1] = null;
                _count--;
                return true;
            }
        }
        return false;
    }

    // ---------- Вивід ----------
    public void DisplayAll()
    {
        Console.WriteLine($"=== Лікарі ({_count} / {MaxDoctors}) ===");
        if (_count == 0)
        {
            Console.WriteLine("Список порожній.");
            return;
        }
        for (int i = 0; i < _count; i++)
        {
            Console.WriteLine(_doctors[i]);
        }
    }

    public void DisplayStats()
    {
        Console.WriteLine("=== Статистика лікарів ===");

        int availableNow = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].IsAvailableNow)
            {
                availableNow++;
            }
        }

        Console.WriteLine($"Всього: {_count}");
        Console.WriteLine($"Доступні зараз: {availableNow}");
        Console.WriteLine("Спеціальності:");

        // Унікальні спеціальності — три вкладені цикли, без допоміжних масивів:
        // i — поточний лікар; j < i — чи зустрічалась ця спеціальність раніше;
        // k — скільки лікарів має цю спеціальність
        for (int i = 0; i < _count; i++)
        {
            bool seenBefore = false;
            for (int j = 0; j < i; j++)
            {
                if (_doctors[j].Speciality == _doctors[i].Speciality)
                {
                    seenBefore = true;
                    break;
                }
            }
            if (seenBefore)
            {
                continue;
            }

            int specialityCount = 0;
            for (int k = 0; k < _count; k++)
            {
                if (_doctors[k].Speciality == _doctors[i].Speciality)
                {
                    specialityCount++;
                }
            }
            Console.WriteLine($"  {ClinicFormatter.FormatSpeciality(_doctors[i].Speciality)}: {specialityCount}");
        }
    }
}
