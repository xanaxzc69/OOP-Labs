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
    // Джерело записів задається один раз у конструкторі й більше не змінюється
    private readonly AppointmentManager _appointments;

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

    // Сума несплаченого. Скасований запис не оплачується, тому в борг не входить
    public static decimal CalculateDebt(IPayable[] items)
    {
        if (items == null) return 0;

        decimal debt = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (IsDebt(items[i])) debt += items[i].Amount;
        }
        return debt;
    }

    // Запис створює борг, якщо він ще не оплачений і не скасований.
    // Перевірка йде через контракт ICancellable, а не через конкретний клас
    private static bool IsDebt(IPayable item)
    {
        if (item == null) return false;
        if (item.IsPaid) return false;

        ICancellable cancellable = item as ICancellable;
        if (cancellable != null && cancellable.IsCancelled) return false;

        return true;
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
            if (IsDebt(payables[i])) matches++;
        }

        IPayable[] result = new IPayable[matches];
        int index = 0;
        for (int i = 0; i < payables.Length; i++)
        {
            if (IsDebt(payables[i])) result[index++] = payables[i];
        }
        return result;
    }

    // Неоплачені записи конкретного пацієнта
    public IPayable[] GetUnpaidByPatient(int patientId)
    {
        Appointment[] appointments = _appointments.GetByPatient(patientId);

        int matches = 0;
        for (int i = 0; i < appointments.Length; i++)
        {
            if (IsDebt(appointments[i])) matches++;
        }

        IPayable[] result = new IPayable[matches];
        int index = 0;
        for (int i = 0; i < appointments.Length; i++)
        {
            if (IsDebt(appointments[i])) result[index++] = appointments[i];
        }
        return result;
    }

    // Борг конкретного пацієнта: вибірка за пацієнтом, а сума — через контракт IPayable
    public decimal GetPatientDebt(int patientId)
    {
        return CalculateDebt(GetUnpaidByPatient(patientId));
    }

    public void DisplayPatientDebt(int patientId)
    {
        IPayable[] unpaid = GetUnpaidByPatient(patientId);

        Console.WriteLine($"=== Борг пацієнта #{patientId} ===");
        if (unpaid.Length == 0)
        {
            Console.WriteLine("Боргів немає.");
            return;
        }

        for (int i = 0; i < unpaid.Length; i++)
        {
            Console.WriteLine($"  {unpaid[i]}");
        }
        Console.WriteLine($"Разом до сплати: {GetPatientDebt(patientId)} грн");
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
