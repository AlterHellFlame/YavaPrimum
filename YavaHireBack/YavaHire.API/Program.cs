
using YavaHire.Core.DataBase;
using YavaHire.Core.Interfaces;
using YavaHire.Core.Services;

namespace YavaHire.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<YavaHireDBContext>();

            builder.Services.AddScoped<IHRService, HRService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi();

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
