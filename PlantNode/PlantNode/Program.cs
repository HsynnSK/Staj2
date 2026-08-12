using Microsoft.EntityFrameworkCore;
using PlantNode.DataAccess.Contexts;
using PlantNode.Components;
using PlantNode.Domain.Strategies;
using PlantNode.Domain.Interfaces;
using PlantNode.Domain.Entities;
using PlantNode.Domain.Entities.Machines;
using PlantNode.Domain.Entities.Plants;
using PlantNode.Domain.Enums;
using PlantNode.Application.Interfaces;
using PlantNode.Application.Services;
using PlantNode.Services;
using PlantNode.Domain.Entities.Products;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ICoilWindingSpeedStrategyFactory, CoilWindingSpeedStrategyFactory>();
builder.Services.AddSingleton<IPlanEventService, PlanEventService>();
builder.Services.AddTransient<IPlantService, PlantService>();
builder.Services.AddTransient<IMachineService, MachineService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IMaterialTypeService, MaterialTypeService>();
builder.Services.AddTransient<IMaintenanceService, MaintenanceService>();
builder.Services.AddTransient<IProductionPlanService, ProductionPlanService>();
builder.Services.AddHostedService<MachineStatusMonitorService>();

builder.Services.AddDbContext<PlantNodeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=(localdb)\\mssqllocaldb;Database=PlantNodeDb;Trusted_Connection=True;MultipleActiveResultSets=true"),
    ServiceLifetime.Transient);

