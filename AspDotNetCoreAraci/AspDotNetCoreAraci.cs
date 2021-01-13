using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AspDotNetCoreAraci
{
    public partial class AspDotNetAraci : Form
    {
        public AspDotNetAraci()
        {
            InitializeComponent();
        }

        public static double Yuzde;
        public static string DosyaYolu = @"c:/AspDotNetAraci";
        public static List<DBNesneler_Type> Nesneler;

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        private void btnHazirla_Click(object sender, EventArgs e)
        {
            string ProjeAdi = txtProjeAdi.Text.Replace(" - ", "").Replace(" ", "").Replace("_", "");


            if (ProjeAdi != string.Empty && txtConString.Text != string.Empty && lstTablolar.SelectedItems.Count>0)
                Task.Run(() => Hazirla(ProjeAdi, txtConString.Text));
        }

        private void Hazirla(string ProjeAdi, string ConString)
        {

            Nesneler = Nesneler.Where(a => lstTablolar.SelectedItems.Contains(a.Tablo)).ToList();

            KlasorleriHazirla(ProjeAdi);

            MainHazirla(ProjeAdi, ConString);
            ProjHazirla(ProjeAdi, ConString);
            StartupHazirla(ProjeAdi, ConString);
            ReadmeHazirla(ProjeAdi, ConString);
            ModelsHazirla(ProjeAdi, ConString);
            BusinessHazirla(ProjeAdi, ConString);
            ControllersHazirla(ProjeAdi, ConString);
            GenelHazirla(ProjeAdi, ConString);
            PropertiesHazirla(ProjeAdi);

            if (Directory.Exists(DosyaYolu + "/" + ProjeAdi))
                Process.Start(DosyaYolu);
        }

        private void PropertiesHazirla(string ProjeAdi)
        {
            string launchSettings = @"

{
  'iisSettings': {
    'windowsAuthentication': false,
    'anonymousAuthentication': true,
    'iisExpress': {
                'applicationUrl': 'http://localhost:5036/'
    }
        },
  'profiles': {
    'IIS Express': {
      'commandName': 'Project',
      'launchBrowser': true,
      'launchUrl': 'swagger/index.html'
    },
    '%ProjeAdi%': {
      'commandName': 'Project',
      'launchBrowser': true,
      'environmentVariables': {
        'ASPNETCORE_ENVIRONMENT': 'Development'
      },
      'applicationUrl': 'https://localhost:5001;http://localhost:5000'
    }
  }
}


                            ";
            launchSettings = ParametreleriGir(launchSettings, "%ProjeAdi%", ProjeAdi);
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Properties/launchSettings.json", launchSettings);

        }

        private void StartupHazirla(string ProjeAdi, string ConString)
        {
            string Startrup = @"


using %ProjeAdi%.Business.IServices;
using %ProjeAdi%.Business.Manager;
using %ProjeAdi%.Genel;
using %ProjeAdi%.Genel.IGenel;
using %ProjeAdi%.Models.Dtos;
using %ProjeAdi%.Models.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace %ProjeAdi%
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

       
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<%ProjeAdi%Context>(options =>
                 options.UseSqlServer( ' %ConString% ' ));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            " + string.Join("\n ", Nesneler.GroupBy(a => a.Tablo).Select(a => "\n services.AddTransient<I" + a.Key + "Service, " + a.Key + "Manager>();")) + @"

            //Her Tablo için comolokko

            var config = new MapperConfiguration(cfg =>
            {
             "
            +
            string.Join("\n ", Nesneler.Where(a => a.PKmi).Select(a => "\n cfg.CreateMap<" + a.Tablo + "Dto, " + a.Tablo + ">().ForMember(x => x." + a.Kolon + ", opt => opt.Ignore());\n cfg.CreateMap<" + a.Tablo + ", " + a.Tablo + "Dto>();"))
            + @" 
            //Her Tablo için comolokko
            });

            IMapper mapper = config.CreateMapper();

            ////Yetkilendirme
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            ////Yetkilendirme

            //swagger denememizi sağlayan güzel bi materyal publishden önce kaldırılıyor
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc('v1', new OpenApiInfo { Title = ' %ProjeAdi% ', Version = 'v1' });
                c.AddSecurityDefinition('Bearer', new OpenApiSecurityScheme
                {
                    Name = 'Authorization',
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = 'Bearer',
                    BearerFormat = 'JWT',
                    In = ParameterLocation.Header,
                    Description = 'JWT Authorization header using the Bearer scheme.'
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = 'Bearer'
                                }
                            },
                            new string[] {}

                    }
                });
            });


            //swagger


            services.AddSingleton(mapper);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            ////Yetkilendirme
            //app.UseAuthentication();
            ////Yetkilendirme
            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint('/swagger/v1/swagger.json', '%ProjeAdi%');
            });
        }
    }
}




