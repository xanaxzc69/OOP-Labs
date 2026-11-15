using System;
using System.Globalization;
using System.Reflection;
using ClinicApp.Attributes;

namespace ClinicApp.Utils;

// Задача 4 (Lab11). Автоматична побудова консольної форми введення.
//
// FormBuilder ніколи не чув про TreatmentPlan. Він створює об'єкт типу T,
// через рефлексію бере перелік його властивостей і для кожної, у якої є
// відкритий сеттер, питає значення в користувача. Підказки до полів
// він теж бере з метаданих — з тих самих атрибутів, що описують правила.
//
// where T : new() — обмеження «у типу є відкритий конструктор без
// параметрів». Без нього рядок new T() не скомпілювався б: компілятор
// не знав би, чи можна взагалі створити T без аргументів.
public static class FormBuilder
{
    public static T Build<T>() where T : new()
    {
        T item = new T();
        Type type = typeof(T);

        Console.WriteLine($"=== Заповнення: {type.Name} ===");
        Console.WriteLine("(порожній рядок — лишити значення за замовчуванням)");
        Console.WriteLine();

        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            // Поля лише для читання (Id, Status, обчислювані властивості)
            // пропускаємо: їх не вводять, вони визначаються самим об'єктом
            if (!property.CanWrite) continue;
            if (property.GetSetMethod() == null) continue;      // сеттер є, але не відкритий
            if (property.GetIndexParameters().Length > 0) continue;

            string prompt = BuildPrompt(property);

            Console.Write(prompt);
            string input = Console.ReadLine();

            // Ввід закінчився (Ctrl+D / перенаправлений файл) — виходимо з форми
            if (input == null) break;

            input = input.Trim();
            if (input.Length == 0) continue;   // лишаємо значення за замовчуванням

            // Некоректне введення не повинно валити програму: повідомляємо
            // і йдемо далі, а остаточне рішення ухвалить валідатор
            if (!TrySetValue(item, property, input))
            {
                Console.WriteLine($"  Не вдалося розпізнати значення для «{property.Name}» " +
                                  $"({ModelValidator.FriendlyTypeName(property.PropertyType)}). " +
                                  "Поле лишилось незмінним.");
            }
        }

        Console.WriteLine();
        return item;
    }

    // Підказка будується з типу властивості та її атрибутів:
    // користувач одразу бачить, що саме від нього хочуть
    private static string BuildPrompt(PropertyInfo property)
    {
        string typeName = ModelValidator.FriendlyTypeName(property.PropertyType);
        string hints = "";

        if (property.GetCustomAttribute<RequiredAttribute>() != null)
        {
            hints += ", обов'язкове";
        }

        MaxLengthAttribute maxLength = property.GetCustomAttribute<MaxLengthAttribute>();
        if (maxLength != null)
        {
            hints += $", до {maxLength.Length} символів";
        }

        MinValueAttribute minValue = property.GetCustomAttribute<MinValueAttribute>();
        if (minValue != null)
        {
            hints += $", не менше {minValue.Minimum}";
        }

        if (property.PropertyType == typeof(DateTime))
        {
            hints += ", формат дд.мм.рррр";
        }

        return $"{property.Name} ({typeName}{hints}): ";
    }

    // Перетворення рядка у потрібний тип. Жодного конкретного поля
    // тут не згадано — лише типи.
    private static bool TrySetValue(object target, PropertyInfo property, string input)
    {
        Type type = property.PropertyType;

        try
        {
            if (type == typeof(string))
            {
                property.SetValue(target, input);
                return true;
            }

            if (type == typeof(int))
            {
                int number;
                if (!int.TryParse(input, out number)) return false;
                property.SetValue(target, number);
                return true;
            }

            if (type == typeof(decimal))
            {
                decimal number;
                if (!decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out number))
                    return false;
                property.SetValue(target, number);
                return true;
            }

            if (type == typeof(double))
            {
                double number;
                if (!double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out number))
                    return false;
                property.SetValue(target, number);
                return true;
            }

            if (type == typeof(bool))
            {
                string lowered = input.ToLower();
                if (lowered == "так" || lowered == "yes" || lowered == "true" || lowered == "1")
                {
                    property.SetValue(target, true);
                    return true;
                }
                if (lowered == "ні" || lowered == "no" || lowered == "false" || lowered == "0")
                {
                    property.SetValue(target, false);
                    return true;
                }
                return false;
            }

            if (type == typeof(DateTime))
            {
                DateTime date;
                if (!DateTime.TryParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture,
                                            DateTimeStyles.None, out date))
                    return false;
                property.SetValue(target, date);
                return true;
            }

            if (type.IsEnum)
            {
                object parsed;
                if (!TryParseEnum(type, input, out parsed)) return false;
                property.SetValue(target, parsed);
                return true;
            }

            // Тип, якого форма не вміє заповнювати
            return false;
        }
        catch (Exception)
        {
            // Сеттер моделі може кинути виняток власної перевірки —
            // для форми це просто «значення не підійшло»
            return false;
        }
    }

    private static bool TryParseEnum(Type enumType, string input, out object value)
    {
        value = null;
        string[] names = Enum.GetNames(enumType);
        foreach (string name in names)
        {
            if (string.Equals(name, input, StringComparison.OrdinalIgnoreCase))
            {
                value = Enum.Parse(enumType, name);
                return true;
            }
        }
        return false;
    }
}
