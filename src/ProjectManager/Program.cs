using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Areas.Identity;
using ProjectManager.Data;
using ProjectManager.Data.Native;
using ProjectManager.Data.ProjectIntegration;
using Serilog;

namespace Company.WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
#if DEBUG
                options.UseSqlServer(connectionString, x => x.MigrationsAssembly("ProjectManager"));
#else
                options.UseNpgsql(connectionString, x => x.MigrationsAssembly("PostgresqlMigrations"));                                                
#endif                
            });
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<UserProfile>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAuthentication().AddWsFederation(options =>
            {
                // MetadataAddress represents the Active Directory instance used to authenticate users.
                options.MetadataAddress = "https://login.microsoftonline.com/9d4d5e00-b133-40e3-8512-28f7f355dbf8/federationmetadata/2007-06/federationmetadata.xml";

                // For AAD, use the Application ID URI from the app registration's Overview blade:
                options.Wtrealm = "api://ffb0aad4-2809-4800-a175-8ea82877f2bb";
            });

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            builder.Services.AddScoped<ProjectService>();
            builder.Services.AddScoped<TaskStateMachine>();
            builder.Services.AddScoped<UINotifier>();
            builder.Services.AddSingleton<GeocodeService>();
            builder.Services.AddScoped<NativeFiles>();
            builder.Services.AddScoped<FeatureFlags>();

            builder.Services.AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });

            builder.Services.AddBlazoriseRichTextEdit();

            string apiKey = builder.Configuration["Logging:ApiKey"];
            string ingestAddress = builder.Configuration["Logging:IngestUrl"];
            var serilog = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Seq(ingestAddress, apiKey: apiKey)
                .CreateLogger();

            builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(serilog));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            string? pathBase = builder.Configuration.GetValue<string?>("PathBase");
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UsePathBase(pathBase);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