";

            Startrup = ParametreleriGir(Startrup, "%ProjeAdi%", ProjeAdi);
            Startrup = ParametreleriGir(Startrup, "%ConString%", ConString);

            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/appsettings.Development.json", "{ 'Logging': { 'LogLevel': { 'Default': 'Information', 'Microsoft': 'Warning', 'Microsoft.Hosting.Lifetime': 'Information' } }, 'AllowedHosts': '*' } ".Replace("'", "\""));
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/appsettings.json", "{ 'Logging': { 'LogLevel': { 'Default': 'Information', 'Microsoft': 'Warning', 'Microsoft.Hosting.Lifetime': 'Information' } }, 'AllowedHosts': '*' } ".Replace("'", "\""));
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Startup.cs", Startrup.Replace("'", "\""));

        }

        private void ReadmeHazirla(string ProjeAdi, string ConString)
        {
            string Readme = @"
Install-Package Microsoft.Data.SqlClient -Version 2.1.1
Install-Package AutoMapper -Version 10.1.1
Install-Package Microsoft.EntityFrameworkCore -Version 5.0.1
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 5.0.1
Install-Package Swashbuckle.AspNetCore -Version 5.6.3
Install-Package Swashbuckle.AspNetCore.SwaggerGen -Version 5.6.3
Install-Package Swashbuckle.AspNetCore.SwaggerUi -Version 5.6.3

            ";
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Readme.txt", Readme);

        }
        private void ProjHazirla(string ProjeAdi, string ConString)
        {
            string csproj = @"
<Project Sdk='Microsoft.NET.Sdk.Web'>

  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include='AutoMapper' Version='10.1.1' />
    <PackageReference Include='Microsoft.EntityFrameworkCore' Version='5.0.1' />
    <PackageReference Include='Microsoft.EntityFrameworkCore.SqlServer' Version='5.0.1' />
    <PackageReference Include='Microsoft.EntityFrameworkCore.Tools' Version='5.0.1'>
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include='Swashbuckle.AspNetCore' Version='5.6.3' />
    <PackageReference Include='Swashbuckle.AspNetCore.SwaggerGen' Version='5.6.3' />
    <PackageReference Include='Swashbuckle.AspNetCore.SwaggerUI' Version='5.6.3' />
  </ItemGroup>

</Project>

            ";
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/" + ProjeAdi + ".csproj", csproj);

        }
        private void MainHazirla(string ProjeAdi, string ConString)
        {
            string Main = @"
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace %ProjeAdi%
{
    public class Program
    {
        public static void Main(string[] args)
        {
             CreateHostBuilder(args).Build().Run();

         
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}


            ";
            Main = ParametreleriGir(Main, "%ProjeAdi%", ProjeAdi);
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Program.cs", Main);

        }

        private void ModelsHazirla(string ProjeAdi, string ConString)
        {
            string WebResponse = @"

                            using System;
                            using System.Collections.Generic;
                            using System.Linq;
                            using System.Threading.Tasks;

                            namespace %ProjeAdi%.Models
                            {
                                public class WebApiResponse
                                {
                                    public int StatusCode { get; set; }
                                    public bool Status { get; set; }
                                }
                            }


                                ";



            WebResponse = ParametreleriGir(WebResponse, "%ProjeAdi%", ProjeAdi);
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Models/WebApiResponse.cs", WebResponse);

            DtosHazirla(ProjeAdi, ConString);
            EntitiesHazirla(ProjeAdi, ConString);
        }


        private void DtosHazirla(string ProjeAdi, string ConString)
        {
            foreach (var item in Nesneler.GroupBy(a => a.Tablo).Select(a => a.Key))
            {
                string Dto = @"
using System;

#nullable disable

namespace %ProjeAdi%.Models.Dtos
{
    public partial class " + item + @"Dto
    {

" +
string.Join("\n", Nesneler.Where(a => a.Tablo == item).Select(a => "public " + a.KolonTipi + " " + a.Kolon + " { get; set; }"))
+ @"
    }
}

                              ";
                Dto = ParametreleriGir(Dto, "%ProjeAdi%", ProjeAdi);
                ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Models/Dtos/" + item + "Dto.cs", Dto);
            }

        }

        private void EntitiesHazirla(string ProjeAdi, string ConString)
        {
            foreach (var item in Nesneler.GroupBy(a => a.Tablo).Select(a => a.Key))
            {
                string Dto = @"
using System;

#nullable disable

namespace %ProjeAdi%.Models.Entities
{
    public partial class " + item + @"
    {

" +
string.Join("\n", Nesneler.Where(a => a.Tablo == item).Select(a => "public " + a.KolonTipi + (a.Nullmu ? "?" : "") + " " + a.Kolon + " { get; set; }"))
+ @"
    }
}

                              ";
                Dto = ParametreleriGir(Dto, "%ProjeAdi%", ProjeAdi);
                ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Models/Entities/" + item + ".cs", Dto);
            }
        }


        private void GenelHazirla(string ProjeAdi, string ConString)
        {
            string Context = @"

using %ProjeAdi%.Models;
using %ProjeAdi%.Models.Entities;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace %ProjeAdi%.Genel
{
    public partial class %ProjeAdi%Context : DbContext
    {
        public %ProjeAdi%Context()
        {

        }

        public %ProjeAdi%Context(DbContextOptions<%ProjeAdi%Context> options): base(options)
        {
        }

      
       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //burada direk config dende alına bilir
                optionsBuilder.UseSqlServer(' %ConString% ');
            }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation('Relational:Collation', 'Turkish_CI_AS');

       
" +
            string.Join("\n", Nesneler.GroupBy(a => new { a.Tablo }).Select(a => "modelBuilder.Entity<" + a.Key.Tablo + ">(entity => { entity.HasKey(e => e." + a.First(d => d.PKmi).Kolon + "); entity.ToTable('" + a.Key.Tablo + "');  }); "))
            + @"

                    //Özel Listelemeler

                    //modelBuilder.Entity<xxxxxxx>(entity =>
                    //{
                    //    entity.HasKey(e => e.xxx);
                    //    entity.Property(e => e.xxx).HasColumnType('decimal(11, 0)').HasColumnName('xxx');

                    //});


                    OnModelCreatingPartial(modelBuilder);
                }

                partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

            }
            }

                            ";
            Context = ParametreleriGir(Context, "%ProjeAdi%", ProjeAdi);
            Context = ParametreleriGir(Context, "%ConString%", ConString);
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Genel/" + ProjeAdi + "Context.cs", Context);


            string GenericRepository = @"


using %ProjeAdi%.Genel.IGenel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;

namespace %ProjeAdi%.Genel
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private %ProjeAdi%Context context;
        private DbSet<T> dbSet;

        public GenericRepository(%ProjeAdi%Context context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            try
            {
                context.Set<T>().Add(entity);
                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                return await Task.FromResult(false);
            }
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        public virtual async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            var result = context.Set<T>().Where(i => true);

            foreach (var includeExpression in includes)
                result = result.Include(includeExpression);

            return await result.ToListAsync();
        }


        public virtual async Task<List<T>> SearchByAsync(Expression<Func<T, bool>> searchBy, params Expression<Func<T, object>>[] includes)
        {
            var result = context.Set<T>().Where(searchBy);

            foreach (var includeExpression in includes)
                result = result.Include(includeExpression);

            return await result.ToListAsync();
        }


        public virtual async Task<T> FindByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var result = context.Set<T>().Where(predicate);

            foreach (var includeExpression in includes)
                result = result.Include(includeExpression);

            return await result.FirstOrDefaultAsync();
        }

        public virtual async Task<bool> Any(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var result = await context.Set<T>().AnyAsync(predicate);
            return result;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                context.Set<T>().Attach(entity);
                context.Entry(entity).State = EntityState.Modified;

                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                return await Task.FromResult(false);
            }
        }

        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> identity, params Expression<Func<T, object>>[] includes)
        {
            var results = context.Set<T>().Where(identity);

            foreach (var includeExpression in includes)
                results = results.Include(includeExpression);
            try
            {
                context.Set<T>().RemoveRange(results);
                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                return await Task.FromResult(false);
            }
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            context.Set<T>().Remove(entity);
            return await Task.FromResult(true);
        }

        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await context.Set<T>().FindAsync(id);
        }


        public virtual async Task<bool> SqlRunAsync(SqlCommand SqlCommand)
        {
            if (SqlCommand.Parameters.Count > 0)
                context.Set<T>().FromSqlRaw(SqlCommand.CommandText, SqlCommand.Parameters);
            else
                context.Set<T>().FromSqlRaw(SqlCommand.CommandText);

            return await Task.FromResult(true);
        }

        public Task<string> SqlResultAsync(SqlCommand SqlCommand, SqlParameter[] Parameter)
        {
             return Task.Run(() => JsonSerializer.Serialize(context.Set<T>().FromSqlRaw( SqlCommand.CommandText, Parameter)));

        }
    }
}



