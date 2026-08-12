using System;
using System.Collections.Generic;
using System.Text;

namespace OrderSystemManagment.Domain.Entities.Module;

public class ModulePermission
{
    public bool CanRead { get; set; } = false;
    public bool CanCreate { get; set; } = false;
    public bool CanUpdate { get; set; } = false;
    public bool CanDelete { get; set; } = false;




    public bool IsUserAdmin(Role role)
    {
        if (role.RoleName == "Admin")
        {
            AllCanPermissionChangedTrue(true, role);
        }
        return false;
    }

    private void AllCanPermissionChangedTrue(bool canAllPermission, Role role) //Admin yetkisi
    {

        CanRead = canAllPermission;
        CanDelete = canAllPermission;
        CanUpdate = canAllPermission;
        CanCreate = canAllPermission;
        if (role.RoleName != "Admin")
        {
            CanDelete = false;
        }


    }
}