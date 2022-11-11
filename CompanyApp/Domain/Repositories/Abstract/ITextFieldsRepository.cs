using CompanyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyApp.Domain.Repositories.Abstract
{
    public interface ITextFieldsRepository
    {
        //делаем выборку всех текстовых полей
        IQueryable<TextField> GetTextFields();
        //выбираем текстовое поле по id
        TextField GetTextFieldById(Guid id);
        //выбираем текстовое поле по кодовому слову
        TextField GetTextFieldByCodeWord(string codeWord);
        //сохраняем изменения в бд
        void SaveTextField(TextField entity);
        //удаляем текстовое поле
        void DeleteTextField(Guid id);
    }
}
