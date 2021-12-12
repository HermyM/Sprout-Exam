using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sprout.Exam.Business.DataTransferObjects
{
    public abstract class BaseSaveEmployeeDto
    {
     
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Tin { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
        [Required]
        public int TypeId { get; set; }
    }
}
