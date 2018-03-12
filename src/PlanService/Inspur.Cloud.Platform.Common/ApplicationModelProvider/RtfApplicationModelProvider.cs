
//using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Inspur.ECP.Rtf.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Inspur.ECP.Rtf.Common.ApplicationModelProvider
{
    public class RtfApplicationModelProvider : DefaultApplicationModelProvider
    {
        public RtfApplicationModelProvider(IOptions<MvcOptions> mvcOptionsAccessor) : base(mvcOptionsAccessor)
        { }
        //
        public override void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            // base.OnProvidersExecuted(context);
        }
        //
        public override void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            List<ApiPart> parts = this.GetApiParts();
            foreach (var part in parts)
            {
                string module = part.ModuleName;
                string asmName = part.Assembly.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? part.Assembly : part.Assembly + ".dll";
                Assembly assembly = Assembly.LoadFrom(asmName);
                if (assembly == null)
                {
                    continue;
                }

                foreach (var ctr in part.Controllers)
                {
                    Type type = assembly.GetType(ctr.Type);
                    if (type == null)
                    {
                        continue;
                    }
                    TypeInfo controllerType = type.GetTypeInfo();
                    var controllerModel = base.CreateControllerModel(controllerType);
                    if (controllerModel == null)
                    {
                        continue;
                    }

                    string tpl = "/api/" + module + "/" + ctr.Api.TrimStart('/');
                    controllerModel.Selectors[0].AttributeRouteModel = new AttributeRouteModel()
                    {
                        Template = tpl
                    };

                    NLogger.Debug(tpl);

                    context.Result.Controllers.Add(controllerModel);
                    controllerModel.Application = context.Result;

                    List<PropertyInfo> propertyInfos = this.GetPropertyInfo(type);
                    foreach (var propertyInfo in propertyInfos)
                    {
                        var propertyModel = base.CreatePropertyModel(propertyInfo);
                        if (propertyModel != null)
                        {
                            propertyModel.Controller = controllerModel;
                            controllerModel.ControllerProperties.Add(propertyModel);
                        }
                    }

                    foreach (var methodInfo in controllerType.AsType().GetMethods())
                    {
                        var actionModel = base.CreateActionModel(controllerType, methodInfo);
                        if (actionModel == null)
                        {
                            continue;
                        }

                        actionModel.Controller = controllerModel;
                        controllerModel.Actions.Add(actionModel);

                        foreach (var parameterInfo in actionModel.ActionMethod.GetParameters())
                        {
                            var parameterModel = base.CreateParameterModel(parameterInfo);
                            if (parameterModel != null)
                            {
                                parameterModel.Action = actionModel;
                                actionModel.Parameters.Add(parameterModel);
                            }
                        }
                    }
                }

            }
            return;



        }


        private List<ApiPart> GetApiParts()
        {
            List<ApiPart> parts = new List<ApiPart>();

            FileInfo[] cfgFiles = GetApiConfigFiles();

            foreach (var jsonFile in cfgFiles)
            {
                IConfiguration cfg = new ConfigurationBuilder()
                    .AddJsonFile(jsonFile.FullName)
                    .Build();
                parts.AddRange(cfg.GetCinfig<ApiPart[]>("ApiParts"));
            }

            return parts;
        }
        private FileInfo[] GetApiConfigFiles()
        {
            DirectoryInfo dir = Directory.CreateDirectory("configs/apis");
            return dir.GetFiles();
        }

        #region 反射调用 PropertyHelper
        private static Type PropertyHelperType = Assembly.Load("Microsoft.AspNetCore.Mvc.ViewFeatures").GetType("Microsoft.Extensions.Internal.PropertyHelper");
        private List<PropertyInfo> GetPropertyInfo(Type type)
        {
            List<PropertyInfo> propInfos = new List<PropertyInfo>();

            object[] saf = PropertyHelperType.InvokeMember("GetProperties", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { type }) as object[];
            foreach (var propertyHelper in saf)
            {
                PropertyInfo property = PropertyHelperType.GetProperty("Property");
                PropertyInfo propertyInfo = property.GetValue(propertyHelper) as PropertyInfo;
                propInfos.Add(propertyInfo);
            }

            return propInfos;
        }
        #endregion



    }
}
