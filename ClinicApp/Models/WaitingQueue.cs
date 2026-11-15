using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

// Задача 2 (Lab09). Власний узагальнений клас-черга.
//
// <T> — параметр типу: клас написаний один раз, а працювати може з будь-яким
// типом елементів. WaitingQueue<Patient> зберігає пацієнтів, WaitingQueue<Doctor>
// зберігала б лікарів, і компілятор у кожному випадку не дасть покласти
// в чергу щось стороннє.
//
// Усередині — готова структура Queue<T> зі стандартної бібліотеки, яка
// реалізує принцип FIFO (first in — first out: перший прийшов, перший вийшов).
// Клас є обгорткою: назовні він дає лише безпечні операції з
// зрозумілими назвами й передбачуваною поведінкою на порожній черзі.
public class WaitingQueue<T>
{
    private Queue<T> _items = new Queue<T>();

    // Скільки елементів зараз у черзі
    public int Count => _items.Count;

    // Зручна ознака: перевіряти IsEmpty читабельніше, ніж Count == 0
    public bool IsEmpty => _items.Count == 0;

    // ---------- Додавання ----------

    // Стати в кінець черги
    public void Enqueue(T item)
    {
        // ReferenceEquals замість item == null: працює для будь-якого T,
        // зокрема й тоді, коли T виявиться структурою
        if (ReferenceEquals(item, null))
            throw new ArgumentNullException(nameof(item), "Не можна поставити в чергу порожнє значення.");

        _items.Enqueue(item);
    }

    // ---------- Отримання ----------

    // Забрати першого з черги (він з черги зникає).
    // Порожня черга — це не збій програми, а ситуація, якої не можна
    // обслужити, тому кидаємо InvalidOperationException зі зрозумілим текстом.
    public T Dequeue()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Черга порожня: немає кого приймати.");

        return _items.Dequeue();
    }

    // Подивитися, хто перший, але не забирати його з черги
    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Черга порожня: немає кого показати.");

        return _items.Peek();
    }

    // Безпечний варіант без винятку: зручний там, де порожня черга
    // очікувана й обробляється звичайним if
    public bool TryDequeue(out T item)
    {
        if (IsEmpty)
        {
            item = default(T);
            return false;
        }
        item = _items.Dequeue();
        return true;
    }

    // ---------- Решта операцій ----------

    // Чи є елемент у черзі (порівняння через EqualityComparer за замовчуванням)
    public bool Contains(T item)
    {
        return _items.Contains(item);
    }

    // Очистити чергу повністю
    public void Clear()
    {
        _items.Clear();
    }

    // Копія черги у порядку обслуговування: перший елемент масиву —
    // той, кого приймуть наступним. Назовні віддаємо саме копію,
    // щоб зовнішній код не міг змінити внутрішній стан черги.
    public T[] ToArray()
    {
        return _items.ToArray();
    }

    public override string ToString()
    {
        return $"Черга очікування: {Count} у черзі";
    }
}
