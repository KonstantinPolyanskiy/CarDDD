using AutoMapper;
using CarDDD.ApplicationServices.Models.AnswerObjects.ServiceResponses;
using CarDDD.ApplicationServices.Repositories;
using CarDDD.ApplicationServices.Services;
using CarDDD.ApplicationServices.Storages;
using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.ValueObjects;
using MockQueryable;
using MockQueryable.Moq;
using Moq;

namespace CarDDD.DomainTests.Application.Services.CarQueryServiceTests;

public class CarsFilterTests
{
    private readonly Mock<ICarRepositoryReader> _readerMock = new();
    private Mock<IPhotoStorage> _photoStorageMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    
    private List<Car> _cars =
    [
        Car.CreateWithoutPhoto("Lada", "Black", 100_000, 100_000,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("Lada", "White", 200_000, 10_000,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("Lada", "Blue", 300_000, 1000,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("Lada", "Orange", 400_000, 0,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,


        Car.CreateWithoutPhoto("Audi", "Black", 300_000, 100_023,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("Audi", "White", 400_000, 10_032,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("Audi", "Blue", 500_000, 1650,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("Audi", "Orange", 1_000_000, 0,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,


        Car.CreateWithoutPhoto("BMW", "Black", 300_050, 123_030,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("BMW", "White", 450_000, 10_195,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("BMW", "Blue", 523_000, 1486,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("BMW", "Orange", 1_300_000, 0,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,


        Car.CreateWithoutPhoto("Fiat", "Black", 230_500, 133_000,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("Fiat", "White", 305_021, 10_132,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("Fiat", "Blue", 484_431, 1650,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car,

        Car.CreateWithoutPhoto("Fiat", "Orange", 737_645, 0,
            ManagerId.From(Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"))).Car
    ];

    public CarsFilterTests()
    {
        var queryMock = _cars.AsQueryable().BuildMock();
        _readerMock.Setup(r => r.CarsQueryAsync()).ReturnsAsync(queryMock);
        _readerMock.Setup(r => r.GetAsync(default, CancellationToken.None)).ReturnsAsync(new Car());
        
        _mapperMock.Setup(m => m.Map<CarInfo>(It.IsAny<Car>()))
            .Returns<Car>(c => new CarInfo { Brand = c.Brand, Color = c.Color, Price = c.Price });
    }

    [Fact]
    public async Task CarQueryService_AllFiltersFlow_Ok()
    {
        var service = new CarQueryService(_readerMock.Object, _photoStorageMock.Object, _mapperMock.Object);
        
        // Фильтруем по цвету - остаются все бренды, ценовой диапазон 100_000 - 300_050
        var colorFilter = new CarFilters { Colors = ["Black"], };
        
        var result = await service.CarByFiltersAsync(colorFilter);
        var availableFilters = result.Value!.AvailableFilters;
        
        availableFilters.Colors.Should().BeEquivalentTo("Black"); // доступен только черный цвет
        availableFilters.Brands.Should().BeEquivalentTo("Audi", "BMW", "Fiat", "Lada"); // доступны все бренды
        
        // Мин. и макс. ценовой диапазон у тестовых данных для черных машин
        availableFilters.PriceRange?.Min.Should().Be(100_000);
        availableFilters.PriceRange?.Max.Should().Be(300_050);
        
        // Все так же по цвету + только BMW и Audi
        var brandAndColorFilter = new CarFilters
        {
            Brands = ["Audi", "BMW"],
            Colors = colorFilter.Colors,
        };
        
        result = await service.CarByFiltersAsync(brandAndColorFilter);
        availableFilters = result.Value!.AvailableFilters;
        
        availableFilters.Colors.Should().BeEquivalentTo("Black"); // все еще только черный цвет
        availableFilters.Brands.Should().BeEquivalentTo("Audi", "BMW"); // по фильтру брендов
        
        // Мин. и макс. ценовой диапазон у тестовых данных для черных машин марки Audi и BMW
        availableFilters.PriceRange?.Min.Should().Be(300_000);
        availableFilters.PriceRange?.Max.Should().Be(300_050);
        
        var priceAndBrandAndColorFilter = new CarFilters
        {
            Brands = brandAndColorFilter.Brands,
            Colors = colorFilter.Colors,
            PriceRange = (300_000, 300_040), // Bmw не должна попасть, ее цена выше
        };
        
        result = await service.CarByFiltersAsync(priceAndBrandAndColorFilter);
        availableFilters = result.Value!.AvailableFilters;
        
        availableFilters.Colors.Should().BeEquivalentTo("Black");
        availableFilters.Brands.Should().BeEquivalentTo("Audi");
        
        // В результате должна быть найдена 1 машина
        result.Value!.Cars.Count.Should().Be(1);
        
    }

    [Fact]
    public async Task CarQueryService_AllAvailableFilters_Ok()
    {
        var service = new CarQueryService(_readerMock.Object, _photoStorageMock.Object, _mapperMock.Object);
        
        var filters = new CarFilters();
        
        var result = await service.CarByFiltersAsync(filters);

        var availableFilters = result.Value!.AvailableFilters;
        
        availableFilters.Brands.Should().BeEquivalentTo("Audi", "BMW", "Fiat", "Lada");
        availableFilters.Colors.Should().BeEquivalentTo("Black", "Blue", "Orange", "White");
        availableFilters.Conditions.Should().BeEquivalentTo(_cars.Select(x => x.Condition.ToString()).Distinct());
        availableFilters.PriceRange?.Min.Should().Be(_cars.Min(c => c.Price));
        availableFilters.PriceRange?.Max.Should().Be(_cars.Max(c => c.Price));
    }

    [Fact]
    public async Task CarQueryService_FilterByBrand_Audi_Ok()
    {
        var service = new CarQueryService(_readerMock.Object, _photoStorageMock.Object, _mapperMock.Object);
        
        var filters = new CarFilters
        {
            Brands = ["Audi"],
        };
        
        var result = await service.CarByFiltersAsync(filters);
        
        var availableFilters = result.Value!.AvailableFilters;
        
        availableFilters.Brands.Should().BeEquivalentTo("Audi");
        availableFilters.Colors.Should().BeEquivalentTo("Black", "Blue", "Orange", "White");
        availableFilters.Conditions.Should().BeEquivalentTo(_cars.Select(x => x.Condition.ToString()).Distinct());
        availableFilters.PriceRange?.Min.Should().Be(300_000);
        availableFilters.PriceRange?.Max.Should().Be(1_000_000);
    }
}