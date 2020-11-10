using busylight_server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace busylight_server
{
    public class Startup
    {
        private const string Title = "Busylight Server";
        private const string Version = "v1";
        private const string KeyName = "ApiKey";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        private class MyOpenApiSecurityScheme : OpenApiSecurityScheme
        {
            public MyOpenApiSecurityScheme(ParameterLocation inWhat)
            {
                this.Name = KeyName;
                this.Description = "some description";
                this.In = inWhat;
                this.Type = SecuritySchemeType.ApiKey;
                this.Scheme = KeyName;
            }
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new Settings());
            services.AddApiKeyAuthentication(a =>
            {
                a.KeyName = KeyName;
                a.ApiKeys = Settings.API_KEYS.Split(',');
            });


            services.AddSignalR();
            services.AddControllers()
                    .AddJsonOptions(j => { j.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });


            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo { Title = Title, Version = Version });


                foreach (var item in new List<ParameterLocation> { ParameterLocation.Query, ParameterLocation.Header })
                {
                    s.AddSecurityDefinition(item.ToString(), new MyOpenApiSecurityScheme(item));
                    s.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = item.ToString() } },
                            Array.Empty<string>()
                        }
                    });
                }

            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Title} {Version}");
            });

            app.UseRouting();
            app.UseHttpMetrics();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<BusyHub>("/BusyHub");
                endpoints.MapMetrics();
            });
        }
    }
}
