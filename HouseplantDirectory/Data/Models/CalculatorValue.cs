using HouseplantDirectory.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseplantDirectory.Data.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class CalculatorValue
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Display(Name = "Введите название вещества")]
        public string Name { get; set; }
        [Display(Name = "Введите тип вещества")]
        public CalculatorSubstanceType SubstanceType { get; set; }
        [Display(Name = "Введите меру веса")]
        public CalculatorQuantityType QuantityType { get; set; }
        [Display(Name = "Введите количество жидкости")]
        public int LiquidAmount { get; set; }
        [Display(Name = "Введите количество вещества")]
        public int SubstanceAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
    }
}
