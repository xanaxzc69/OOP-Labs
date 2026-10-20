namespace ClinicApp.Interfaces;

// Задача 2. Контракт «це можна скасувати».
// Коду, який скасовує кілька об'єктів одним викликом, не важливо,
// що це за об'єкти, — йому потрібен лише цей контракт.
public interface ICancellable
{
    // Чи вже скасовано
    bool IsCancelled { get; }

    // Причина скасування; порожній рядок, якщо не скасовано
    string CancellationReason { get; }

    // Скасувати. Повертає false, якщо скасування вже неможливе
    bool Cancel(string reason = "");
}
