using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StudentFee.Core.Common;
using StudentFee.Core.DTOs;
using StudentFee.Core.Entities;
using StudentFee.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace StudentFee.API.Endpoints;

public static class StudentEndpoints
{
    public static IEndpointRouteBuilder MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/students");

        group.MapGet("/", async (IStudentRepository repository) =>
        {
            var students =await repository.GetAllAsync();

            return Results.Ok(ApiResponse<IEnumerable<Student>>.SuccessResponse(students));
        });

        group.MapGet("/{id:guid}", async (Guid id,IStudentRepository repository) =>
        {
            var student =await repository.GetByIdAsync(id);

            return student is null ? Results.NotFound(): Results.Ok(ApiResponse<Student>.SuccessResponse(student));
        });

        group.MapPost("/", async (CreateStudentDto dto,IStudentRepository repository,IValidator<CreateStudentDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errorsDictionary = validationResult.ToDictionary();
                return Results.BadRequest(ApiResponse<object>.FailResponse("Validation failed.", errorsDictionary));
                //return Results.BadRequest(ApiResponse<string>.FailResponse(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            var student = new Student
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow
            };

            await repository.CreateAsync(student);

            return Results.Ok(ApiResponse<Guid>.SuccessResponse(student.Id,"Student created"));
        });

        group.MapPut("/{id:guid}", async ( Guid id,UpdateStudentDto dto,IStudentRepository repository,  IValidator<UpdateStudentDto> validator) =>
        {
            var student = await repository.GetByIdAsync(id);

            if (student is null)
                return Results.NotFound();

            student.Name = dto.Name;
            student.Email = dto.Email;
            student.Phone = dto.Phone;
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errorsDictionary = validationResult.ToDictionary();
                return Results.BadRequest(ApiResponse<object>.FailResponse("Validation failed.", errorsDictionary));
                //return Results.BadRequest(ApiResponse<string>.FailResponse(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }
            await repository.UpdateAsync(student);

            return Results.Ok(ApiResponse<string>.SuccessResponse(null,"Student updated"));
        });

        group.MapDelete("/{id:guid}", async (Guid id,IStudentRepository repository) =>
        {
            await repository.DeleteAsync(id);
            return Results.Ok(ApiResponse<string>.SuccessResponse(null,"Student deleted"));
        });
        
        group.MapGet("/{id:guid}/with-fees", async (
            Guid id,
            IStudentRepository repository) =>
        {
            var student = await repository.GetStudentWithFeesAsync(id);

            return student is null
                ? Results.NotFound()
                : Results.Ok(
                    ApiResponse<StudentWithFeesDto>
                    .SuccessResponse(student));
        });


        group.MapGet("/paged", async (int page,int pageSize,IStudentRepository repo) =>
        {
            var result = await repo.GetPagedAsync(page, pageSize);

            return Results.Ok(ApiResponse<object>.SuccessResponse(result));
        });
        group.MapGet("/search", async (
    string keyword,
    IStudentRepository repo) =>
        {
            var result = await repo.SearchAsync(keyword);

            return Results.Ok(ApiResponse<object>.SuccessResponse(result));
        });
        return app;
    }

}