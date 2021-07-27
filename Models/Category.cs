using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOlkin.HelpClasses.Enums;
using VOlkin.Models;

namespace VOlkin
{
    public class Category : TransactionObject
    {
        public Category(string categoryName, CategoryTypeEnum categoryTypeEnum)
        {
            TransactionObjectName = categoryName;
            CategoryType = categoryTypeEnum;
        }

        private Category() { }

        [Required]
        public CategoryTypeEnum CategoryType { get; private set; }
        
    }
}