builder.Services.AddTransient<IPlantNodeDbContext>(provider => provider.GetRequiredService<PlantNodeDbContext>());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PlantNodeDbContext>();
    
    // Seed 10 MaterialTypes if empty
    if (!context.Set<MaterialType>().Any())
    {
        context.Set<MaterialType>().AddRange(
            new MaterialType { Name = "Stretch Film", MaterialCode = "MAT-STR", MaxMachineSpeed = 1.0f, TensionLimit = 15.0m, AccelerationRate = 1.5m, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new MaterialType { Name = "Packaging Film", MaterialCode = "MAT-PKG", MaxMachineSpeed = 0.8f, TensionLimit = 12.0m, AccelerationRate = 1.0m, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new MaterialType { Name = "Aluminum Foil", MaterialCode = "MAT-ALU", MaxMachineSpeed = 0.5f, TensionLimit = 8.0m, AccelerationRate = 0.5m, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new MaterialType { Name = "Kraft and Carton", MaterialCode = "MAT-KRF", MaxMachineSpeed = 0.9f, TensionLimit = 25.0m, AccelerationRate = 2.0m, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new MaterialType { Name = "Carbon Fiber", MaterialCode = "MAT-CAR", MaxMachineSpeed = 0.4f, TensionLimit = 5.0m, AccelerationRate = 0.3m, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new MaterialType { Name = "Tissue Paper", MaterialCode = "MAT-TIS", MaxMachineSpeed = 1.1f, TensionLimit = 10.0m, AccelerationRate = 1.2m, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new MaterialType { Name = "Adhesive Label", MaterialCode = "MAT-ADH", MaxMachineSpeed = 0.7f, TensionLimit = 14.0m, AccelerationRate = 0.8m, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new MaterialType { Name = "Textile Fabric", MaterialCode = "MAT-TEX", MaxMachineSpeed = 0.95f, TensionLimit = 20.0m, AccelerationRate = 1.6m, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new MaterialType { Name = "Metal Sheet", MaterialCode = "MAT-MET", MaxMachineSpeed = 0.3f, TensionLimit = 35.0m, AccelerationRate = 0.4m, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new MaterialType { Name = "Copper Wire Coating", MaterialCode = "MAT-COP", MaxMachineSpeed = 1.2f, TensionLimit = 18.0m, AccelerationRate = 1.8m, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now }
        );
        context.SaveChanges();
    }

    // Seed 15 Products using all 10 MaterialTypes if empty
    if (!context.Products.Any())
    {
        var mats = context.Set<MaterialType>().ToList();
        var mStr = mats.First(m => m.MaterialCode == "MAT-STR");
        var mPkg = mats.First(m => m.MaterialCode == "MAT-PKG");
        var mAlu = mats.First(m => m.MaterialCode == "MAT-ALU");
        var mKrf = mats.First(m => m.MaterialCode == "MAT-KRF");
        var mCar = mats.First(m => m.MaterialCode == "MAT-CAR");
        var mTis = mats.First(m => m.MaterialCode == "MAT-TIS");
        var mAdh = mats.First(m => m.MaterialCode == "MAT-ADH");
        var mTex = mats.First(m => m.MaterialCode == "MAT-TEX");
        var mMet = mats.First(m => m.MaterialCode == "MAT-MET");
        var mCop = mats.First(m => m.MaterialCode == "MAT-COP");

        context.Products.AddRange(
            new Product { Name = "Standart Streç Film (50cm)", MaterialType = mStr, ProductCode = "PRD-STR-01", Barcode = "8680001001", Description = "Standart endüstriyel sarım streç filmi.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Gıda Tipi Streç Film", MaterialType = mStr, ProductCode = "PRD-STR-02", Barcode = "8680001002", Description = "Gıda paketlemeye uygun streç film.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Gıda Sınıfı Ambalaj Filmi", MaterialType = mPkg, ProductCode = "PRD-PKG-01", Barcode = "8680001003", Description = "Yüksek mukavemetli gıda ambalajı.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "BOPP Ambalaj Filmi", MaterialType = mPkg, ProductCode = "PRD-PKG-02", Barcode = "8680001004", Description = "BOPP şeffaf paketleme filmi.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Endüstriyel Alüminyum Folyo", MaterialType = mAlu, ProductCode = "PRD-ALU-01", Barcode = "8680001005", Description = "Isı yalıtımlı alüminyum folyo rulosu.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Bobin Kraft Kağıdı (80g)", MaterialType = mKrf, ProductCode = "PRD-KRF-01", Barcode = "8680001006", Description = "Mukavva ve ambalajlık kraft kağıdı.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Yüksek Mukavemetli Karbon Fiber", MaterialType = mCar, ProductCode = "PRD-CAR-01", Barcode = "8680001007", Description = "Hafif ve dayanıklı karbon elyaf bobini.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Yumuşak Selüloz Peçete Kağıdı", MaterialType = mTis, ProductCode = "PRD-TIS-01", Barcode = "8680001008", Description = "Çift katlı selüloz bobin kağıdı.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Termal Yapışkanlı Etiket", MaterialType = mAdh, ProductCode = "PRD-ADH-01", Barcode = "8680001009", Description = "Barkod yazıcılar için termal etiket.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Örme Tekstil Kumaş Rulosu", MaterialType = mTex, ProductCode = "PRD-TEX-01", Barcode = "8680001010", Description = "Sentetik örme kumaş bobini.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Galvaniz Sac Şerit Rulo", MaterialType = mMet, ProductCode = "PRD-MET-01", Barcode = "8680001011", Description = "Endüstriyel galvaniz sac metal şerit.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Bakır İletken Kablo İzolesi", MaterialType = mCop, ProductCode = "PRD-COP-01", Barcode = "8680001012", Description = "İzoleli bakır tel bobini.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Krep Peçete Kağıdı (Endüstriyel)", MaterialType = mTis, ProductCode = "PRD-TIS-02", Barcode = "8680001013", Description = "Endüstriyel krep bobin kağıt.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Oluklu Mukavva Karton Rulosu", MaterialType = mKrf, ProductCode = "PRD-KRF-02", Barcode = "8680001014", Description = "Karton kutu yapımı için mukavva.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
            new Product { Name = "Kuşe Etiket Kağıdı", MaterialType = mAdh, ProductCode = "PRD-ADH-02", Barcode = "8680001015", Description = "Kuşe kağıt yapışkanlı bobin.", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now }
        );
        context.SaveChanges();
    }
    
    // Seed 9 Plants and 19 Machines if empty
    if (!context.Plants.Any())
    {
        var cw1 = new CoilWindingPlant { Name = "Kocaeli Bobin Sarım Tesisleri", PlantCode = "PLT-CW-01", Type = PlantType.Manufacturing, MaxSupportedWireGauge = 4.5m, MaxAnnealingTemperatureCelsius = 850, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
        var cw2 = new CoilWindingPlant { Name = "İzmir Tel ve Sarım Fabrikası", PlantCode = "PLT-CW-02", Type = PlantType.Manufacturing, MaxSupportedWireGauge = 3.0m, MaxAnnealingTemperatureCelsius = 750, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
        var cw3 = new CoilWindingPlant { Name = "Bursa Motor Bobinaj Tesisi", PlantCode = "PLT-CW-03", Type = PlantType.Manufacturing, MaxSupportedWireGauge = 5.0m, MaxAnnealingTemperatureCelsius = 900, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
        var cw4 = new CoilWindingPlant { Name = "Ankara Trafo Sarım Atölyesi", PlantCode = "PLT-CW-04", Type = PlantType.Manufacturing, MaxSupportedWireGauge = 8.0m, MaxAnnealingTemperatureCelsius = 1000, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
        var cw5 = new CoilWindingPlant { Name = "Adana Kablo ve Sarım Atölyesi", PlantCode = "PLT-CW-05", Type = PlantType.Manufacturing, MaxSupportedWireGauge = 4.0m, MaxAnnealingTemperatureCelsius = 800, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
        var cw6 = new CoilWindingPlant { Name = "Gaziantep İplik Sarım Tesisi", PlantCode = "PLT-CW-06", Type = PlantType.Manufacturing, MaxSupportedWireGauge = 2.5m, MaxAnnealingTemperatureCelsius = 600, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };

        var any1 = new AnyPlant { Name = "Gebze Genel Amaçlı Üretim Kampüsü", PlantCode = "PLT-ANY-01", Type = PlantType.Manufacturing, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
        var any2 = new AnyPlant { Name = "Manisa Montaj ve Depolama Kompleksi", PlantCode = "PLT-ANY-02", Type = PlantType.Assembly, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
        var any3 = new AnyPlant { Name = "Eskişehir Paketleme ve Lojistik Merkezi", PlantCode = "PLT-ANY-03", Type = PlantType.Packaging, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };

        context.Plants.AddRange(cw1, cw2, cw3, cw4, cw5, cw6, any1, any2, any3);
        context.SaveChanges();

        if (!context.Machines.Any())
        {
            var pList = context.Products.Include(p => p.MaterialType).ToList();
            var prodStr = pList.First(p => p.ProductCode == "PRD-STR-01");
            var prodPkg = pList.First(p => p.ProductCode == "PRD-PKG-01");
            var prodAlu = pList.First(p => p.ProductCode == "PRD-ALU-01");
            var prodKrf = pList.First(p => p.ProductCode == "PRD-KRF-01");
            var prodCar = pList.First(p => p.ProductCode == "PRD-CAR-01");
            var prodTis = pList.First(p => p.ProductCode == "PRD-TIS-01");

            context.Machines.AddRange(
                // 6 CoilWindingMachines under 6 CoilWindingPlants
                new CoilWindingMachine { Name = "Alpha Sarım Hattı", MachineCode = "MAC-CW-01", Plant = cw1, SerialNumber = "SN-CW-1001", IsActive = true, Width = 120, Height = 150, Depth = 90, Status = MachineStatus.Active, MaxRpm = 1500, MinWireDiameter = 0.2m, MaxWireDiameter = 2.5m, MaxCoilDiameter = 300, SpindleCount = 4, TargetTurnCount = 500, WireTension = 15.5m, TargetSpeed = 25.0f, CurrentProduct = prodStr, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new CoilWindingMachine { Name = "Beta Sarım Makinesi", MachineCode = "MAC-CW-02", Plant = cw2, SerialNumber = "SN-CW-1002", IsActive = true, Width = 110, Height = 140, Depth = 85, Status = MachineStatus.Active, MaxRpm = 1200, MinWireDiameter = 0.1m, MaxWireDiameter = 1.8m, MaxCoilDiameter = 250, SpindleCount = 2, TargetTurnCount = 400, WireTension = 12.0m, TargetSpeed = 22.5f, CurrentProduct = prodPkg, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new CoilWindingMachine { Name = "Gamma Motor Bobinaj", MachineCode = "MAC-CW-03", Plant = cw3, SerialNumber = "SN-CW-1003", IsActive = true, Width = 130, Height = 160, Depth = 95, Status = MachineStatus.Idle, MaxRpm = 1800, MinWireDiameter = 0.3m, MaxWireDiameter = 3.0m, MaxCoilDiameter = 350, SpindleCount = 6, TargetTurnCount = 600, WireTension = 22.0m, TargetSpeed = 18.0f, CurrentProduct = prodAlu, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new CoilWindingMachine { Name = "Delta Trafo Sarma", MachineCode = "MAC-CW-04", Plant = cw4, SerialNumber = "SN-CW-1004", IsActive = true, Width = 200, Height = 220, Depth = 150, Status = MachineStatus.Active, MaxRpm = 800, MinWireDiameter = 0.5m, MaxWireDiameter = 5.0m, MaxCoilDiameter = 500, SpindleCount = 1, TargetTurnCount = 1000, WireTension = 30.0m, TargetSpeed = 12.0f, CurrentProduct = prodKrf, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new CoilWindingMachine { Name = "Epsilon Kablo Sarım", MachineCode = "MAC-CW-05", Plant = cw5, SerialNumber = "SN-CW-1005", IsActive = true, Width = 115, Height = 145, Depth = 88, Status = MachineStatus.Active, MaxRpm = 1400, MinWireDiameter = 0.15m, MaxWireDiameter = 2.0m, MaxCoilDiameter = 280, SpindleCount = 4, TargetTurnCount = 450, WireTension = 14.5m, TargetSpeed = 20.0f, CurrentProduct = prodStr, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new CoilWindingMachine { Name = "Zeta İplik Sarıcı", MachineCode = "MAC-CW-06", Plant = cw6, SerialNumber = "SN-CW-1006", IsActive = true, Width = 95, Height = 130, Depth = 75, Status = MachineStatus.Maintenance, MaxRpm = 2000, MinWireDiameter = 0.05m, MaxWireDiameter = 1.2m, MaxCoilDiameter = 180, SpindleCount = 8, TargetTurnCount = 800, WireTension = 8.5m, TargetSpeed = 27.5f, CurrentProduct = prodCar, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },

                // AnyPlant #1: 3 AnyMachine, 1 CoilWindingMachine
                new AnyMachine { Name = "Enjeksiyon Pres Makinesi A", MachineCode = "MAC-ANY-01", Plant = any1, SerialNumber = "SN-ANY-2001", IsActive = true, Width = 180, Height = 200, Depth = 120, Status = MachineStatus.Active, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new AnyMachine { Name = "Konveyör Taşıma Hattı 1", MachineCode = "MAC-ANY-02", Plant = any1, SerialNumber = "SN-ANY-2002", IsActive = true, Width = 300, Height = 100, Depth = 80, Status = MachineStatus.Active, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new AnyMachine { Name = "Hidrolik Güç Ünitesi B", MachineCode = "MAC-ANY-03", Plant = any1, SerialNumber = "SN-ANY-2003", IsActive = true, Width = 100, Height = 120, Depth = 80, Status = MachineStatus.Active, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new CoilWindingMachine { Name = "Omega Yardımcı Sarım Ünitesi", MachineCode = "MAC-CW-07", Plant = any1, SerialNumber = "SN-CW-1007", IsActive = true, Width = 120, Height = 150, Depth = 90, Status = MachineStatus.Active, MaxRpm = 1500, MinWireDiameter = 0.2m, MaxWireDiameter = 2.5m, MaxCoilDiameter = 300, SpindleCount = 4, TargetTurnCount = 500, WireTension = 15.5m, TargetSpeed = 25.0f, CurrentProduct = prodStr, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },

                // AnyPlant #2: 2 AnyMachine, 1 CoilWindingMachine
                new AnyMachine { Name = "Robotik Kol Hücresi R1", MachineCode = "MAC-ANY-04", Plant = any2, SerialNumber = "SN-ANY-2004", IsActive = true, Width = 150, Height = 180, Depth = 150, Status = MachineStatus.Active, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new AnyMachine { Name = "Lazer Kesim İstasyonu L1", MachineCode = "MAC-ANY-05", Plant = any2, SerialNumber = "SN-ANY-2005", IsActive = true, Width = 220, Height = 190, Depth = 140, Status = MachineStatus.Idle, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new CoilWindingMachine { Name = "Sigma Entegre Sarım", MachineCode = "MAC-CW-08", Plant = any2, SerialNumber = "SN-CW-1008", IsActive = true, Width = 110, Height = 140, Depth = 85, Status = MachineStatus.Active, MaxRpm = 1200, MinWireDiameter = 0.1m, MaxWireDiameter = 1.8m, MaxCoilDiameter = 250, SpindleCount = 2, TargetTurnCount = 400, WireTension = 12.0m, TargetSpeed = 22.5f, CurrentProduct = prodPkg, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },

                // AnyPlant #3: 5 AnyMachine, 1 CoilWindingMachine
                new AnyMachine { Name = "Ambalaj Koli Kapatıcı", MachineCode = "MAC-ANY-06", Plant = any3, SerialNumber = "SN-ANY-2006", IsActive = true, Width = 140, Height = 130, Depth = 95, Status = MachineStatus.Active, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new AnyMachine { Name = "Otomatik Çemberleme C1", MachineCode = "MAC-ANY-07", Plant = any3, SerialNumber = "SN-ANY-2007", IsActive = true, Width = 100, Height = 140, Depth = 90, Status = MachineStatus.Active, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new AnyMachine { Name = "Etiketleme Aplikatörü E1", MachineCode = "MAC-ANY-08", Plant = any3, SerialNumber = "SN-ANY-2008", IsActive = true, Width = 80, Height = 150, Depth = 60, Status = MachineStatus.Active, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new AnyMachine { Name = "Palet Sarıcı Streç P1", MachineCode = "MAC-ANY-09", Plant = any3, SerialNumber = "SN-ANY-2009", IsActive = true, Width = 180, Height = 240, Depth = 180, Status = MachineStatus.Active, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new AnyMachine { Name = "Karton Kutu Erektör", MachineCode = "MAC-ANY-10", Plant = any3, SerialNumber = "SN-ANY-2010", IsActive = true, Width = 160, Height = 170, Depth = 110, Status = MachineStatus.Active, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new CoilWindingMachine { Name = "Tau Yardımcı Sarıcı", MachineCode = "MAC-CW-09", Plant = any3, SerialNumber = "SN-CW-1009", IsActive = true, Width = 130, Height = 160, Depth = 95, Status = MachineStatus.Active, MaxRpm = 1800, MinWireDiameter = 0.3m, MaxWireDiameter = 3.0m, MaxCoilDiameter = 350, SpindleCount = 6, TargetTurnCount = 600, WireTension = 22.0m, TargetSpeed = 18.0f, CurrentProduct = prodTis, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now }
            );
            context.SaveChanges();
        }
    }
}

app.Run();
