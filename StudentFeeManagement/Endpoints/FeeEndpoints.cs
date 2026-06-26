using FluentValidation;
using StudentFee.Core.Common;
using StudentFee.Core.DTOs;
using StudentFee.Core.Entities;
using StudentFee.Core.Interfaces;

namespace StudentFee.API.Endpoints;

public static class FeeEndpoints
{
    public static IEndpointRouteBuilder MapFeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/fees");

        // GET ALL
        group.MapGet("/", async (IFeeRepository repo) =>
        {
            var fees = await repo.GetAllAsync();
            return Results.Ok(ApiResponse<IEnumerable<FeeList>>.SuccessResponse(fees));
        });

        // GET BY ID
        group.MapGet("/{id:guid}", async (Guid id, IFeeRepository repo) =>
        {
            var fee = await repo.GetByIdAsync(id);
            return fee is null ? Results.NotFound() : Results.Ok(ApiResponse<Fee>.SuccessResponse(fee));
        });

        // CREATE
        group.MapPost("/", async (CreateFeeDto dto, IFeeRepository repo, IValidator<CreateFeeDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errorsDictionary = validationResult.ToDictionary();
                return Results.BadRequest(ApiResponse<object>.FailResponse("Validation failed.", errorsDictionary));
            }

            var fee = new Fee
            {
                Id = Guid.NewGuid(),
                StudentId = dto.StudentId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                Remarks = dto.Remarks
            };

            await repo.CreateAsync(fee);
            return Results.Ok(ApiResponse<Guid>.SuccessResponse(fee.Id, "Fee created"));
        });

        // UPDATE
        group.MapPut("/{id:guid}", async (Guid id, UpdateFeeDto dto, IFeeRepository repo) =>
        {
            var fee = await repo.GetByIdAsync(id);

            if (fee is null)
                return Results.NotFound();

            fee.Amount = dto.Amount;
            fee.PaymentDate = dto.PaymentDate;
            fee.Remarks = dto.Remarks;

            await repo.UpdateAsync(fee);
            return Results.Ok(ApiResponse<string>.SuccessResponse(null, "Fee updated"));
        });

        // DELETE
        group.MapDelete("/{id:guid}", async (Guid id, IFeeRepository repo) =>
        {
            await repo.DeleteAsync(id);
            return Results.Ok(ApiResponse<string>.SuccessResponse(null, "Fee deleted"));
        });

        return app;
    }
}