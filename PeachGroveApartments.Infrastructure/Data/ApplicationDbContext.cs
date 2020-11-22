﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeachGroveApartments.Core.Models;
using PeachGroveApartments.Infrastructure.Identity;
using PeachGroveApartments.Infrastructure.Models;

namespace PeachGroveApartments.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<AptUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AptUser> AptUsers { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<WaterBill> WaterBills { get; set; }
        public DbSet<ElectricBill> ElectricBills { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<FloorPlan> FloorPlans { get; set; }
    }
}