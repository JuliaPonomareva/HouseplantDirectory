using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HouseplantDirectory.Enums
{
    public enum CalculatorSubstanceType
    {
        [Display(Name = "Удобрение")]
        Fertilizer = 0,
        [Display(Name = "Инсектицид")]
        Insecticide = 1,
        [Display(Name = "Гербицид")]
        Herbicide = 2,
        [Display(Name = "Стимулятор")]
        Stimulant = 3,
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }
    }
}
