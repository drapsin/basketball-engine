using System;
using System.ComponentModel.DataAnnotations;
namespace nba_mvc.Models

{
    public abstract class BaseId
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}