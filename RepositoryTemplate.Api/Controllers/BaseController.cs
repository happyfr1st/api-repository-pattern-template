using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepositoryTemplate.Repository.Repositories.Interfaces;

namespace RepositoryTemplate.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IMapper _mapper;
    
    public BaseController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
}