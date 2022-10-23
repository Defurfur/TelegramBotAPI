using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaSchedule.Models
{
    public interface IIdentifyable<T>
    {
        [Key]
        public T Id { get; set; }
        
    }
}
