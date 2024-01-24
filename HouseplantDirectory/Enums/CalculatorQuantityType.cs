using System.ComponentModel.DataAnnotations;

namespace HouseplantDirectory.Enums
{
    public enum CalculatorQuantityType
    {
        [Display(Name = "Миллилитр")]
        Milliliter = 0,
        [Display(Name = "Грамм")]
        Gram = 1,
        [Display(Name = "Милиграмм")]
        Milligram = 2
    }
}
