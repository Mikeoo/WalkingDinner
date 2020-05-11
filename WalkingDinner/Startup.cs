using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WalkingDinner.Database;

namespace WalkingDinner {

    public class Startup {

        public Startup( IConfiguration configuration ) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services ) {

            services.AddSession();

            services.AddAuthentication( "BasicAuthentication" ).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>( "BasicAuthentication", null );

            services.AddDbContext<DatabaseContext>( opt => opt.UseSqlServer( Configuration.GetConnectionString( "DefaultConnection" ) ) );

            services.AddRazorPages()
                    .AddRazorPagesOptions( options => {
                        options.Conventions.AuthorizeFolder( "/Invitation" );
                        options.Conventions.AuthorizeFolder( "/Management", "AdminOnly" );
                    } );

            services.AddAuthorization( options => {
                options.AddPolicy( "AdminOnly", policy => policy.RequireAssertion( o => o.User.FindFirstValue( ClaimTypes.Role ) == "Admin" ) );
            } );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env ) {

            var cultureInfo = new CultureInfo("nl-NL");
            CultureInfo.DefaultThreadCurrentCulture     = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture   = cultureInfo;

            if ( env.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler( "/Error" );
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints( endpoints => {
                endpoints.MapRazorPages();
            } );
        }
    }
}
