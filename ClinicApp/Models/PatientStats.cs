using System;

namespace ClinicApp.Models;

// Задача 2 (Lab10). Аналітичний знімок по пацієнту — за тією самою
// схемою, що й DoctorStats: незмінний об'єкт із власним природним порядком.
//
// Природний порядок тут — активність: що більше візитів зробив пацієнт,
// то вище він у списку.
public class PatientStats : IComparable<PatientStats>
{
    public int PatientId { get; }
    public string PatientName { get; }
    public int VisitCount { get; }
    public decimal TotalSpent { get; }
    public DateTime LastVisitDate { get; }

    public PatientStats(int patientId, string patientName, int visitCount,
                        decimal totalSpent, DateTime lastVisitDate)
    {
        PatientId = patientId;
        PatientName = patientName;
        VisitCount = visitCount;
        TotalSpent = totalSpent;
        LastVisitDate = lastVisitDate;
    }

    public bool HasVisits => VisitCount > 0;

    // ---------- Природний порядок: найактивніший перший ----------
    public int CompareTo(PatientStats other)
    {
        if (other == null) return -1;

        // Розвернуте порівняння дає спадання за кількістю візитів
        int byVisits = other.VisitCount.CompareTo(VisitCount);
        if (byVisits != 0) return byVisits;

        // Однакова кількість візитів — вище той, хто витратив більше
        int bySpent = other.TotalSpent.CompareTo(TotalSpent);
        if (bySpent != 0) return bySpent;

        return PatientId.CompareTo(other.PatientId);
    }

    public override string ToString()
    {
        // Пацієнт без жодного візиту не повинен виглядати так, ніби
        // він приходив 1 січня 1 року: замість порожньої дати ставимо прочерк
        string lastVisit = HasVisits
            ? LastVisitDate.ToString("dd.MM.yyyy")
            : "—";

        return $"[{PatientId}] {PatientName} | візитів: {VisitCount} | " +
               $"витрачено: {TotalSpent} грн | останній візит: {lastVisit}";
    }
}
