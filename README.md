

```

openapi-generator-cli generate -i sysmlv2-api-spec/openapi_client.yml -g csharp -o sysml-v2-client

```


```
dotnet Ecore2Code -f -l CS -n mcp.Src.Models -o mcp/Src/Models/SysML SysML.ecore --no-events -x --primitive-types --no-changed --no-changing --collectionsAreElements
```