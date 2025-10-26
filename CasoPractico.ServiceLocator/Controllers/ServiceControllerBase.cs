using Microsoft.AspNetCore.Mvc;
using CasoPractico.Model.DTOs;                       
using CasoPractico.ServiceLocator.Helper;
using System.Text.Json;

namespace CasoPractico.ServiceLocator.Controllers
{
    public class ServiceControllerBase : ControllerBase
    {

        protected readonly Dictionary<string, Func<Task<IEnumerable<object>>>> ListResolvers;
        protected readonly Dictionary<string, Func<object, Task<object>>> CreateResolvers;
        protected readonly Dictionary<string, Func<string, object, Task<bool>>> UpdateResolvers;
        protected readonly Dictionary<string, Func<string, Task<bool>>> DeleteResolvers;

        protected ServiceControllerBase(IServiceMapper serviceMapper)
        {
            // ReadResolvers 
            ListResolvers = new()
            {
                ["Tasks"] = async () =>
                {
                    // defer resolution until invocation time
                    var service = await serviceMapper.GetServiceAsync<TaskDTO>("Tasks");
                    var data = await service.GetDataAsync();
                    return data.Cast<object>();
                }
            };

            // CreateResolvers

            CreateResolvers = new()
            {
                ["Tasks.cud"] = async (body) =>
                {
                    var service = await serviceMapper.GetServiceAsync<TaskDTO>("Tasks.cud");
                    var json = JsonSerializer.Serialize(body);

                    var created = await service.CreateDataAsync(json);
                    return created!;
                }
            };

            // UpdateResolvers

            UpdateResolvers = new()
            {
                ["Tasks.cud"] = async (id, body) =>
                {
                    var service = await serviceMapper.GetServiceAsync<TaskDTO>("Tasks.cud");
                    var dto = (TaskDTO)body;
                    var json = JsonSerializer.Serialize(body);

                    var ok = await service.UpdateDataAsync(id, json);
                    return ok;
                }
            };

            //DeleteResolvers

            DeleteResolvers = new()
            {
                ["Tasks.cud"] = async (id) =>
                {
                    var service = await serviceMapper.GetServiceAsync<TaskDTO>("Tasks.cud");
                    var ok = await service.DeleteDataAsync(id);
                    return ok;
                }
            };
        }
    }
}
