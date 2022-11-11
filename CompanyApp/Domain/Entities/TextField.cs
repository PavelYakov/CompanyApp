using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyApp.Domain.Entities
{
    public class TextField : EntityBase
    {
        //Required: данный атрибут указывает,
        //что свойство должно быть обязательно установлено,
        //обязательно должно иметь какое-либо значение.
        [Required]
        public string CodeWord { get; set; }

        [Display(Name = "Название страницы (заголовок)")]
        public override string Title { get; set; } = "Информационная страница";

        [Display(Name = "Содержание страницы")]
        public override string Text { get; set; } = "Содержание заполняется администратором";


    }
}
