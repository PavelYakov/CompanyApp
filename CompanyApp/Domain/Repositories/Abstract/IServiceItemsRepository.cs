using CompanyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyApp.Domain.Repositories.Abstract
{
    public interface IServiceItemsRepository
    {
        //делаем выборку всех услуг
        IQueryable<ServiceItem> GetServiceItems();
        //выбираем услугу по id
        ServiceItem GetServiceItemById(Guid id);
        
        //сохраняем изменения в бд
        void SaveServiceItem(ServiceItem entity);
        //удаляем услугу
        void DeleteServiceItem(Guid id);
        // пользователь нажимает на кнопку и проихводится
        // желанное действие из предложенного сверху списка
    }
}
