using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.Specifications;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainServices.Services;

/// <summary>
/// <see cref="ICarDomainService"/>
/// </summary>
public class DomainCarService : ICarDomainService
{
    public CreateCarResult CreateCar(CreateCarSpec s)
    {
        if (s.Photo is null)
        {
            return Car.CreateWithoutPhoto(
                s.Brand,
                s.Color,
                s.Price,
                s.Mileage,
                ManagerId.From(s.Manager.Id.Value)
            );
        }

        return Car.CreateWithPhoto(
            s.Brand,
            s.Color,
            s.Price,
            s.Mileage,
            s.Photo.Extension,
            ManagerId.From(s.Manager.Id.Value)
        );
    }

    public UpdateCarResult UpdateCar(Car car, UpdateCarSpec s)
    {
        // Нужно обновить менеджера
        if (s.AssignManagerSpec is not null)
        {
            var newManagerId = ManagerId.From(s.AssignManagerSpec.AssignmentManager.Id.Value);

            var change = car.ChangeManager(newManagerId, s.Employer);
            if (change.Status is not UpdateCarAction.Success)
                return change;

            s = s with {Employer = Employer.From(newManagerId.Value, s.AssignManagerSpec.AssignmentManager.Roles)};
        }
        
        // Нужно ли прикрепить фото
        if (s.PhotoSpec is not null)
        {
            if (s.PhotoSpec.OnlyIfNonePhoto)
            {
                var attach = car.AttachPhotoIfNone(s.PhotoSpec.Extension, s.Employer);
                if (attach.Status is not UpdateCarAction.Success)
                    return attach;
            }
            else
            {
                var attach = car.AttachOrReplacePhoto(s.PhotoSpec.Extension, s.Employer);
                if (attach.Status is not UpdateCarAction.Success)
                    return attach;
            }
        }
        
        // Нужно ли обновить базовые аттрибуты машины
        if (s.BasisAttributesSpec is not null)
        {
            var updated = car.Update(
                s.BasisAttributesSpec.Brand,
                s.BasisAttributesSpec.Color,
                s.BasisAttributesSpec.Price,
                s.BasisAttributesSpec.Mileage,
                s.Employer
            );
            if (updated.Status is not UpdateCarAction.Success)
                return updated;
        }
            
        return new UpdateCarResult(UpdateCarAction.Success);
    }
}