using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using workbench.Models;

namespace workbench.Models;
    public sealed class Student
    {
        public int Id { get; set; }
        public string StudentNumber {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Department {  get; set; } = string.Empty;
        public string Phone {  get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
