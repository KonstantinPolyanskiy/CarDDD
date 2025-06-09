using System.Security.Claims;
using CarDDD.Api.Extensions;
using CarDDD.Api.Models;
using CarDDD.ApplicationServices.Models.RequestModels;
using CarDDD.ApplicationServices.Services;
using CarDDD.DomainServices.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDDD.Api.Controllers;

[ApiController]
[Route("api/employer/car")]
public class CarController(ICarMutableService carService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = nameof(Role.CarManager))]
    public async Task<IActionResult> AddNewCar([FromForm] AddCarRequest request)
    {
        var employerIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (employerIdValue == null)
            return Unauthorized("required have user id in claims");

        var employerRoles = User.GetEmployersRoles();
        if (employerRoles.Count == 0)
            return Unauthorized("required have employer roles");

        string extension = string.Empty;
        byte[]? data = null;

        if (request.Photo != null)
        {
            extension = Path.GetExtension(request.Photo.FileName).ToLowerInvariant().TrimStart('.');
            
            await using var ms = new MemoryStream();
            await request.Photo.CopyToAsync(ms);
            
            data = ms.ToArray();
        }

        var createRequest = new CreateCarRequest
        {
            Brand = request.Brand,
            Color = request.Color,
            Price = request.Price,
            Mileage = request.Mileage,
            EmployerId = Guid.Parse(employerIdValue),
            EmployerRoles = employerRoles,
            PhotoExtension = extension,
            PhotoData = data
        };
        
        var result = await carService.AddNewCarAsync(createRequest);
        if (result.IsFailure)
            return BadRequest(result.Error);
        
        return Ok(result.Value);
    }

    [HttpPatch]
    [Authorize(Roles = nameof(Role.CarManager))]
    public async Task<IActionResult> UpdateCar([FromRoute] Guid carId, [FromForm] UpdateCarRequest request)
    {
        var employerIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (employerIdValue == null)
            return Unauthorized("required have user id in claims");
        
        var employerRoles = User.GetEmployersRoles();
        if (employerRoles.Count == 0)
            return Unauthorized("required have employer roles");
        
        byte[]? data = null;
        string? extension  = null;
        if (request.PhotoFile != null)
        {
            extension = Path.GetExtension(request.PhotoFile.FileName).TrimStart('.');
            
            using var ms = new MemoryStream();
            await request.PhotoFile.CopyToAsync(ms);
            
            data = ms.ToArray();
        }

        var editReq = new EditCarRequest
        {
            CarId = carId,
            Brand = request.Brand,
            Color = request.Color,
            Price = request.Price,
            Mileage = request.Mileage,
            PhotoExtension = extension,
            PhotoData = data,
            OnlyIfNonePhoto = request.OnlyIfNonePhoto,
            NewManagerId = request.NewManagerId,
            NewManagerRoles = request.NewManagerRoles?.ToArray(),
            EmployerId = Guid.Parse(employerIdValue),
            EmployerRoles = employerRoles,
        };
        
        var result = await carService.UpdateCarAsync(editReq);
        if (result.IsFailure)
            return BadRequest(result.Error);
        
        return Ok(result.Value);
    }
}

