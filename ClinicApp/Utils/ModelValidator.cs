using System;
using System.Reflection;
using ClinicApp.Attributes;

namespace ClinicApp.Utils;

// Задача 3 (Lab11). Універсальний валідатор на рефлексії.
//
// Клас не знає жодного конкретного типу застосунку: ні TreatmentPlan,
// ні Patient, ні їхніх полів. У коді немає і не повинно бути перевірок
// на кшталт if (plan.Diagnosis == ""). Валідатор працює так:
//
//   1) питає в об'єкта його тип             — obj.GetType();
//   2) бере перелік властивостей типу       — type.GetProperties();
//   3) для кожної властивості питає атрибути — GetCustomAttribute<...>();
//   4) якщо правило є, читає значення        — property.GetValue(obj)
//      і перевіряє його за цим правилом.
//
// Саме тому додати нове поле в модель можна, не торкаючись валідатора:
// достатньо повісити атрибут — і правило почне діяти.
public static class ModelValidator
{
    public static ValidationResult Validate(object obj)
    {
        ValidationResult result = new ValidationResult();

        if (obj == null)
        {
            result.AddError("Об'єкт не задано (null).");
            return result;
        }

        Type type = obj.GetType();

        // Беремо лише відкриті властивості самого об'єкта
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            // Властивість без геттера прочитати неможливо, тому пропускаємо
            if (!property.CanRead) continue;

            // Індексатори (this[int]) значення без аргументу не мають
            if (property.GetIndexParameters().Length > 0) continue;

            object value;
            try
            {
                value = property.GetValue(obj);
            }
            catch (Exception)
            {
                // Якщо геттер обчислюваної властивості кинув виняток,
                // валідація решти полів через це зупинятися не повинна
                result.AddError($"Поле «{property.Name}»: не вдалося прочитати значення.");
                continue;
            }

            CheckRequired(property, value, result);
            CheckMaxLength(property, value, result);
            CheckMinValue(property, value, result);
        }

        return result;
    }

    // ---------- Правило [Required] ----------
    private static void CheckRequired(PropertyInfo property, object value, ValidationResult result)
    {
        RequiredAttribute attribute = property.GetCustomAttribute<RequiredAttribute>();
        if (attribute == null) return;

        // Крайові випадки: порожнє значення — це і null, і порожній рядок,
        // і рядок з самих пробілів. Останній випадок найпідступніший:
        // візуально поле ніби заповнене, а змісту в ньому немає.
        bool empty;
        if (value == null)
        {
            empty = true;
        }
        else if (value is string text)
        {
            empty = string.IsNullOrWhiteSpace(text);
        }
        else
        {
            // Значущий тип (int, decimal, DateTime, enum) завжди має значення,
            // тому порожнім бути не може
            empty = false;
        }

        if (empty)
        {
            result.AddError(attribute.BuildError(property.Name));
        }
    }

    // ---------- Правило [MaxLength] ----------
    private static void CheckMaxLength(PropertyInfo property, object value, ValidationResult result)
    {
        MaxLengthAttribute attribute = property.GetCustomAttribute<MaxLengthAttribute>();
        if (attribute == null) return;

        // Незаповнене поле довжину не перевищує: про його відсутність
        // має сказати [Required], якщо воно справді обов'язкове.
        // Інакше одне порожнє поле давало б дві помилки одразу.
        if (value == null) return;

        string text = value as string;
        if (text == null) return;   // правило стосується лише рядків

        if (text.Length > attribute.Length)
        {
            result.AddError(attribute.BuildError(property.Name, text.Length));
        }
    }

    // ---------- Правило [MinValue] ----------
    private static void CheckMinValue(PropertyInfo property, object value, ValidationResult result)
    {
        MinValueAttribute attribute = property.GetCustomAttribute<MinValueAttribute>();
        if (attribute == null) return;

        if (value == null) return;

        // Межа зберігається як double, тому значення властивості
        // (int, decimal, double) приводимо до того ж типу.
        // Нечислове поле з таким атрибутом — помилка не даних, а розмітки
        // моделі, тому про неї теж варто сказати прямо.
        double number;
        try
        {
            number = Convert.ToDouble(value);
        }
        catch (Exception)
        {
            result.AddError($"Поле «{property.Name}»: правило мінімального значення " +
                            "застосовано до нечислового поля.");
            return;
        }

        if (number < attribute.Minimum)
        {
            result.AddError(attribute.BuildError(property.Name, number));
        }
    }

    // ---------- Будова типу ----------
    //
    // Метод показує, що рефлексія дозволяє дізнатися структуру типу,
    // жодного разу не створивши його екземпляр: на вхід подається Type,
    // а не об'єкт.
    public static void PrintInfo(Type type)
    {
        if (type == null)
        {
            Console.WriteLine("Тип не задано.");
            return;
        }

        Console.WriteLine($"=== Будова типу {type.Name} ===");
        Console.WriteLine($"Простір імен: {type.Namespace}");

        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        Console.WriteLine($"Відкритих властивостей: {properties.Length}");
        Console.WriteLine();

        foreach (PropertyInfo property in properties)
        {
            string access = property.CanWrite ? "читання й запис" : "лише читання";
            Console.WriteLine($"  {property.Name} : {FriendlyTypeName(property.PropertyType)} ({access})");

            RequiredAttribute required = property.GetCustomAttribute<RequiredAttribute>();
            if (required != null)
            {
                Console.WriteLine("      [Required] — обов'язкове до заповнення");
            }

            MaxLengthAttribute maxLength = property.GetCustomAttribute<MaxLengthAttribute>();
            if (maxLength != null)
            {
                Console.WriteLine($"      [MaxLength({maxLength.Length})] — не довше за {maxLength.Length} символів");
            }

            MinValueAttribute minValue = property.GetCustomAttribute<MinValueAttribute>();
            if (minValue != null)
            {
                Console.WriteLine($"      [MinValue({minValue.Minimum})] — не менше за {minValue.Minimum}");
            }

            if (required == null && maxLength == null && minValue == null)
            {
                Console.WriteLine("      (правил перевірки немає)");
            }
        }
        Console.WriteLine();
    }

    // Коротка й зрозуміла назва типу замість повної (System.Int32 -> int)
    public static string FriendlyTypeName(Type type)
    {
        if (type == typeof(int)) return "int";
        if (type == typeof(decimal)) return "decimal";
        if (type == typeof(double)) return "double";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(string)) return "string";
        if (type == typeof(DateTime)) return "DateTime";
        return type.Name;
    }
}
