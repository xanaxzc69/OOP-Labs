using System;
using ClinicApp.Interfaces;
using ClinicApp.Models;

namespace ClinicApp.Managers;

// Задача 3. Менеджер рахунків.
// Головна ідея: усі розрахунки спираються лише на контракт IPayable.
// Клас не знає, що саме він рахує — прийоми, аналізи чи щось інше,
// що з'явиться пізніше. Достатньо, щоб об'єкт умів показати Amount і IsPaid.
public class BillingManager
{
    private AppointmentManager _appointments;

    public BillingManager(AppointmentManager appointments)
    {
        if (appointments == null)
            throw new ArgumentException("Менеджер записів обов'язковий");

        _appointments = appointments;
    }

    // ---------- Розрахунки на рівні контракту ----------

    // Загальна сума по будь-якому набору оплачуваних об'єктів
    public static decimal CalculateTotal(IPayable[] items)
    {
        if (items == null) return 0;

        decimal total = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;
            total += items[i].Amount;
        }
        return total;
    }

    // Сума несплаченого
    public static decimal CalculateDebt(IPayable[] items)
    {
        if (items == null) return 0;

        decimal debt = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;
            if (!items[i].IsPaid) debt += items[i].Amount;
        }
        return debt;
    }

    // ---------- Робота з записами клініки ----------

    // Усі записи як масив контрактів: далі код бачить лише IPayable
    public IPayable[] GetAllPayables()
    {
        Appointment[] appointments = _appointments.GetAll();
        IPayable[] result = new IPayable[appointments.Length];
        for (int i = 0; i < appointments.Length; i++)
        {
            result[i] = appointments[i];
        }
        return result;
    }

    // Лише неоплачені — двопрохідний патерн
    public IPayable[] GetUnpaid()
    {
        IPayable[] payables = GetAllPayables();

        int matches = 0;
        for (int i = 0; i < payables.Length; i++)
        {
            if (!payables[i].IsPaid) matches++;
        }

        IPayable[] result = new IPayable[matches];
        int index = 0;
        for (int i = 0; i < payables.Length; i++)
        {
            if (!payables[i].IsPaid) result[index++] = payables[i];
        }
        return result;
    }

    public decimal GetTotalDebt()
    {
        return CalculateDebt(GetAllPayables());
    }

    public decimal GetTotalPaid()
    {
        decimal paid = 0;
        IPayable[] payables = GetAllPayables();
        for (int i = 0; i < payables.Length; i++)
        {
            if (payables[i].IsPaid) paid += payables[i].Amount;
        }
        return paid;
    }

    // Оплатити конкретний запис за його номером.
    // Пошук іде по копії з GetAll(): внутрішнє сховище менеджера
    // лишається закритим, як і його приватний FindById
    public bool Pay(int appointmentId)
    {
        Appointment[] appointments = _appointments.GetAll();
        for (int i = 0; i < appointments.Length; i++)
        {
            if (appointments[i].Id == appointmentId)
            {
                // Працюємо через контракт, а не через конкретний тип
                IPayable payable = appointments[i];
                return payable.MarkPaid();
            }
        }
        return false;
    }

    // ---------- Вивід ----------

    // Повний список рахунків: меню лише викликає цей метод
    public void DisplayAllPayables()
    {
        IPayable[] payables = GetAllPayables();

        Console.WriteLine($"=== Рахунки ({payables.Length}) ===");
        if (payables.Length == 0)
        {
            Console.WriteLine("Рахунків немає.");
            return;
        }

        for (int i = 0; i < payables.Length; i++)
        {
            string status = payables[i].IsPaid ? "оплачено" : "не оплачено";
            Console.WriteLine($"  {payables[i]} -> {payables[i].Amount} грн, {status}");
        }
        Console.WriteLine($"Разом: {CalculateTotal(payables)} грн");
    }

    public void DisplayUnpaid()
    {
        IPayable[] unpaid = GetUnpaid();

        Console.WriteLine($"=== Неоплачені рахунки ({unpaid.Length}) ===");
        if (unpaid.Length == 0)
        {
            Console.WriteLine("Боргів немає.");
            return;
        }

        for (int i = 0; i < unpaid.Length; i++)
        {
            Console.WriteLine($"  {unpaid[i]}");
        }
        Console.WriteLine($"Разом до сплати: {CalculateDebt(unpaid)} грн");
    }

    public void DisplayReport()
    {
        Console.WriteLine("=== Рахунки клініки ===");
        Console.WriteLine($"Усього записів: {GetAllPayables().Length}");
        Console.WriteLine($"Оплачено: {GetTotalPaid()} грн");
        Console.WriteLine($"Заборгованість: {GetTotalDebt()} грн");
        Console.WriteLine($"Загальна вартість послуг: {CalculateTotal(GetAllPayables())} грн");
    }
}
