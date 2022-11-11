using CompanyApp.Domain.Entities;
using CompanyApp.Domain.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyApp.Domain.Repositories.EntityFramework
{
    public class EFTextFieldsRepository : ITextFieldsRepository
    {
        //связываем этот класс с AppDbContext т.е. с базой данных
        private readonly AppDbContext context;
        public EFTextFieldsRepository(AppDbContext context)
        {
            this.context = context;
        }

        public IQueryable<TextField> GetTextFields()
        {
            //Обращаемся к констесту и выбираем все записи из TextFields
            return context.TextFields;
        }

        public TextField GetTextFieldById(Guid id)
        {
            //Обращаемся к констесту и выбираем одну запись из TextFields
            return context.TextFields.FirstOrDefault(x => x.Id == id);
        }

        public TextField GetTextFieldByCodeWord(string codeWord)
        {
            //Обращаемся к констесту и выбираем по ключевому слову  из TextFields
            return context.TextFields.FirstOrDefault(x => x.CodeWord == codeWord);
        }

        public void SaveTextField(TextField entity)
        {
            
            if (entity.Id == default)
                // Если запись не равно дефолтному значению, значит у ее нет id и мы ее помечаем новым ключем.
                // Далее ее добавят в бд
                context.Entry(entity).State = EntityState.Added;
            else
                // Id совпало, но данные поменялись (Модифицировались), значит нужно изменить и пересохранить
                context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges(); // сохраняем изменения
        }
        //Удаляем
        public void DeleteTextField(Guid id)
        {
            context.TextFields.Remove(new TextField() { Id = id });
            context.SaveChanges();
        }
    }
}
