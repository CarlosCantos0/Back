using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Modelos.Auth;
using Newtonsoft.Json.Serialization;
using PruebasBackendDomain.Context;
using PruebasBackendDomain.Interfaces.UnitOfWork;
using PruebasBackendDominio.UnitOfWork;
using PruebasBackendModelo.Modelo;
using sdCommon.Enums;
using sdDomain.Clases;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddDbContext<PruebasBackendContext>(
                options =>options.UseSqlServer(Configuration.GetConnectionString("DbConnection"))
            );


            services.AddIdentity<AppUser, AppRole>(opts =>
            {
                opts.SignIn.RequireConfirmedEmail = true;
                opts.Password.RequireDigit = true;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequiredLength = 5;
            })
            .AddEntityFrameworkStores<PruebasBackendContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidIssuer = Configuration["Auth:Jwt:Issuer"],
                    ValidAudience = Configuration["Auth:Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"]))
                };
            });


            //Forzar Autorizacion          
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });



            services.AddEntityFrameworkSqlServer();
            services.AddOptions();


            // Publicar Unidades de Trabajo -------------------------------------------------
            services.AddScoped<IUsuariosUnitOfWork, UsuariosUnitOfWork>();
            //services.AddScoped<IClientesUnitOfWork, ClientesUnitOfWork>();
            services.AddScoped<IElementosUnitOfWork, ElementosUnitOfWork>();
            services.AddScoped<IEsquemasUnitOfWork, EsquemasUnitOfWork>();


            //-------------------------------------------------------------------------------


            // Habilitamos los CORS
            services.AddCors(o => o.AddPolicy("EnableCORS", builder =>
            {
                builder.SetIsOriginAllowed(_ => true)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));

            services.AddControllers();

            //services.AddSwaggerGen();
            services.AddSwaggerGen(c =>
            {
                
                
                // Utilizar token para autenticar en Swagger
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Autorización Standar, Usar Bearer. Ejemplo \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            // Para los emails (si no se pone esto, no funciona el controller de usuarios
            services.AddScoped<sdEmailSender>();

            services.AddTransient<Func<TypeEmailServiceEnum, IEmailSender>>(serviceProvider => key =>
            {
                return serviceProvider.GetService<sdEmailSender>();
            });

            services.AddSignalR();
            //services.AddEntityFrameworkSqlServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}" + "/swagger/v1/swagger.json", "Pruebas Backend");
            });



            app.UseRouting();

            app.UseCors("EnableCORS");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
