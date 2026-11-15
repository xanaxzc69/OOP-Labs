using System;
using System.Collections.Generic;
using ClinicApp.Enums;
using ClinicApp.Models;
using ClinicApp.Utils;

namespace ClinicApp.Managers;

// Задача 4 (Lab11). Менеджер планів лікування.
//
// Особливість цього менеджера: він не перевіряє поля сам. Усі правила
// описані атрибутами в самій моделі, а менеджер лише запускає
// ModelValidator і вирішує, що робити з результатом. Невалідний план
// у сховище не потрапляє взагалі — краще відмовити одразу, ніж зберегти
// запис із порожнім діагнозом і розбиратися з ним пізніше.
public class TreatmentPlanManager
{
    private readonly List<TreatmentPlan> _plans = new List<TreatmentPlan>();

    public int Count => _plans.Count;

    // ---------- Create ----------

    // Повертає true, лише якщо план пройшов перевірку й був доданий
    public bool Add(TreatmentPlan plan)
    {
        if (plan == null)
        {
            Console.WriteLine("План не задано.");
            return false;
        }

        ValidationResult validation = ModelValidator.Validate(plan);
        if (!validation.IsValid)
        {
            Console.WriteLine("План не додано — виявлено помилки:");
            validation.Display("Перевірка плану лікування");
            return false;
        }

        _plans.Add(plan);
        Console.WriteLine($"План лікування [{plan.Id}] додано.");
        return true;
    }

    // ---------- Read ----------

    public List<TreatmentPlan> GetAll()
    {
        return new List<TreatmentPlan>(_plans);
    }

    // null, якщо плану з таким номером немає
    public TreatmentPlan GetById(int id)
    {
        foreach (TreatmentPlan plan in _plans)
        {
            if (plan.Id == id) return plan;
        }
        return null;
    }

    public List<TreatmentPlan> GetByPatient(int patientId)
    {
        List<TreatmentPlan> found = new List<TreatmentPlan>();
        foreach (TreatmentPlan plan in _plans)
        {
            if (plan.PatientId == patientId) found.Add(plan);
        }
        return found;
    }

    public List<TreatmentPlan> GetByStatus(TreatmentStatus status)
    {
        List<TreatmentPlan> found = new List<TreatmentPlan>();
        foreach (TreatmentPlan plan in _plans)
        {
            if (plan.Status == status) found.Add(plan);
        }
        return found;
    }

    // ---------- Переходи стану ----------
    //
    // Менеджер лише знаходить план і делегує рішення самому об'єкту:
    // які переходи допустимі, вирішує модель, а не менеджер.

    public bool Activate(int id)
    {
        TreatmentPlan plan = GetById(id);
        if (plan == null) return false;
        return plan.Activate();
    }

    public bool Complete(int id)
    {
        TreatmentPlan plan = GetById(id);
        if (plan == null) return false;
        return plan.Complete();
    }

    public bool Cancel(int id)
    {
        TreatmentPlan plan = GetById(id);
        if (plan == null) return false;
        return plan.Cancel();
    }

    // ---------- Delete ----------

    public bool Remove(int id)
    {
        TreatmentPlan plan = GetById(id);
        if (plan == null) return false;
        return _plans.Remove(plan);
    }

    // ---------- Вивід ----------

    public void DisplayAll()
    {
        Console.WriteLine($"=== Плани лікування ({_plans.Count}) ===");
        if (_plans.Count == 0)
        {
            Console.WriteLine("Планів ще немає.");
            return;
        }
        foreach (TreatmentPlan plan in _plans)
        {
            Console.WriteLine(plan);
        }
    }
}
