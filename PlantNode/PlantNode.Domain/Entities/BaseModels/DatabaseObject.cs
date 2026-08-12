using System;
using System.Collections.Generic;
using System.Text;

namespace PlantNode.Domain.Entities.BaseModels
{
   
    public class DatabaseObject
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false; //SoftDelete


    }

}
