using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sprout.Exam.Business.DataTransferObjects
{
    public class EditEmployeeDto: BaseSaveEmployeeDto
    {
        [Required]
        public int Id { get; set; }
    }
}
