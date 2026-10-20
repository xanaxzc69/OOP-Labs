using System;

namespace ClinicApp.Interfaces;

// Задача 3. Контракт «у цієї сутності є розклад».
// Його реалізує Doctor, а не Appointment: розклад мають саме лікарі.
public interface ISchedulable
{
    // Чи потрапляє вказаний момент часу в розклад
    bool CanSchedule(DateTime at);

    // Вільні цілогодинні слоти на вказаний день
    DateTime[] GetAvailableSlots(DateTime date, int slotCount);
}
