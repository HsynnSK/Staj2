using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOMManagement.Domain.Entities.BOMs;
using BOMManagement.Domain.Entities.Items;
using BOMManagement.Domain.Entities.Production;
using BOMManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BOMManagement.DataAccess
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(BOMDbContext context)
        {
            // Only seed if there are no Items in the database
            if (await context.Items.AnyAsync())
            {
                return;
            }

            // 1. Fetch Material Types and Warehouses (seeded by EF Core Configurations)
            var plasticType = await context.MaterialTypes.FirstOrDefaultAsync(m => m.Code == "MT-PLASTIK") ?? new MaterialType { Code = "MT-PLASTIK", Name = "Plastik Malzeme" };
            var metalType = await context.MaterialTypes.FirstOrDefaultAsync(m => m.Code == "MT-METAL") ?? new MaterialType { Code = "MT-METAL", Name = "Metal Malzeme" };
            var elecType = await context.MaterialTypes.FirstOrDefaultAsync(m => m.Code == "MT-ELEKTRONIK") ?? new MaterialType { Code = "MT-ELEKTRONIK", Name = "Elektronik Bileşen" };
            var montajType = await context.MaterialTypes.FirstOrDefaultAsync(m => m.Code == "MT-MONTAJ") ?? new MaterialType { Code = "MT-MONTAJ", Name = "Montaj Bileşeni" };
            var mekanikType = await context.MaterialTypes.FirstOrDefaultAsync(m => m.Code == "MT-MEKANIK") ?? new MaterialType { Code = "MT-MEKANIK", Name = "Mekanik Bileşen" };
            var mamulType = await context.MaterialTypes.FirstOrDefaultAsync(m => m.Code == "MT-MAMUL") ?? new MaterialType { Code = "MT-MAMUL", Name = "Mamul Ürün" };

            var whAna = await context.Warehouses.FirstOrDefaultAsync(w => w.WarehouseCode == "WH-01") ?? new Warehouse { WarehouseCode = "WH-01", WarehouseName = "Ana Depo" };
            var whHam = await context.Warehouses.FirstOrDefaultAsync(w => w.WarehouseCode == "WH-02") ?? new Warehouse { WarehouseCode = "WH-02", WarehouseName = "Hammadde Deposu" };
            var whYari = await context.Warehouses.FirstOrDefaultAsync(w => w.WarehouseCode == "WH-03") ?? new Warehouse { WarehouseCode = "WH-03", WarehouseName = "Yarı Mamül Deposu" };

            // 2. Add Items
            var itemsList = new List<Item>
            {
                new Item
                {
                    ItemCode = "RAW-ABS-PLASTIC",
                    ItemName = "ABS Plastik Hammadde Granül (Kg)",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = montajType,
                    MinimumStockLevel = 100,
                    StockQuantity = 500
                },
                new Item
                {
                    ItemCode = "RAW-MOUSE-PCB",
                    ItemName = "Özel Tasarım Mouse Optik PCB Kartı",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = elecType,
                    MinimumStockLevel = 200,
                    StockQuantity = 450
                },
                new Item
                {
                    ItemCode = "RAW-MOUSE-SENSOR",
                    ItemName = "26K DPI Optik Mouse Sensörü",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = elecType,
                    MinimumStockLevel = 200,
                    StockQuantity = 400
                },
                new Item
                {
                    ItemCode = "RAW-OMRON-SWITCH",
                    ItemName = "Mekanik Mouse Switch (Mikro Anahtar)",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = elecType,
                    MinimumStockLevel = 500,
                    StockQuantity = 1200
                },
                new Item
                {
                    ItemCode = "RAW-KEY-SWITCH",
                    ItemName = "Klavye Mekanik Red Switch (Tuş Anahtarı)",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = elecType,
                    MinimumStockLevel = 5000,
                    StockQuantity = 15000
                },
                new Item
                {
                    ItemCode = "RAW-KEY-PCB",
                    ItemName = "RGB Destekli Hot-Swap Klavye PCB",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = elecType,
                    MinimumStockLevel = 100,
                    StockQuantity = 250
                },
                new Item
                {
                    ItemCode = "RAW-STEEL-SHEET",
                    ItemName = "1.5mm Lazer Kesim Çelik Sac Kasa Plakası",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = mekanikType,
                    MinimumStockLevel = 50,
                    StockQuantity = 120
                },
                new Item
                {
                    ItemCode = "RAW-TEMP-GLASS",
                    ItemName = "Kasa Yan Paneli İçin Temperli Cam",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = mekanikType,
                    MinimumStockLevel = 50,
                    StockQuantity = 110
                },
                new Item
                {
                    ItemCode = "RAW-GPU-RTX4080",
                    ItemName = "NVIDIA RTX 4080 Super Ekran Kartı",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = elecType,
                    MinimumStockLevel = 20,
                    StockQuantity = 35
                },
                new Item
                {
                    ItemCode = "RAW-CPU-I9",
                    ItemName = "Intel Core i9-14900K İşlemci",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = elecType,
                    MinimumStockLevel = 20,
                    StockQuantity = 40
                },
                new Item
                {
                    ItemCode = "RAW-RAM-32GB-KIT",
                    ItemName = "32GB (2x16GB) DDR5 RGB RAM Kit",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = elecType,
                    MinimumStockLevel = 40,
                    StockQuantity = 90
                },
                new Item
                {
                    ItemCode = "RAW-LIQUID-COOLER",
                    ItemName = "360mm ARGB Sıvı Soğutma Seti",
                    ItemType = ItemType.RawMaterial,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whAna,
                    MaterialType = montajType,
                    MinimumStockLevel = 20,
                    StockQuantity = 50
                },
                new Item
                {
                    ItemCode = "FG-MONITOR-27",
                    ItemName = "27 inc 240Hz 0.5ms OLED Gaming Monitör",
                    ItemType = ItemType.FinishedGood,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whYari,
                    MaterialType = mamulType,
                    MinimumStockLevel = 10,
                    StockQuantity = 15
                },
                new Item
                {
                    ItemCode = "SUB-CASE-FRAME",
                    ItemName = "Kendi Dökümümüz İskelet Kasa (Boş)",
                    ItemType = ItemType.SubAssembly,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whHam,
                    MaterialType = mekanikType,
                    MinimumStockLevel = 15,
                    StockQuantity = 25
                },
                new Item
                {
                    ItemCode = "SUB-MOUSE-ASSY",
                    ItemName = "Kendi İmalatımız Makro Oyuncu Faresi",
                    ItemType = ItemType.SubAssembly,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whHam,
                    MaterialType = elecType,
                    MinimumStockLevel = 30,
                    StockQuantity = 50
                },
                new Item
                {
                    ItemCode = "SUB-KEYBOARD-ASSY",
                    ItemName = "Kendi İmalatımız %100 Mekanik RGB Klavye",
                    ItemType = ItemType.SubAssembly,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whHam,
                    MaterialType = elecType,
                    MinimumStockLevel = 30,
                    StockQuantity = 60
                },
                new Item
                {
                    ItemCode = "SUB-GAMING-PC",
                    ItemName = "Toplanmış İçi Dolu Gaming PC Kasası",
                    ItemType = ItemType.SubAssembly,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whHam,
                    MaterialType = elecType,
                    MinimumStockLevel = 10,
                    StockQuantity = 12
                },
                new Item
                {
                    ItemCode = "FG-GAMING-SETUP-PRO",
                    ItemName = "Ultimate Pro RGB Gaming Oyuncu Seti (Tam Paket)",
                    ItemType = ItemType.FinishedGood,
                    MainUnit = UnitType.Piece,
                    DefaultWarehouse = whYari,
                    MaterialType = mamulType,
                    MinimumStockLevel = 5,
                    StockQuantity = 8
                }
            };

            context.Items.AddRange(itemsList);
            await context.SaveChangesAsync();

            // 3. Add Work Orders for Items
            var workOrdersList = new List<WorkOrder>
            {
                new WorkOrder { ItemCode = "RAW-ABS-PLASTIC", WorkOrderNo = "WO-ABS", WarehouseCode = "WH-01", ManufacturingCode = "PLASTIK", WorkCode = "ENJEKSIYON", Description = "ABS Plastik hammadde granülleri enjeksiyon makinesine beslenerek mouse/klavye parça gövdeleri üretilir.", WorkCenterCode = "WC-ENJEKSIYON", SetupTime = 10, RunTime = 2 },
                new WorkOrder { ItemCode = "RAW-MOUSE-PCB", WorkOrderNo = "WO-M-PCB", WarehouseCode = "WH-01", ManufacturingCode = "ELEKTRONIK", WorkCode = "SMT", Description = "PCB kartı üzerine SMT dizgi makinesinde direnç, kondansatör ve mikroçipler dizilerek lehimlenir.", WorkCenterCode = "WC-SMT", SetupTime = 15, RunTime = 3 },
                new WorkOrder { ItemCode = "RAW-MOUSE-SENSOR", WorkOrderNo = "WO-M-SEN", WarehouseCode = "WH-01", ManufacturingCode = "ELEKTRONIK", WorkCode = "SMT", Description = "Optik fare sensörü PCB üzerindeki sokete yerleştirilip otomatik optik muayeneden (AOI) geçirilir.", WorkCenterCode = "WC-SMT", SetupTime = 5, RunTime = 1 },
                new WorkOrder { ItemCode = "RAW-OMRON-SWITCH", WorkOrderNo = "WO-OMRON", WarehouseCode = "WH-01", ManufacturingCode = "ELEKTRONIK", WorkCode = "LEHIM", Description = "Mikro anahtar butonlar PCB üzerindeki slotlara yerleştirilerek dalga lehim makinesinde sabitlenir.", WorkCenterCode = "WC-LEHIM", SetupTime = 8, RunTime = 2 },
                new WorkOrder { ItemCode = "RAW-KEY-SWITCH", WorkOrderNo = "WO-K-SW", WarehouseCode = "WH-01", ManufacturingCode = "MONTAJ", WorkCode = "MONTAJ", Description = "Mekanik switch tuş anahtarları klavye iskeleti üzerine teker teker basılarak oturtulur.", WorkCenterCode = "WC-MONTAJ", SetupTime = 15, RunTime = 10 },
                new WorkOrder { ItemCode = "RAW-KEY-PCB", WorkOrderNo = "WO-K-PCB", WarehouseCode = "WH-01", ManufacturingCode = "ELEKTRONIK", WorkCode = "SMT", Description = "Hot-Swap klavye PCB kartı test ünitesine bağlanarak tüm LED ve switch bağlantı yolları kontrol edilir.", WorkCenterCode = "WC-SMT", SetupTime = 10, RunTime = 4 },
                new WorkOrder { ItemCode = "RAW-STEEL-SHEET", WorkOrderNo = "WO-STEEL", WarehouseCode = "WH-01", ManufacturingCode = "MEKANIK", WorkCode = "LAZER", Description = "Çelik sac plakalar CNC lazer kesim tezgahında büküm ve kasa ölçülerine göre kesilir.", WorkCenterCode = "WC-CNC", SetupTime = 20, RunTime = 5 },
                new WorkOrder { ItemCode = "RAW-TEMP-GLASS", WorkOrderNo = "WO-GLASS", WarehouseCode = "WH-01", ManufacturingCode = "MEKANIK", WorkCode = "MONTAJ", Description = "Temperli cam yan panel, kasa kapağına conta ve koruyucu vidalar ile monte edilerek montaj hattına sevk edilir.", WorkCenterCode = "WC-MONTAJ", SetupTime = 5, RunTime = 2 },
                new WorkOrder { ItemCode = "RAW-GPU-RTX4080", WorkOrderNo = "WO-GPU", WarehouseCode = "WH-01", ManufacturingCode = "MONTAJ", WorkCode = "MONTAJ", Description = "RTX 4080 Ekran kartı, anakart üzerindeki PCIe slotuna takılarak kilit mandalı kapatılır ve vidalanır.", WorkCenterCode = "WC-MONTAJ", SetupTime = 5, RunTime = 3 },
                new WorkOrder { ItemCode = "RAW-CPU-I9", WorkOrderNo = "WO-CPU", WarehouseCode = "WH-01", ManufacturingCode = "ELEKTRONIK", WorkCode = "MONTAJ", Description = "i9 işlemci anakart soketine yerleştirilip sıkıştırma kolu kapatılır, termal macun uygulanır.", WorkCenterCode = "WC-MONTAJ", SetupTime = 3, RunTime = 2 },
                new WorkOrder { ItemCode = "RAW-RAM-32GB-KIT", WorkOrderNo = "WO-RAM", WarehouseCode = "WH-01", ManufacturingCode = "ELEKTRONIK", WorkCode = "MONTAJ", Description = "RGB DDR5 RAM modülleri dual-channel çalışacak şekilde anakart RAM slotlarına takılır.", WorkCenterCode = "WC-MONTAJ", SetupTime = 2, RunTime = 1 },
                new WorkOrder { ItemCode = "RAW-LIQUID-COOLER", WorkOrderNo = "WO-COOLER", WarehouseCode = "WH-01", ManufacturingCode = "MONTAJ", WorkCode = "MONTAJ", Description = "Sıvı soğutma radyatörü kasa üst paneline vidalanır, pompa bloğu işlemci üzerine sabitlenir.", WorkCenterCode = "WC-MONTAJ", SetupTime = 10, RunTime = 8 },
                new WorkOrder { ItemCode = "FG-MONITOR-27", WorkOrderNo = "WO-MONITOR", WarehouseCode = "WH-03", ManufacturingCode = "MAMUL", WorkCode = "TEST", Description = "Hazır gaming monitör, kalite kontrol testlerinden (ölü piksel vb.) geçirilerek kutusuna yerleştirilir.", WorkCenterCode = "WC-TEST", SetupTime = 5, RunTime = 5 },
                new WorkOrder { ItemCode = "SUB-CASE-FRAME", WorkOrderNo = "WO-SUB-FRAME", WarehouseCode = "WH-02", ManufacturingCode = "MEKANIK", WorkCode = "BUKUM", Description = "Lazer kesim saclar CNC büküm tezgahında bükülerek kasa iskeleti birleştirilir ve boyanır.", WorkCenterCode = "WC-CNC", SetupTime = 20, RunTime = 15 },
                new WorkOrder { ItemCode = "SUB-MOUSE-ASSY", WorkOrderNo = "WO-SUB-MOUSE", WarehouseCode = "WH-02", ManufacturingCode = "MONTAJ", WorkCode = "MONTAJ", Description = "Mouse PCB, sensör ve switch'ler enjeksiyon gövde içine yerleştirilerek alt kapak kapatılır ve vidalanır.", WorkCenterCode = "WC-MONTAJ", SetupTime = 15, RunTime = 12 },
                new WorkOrder { ItemCode = "SUB-KEYBOARD-ASSY", WorkOrderNo = "WO-SUB-KEYB", WarehouseCode = "WH-02", ManufacturingCode = "MONTAJ", WorkCode = "MONTAJ", Description = "Klavye PCB, switchler ve tuş takımları gövdeyle birleştirilerek test ünitesinde tüm tuşlar kontrol edilir.", WorkCenterCode = "WC-MONTAJ", SetupTime = 20, RunTime = 20 },
                new WorkOrder { ItemCode = "SUB-GAMING-PC", WorkOrderNo = "WO-SUB-PC", WarehouseCode = "WH-02", ManufacturingCode = "MONTAJ", WorkCode = "MONTAJ", Description = "Boş kasa içerisine anakart, işlemci, RAM, ekran kartı ve sıvı soğutma monte edilerek kablolama yapılır.", WorkCenterCode = "WC-MONTAJ", SetupTime = 30, RunTime = 40 },
                new WorkOrder { ItemCode = "FG-GAMING-SETUP-PRO", WorkOrderNo = "WO-FG-SETUP", WarehouseCode = "WH-03", ManufacturingCode = "MAMUL", WorkCode = "PAKET", Description = "Gaming PC, OLED monitör, mekanik klavye ve fare paletlenerek tek bir gaming oyuncu set paketi halinde ambalajlanır.", WorkCenterCode = "WC-MONTAJ", SetupTime = 15, RunTime = 15 }
            };

            context.WorkOrders.AddRange(workOrdersList);
            await context.SaveChangesAsync();

            // 4. Add BOM Headers and Lines
            var bomFrame = new BOMHeader
            {
                BOMCode = "BOM-SUB-FRAME-V1",
                ParentItemCode = "SUB-CASE-FRAME",
                BaseQuantity = 1,
                UnitCode = UnitType.Piece,
                IsActive = true,
                Version = 1,
                Lines = new List<BOMLine>
                {
                    new BOMLine { ChildItemCode = "RAW-STEEL-SHEET", Quantity = 2.5m, UnitCode = UnitType.Piece, ScrapRate = 2.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false },
                    new BOMLine { ChildItemCode = "RAW-TEMP-GLASS", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 1.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false }
                }
            };

            var bomMouse = new BOMHeader
            {
                BOMCode = "BOM-SUB-MOUSE-V1",
                ParentItemCode = "SUB-MOUSE-ASSY",
                BaseQuantity = 1,
                UnitCode = UnitType.Piece,
                IsActive = true,
                Version = 1,
                Lines = new List<BOMLine>
                {
                    new BOMLine { ChildItemCode = "RAW-ABS-PLASTIC", Quantity = 0.1200m, UnitCode = UnitType.Piece, ScrapRate = 3.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false },
                    new BOMLine { ChildItemCode = "RAW-MOUSE-PCB", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false },
                    new BOMLine { ChildItemCode = "RAW-MOUSE-SENSOR", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false },
                    new BOMLine { ChildItemCode = "RAW-OMRON-SWITCH", Quantity = 2.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false }
                }
            };

            var bomKeyboard = new BOMHeader
            {
                BOMCode = "BOM-SUB-KEYB-V1",
                ParentItemCode = "SUB-KEYBOARD-ASSY",
                BaseQuantity = 1,
                UnitCode = UnitType.Piece,
                IsActive = true,
                Version = 1,
                Lines = new List<BOMLine>
                {
                    new BOMLine { ChildItemCode = "RAW-ABS-PLASTIC", Quantity = 0.4500m, UnitCode = UnitType.Piece, ScrapRate = 2.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false },
                    new BOMLine { ChildItemCode = "RAW-KEY-PCB", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false },
                    new BOMLine { ChildItemCode = "RAW-KEY-SWITCH", Quantity = 104.0m, UnitCode = UnitType.Piece, ScrapRate = 1.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false }
                }
            };

            var bomPC = new BOMHeader
            {
                BOMCode = "BOM-SUB-PC-V1",
                ParentItemCode = "SUB-GAMING-PC",
                BaseQuantity = 1,
                UnitCode = UnitType.Piece,
                IsActive = true,
                Version = 1,
                Lines = new List<BOMLine>
                {
                    new BOMLine { ChildItemCode = "SUB-CASE-FRAME", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-02", IsSubAssembly = true },
                    new BOMLine { ChildItemCode = "RAW-GPU-RTX4080", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false },
                    new BOMLine { ChildItemCode = "RAW-CPU-I9", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false },
                    new BOMLine { ChildItemCode = "RAW-RAM-32GB-KIT", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false },
                    new BOMLine { ChildItemCode = "RAW-LIQUID-COOLER", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-01", IsSubAssembly = false }
                }
            };

            var bomSetup = new BOMHeader
            {
                BOMCode = "BOM-FG-SETUP-V1",
                ParentItemCode = "FG-GAMING-SETUP-PRO",
                BaseQuantity = 1,
                UnitCode = UnitType.Piece,
                IsActive = true,
                Version = 1,
                Lines = new List<BOMLine>
                {
                    new BOMLine { ChildItemCode = "SUB-GAMING-PC", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-02", IsSubAssembly = true },
                    new BOMLine { ChildItemCode = "FG-MONITOR-27", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-03", IsSubAssembly = false },
                    new BOMLine { ChildItemCode = "SUB-KEYBOARD-ASSY", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-02", IsSubAssembly = true },
                    new BOMLine { ChildItemCode = "SUB-MOUSE-ASSY", Quantity = 1.0m, UnitCode = UnitType.Piece, ScrapRate = 0.00m, ConsumptionWarehouse = "WH-02", IsSubAssembly = true }
                }
            };

            context.BOMHeaders.AddRange(bomFrame, bomMouse, bomKeyboard, bomPC, bomSetup);
            await context.SaveChangesAsync();
        }
    }
}
