using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Enums;

namespace HelpDesk.Core.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