";

            GenericRepository = ParametreleriGir(GenericRepository, "%ProjeAdi%", ProjeAdi);
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Genel/GenericRepository.cs", GenericRepository);

            string UnitOfWork = @"

using %ProjeAdi%.Genel.IGenel;
using %ProjeAdi%.Models.Entities;
using System;
using System.Threading.Tasks;

namespace %ProjeAdi%.Genel
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly %ProjeAdi%Context _context;
" +
            string.Join("\n", Nesneler.GroupBy(a => new { a.Tablo }).Select(a => " private IGenericRepository<" + a.Key.Tablo + "> _" + a.Key.Tablo + "Repository;"))
               + @"
       
        //Tüm Repolar için nesne
        
        
        
        //Özel Sorgu
        // private IGenericRepository<SadeceTcno> _SadeceRepository;
   
        public UnitOfWork(%ProjeAdi%Context context)
        {
            _context = context;
        }

" +
            string.Join("\n", Nesneler.GroupBy(a => new { a.Tablo }).Select(a => "  public IGenericRepository<" + a.Key.Tablo + "> " + a.Key.Tablo + "Repository { get { return _" + a.Key.Tablo + "Repository ?? (_" + a.Key.Tablo + "Repository = new GenericRepository<" + a.Key.Tablo + ">(_context)); } }"))
               + @"

        //Tüm Repolar için kapsul

        //Özel Sorgu
        //public IGenericRepository<SadeceTcno> SadeceRepository
        //{
        //    get { return _SadeceRepository ?? (_SadeceRepository = new GenericRepository<SadeceTcno>(_context)); }
        //}



       

        // En Güzeli Tran Tran baba
        public async Task Commit()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _context.Dispose();
                    transaction.Rollback();
                }

            }

        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

            ";
            UnitOfWork = ParametreleriGir(UnitOfWork, "%ProjeAdi%", ProjeAdi);
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Genel/UnitOfWork.cs", UnitOfWork);



            IGenelHazirla(ProjeAdi, ConString);
        }

        private void IGenelHazirla(string ProjeAdi, string ConString)
        {
            string IGenericRepository = @"

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace %ProjeAdi%.Genel.IGenel
{
    public interface IGenericRepository<T>
    {
        public Task<bool> AddAsync(T entity);
        public Task<List<T>> GetAllAsync();
        public Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        public Task<List<T>> SearchByAsync(Expression<Func<T, bool>> searchBy, params Expression<Func<T, object>>[] includes);
        public Task<T> FindByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        public Task<bool> UpdateAsync(T entity);
        public Task<bool> DeleteAsync(Expression<Func<T, bool>> identity, params Expression<Func<T, object>>[] includes);
        public Task<bool> DeleteAsync(T entity);
        public Task<bool> Any(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        public Task<T> GetByIdAsync(object id);
        public Task<bool> SqlRunAsync(SqlCommand SqlCommand);
        public Task<string> SqlResultAsync(SqlCommand SqlCommand,SqlParameter[] Parameter);
    }
}

                            ";
            IGenericRepository = ParametreleriGir(IGenericRepository, "%ProjeAdi%", ProjeAdi);
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Genel/IGenel/IGenericRepository.cs", IGenericRepository);


            string IUnitOfWork = @"

using %ProjeAdi%.Models.Entities;
using System;
using System.Threading.Tasks;

namespace %ProjeAdi%.Genel.IGenel
{
    public interface IUnitOfWork : IDisposable
    {
        //Tüm Tablolar için
" +
            string.Join("\n", Nesneler.GroupBy(a => new { a.Tablo }).Select(a => "  IGenericRepository<" + a.Key.Tablo + "> " + a.Key.Tablo + "Repository { get; }"))
               + @"
       

        //Özel Sorgu
        //IGenericRepository<SadeceTcno> SadeceRepository { get; }
      

        Task Commit();
    }
}
                            ";
            IUnitOfWork = ParametreleriGir(IUnitOfWork, "%ProjeAdi%", ProjeAdi);
            ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Genel/IGenel/IUnitOfWork.cs", IUnitOfWork);
        }

        private void ControllersHazirla(string ProjeAdi, string ConString)
        {
            foreach (var item in Nesneler.GroupBy(a => a.Tablo).Select(a => a.Key))
            {
                string Controller = @"

using %ProjeAdi%.Business.IServices;
using %ProjeAdi%.Models.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace %ProjeAdi%.Controllers
{
    [Route('api /[controller]')]
     [ApiController]
    public class " + item + @"ApiController : ControllerBase
        {





            private I" + item + @"Service _" + item + @"Service;
 //private IKullaniciYetkileriService _IKullaniciYetkileriService; farklı biryeden veri çekmek istersek
            private IMapper _mapper;
            public " + item + @"ApiController(I" + item + @"Service " + item + @"Service,//IKullaniciYetkileriService KullaniciYetkileriService,
                IMapper mapper)
            {
                _" + item + @"Service = " + item + @"Service;
 // _IKullaniciYetkileriService = KullaniciYetkileriService;
                _mapper = mapper;
            }
            // GET: api/controller
            [HttpGet]
            public async Task<IActionResult> Get()
            {
                try
                {
                    var " + item + @"List = await _" + item + @"Service.GetAsync();
                    if (" + item + @"List == null)
                        return NotFound();

                    return Ok(" + item + @"List);

                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }



            // GET: api/controller/5
            [HttpGet]
            [Route('getById')]
            public async Task<IActionResult> Get(int id)
            {
                try
                {
                    var " + item + @" = await _" + item + @"Service.GetByIdAsync(id);
                    if (" + item + @" == null)
                        return NotFound();

                    return Ok(" + item + @");
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }

            // POST: api/controller
            [HttpPost]
            public async Task<IActionResult> Post([FromBody] " + item + @"Dto " + item + @"Model)
            {
                try
                {
                    var " + item + @"Id = await _" + item + @"Service.AddAsync(" + item + @"Model);
                    if (" + item + @"Id > 0)
                        return Ok(" + item + @"Id);
                    else
                        return BadRequest('An Error Occured While Creating New " + item + @"');
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            // PUT: api/controller/5
            [HttpPut]
            public async Task<IActionResult> Put(int? id, [FromBody] " + item + @"Dto " + item + @"Model)
            {
                try
                {
                    if (id == null)
                        return BadRequest();

                    var result = await _" + item + @"Service.UpdateAsync(id, " + item + @"Model);
                    if (result.Status)
                        return Ok();
                    else
                        return NotFound();
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            // DELETE: api/controller/5
            [HttpDelete]
            public async Task<IActionResult> Delete(int? id)
            {
                try
                {
                    if (id == null)
                        return BadRequest();

                    var result = await _" + item + @"Service.DeleteAsync(id);
                    if (result.Status)
                        return Ok();
                    else
                        return NotFound();
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }


         //[HttpPost]
        //[Route('KullaniciAdi')]
        //[Route('Password')]
        //[Route('KullaniciLogin')]

        //public async Task<IActionResult> KullaniciLogin(string KullaniciAdi, string Password, bool Hatirla = false)

        //{


        //    try
        //    {

        //        if (KullaniciAdi == null || Password == null || KullaniciAdi == string.Empty || Password == string.Empty)
        //            return BadRequest();

        //        var Kullanici = await _KullanicilarService.Login(KullaniciAdi, Password);


        //        if (ModelState.IsValid)
        //        {


        //            if (Kullanici != null && !User.Identity.IsAuthenticated)
        //            {
        //                List<Claim> KullaniciClaims = new List<Claim>();

        //                KullaniciClaims.Add(new Claim('KullaniciID', Kullanici.KullaniciID.ToString()));
        //                //KullaniciClaims.Add(new Claim(ClaimTypes.Surname, isUser.SurName.ToString()));


        //                var Yetkiler = await _IKullaniciYetkileriService.KullaniciYetkileriGetir(Kullanici.KullaniciID);

        //                KullaniciClaims.Add(new Claim('YetkiID', Yetkiler.ToList().First().Fk_YetkiID.ToString()));
        //                //Veritabanımızdaki role tablosunda kullanıcı hakkında roller varsa onlarıda ekliyoruz



        //                var GirisYapanKullanici = new ClaimsIdentity(KullaniciClaims, CookieAuthenticationDefaults.AuthenticationScheme);

        //                var authProperties = new AuthenticationProperties
        //                {
        //                    IsPersistent = Hatirla
        //                };

        //                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);//varsa temizle

        //                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(GirisYapanKullanici), authProperties);


        //                return Ok();
        //                //Sadece üye olan kullanıcıların göreceği sayfaya yönlendirme
        //            }
        //            else
        //                return BadRequest();
        //        }
        //        else
        //            return BadRequest();

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }




        //}

        //[HttpPost]
        //[Route('KullaniciLogout')]
        //public async Task<IActionResult> KullaniciLogout()

        //{


        //    try
        //    {

        //        var Kullanici = User;


        //        if (ModelState.IsValid)
        //        {


        //            if (Kullanici.Identity.IsAuthenticated)
        //            {


        //                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);//varsa temizle

        //                return Ok();
        //                //Sadece üye olan kullanıcıların göreceği sayfaya yönlendirme
        //            }
        //            else
        //                return BadRequest();
        //        }
        //        else
        //            return BadRequest();

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }




        //}





            }
    }

                            ";
                Controller = ParametreleriGir(Controller, "%ProjeAdi%", ProjeAdi);
                ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Controllers/" + item + "Controller.cs", Controller);
            }
        }

        private void BusinessHazirla(string ProjeAdi, string ConString)
        {
            IServisesHazirla(ProjeAdi, ConString);
            ManagersHazirla(ProjeAdi, ConString);
        }

        private void ManagersHazirla(string ProjeAdi, string ConString)
        {
            foreach (var item in Nesneler.GroupBy(a => a.Tablo).Select(a => a.Key))
            {
                string Managers = @"

using %ProjeAdi%.Business.IServices;
using %ProjeAdi%.Genel.IGenel;
using %ProjeAdi%.Models;
using %ProjeAdi%.Models.Dtos;
using %ProjeAdi%.Models.Entities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace %ProjeAdi%.Business.Manager
{
    public class " + item + @"Manager : I" + item + @"Service
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<" + item + @"Manager> _logger;
        private readonly IMapper _mapper;

        public " + item + @"Manager(IUnitOfWork unit,
            ILogger<" + item + @"Manager> logger,
            IMapper mapper)
        {
            _uow = unit;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<int> AddAsync(" + item + @"Dto " + item + @"Dto)
        {
            try
            {
                var _" + item + @" = _mapper.Map<" + item + @">(" + item + @"Dto);
                await _uow." + item + @"Repository.AddAsync(_" + item + @");
                await _uow.Commit();
                return _" + item + @"." + Nesneler.Where(a => a.Tablo == item && a.PKmi).First().Kolon + @";
            }
            catch (Exception e)
            {
                _logger.LogError(e, 'Exception Error During Add " + item + @"', " + item + @"Dto);
                throw e;
        }

    }


    public async Task<WebApiResponse> UpdateAsync(int? " + item + @"Id, " + item + @"Dto " + item + @"Dto)
    {
        try
        {
            var _" + item + @" = await _uow." + item + @"Repository.GetByIdAsync(" + item + @"Id);
            if (_" + item + @" != null)
            {" +
string.Join("\n ", Nesneler.Where(a => a.Tablo == item && !a.PKmi).Select(a => "_" + a.Tablo + @"." + a.Kolon + " = " + a.Tablo + @"Dto." + a.Kolon + ";"))
                + @"
                bool result = await _uow." + item + @"Repository.UpdateAsync(_" + item + @");

                await _uow.Commit();

                return new WebApiResponse()
                {
                    StatusCode = result ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest,
                    Status = result
                };
            }
            else
            {
                return new WebApiResponse()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Status = false
                };
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, 'Exception Error During Update " + item + @"', " + item + @"Dto);
            throw e;
        }
    }

    public async Task<WebApiResponse> DeleteAsync(int? " + item + @"Id)
    {
        try
        {
            var _" + item + @" = await _uow." + item + @"Repository.FindByAsync(a => a." + Nesneler.Where(a => a.Tablo == item && a.PKmi).First().Kolon + @" == " + item + @"Id);
            if (_" + item + @" != null)
            {
                await _uow." + item + @"Repository.DeleteAsync(_" + item + @");
                await _uow.Commit();
                return new WebApiResponse()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Status = true
                };
            }
            else
            {
                return new WebApiResponse()
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Status = false
                };
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, 'Exception Error During Delete " + item + @"', " + item + @"Id);
            throw e;
        }
    }

    public async Task<IEnumerable<" + item + @">> GetAsync()
    {
        try
        {
            return await _uow." + item + @"Repository.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public async Task<" + item + @"> GetByIdAsync(int? " + item + @"Id)
    {
        try
        {
            var result = await _uow." + item + @"Repository.GetByIdAsync(" + item + @"Id);
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


//Özelsorgular
        //public async Task<Kullanicilar> Login(string KullaniciAdi, string Password)
        //{
        //    try
        //    {
        //        var Parameters = new List<SqlParameter> { new SqlParameter { ParameterName = '@K', Value = int.Parse(KullaniciAdi) }, new SqlParameter { ParameterName = '@P', Value = Password } };
        //        SqlCommand Command = new SqlCommand();
        //        Command.CommandText = 'SELECT * FROM Kullanicilar WHERE AktifMi = 1 AND Sicil = @K AND pwdcompare(@P, Sifre, 0) = 1';
        //        SqlParameter [] P = { new SqlParameter { ParameterName = '@K', Value = KullaniciAdi }, new SqlParameter { ParameterName = '@P', Value = Password } };
        //        return JsonSerializer.Deserialize<List<Kullanicilar>>(await _uow.KullanicilarRepository.SqlResultAsync(Command, P)).ToArray()[0];

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        }
        }


                            ";
                Managers = ParametreleriGir(Managers, "%ProjeAdi%", ProjeAdi);
                ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Business/Managers/" + item + "Manager.cs", Managers);
            }
        }


        private void IServisesHazirla(string ProjeAdi, string ConString)
        {
            foreach (var item in Nesneler.GroupBy(a => a.Tablo).Select(a => a.Key))
            {
                string IServices = @"


using %ProjeAdi%.Models;
using %ProjeAdi%.Models.Dtos;
using %ProjeAdi%.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace %ProjeAdi%.Business.IServices
{
    public interface I" + item + @"Service
    {
        Task<int> AddAsync(" + item + @"Dto entity);
        Task<WebApiResponse> UpdateAsync(int? " + item + @"Id, " + item + @"Dto entity);
        Task<WebApiResponse> DeleteAsync(int? " + item + @"Id);
        Task<IEnumerable<" + item + @">> GetAsync();
        Task<" + item + @"> GetByIdAsync(int? " + item + @"Id);
        
        //özel Sorgular
        // Task<Kullanicilar> Login(string KullaniciAdi, string Password);
    }
}


                            ";
                IServices = ParametreleriGir(IServices, "%ProjeAdi%", ProjeAdi);
                ClassKaydet(DosyaYolu + "/" + ProjeAdi + "/Business/IServices/I" + item + "Service.cs", IServices);
            }
        }


        private void ClassKaydet(string DosyaYoluAdi, string text)
        {
            FileStream fParameter = new FileStream(DosyaYoluAdi, FileMode.Create, FileAccess.Write);
            StreamWriter m_WriterParameter = new StreamWriter(fParameter);
            m_WriterParameter.BaseStream.Seek(0, SeekOrigin.End);
            m_WriterParameter.Write(text);
            m_WriterParameter.Flush();
            m_WriterParameter.Close();
        }

        private string ParametreleriGir(string input, string Eski, string Yeni)
        {

            return Regex.Replace(input, Eski, Yeni).Replace("'", "\"");
        }




        private List<DBNesneler_Type> DBNesneleriGetir(string ConString)
        {
            List<DBNesneler_Type> List = new List<DBNesneler_Type>();

            string Query = @"

                           SELECT
                              T.TABLE_NAME as Tablo 
                              ,K.COLUMN_NAME as Kolon
                              ,(case when K.DATA_TYPE ='datetime' then 'DateTime'when K.DATA_TYPE ='date' then 'DateTime' when K.DATA_TYPE ='nvarchar' then 'string' when K.DATA_TYPE ='nchar' then 'string' when K.DATA_TYPE ='varbinary' then 'byte[]' when K.DATA_TYPE ='bit' then 'bool' else K.DATA_TYPE end) as KolonTipi 
                              ,case when K.COLUMN_NAME=PK.PrimaryKey then 'true' else 'false' end as PKmi
                              ,case when K.IS_NULLABLE='Yes' then 'true' else 'false' end as Nullmu,*

                            FROM
                              INFORMATION_SCHEMA.TABLES as T left join
                              INFORMATION_SCHEMA.COLUMNS as K on
                              T.TABLE_NAME=K.TABLE_NAME left join
                              (
	                            SELECT
	                              T.TABLE_NAME,K.COLUMN_NAME as PrimaryKey
	                            FROM
	                              INFORMATION_SCHEMA.TABLES as T left join
	                              INFORMATION_SCHEMA.KEY_COLUMN_USAGE as K on
	                              T.TABLE_NAME=K.TABLE_NAME and not CONSTRAINT_NAME like'FK_%'
                              )as PK  on
                              PK.TABLE_NAME=T.TABLE_NAME 
							WHERE 
							 NOT T.TABLE_NAME like '%spt_%' and NOT T.TABLE_NAME like '%MSreplication_%' and NOT T.TABLE_NAME like 'sys%' and TABLE_TYPE='BASE TABLE'
                            ";

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                                List.Add(new DBNesneler_Type { Tablo = dr[0].ToString(), Kolon = dr[1].ToString(), KolonTipi = dr[2].ToString(), PKmi = bool.Parse(dr[3].ToString()), Nullmu = bool.Parse(dr[4].ToString()) });

                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }


            return List;
        }

        private void KlasorleriHazirla(string ProjeAdi)
        {

            if (!Directory.Exists(DosyaYolu))//Anadosya
                Directory.CreateDirectory(DosyaYolu);

            if (!Directory.Exists(DosyaYolu + "/" + ProjeAdi))//ProjeDosyasi
                Directory.CreateDirectory(DosyaYolu + "/" + ProjeAdi);
            else
            {
                DirectoryInfo di = new DirectoryInfo(DosyaYolu + "/" + ProjeAdi);

                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    foreach (FileInfo file in dir.GetFiles())
                        file.Delete();

                    dir.Delete(true);
                }
            }
            if (!Directory.Exists(DosyaYolu + "/" + ProjeAdi + "/Genel"))//Genel
                Directory.CreateDirectory(DosyaYolu + "/" + ProjeAdi + "/Genel");

            if (!Directory.Exists(DosyaYolu + "/" + ProjeAdi + "/Properties"))//Properties
                Directory.CreateDirectory(DosyaYolu + "/" + ProjeAdi + "/Properties");

            if (!Directory.Exists(DosyaYolu + "/" + ProjeAdi + "/Genel/IGenel"))//Genel//IGenel
                Directory.CreateDirectory(DosyaYolu + "/" + ProjeAdi + "/Genel/IGenel");

            if (!Directory.Exists(DosyaYolu + "/" + ProjeAdi + "/Business"))//Business
                Directory.CreateDirectory(DosyaYolu + "/" + ProjeAdi + "/Business");

            if (!Directory.Exists(DosyaYolu + "/" + ProjeAdi + "/Controllers"))//Controllers
                Directory.CreateDirectory(DosyaYolu + "/" + ProjeAdi + "/Controllers");

            if (!Directory.Exists(DosyaYolu + "/" + ProjeAdi + "/Business/IServices"))//Business//IServices
                Directory.CreateDirectory(DosyaYolu + "/" + ProjeAdi + "/Business/IServices");

            if (!Directory.Exists(DosyaYolu + "/" + ProjeAdi + "/Business/Managers"))//Business//Managers
                Directory.CreateDirectory(DosyaYolu + "/" + ProjeAdi + "/Business/Managers");

            if (!Directory.Exists(DosyaYolu + "/" + ProjeAdi + "/Models"))//Model
                Directory.CreateDirectory(DosyaYolu + "/" + ProjeAdi + "/Models");

            if (!Directory.Exists(DosyaYolu + "/" + ProjeAdi + "/Models/Dtos"))//Model//Dtos
                Directory.CreateDirectory(DosyaYolu + "/" + ProjeAdi + "/Models/Dtos");

            if (!Directory.Exists(DosyaYolu + "/" + ProjeAdi + "/Models/Entities"))//Model//Entities
                Directory.CreateDirectory(DosyaYolu + "/" + ProjeAdi + "/Models/Entities");


        }

        private void btnKlasor_Click(object sender, EventArgs e)
        {
            string ProjeAdi = txtProjeAdi.Text.Replace("-", "").Replace(" ", "").Replace("_", "");

            if (ProjeAdi != string.Empty)
                KlasorleriHazirla(ProjeAdi);

            if (Directory.Exists(DosyaYolu + "/" + ProjeAdi))
                Process.Start(DosyaYolu);
        }

        private void txtConString_TextChanged(object sender, EventArgs e)
        {
            string ProjeAdi = txtProjeAdi.Text.Replace(" - ", "").Replace(" ", "").Replace("_", "");


            if (ProjeAdi != string.Empty && txtConString.Text != string.Empty)
            {
                Nesneler = DBNesneleriGetir(txtConString.Text);

                lstTablolar.DataSource=null;
                lstTablolar.DataSource = Nesneler.GroupBy(a => a.Tablo).Select(a =>  a.Key ).ToList();
            }

        }

        private void txtProjeAdi_TextChanged(object sender, EventArgs e)
        {
            string ProjeAdi = txtProjeAdi.Text.Replace(" - ", "").Replace(" ", "").Replace("_", "");


            if (ProjeAdi != string.Empty && txtConString.Text != string.Empty)
            {
                Nesneler = DBNesneleriGetir(txtConString.Text);

                lstTablolar.DataSource = null;
                lstTablolar.DataSource = Nesneler.GroupBy(a => a.Tablo).Select(a => a.Key).ToList();
            }
        }
    }
}
