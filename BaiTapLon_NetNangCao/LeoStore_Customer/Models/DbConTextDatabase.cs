using Microsoft.EntityFrameworkCore;
using Sport_Shop.Areas.Admin.Models;

namespace LeoStore_Customer.Models
{
    public class DbConTextDatabase : DbContext
    {
        public DbConTextDatabase(DbContextOptions options) : base(options)
        {
        }

    }
}
