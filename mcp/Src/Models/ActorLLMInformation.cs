public class ActorLLMInformation
{
    public Guid actorUuid {get; set;}
    public Guid projectUuid {get; set;}
    public string actorName {get; set;}

    public ActorLLMInformation(Guid actorUuid, Guid projectUuid, string actorName) {
        this.actorUuid = actorUuid;
        this.projectUuid = projectUuid;
        this.actorName = actorName;
    }
}