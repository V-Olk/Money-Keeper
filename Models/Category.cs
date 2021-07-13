using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOlkin
{
    public class Category
    {
        [Key]
        public int CategoryId { get; private set; }
        [Required]
        public string CategoryName { get; private set; }
        public override string ToString() => CategoryName;
        
        //TODO: Добавить enum с типом транзакции доход/расход
    }
}
