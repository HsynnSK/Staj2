using OrderSystemManagment.Domain.Entities.BaseModels;
using OrderSystemManagment.Domain.Entities.Module;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace OrderSystemManagment.Domain.Entities;
public class Role : DatabaseObject
{
    public string RoleName { get; set; } = string.Empty;

    public ModulePermission UserManagement { get; set; } = new();
    
    public ModulePermission OrderManagement { get; set; } = new();

}

