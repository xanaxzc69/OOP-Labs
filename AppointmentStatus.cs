namespace ClinicApp;

// Задача 1. Три стани запису.
// Раніше статус зберігався рядком ("Scheduled"), і одруківку "Sheduled"
// ніхто не помітив би до виконання. Тепер допустимі значення перевіряє компілятор.
public enum AppointmentStatus
{
    Scheduled,
    Completed,
    Cancelled
}
