using AutoMapper;
using BOMManagement.Application.DTOs.Item;
using BOMManagement.Domain.Entities.Items;
using BOMManagement.Domain.Entities.BOMs;
using BOMManagement.Domain.Entities.Production;
using BOMManagement.Application.DTOs.BOM;
using BOMManagement.Application.DTOs.WorkOrder;
using BOMManagement.Application.DTOs.PurchaseOrder;
using BOMManagement.Application.DTOs.Production;

namespace BOMManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Item mappings
        CreateMap<Item, ItemListDto>()
            .ForMember(dest => dest.MaterialTypeName, opt => opt.MapFrom(src => src.MaterialType != null ? src.MaterialType.Name : string.Empty))
            .ForMember(dest => dest.DefaultWarehouseCode, opt => opt.MapFrom(src => src.DefaultWarehouse != null ? src.DefaultWarehouse.WarehouseCode : string.Empty));

        CreateMap<Item, ItemDetailDto>()
            .ForMember(dest => dest.MaterialTypeId, opt => opt.MapFrom(src => src.MaterialType != null ? src.MaterialType.Id : 0))
            .ForMember(dest => dest.MaterialTypeCode, opt => opt.MapFrom(src => src.MaterialType != null ? src.MaterialType.Code : string.Empty))
            .ForMember(dest => dest.MaterialTypeName, opt => opt.MapFrom(src => src.MaterialType != null ? src.MaterialType.Name : string.Empty))
            .ForMember(dest => dest.DefaultWarehouseId, opt => opt.MapFrom(src => src.DefaultWarehouse != null ? src.DefaultWarehouse.Id : (int?)null))
            .ForMember(dest => dest.DefaultWarehouseCode, opt => opt.MapFrom(src => src.DefaultWarehouse != null ? src.DefaultWarehouse.WarehouseCode : string.Empty))
            .ForMember(dest => dest.DefaultWarehouseName, opt => opt.MapFrom(src => src.DefaultWarehouse != null ? src.DefaultWarehouse.WarehouseName : string.Empty));

        CreateMap<ItemCreateDto, Item>()
            .ForMember(dest => dest.MaterialType, opt => opt.Ignore())
            .ForMember(dest => dest.DefaultWarehouse, opt => opt.Ignore())
            .ForMember(dest => dest.Recipes, opt => opt.Ignore());

        CreateMap<ItemUpdateDto, Item>()
            .ForMember(dest => dest.MaterialType, opt => opt.Ignore())
            .ForMember(dest => dest.DefaultWarehouse, opt => opt.Ignore())
            .ForMember(dest => dest.Recipes, opt => opt.Ignore());

        CreateMap<Warehouse, BOMManagement.Application.DTOs.Warehouse.WarehouseDto>();
        CreateMap<MaterialType, BOMManagement.Application.DTOs.MaterialType.MaterialTypeDto>();

        // BOM Mappings
        CreateMap<BOMHeader, BOMHeaderListDto>();
        CreateMap<BOMHeader, BOMHeaderDetailDto>();
        CreateMap<BOMHeaderCreateDto, BOMHeader>()
            .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.Lines));
        CreateMap<BOMHeaderUpdateDto, BOMHeader>()
            .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.Lines));

        CreateMap<BOMLine, BOMLineDto>();
        CreateMap<BOMLineDto, BOMLine>()
            .ForMember(dest => dest.BOMHeader, opt => opt.Ignore());

        // WorkOrder Mappings
        CreateMap<WorkOrder, WorkOrderListDto>();
        CreateMap<WorkOrder, WorkOrderDetailDto>();
        CreateMap<WorkOrderCreateDto, WorkOrder>();
        CreateMap<WorkOrderUpdateDto, WorkOrder>();

        // Production Mappings
        CreateMap<Production, ProductionListDto>();
        CreateMap<Production, ProductionDetailDto>();
        CreateMap<ProductionCreateDto, Production>()
            .ForMember(dest => dest.Status, opt => opt.Ignore());

        // PurchaseOrder Mappings
        CreateMap<PurchaseOrder, PurchaseOrderListDto>();
        CreateMap<PurchaseOrderCreateDto, PurchaseOrder>()
            .ForMember(dest => dest.Status, opt => opt.Ignore());
    }
}
