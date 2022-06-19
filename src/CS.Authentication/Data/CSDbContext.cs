using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CS.Authentication.Data
{
    public class CSDbContext: IdentityDbContext
    {
        public CSDbContext(DbContextOptions<CSDbContext> options) : base(options)
        {

        }
    }
}
