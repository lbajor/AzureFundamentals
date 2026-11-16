using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TangyAzureFunc.Models
{
    public class GroceryItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
