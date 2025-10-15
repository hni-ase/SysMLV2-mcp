public class UseCaseLLMInformation
{
    public Guid useCaseUuid {get; set;}
    public Guid projectUuid {get; set;}
    public string useCaseName {get; set;}

    public UseCaseLLMInformation(Guid useCaseUuid, Guid projectUuid, string name) {
        this.useCaseUuid = useCaseUuid;
        this.projectUuid = projectUuid;
        this.useCaseName = name;
    }
}