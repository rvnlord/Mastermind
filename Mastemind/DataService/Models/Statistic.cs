using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataService.Models
{
    [Table("tblStatistics")]
    [DataContract]
    public class Statistic
    {
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int CodeLength { get; set; }
        [DataMember]
        public int Tries { get; set; }
    }
}
