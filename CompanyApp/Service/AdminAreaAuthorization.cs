using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using System;
using System.Linq;

namespace CompanyApp.Service
{
    // суть даного соглашения  AdminAreaAuthorization заключается в том, что для какого либо контроллера в котором есть
    //атрибут [Area:("Admin")] , то в этом случае мы подключаем авторизацию и аутентифекацию и в стартапе 
    // определяем специальное правило => политика в этой области такая, что пользователь должен быть admin
    public class AdminAreaAuthorization : IControllerModelConvention
    {
        // area  - область для которой сделано соглашение
        // police - политика которая будет действовать для этой области
        private readonly string area;
        private readonly string policy;
        public AdminAreaAuthorization(string area, string policy)
        {
            this.area = area;
            this.policy = policy;
        }
        // данным методом мы для контроллера проверяем его атрибут
        public void Apply(ControllerModel controller)
        {
            // если присутсвтует атрибут AreaAttribute то =>
            if (controller.Attributes.Any(a =>
                    a is AreaAttribute && (a as AreaAttribute).RouteValue.Equals(area, StringComparison.OrdinalIgnoreCase))
                || controller.RouteValues.Any(r =>
                    r.Key.Equals("area", StringComparison.OrdinalIgnoreCase) && r.Value.Equals(area, StringComparison.OrdinalIgnoreCase)))
            {
                // => мы добавляем фильтр для контроллера AuthorizeFilter
                // (отправляем пользователя на авторизацию)
                controller.Filters.Add(new AuthorizeFilter(policy));
            }
        }
    }
}
