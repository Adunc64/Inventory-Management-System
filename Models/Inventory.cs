using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace InventoryManagement.Models
{
    public class Inventory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        // Foreign key to IdentityUser
        [Required]
        public string OwnerId { get; set; } = string.Empty;

        public IdentityUser? Owner { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; } //optimistic concurrency
    }
}