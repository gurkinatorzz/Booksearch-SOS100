using Microsoft.EntityFrameworkCore;
using ReservationServices.Models;

namespace ReservationServices.Data;

public class ReservationServiceDbContext : DbContext
{
    public ReservationServiceDbContext(DbContextOptions<ReservationServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<ReservationModels> Reservations { get; set; }
}