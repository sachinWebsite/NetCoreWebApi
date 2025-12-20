using Application.DTOs;
using MediatR;

namespace Application.UseCases;

using Application.DTOs;
public class GetUserQuery : IRequest<Result<UserDto>>
{
    public Guid Id { get; set; }
}