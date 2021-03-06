using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sprout.Exam.DataAccess.Entity
{

    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        public string Tin { get; set; }

        public DateTime Birthdate { get; set; }

        [Column("EmployeeTypeId")]
        public int TypeId { get; set; }

        public bool isDeleted { get; set; }
    }
}
