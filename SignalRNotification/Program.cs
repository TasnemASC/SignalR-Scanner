using Microsoft.OpenApi.Models;
using SignalRNotification.Hubs;

namespace SignalRNotification
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("*",
                    builder => builder.WithOrigins("http://localhost:4200")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials());
            });
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSignalR();
            // builder.Services.AddScoped<IHubContext, HubContext>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                //app.use
            }
            app.UseCors("*");


            app.UseStaticFiles();

            app.UseRouting();

            // app.UseAuthorization();
            app.UseSwagger();

            // Enable middleware to serve SwaggerUI (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name v1");
                c.RoutePrefix = string.Empty; // Set the UI route prefix to empty string (root)
            });
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapControllerRoute(
               name: "default",
               pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }
}