dotnet ef migrations add MigrationName --project "./ReaSchedule.DAL/ReaSchedule.DAL.csproj" --startup-project "./REAScheduleService/REAScheduleService.csproj"

dotnet ef database update --startup-project "./REAScheduleService/REAScheduleService.csproj"  