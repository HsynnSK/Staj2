using OrderSystemManagment.Domain.Entities.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;



namespace OrderSystemManagment.Domain.Entities
{
    public class User : DatabaseObject
    {
        public string? UserName { get; set; } 
        public string? Password { get; set; }

        public Role? Role { get; set; } // RoleName

    }
}
