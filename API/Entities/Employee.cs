using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string LastName { get; set; }

        [RegularExpression("^\\+?[1-9][0-9]{7,14}$")]
        public required string PhoneNumber { get; set; }

        public required string Email { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        public double MonthlySalary { get; set; }
        
        [JsonIgnore]
        public List<WorkTask>? Tasks { get; set; }

        [JsonIgnore]
        public List<Completed>? Completed { get; set; }
    }
}