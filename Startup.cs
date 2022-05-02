using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TenantSearchAPI.Data;
using TenantSearchAPI.Data.Repositories;
using TenantSearchAPI.Auth;
using TenantSearchAPI.Data.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using TenantSearchAPI.Auth.Model;
using System.Configuration;
using RelicsAPI.Auth;
using TenantSearch;
using TenantSearch.Parsers;

namespace TenantSearchAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TenantSearchContext>(); // toxic
            services.AddAutoMapper(typeof(Startup));

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddTransient<DatabaseSeeder, DatabaseSeeder>();
            services.AddTransient<ITenantsRepository, TenantsRepository>();
            services.AddTransient<IHobbiesRepository, HobbiesRepository>();
            services.AddTransient<IApartmentsRepository, ApartmentsRepository>();
            services.AddTransient<ILandlordsRepository, LandlordsRepository>();
            services.AddTransient<IReviewsRepository, ReviewsRepository>();
            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddTransient<IApplicationsRepository, ApplicationsRepository>();
            services.AddTransient<UserBehaviorDatabase>();
            services.AddTransient<IUserBehaviorTransformer, UserBehaviorTransformer>();
            services.AddSingleton<IAuthorizationHandler, SameUserAuthorizationHandler>();

            services.AddIdentity<User, IdentityRole>(options =>
            {
                var allowed = options.User.AllowedUserNameCharacters
                    + "ąčęėįšųūž";

                options.User.AllowedUserNameCharacters = allowed;
            })
                .AddEntityFrameworkStores<TenantSearchContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.SameUser, policy => policy.Requirements.Add(new SameUserRequirement())); ;
            });

            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            services.AddSingleton(tokenValidationParams);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParams;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.SameUser, policy => policy.Requirements.Add(new SameUserRequirement())); ;
            });


            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("MyPolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
