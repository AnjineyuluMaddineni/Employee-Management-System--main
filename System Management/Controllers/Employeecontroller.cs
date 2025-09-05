using AutoMapper;
using Company.BLL.Interfaces;
using Company.BLL.Repositories;
using Company.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System_Management.Models;

namespace System_Management.Controllers
{
    public class Employeecontroller : Controller
    {
        private readonly ILogger<Employeecontroller> _logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public Employeecontroller(ILogger<Employeecontroller> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [Authorize(Roles = "HR,Admin,User")]
        [HttpGet]
        public async Task<IActionResult> Index(string email, int pageNumber = 1, int pageSize = 5)
        {
            _logger.LogInformation("Fetching employee list at {time}", DateTime.Now);
            try
            {
                var spec = new EmployeeSpecification(email);
                var employees = await unitOfWork.Repository<Employee>().GetAllSpecAsync(spec);

                if (employees is null || !employees.Any())
                    return View(new PaginatedList<EmployeeViewModel>(new List<EmployeeViewModel>(), 0, pageNumber, pageSize));

                var empmap = mapper.Map<IReadOnlyList<Employee>, IReadOnlyList<EmployeeViewModel>>(employees);

                var paginatedEmployees = PaginatedList<EmployeeViewModel>.Create(
                    empmap.AsQueryable(), pageNumber, pageSize);

                return View(paginatedEmployees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employees with email={Email}", email);
                throw; // or handle gracefully
            }
            
        }


        [Authorize(Roles = "HR,Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var department = await unitOfWork.Repository<Department>().GetAllAsync();
            ViewBag.dept = department;


            return View();
        }
        [Authorize(Roles = "HR,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var empmapp = mapper.Map<EmployeeViewModel, Employee>(model);
                    if (model.UploadImage is not null)
                        empmapp.Image = DocumentSetting.UploadFile(model.UploadImage, "images");
                    await unitOfWork.Repository<Employee>().AddAsync(empmapp);
                    await unitOfWork.Complete();

                    // 🔹 Log creation
                    _logger.LogInformation("Employee created successfully. EmployeeId={EmployeeId}, Name={Name}, CreatedBy={User}",
                                           empmapp.Id, empmapp.LastName, User.Identity?.Name);

                    TempData["res"] = "An employee has been added successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {

                    throw;
                }
               
            }
            //var department = await unitOfWork.Repository<Department>().GetAllAsync();
            //ViewBag.dept = department;
            return View(model);
        }
        [Authorize(Roles = "HR,Admin")]
        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            if (id == null)
                return BadRequest();

            var emp = await unitOfWork.Repository<Employee>().GetByIdAsync(id);
            if (emp is null)
                return NotFound();
            var empmapp = mapper.Map<Employee, EmployeeViewModel>(emp);
            return View(empmapp);
        }
        [Authorize(Roles = "HR,Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var department = await unitOfWork.Repository<Department>().GetAllAsync();
                ViewBag.dept = department;
         

            return await Details(id);

        }
        [Authorize(Roles = "HR,Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {

            if (ModelState.IsValid)
            {
                var empmapp = mapper.Map<EmployeeViewModel, Employee>(model);
                empmapp.Image = DocumentSetting.UploadFile(model.UploadImage, "images");

                unitOfWork.Repository<Employee>().Update(empmapp);
                await unitOfWork.Complete();
                TempData["res"] = "An employee has been edited successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        [Authorize(Roles = "HR,Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            return await Details(id);
        }
        [Authorize(Roles = "HR,Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var empmapp = mapper.Map<EmployeeViewModel, Employee>(model);
                    unitOfWork.Repository<Employee>().Delete(empmapp);
                    await unitOfWork.Complete();
                    TempData["res"] = "An employee has been deleted successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View(model);
        }



    }
}
