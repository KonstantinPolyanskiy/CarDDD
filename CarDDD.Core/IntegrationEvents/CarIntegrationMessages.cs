using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.EntityObjects;
using MediatR;

namespace CarDDD.Core.IntegrationEvents;

public record CarCreatedWithoutPhotoMessage(Guid CarId, string ManagerEmail, string ManagerFullName);
