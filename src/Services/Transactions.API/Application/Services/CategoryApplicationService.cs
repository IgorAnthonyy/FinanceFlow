using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Transactions.API.Application.DTOs;
using Transactions.API.Application.Interfaces;
using Transactions.API.Domain.Entities;
using Transactions.API.Domain.Interfaces;
using FinanceFlow.SharedKernel.Entities;
using FinanceFlow.SharedKernel.Applications;

namespace Transactions.API.Application.Services;

public class CategoryApplicationService : BaseApplication, ICategoryApplicationService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryApplicationService(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        UserData userData) : base(userData)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryResponse> RegisterCategoryAsync(CategoryCreate request)
    {
        var category = _mapper.Map<Category>(request);

        category.PrepareInsert(UserData);

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<IEnumerable<CategoryResponse>> GetCategoriesAsync()
    {
        var list = await _categoryRepository.GetCategoriesAsync(UserData.Id);
        return _mapper.Map<IEnumerable<CategoryResponse>>(list);
    }
}
