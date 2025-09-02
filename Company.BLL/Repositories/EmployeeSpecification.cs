using Company.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.BLL.Repositories
{
    public class EmployeeSpecification:BaseSpecification<Employee> 
    {
        public EmployeeSpecification()
        {
            INclude.Add(x => x.department);

        }
        public EmployeeSpecification(string? email)
        {
            if (!string.IsNullOrEmpty(email))
                WH = x =>
                    x.Email.ToLower().Contains(email.ToLower()) ||
                    x.FirstName.ToLower().Contains(email.ToLower()) ||
                    x.LastName.ToLower().Contains(email.ToLower()) ||
                    x.PhoneNumber.ToLower().Contains(email.ToLower()) ||
                    x.Address.ToLower().Contains(email.ToLower()) ||
                    (x.department != null && x.department.Department_Name.ToLower().Contains(email.ToLower()));

            INclude.Add(x => x.department);
        }

        public EmployeeSpecification(int? id):base(x=>x.Id==id)
        {
            INclude.Add(x => x.department);

        }
    }
}
