using LMS.Blazor.Client.Models;
using LMS.Blazor.Client.Services;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ModuleDtos;
using LMS.Shared.Enums;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Web;

namespace LMS.Blazor.Client.Pages.Modules;

public partial class ModuleComponent
{
    private List<ModuleDto>? modules { get; set; } = [];
    [Parameter]
    public int CourseId { get; set; }

    [Inject] private IApiService _apiService { get; set; } = default!;
    [Inject] private NavigationManager _navigationManager { get; set; } = default!;
    [Inject] private ModuleStateService _moduleState { get; set; } = default!;

    private string? errorMessage;
    private bool isLoading { get; set; } = true;

    private void Edit(ModuleDto module)
    {
        _moduleState.Module = module;
        _navigationManager.NavigateTo($"/courses/{CourseId}/module/{module.Id}/edit");
    }

    private void Delete(ModuleDto module)
    {
        _moduleState.Module = module;
        _navigationManager.NavigateTo($"/courses/{CourseId}/modules/{module.Id}/delete");
    }



    protected override async Task OnInitializedAsync()
    {

        isLoading = true;
        try
        {
            await ApiCallResult();
        }
        finally
        {
            isLoading = false;
        }
    }
    private async Task<Result<IEnumerable<ModuleDto>>> ApiCall()
    {
        var requestParams = new ModuleRequestParams()
        {
            OrderBy = OrderByParams.DateAsc,
            PageSize = 100,
            IncludeActivities = true
        };
        var queryString = ObjectToQueryString(requestParams);
        try
        {
            var response = await _apiService.CallApiAsync<IEnumerable<ModuleDto>>($"api/courses/{CourseId}/Module?{queryString}");
            modules = response?.ToList();
            return Result<IEnumerable<ModuleDto>>.Ok(response);
        }
        catch (Exception ex)
        {
            try
            {
                using var doc = JsonDocument.Parse(ex.Message);
                var detail = doc.RootElement.GetProperty("detail").GetString();
                return Result<IEnumerable<ModuleDto>>.Fail(detail ?? "An error occurred.");
            }
            catch
            {
                return Result<IEnumerable<ModuleDto>>.Fail("An error occurred.");
            }
        }
    }
    private async Task ApiCallResult()
    {
        errorMessage = null;
        var result = await ApiCall();
        if (result.Success)
        {
            modules = result?.Data?.ToList();
        }
        else
        {
            errorMessage = result.ErrorMessage;
        }
    }

    //convert object to query string, ex: OrderBy=DateAsc&PageSize=100&IncludeActivities=True
    public string ObjectToQueryString(object obj)
    {
        var properties = from property in obj.GetType().GetProperties()
                         where property.GetValue(obj, null) != null
                         select property.Name + "=" + HttpUtility.UrlEncode(property.GetValue(obj, null)!.ToString());

        return string.Join("&", properties.ToArray());
    }
}
