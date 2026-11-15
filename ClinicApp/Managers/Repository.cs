using System;
using System.Collections.Generic;
using ClinicApp.Interfaces;

namespace ClinicApp.Managers;

// Задача 4 (Lab09). Узагальнене сховище з CRUD-операціями.
//
// where T : IIdentifiable — обмеження параметра типу. Без нього рядок
// item.Id не скомпілювався б: компілятор не знав би, що в довільного T
// узагалі є така властивість. Обмеження перетворює T із «будь-чого»
// на «будь-що, в чого точно є Id», і саме це дає змогу написати
// GetById() та Remove() один раз для всіх сутностей клініки.
//
// WaitingQueue<T> із Задачі 2 обмежень не має: черзі байдуже, що саме
// в ній лежить, бо вона лише дотримується порядку FIFO. Щойно класу
// треба зазирнути всередину елемента — з'являється обмеження.
public class Repository<T> where T : IIdentifiable
{
    private List<T> _items = new List<T>();

    public int Count => _items.Count;

    // ---------- Create ----------

    // Повертає false, якщо елемент порожній або такий Id уже зайнятий:
    // ідентифікатор має лишатися унікальним у межах сховища
    public bool Add(T item)
    {
        if (ReferenceEquals(item, null)) return false;

        if (Contains(item.Id))
        {
            Console.WriteLine($"Елемент з Id {item.Id} уже є у сховищі.");
            return false;
        }

        _items.Add(item);
        return true;
    }

    // ---------- Read ----------

    // Якщо елемента немає, повертаємо default(T) — для класів це null.
    // Виняток тут недоречний: «не знайдено» — звичайний результат пошуку,
    // а не помилка в роботі програми.
    public T GetById(int id)
    {
        foreach (T item in _items)
        {
            if (item.Id == id) return item;
        }
        return default(T);
    }

    // Безпечний варіант для випадків, коли null-перевірка незручна
    public bool TryGetById(int id, out T item)
    {
        foreach (T candidate in _items)
        {
            if (candidate.Id == id)
            {
                item = candidate;
                return true;
            }
        }
        item = default(T);
        return false;
    }

    public bool Contains(int id)
    {
        foreach (T item in _items)
        {
            if (item.Id == id) return true;
        }
        return false;
    }

    // Копія вмісту: зовнішній код не повинен мати доступу до самого списку
    public List<T> GetAll()
    {
        return new List<T>(_items);
    }

    // ---------- Delete ----------

    // bool як відповідь на питання «чи вдалося видалити»:
    // true — елемент був і його прибрано, false — такого Id у сховищі немає
    public bool Remove(int id)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Id == id)
            {
                _items.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        _items.Clear();
    }

    // ---------- Вивід ----------

    public void DisplayAll(string title = "Сховище")
    {
        Console.WriteLine($"=== {title} ({_items.Count}) ===");
        if (_items.Count == 0)
        {
            Console.WriteLine("Порожньо.");
            return;
        }
        foreach (T item in _items)
        {
            Console.WriteLine(item);
        }
    }
}
