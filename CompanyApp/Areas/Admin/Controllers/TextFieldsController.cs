using CompanyApp.Domain;
using CompanyApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using CompanyApp.Service;

namespace CompanyApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TextFieldsController : Controller
    {
        //внедряем датамэнэджер, чтобы иметь доступ к доменной модели (базе данных)
        private readonly DataManager dataManager;
        public TextFieldsController(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }
        public IActionResult Edit(string codeWord)
        {
            var entity = dataManager.TextFields.GetTextFieldByCodeWord(codeWord);
            return View(entity);
        }
        [HttpPost]
        public IActionResult Edit(TextField model) // В model приходит моделька с HTML  формы
        {
            if (ModelState.IsValid) // проверяем ее на валидность
            {
                dataManager.TextFields.SaveTextField(model); // сохраняем ее в бд 
                // и перенапрявляем на HomeController.Index=>
                return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).CutController());
            }
            return View(model);
        }
    }
}
