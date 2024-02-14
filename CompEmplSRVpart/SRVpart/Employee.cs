// Author: Tymoshchuk Maksym
// Created On : 14/02/2024
// Last Modified On :
// Description: Класс работчник
// Project: SRVpart

using System;
using System.Collections.Generic;

namespace SRVpart
{
    internal class Employee
    {
        public int PersonnelNumber { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Title { get; set; }

        public string BuisnessPhone { get; set; }

        public List<string> WorkPhone { get; set; }

        public int IdentificationCode { get; set; }

        public DateTime EmploymentDate { get; set; }

        public DateTime?FireDate { get; set; }
    }
}
