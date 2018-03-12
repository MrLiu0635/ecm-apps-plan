FROM microsoft/aspnetcore:2.0.0
WORKDIR /app
COPY src/PlanWeb/ecm-apps-planmanager/publish /app/
EXPOSE 5000
ENTRYPOINT ["dotnet","Inspur.EcmCloud.Apps.PlanManager.Main.dll"]