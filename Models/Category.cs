using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOlkin.HelpClasses.Enums;

namespace VOlkin
{
    public class Category
    {
        public Category(string categoryName, CategoryTypeEnum categoryTypeEnum)
        {
            CategoryName = categoryName;
            CategoryType = categoryTypeEnum;
        }

        private Category() { }

        [Key]
        public int CategoryId { get; private set; }
        [Required]
        public string CategoryName { get; private set; }
        [Required]
        public CategoryTypeEnum CategoryType { get; private set; }
        public override string ToString() => CategoryName;
    }
}
